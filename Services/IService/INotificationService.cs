using CareDev.Models;

namespace CareDev.Services.IService
{
    public interface INotificationService
    {
        Task NotifyAsync(string userId, string message);
        Task<IEnumerable<Notification>> GetUserNotificationsAsync(string userId);
        Task MarkAsReadAsync(int notificationId);
    }
}
