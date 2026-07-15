using Microsoft.EntityFrameworkCore;
using TeraQuotation.Data;
using TeraQuotation.Models;

namespace TeraQuotation.Services;

public class AuthService : IAuthService
{
    private readonly AppDbContext _db;

    public AuthService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<bool> IsFirstTimeAsync()
    {
        return !await _db.Users.AnyAsync();
    }

    public async Task<(bool Success, string Error)> SetPasswordAsync(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
            return (false, "كلمة المرور لا يمكن أن تكون فارغة");
        if (password.Length < 4)
            return (false, "كلمة المرور يجب أن تكون 4 أحرف على الأقل");

        // Remove existing user (if any) and create new one
        var existing = await _db.Users.ToListAsync();
        _db.Users.RemoveRange(existing);

        var user = new User
        {
            Username = "admin",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
            CreatedAt = DateTime.UtcNow
        };
        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        return (true, string.Empty);
    }

    public async Task<(bool Success, string Error)> ValidatePasswordAsync(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
            return (false, "الرجاء إدخال كلمة المرور");

        var user = await _db.Users.FirstOrDefaultAsync();
        if (user == null)
            return (false, "لم يتم تعيين كلمة مرور بعد");

        if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            return (false, "كلمة المرور غير صحيحة");

        return (true, string.Empty);
    }
}
