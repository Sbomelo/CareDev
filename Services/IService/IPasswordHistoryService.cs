using CareDev.Models;

namespace CareDev.Services.IService
{
    public interface IPasswordHistoryService
    {
        Task<bool> IsInHistoryAsync(ApplicationUser user, string newPassword); // checks current + previous N
        Task AddToHistoryAsync(ApplicationUser user, string newPasswordHash);
    }
}
