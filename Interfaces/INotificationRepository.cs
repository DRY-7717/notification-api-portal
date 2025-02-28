using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using notificationapi.Dtos.Notification.Request;
using notificationapi.Models;

namespace notificationapi.Interfaces
{
    public interface INotificationRepository
    {
        Task<List<ViewNotification>> GetAllNotificationsAsync(string userLoginId);
        Task<List<ViewNotification>> GetUnreadNotificationsAsync(string userLoginId);
        Task<ViewNotification?> GetNotificationByIdAsync(string userLoginId, int id);
        Task<List<Notification>> CreateNotificationAsync(List<Notification> notifModel);
        Task<Notification?> UpdateNotificationAsync(int id, UpdateNotificationRequestDto updateDto);
        Task<List<Notification>> BulkUpdateNotificationAsync(string userLoginId, string isread);

    }
}