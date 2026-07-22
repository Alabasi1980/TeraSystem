using System.Diagnostics;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;

namespace WarehouseDashboard.Web.Infrastructure;

public class OpenCodeGoAdapter : IAIProvider
{
    private readonly AIAssistantOptions _options;
    private readonly HttpClient _httpClient;
    private readonly ILogger<OpenCodeGoAdapter> _logger;

    public OpenCodeGoAdapter(
        IOptions<AIAssistantOptions> options,
        HttpClient httpClient,
        ILogger<OpenCodeGoAdapter> logger)
    {
        _options = options.Value;
        _httpClient = httpClient;
        _httpClient.Timeout = TimeSpan.FromSeconds(_options.TimeoutSeconds);
        _logger = logger;
    }

    public async Task<AIAssistantResponse> SendAsync(AIAssistantRequest request, CancellationToken ct = default)
    {
        var stopwatch = Stopwatch.StartNew();

        try
        {
            // Build the system prompt — append card-specific instructions when provided.
            var systemPrompt = request.SystemPrompt;
            if (!string.IsNullOrEmpty(request.CardAssistantPrompt))
            {
                systemPrompt = $"{systemPrompt}\n\nتعليمات خاصة لهذه البطاقة:\n{request.CardAssistantPrompt}";
            }

            var payload = new
            {
                model = _options.ModelId,
                messages = new[]
                {
                    new { role = "system", content = systemPrompt },
                    new { role = "user", content = request.UserMessage }
                },
                max_tokens = request.MaxOutputTokens > 0 ? request.MaxOutputTokens : _options.MaxOutputTokens,
                thinking = new { type = "disabled" }
            };

            var jsonPayload = JsonSerializer.Serialize(payload);
            var httpContent = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            using var httpRequest = new HttpRequestMessage(HttpMethod.Post, _options.BaseUrl)
            {
                Content = httpContent
            };
            httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _options.ApiKey);

            using var httpResponse = await _httpClient.SendAsync(httpRequest, ct);
            stopwatch.Stop();

            var responseBody = await httpResponse.Content.ReadAsStringAsync();

            if (!httpResponse.IsSuccessStatusCode)
            {
                _logger.LogError(
                    "OpenCodeGo API returned HTTP {StatusCode}. Response body length: {BodyLength}",
                    (int)httpResponse.StatusCode,
                    responseBody.Length);
                return new AIAssistantResponse
                {
                    Success = false,
                    ErrorMessage = $"The AI service returned an error (HTTP {(int)httpResponse.StatusCode}).",
                    ResponseTimeMs = stopwatch.ElapsedMilliseconds
                };
            }

            // Parse the OpenAI-compatible response: choices[0].message.content
            using var doc = JsonDocument.Parse(responseBody);
            var root = doc.RootElement;

            // Get the message object from the first choice
            JsonElement message;
            string? content = null;
            try
            {
                message = root.GetProperty("choices")[0].GetProperty("message");
                // Try content first, then refusal
                if (message.TryGetProperty("content", out var contentProp) && contentProp.ValueKind == JsonValueKind.String)
                    content = contentProp.GetString();

                // If content is null/empty, check for refusal
                if (string.IsNullOrEmpty(content) && message.TryGetProperty("refusal", out var refusalProp))
                {
                    _logger.LogWarning("AI provider refused: {Refusal}", refusalProp.GetString());
                    return new AIAssistantResponse
                    {
                        Success = false,
                        ErrorMessage = "تعذر تحليل البيانات: رفضت الخدمة الطلب.",
                        ResponseTimeMs = stopwatch.ElapsedMilliseconds
                    };
                }
            }
            catch (Exception parseEx)
            {
                _logger.LogError(parseEx, "Failed to parse AI response body: {Body}", responseBody);
                return new AIAssistantResponse
                {
                    Success = false,
                    ErrorMessage = $"استجابة غير متوقعة: {responseBody[..Math.Min(responseBody.Length, 300)]}",
                    ResponseTimeMs = stopwatch.ElapsedMilliseconds
                };
            }

            // If content is empty, return error (diagnostic info removed now)
            if (string.IsNullOrEmpty(content))
            {
                _logger.LogWarning("AI response has empty content. Finish reason: {Reason}",
                    root.GetProperty("choices")[0].TryGetProperty("finish_reason", out var fr) ? fr.GetString() : "unknown");
                return new AIAssistantResponse
                {
                    Success = false,
                    ErrorMessage = "لم يتمكن المساعد من تحليل البطاقة حالياً.",
                    ResponseTimeMs = stopwatch.ElapsedMilliseconds
                };
            }

            var tokensUsed = 0;
            if (root.TryGetProperty("usage", out var usage) &&
                usage.TryGetProperty("total_tokens", out var totalTokens))
            {
                tokensUsed = totalTokens.GetInt32();
            }

            return new AIAssistantResponse
            {
                Content = content ?? string.Empty,
                Success = string.IsNullOrEmpty(content) ? false : true,
                ErrorMessage = string.IsNullOrEmpty(content) ? "الرد فارغ من الخدمة." : null,
                TokensUsed = tokensUsed,
                ResponseTimeMs = stopwatch.ElapsedMilliseconds
            };
        }
        catch (HttpRequestException ex)
        {
            stopwatch.Stop();
            _logger.LogError(ex, "HTTP error contacting the AI provider.");
            return new AIAssistantResponse
            {
                Success = false,
                ErrorMessage = "A network error occurred while contacting the AI service.",
                ResponseTimeMs = stopwatch.ElapsedMilliseconds
            };
        }
        catch (TaskCanceledException ex) when (!ct.IsCancellationRequested)
        {
            // HttpClient timeout — the caller's token was not cancelled.
            stopwatch.Stop();
            _logger.LogError(ex, "AI provider request timed out after {TimeoutSeconds}s.", _options.TimeoutSeconds);
            return new AIAssistantResponse
            {
                Success = false,
                ErrorMessage = "The AI service request timed out.",
                ResponseTimeMs = stopwatch.ElapsedMilliseconds
            };
        }
        catch (TaskCanceledException)
        {
            stopwatch.Stop();
            return new AIAssistantResponse
            {
                Success = false,
                ErrorMessage = "The AI request was cancelled.",
                ResponseTimeMs = stopwatch.ElapsedMilliseconds
            };
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.LogError(ex, "Unexpected error in AI provider adapter.");
            return new AIAssistantResponse
            {
                Success = false,
                ErrorMessage = "An unexpected error occurred while processing the AI request.",
                ResponseTimeMs = stopwatch.ElapsedMilliseconds
            };
        }
    }
}
