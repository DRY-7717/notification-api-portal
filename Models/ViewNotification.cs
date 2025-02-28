using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace notificationapi.Models
{
    [Keyless]
    public class ViewNotification
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
        public static string GetViewNotificationSQL()
        {
            return string.Format(@"SELECT t1.*, t2.userid as namalengkappengirim, t3.userid as namalengkappenerima
                        FROM [dbo].[NOTIFICATIONS] AS t1
                        LEFT JOIN [Master].[MASTERUSER] AS t2
                        ON t1.fromuser = t2.userid
                        LEFT JOIN [Master].[MASTERUSER] AS t3
                        ON t1.touser = t3.userid");
        }
    }
}