using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace notificationapi.Dtos.Notification
{
    public class ViewNotificationDto
    {
    
        public int id { get; set; }
        public DateTime? ndate { get; set; } = DateTime.Now;
        public string? fromuser { get; set; }
        public string? touser { get; set; }
        public string? title { get; set; }
        public string? sub_title { get; set; }
        public string? body { get; set; }
        public string? type { get; set; }
        public string? isread { get; set; }
        public string? modul { get; set; }
        public string? description { get; set; }
        public string? url { get; set; }
        public string? namalengkappengirim { get; set; }
        public string? namalengkappenerima { get; set; }
    }
}