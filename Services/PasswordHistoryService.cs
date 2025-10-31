using CareDev.Data;
using CareDev.Models;
using CareDev.Services.IService;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CareDev.Services
{
    public class PasswordHistoryService :IPasswordHistoryService
    {
        private readonly ApplicationDbContext _db;
        private readonly IPasswordHasher<ApplicationUser> _hasher;
        private readonly int _historyLimit = 5; // store/check last 5 passwords

        public PasswordHistoryService(ApplicationDbContext db, IPasswordHasher<ApplicationUser> hasher)
        {
            _db = db;
            _hasher = hasher;
        }

        // Checks whether the plaintext newPassword equals current or any last N stored hashes
        public async Task<bool> IsInHistoryAsync(ApplicationUser user, string newPassword)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));
            // 1) check against current password hash
            var currentHash = user.PasswordHash;
            if (!string.IsNullOrEmpty(currentHash))
            {
                var res = _hasher.VerifyHashedPassword(user, currentHash, newPassword);
                if (res == PasswordVerificationResult.Success) return true;
            }

            // 2) check against history entries
            var hist = await _db.PasswordHistories
                                .Where(h => h.UserId == user.Id)
                                .OrderByDescending(h => h.CreatedAt)
                                .Take(_historyLimit)
                                .ToListAsync();

            foreach (var h in hist)
            {
                var fakeUser = new ApplicationUser { Id = user.Id, PasswordHash = h.PasswordHash };
                var r = _hasher.VerifyHashedPassword(fakeUser, h.PasswordHash, newPassword);
                if (r == PasswordVerificationResult.Success) return true;
            }

            return false;
        }

        // Adds the given hashed password to history and keep only latest _historyLimit
        public async Task AddToHistoryAsync(ApplicationUser user, string newPasswordHash)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));
            if (string.IsNullOrEmpty(newPasswordHash)) throw new ArgumentNullException(nameof(newPasswordHash));

            var entry = new PasswordHistory
            {
                UserId = user.Id,
                PasswordHash = newPasswordHash,
                CreatedAt = DateTime.UtcNow
            };

            _db.PasswordHistories.Add(entry);
            await _db.SaveChangesAsync();

            // cleanup: keep only last _historyLimit rows
            var toDelete = await _db.PasswordHistories
                .Where(h => h.UserId == user.Id)
                .OrderByDescending(h => h.CreatedAt)
                .Skip(_historyLimit)
                .ToListAsync();

            if (toDelete.Any())
            {
                _db.PasswordHistories.RemoveRange(toDelete);
                await _db.SaveChangesAsync();
            }
        }
  
    }
}
