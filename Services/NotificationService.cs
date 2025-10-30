using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using CareDev.Data;
using CareDev.Hubs;
using CareDev.Models;
using CareDev.Services;
using CareDev.Services.IService;
using NotificationHub = CareDev.Models.NotificationHub;

namespace YourNamespace.Services.Implementation
{
    public class NotificationService : INotificationService
    {
        private readonly ApplicationDbContext _context;
        private readonly IHubContext<NotificationHub> _hubContext;
        public NotificationService(ApplicationDbContext context, IHubContext<NotificationHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }

        public async Task NotifyAsync(string userId, string message)
        {
            var note = new Notification { UserId = userId, Message = message, CreatedAt = DateTime.UtcNow };
            _context.Notifications.Add(note);
            await _context.SaveChangesAsync();

            // Send real-time to connected user
            await _hubContext.Clients.User(userId).SendAsync("ReceiveNotification", message);
        }

        public async Task<IEnumerable<Notification>> GetUserNotificationsAsync(string userId)
        {
            return await _context.Notifications
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();
        }

        public async Task MarkReadAsync(int notificationId)
        {
            var n = await _context.Notifications.FindAsync(notificationId);
            if (n == null) return;
            n.IsRead = true;
            await _context.SaveChangesAsync();
        }
        // inside NotificationService class (Services/Implementation/NotificationService.cs)

        public async Task MarkAsReadAsync(int notificationId)
        {
            // find the notification
            var notification = await _context.Notifications.FindAsync(notificationId);
            if (notification == null) return; // or throw if you prefer

            if (!notification.IsRead)
            {
                notification.IsRead = true;
                await _context.SaveChangesAsync();
            }

            // Optionally notify the user real-time that the notification was marked read
            // await _hubContext.Clients.User(notification.UserId).SendAsync("NotificationMarkedRead", notificationId);
        }

    }
}
