using CareDev.Data;
using CareDev.Models;
using CareDev.Services.IService;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace CareDev.Services
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
            // Save to DB
            var note = new Notification { UserId = userId, Message = message };
            _context.Notifications.Add(note);
            await _context.SaveChangesAsync();
            // Push real-time (SignalR)
            await _hubContext.Clients.User(userId).SendAsync("ReceiveNotification", message);
        }
        public async Task<List<Notification>> GetUserNotificationsAsync(string userId)
        {
            return await _context.Notifications
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();
        }
    }
}
