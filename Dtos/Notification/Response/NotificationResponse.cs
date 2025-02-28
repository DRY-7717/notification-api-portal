using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace notificationapi.Dtos.Notification.Response
{
    public class NotificationResponse
    {
        public int? status { get; set; }
        public string? message { get; set; }
        public object? data { get; set; }
    }
}