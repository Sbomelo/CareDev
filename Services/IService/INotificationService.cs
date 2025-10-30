using CareDev.Models;

namespace CareDev.Services.IService
{
    public interface INotificationService
    {
        Task NotifyAsync(string userId, string message);
        Task<List<Notification>> GetUserNotificationsAsync(string userId);
    }
}
