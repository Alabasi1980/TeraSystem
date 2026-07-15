using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WarehouseDashboard.Web.Data;
using WarehouseDashboard.Web.Models;
using BCrypt.Net;

namespace WarehouseDashboard.Web.Pages.AdminSecurePanel;

/// <summary>
/// Admin Panel login (password only, single shared password — Phase 1).
///
/// Behaviour:
/// - If an <see cref="AdminPassword"/> row exists: verify the submitted password
///   against the stored BCrypt hash via <see cref="BCrypt.Net.BCrypt.Verify"/>.
/// - If NO row exists (first run): hash the submitted password with
///   <see cref="BCrypt.Net.BCrypt.HashPassword"/> and store it as the singleton
///   admin password row. This is the one-time setup step.
///
/// The plaintext password is NEVER persisted — only the BCrypt hash.
/// </summary>
public class LoginModel : PageModel
{
    private readonly WarehouseDashboardDbContext _db;
    private readonly ILogger<LoginModel> _logger;
    private const string SessionKey = "AdminAuthenticated";

    public LoginModel(WarehouseDashboardDbContext db, ILogger<LoginModel> logger)
    {
        _db = db;
        _logger = logger;
    }

    [BindProperty]
    public string Password { get; set; } = string.Empty;

    public string? ErrorMessage { get; set; }

    public IActionResult OnGet()
    {
        // Already authenticated -> go straight to the admin home.
        if (HttpContext.Session.GetString(SessionKey) == "true")
        {
            return RedirectToPage("/admin-secure-panel/Index");
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (string.IsNullOrEmpty(Password))
        {
            ErrorMessage = "الرجاء إدخال كلمة المرور.";
            return Page();
        }

        try
        {
            // Singleton admin row (Id = 1 after first insert on an empty DB).
            var admin = await _db.AdminPasswords.OrderBy(a => a.Id).FirstOrDefaultAsync();

            if (admin is null)
            {
                // First run: create the singleton admin password.
                var hash = BCrypt.Net.BCrypt.HashPassword(Password);
                _db.AdminPasswords.Add(new AdminPassword { PasswordHash = hash });
                await _db.SaveChangesAsync();
            }
            else
            {
                var valid = BCrypt.Net.BCrypt.Verify(Password, admin.PasswordHash);
                if (!valid)
                {
                    ErrorMessage = "كلمة المرور غير صحيحة.";
                    return Page();
                }
            }

            HttpContext.Session.SetString(SessionKey, "true");
            return RedirectToPage("/admin-secure-panel/Index");
        }
        catch (Exception ex)
        {
            // Never leak internals to the client; log server-side only.
            _logger.LogError(ex, "Admin login failed.");
            ErrorMessage = "تعذر الاتصال بقاعدة البيانات. يرجى المحاولة لاحقاً.";
            return Page();
        }
    }
}
