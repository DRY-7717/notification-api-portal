using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using notificationapi.Data;
using notificationapi.Dtos.Notification.Request;
using notificationapi.Interfaces;
using notificationapi.Models;

namespace notificationapi.Repository
{
    public class NotificationRepository : INotificationRepository
    {

        private readonly ApplicationDbContext _context;

        public NotificationRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<ViewNotification>> GetAllNotificationsAsync(string userLoginId)
        {
            return await _context.ViewNotifications.FromSqlRaw(ViewNotification.GetViewNotificationSQL()).Where(n => n.touser == userLoginId).OrderByDescending(n => n.ndate).ToListAsync();
        }

        public async Task<ViewNotification?> GetNotificationByIdAsync(string userLoginId, int id)
        {
            var notification = await _context.ViewNotifications.FromSqlRaw(ViewNotification.GetViewNotificationSQL()).Where(s => s.touser == userLoginId && s.id == id).FirstOrDefaultAsync();

            if (notification == null)
            {
                return null;
            }

            return notification;
        }


        public async Task<List<ViewNotification>> GetUnreadNotificationsAsync(string userLoginId)
        {
            return await _context.ViewNotifications.FromSqlRaw(ViewNotification.GetViewNotificationSQL()).Where(s => s.touser == userLoginId && s.isread == "0").OrderByDescending(s => s.ndate).ToListAsync();
        }

        public async Task<List<Notification>> CreateNotificationAsync(List<Notification> notifModel)
        {
            await _context.Notifications.AddRangeAsync(notifModel);
            await _context.SaveChangesAsync();
            return notifModel;
        }

        public async Task<Notification?> UpdateNotificationAsync(int id, UpdateNotificationRequestDto updateDto)
        {
            var existingNotification = await _context.Notifications.FirstOrDefaultAsync(s => s.id == id);

            if (existingNotification == null)
            {
                return null;
            }

            existingNotification.fromuser = updateDto.fromuser;
            existingNotification.touser = updateDto.touser;
            existingNotification.title = updateDto.title;
            existingNotification.sub_title = updateDto.sub_title;
            existingNotification.body = updateDto.body;
            existingNotification.type = updateDto.type;
            existingNotification.isread = updateDto.isread;
            existingNotification.modul = updateDto.modul;
            existingNotification.description = updateDto.description;
            existingNotification.url = updateDto.url;

            await _context.SaveChangesAsync();

            return existingNotification;
        }


        public async Task<List<Notification>> BulkUpdateNotificationAsync(string userLoginId, string isread)
        {
            var existingNotification = await _context.Notifications.Where(notif => notif.touser == userLoginId).ToListAsync();

            if (!existingNotification.Any())
            {
                return new List<Notification>();
            }

            existingNotification.ForEach(notif => notif.isread = isread);
            await _context.SaveChangesAsync();

            return existingNotification;
        }
    }
}