using System.Diagnostics;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;

namespace WarehouseDashboard.Web.Infrastructure;

public class OpenCodeGoAdapter : IAIProvider
{
    private readonly AIAssistantOptions _options;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<OpenCodeGoAdapter> _logger;

    public OpenCodeGoAdapter(
        IOptions<AIAssistantOptions> options,
        IHttpClientFactory httpClientFactory,
        ILogger<OpenCodeGoAdapter> logger)
    {
        _options = options.Value;
        _httpClientFactory = httpClientFactory;
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
                max_tokens = request.MaxOutputTokens > 0 ? request.MaxOutputTokens : _options.MaxOutputTokens
            };

            var jsonPayload = JsonSerializer.Serialize(payload);
            var httpContent = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            var httpClient = _httpClientFactory.CreateClient();
            httpClient.Timeout = TimeSpan.FromSeconds(_options.TimeoutSeconds);

            using var httpRequest = new HttpRequestMessage(HttpMethod.Post, _options.BaseUrl)
            {
                Content = httpContent
            };
            httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _options.ApiKey);

            using var httpResponse = await httpClient.SendAsync(httpRequest, ct);
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

            var content = root
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString() ?? string.Empty;

            var tokensUsed = 0;
            if (root.TryGetProperty("usage", out var usage) &&
                usage.TryGetProperty("total_tokens", out var totalTokens))
            {
                tokensUsed = totalTokens.GetInt32();
            }

            return new AIAssistantResponse
            {
                Content = content,
                Success = true,
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
