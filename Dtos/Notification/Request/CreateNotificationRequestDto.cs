using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace notificationapi.Dtos.Notification.Request
{
    public class CreateNotificationRequestDto
    {
        [MaxLength(100, ErrorMessage = "From user cannot be over 100 words")]
        public string? fromuser { get; set; }


        [MaxLength(100, ErrorMessage = "To user cannot be over 100 words")]
        public string? touser { get; set; }


        [MaxLength(100, ErrorMessage = "Title cannot be over 100 words")]
        public string? title { get; set; }

        [MaxLength(100, ErrorMessage = "Title cannot be over 100 words")]
        public string? sub_title { get; set; }


        [MaxLength(500, ErrorMessage = "Body cannot be over 500 words")]
        public string? body { get; set; }


        [DefaultValue("0")]
        [MaxLength(1, ErrorMessage = "Type cannot be over 1 words")]
        public string? type { get; set; }


        [DefaultValue("0")]
        [MaxLength(1, ErrorMessage = "Isread cannot be over 1 words")]
        public string? isread { get; set; } = "0";


        [MaxLength(100, ErrorMessage = "Modul cannot be over 100 words")]
        public string? modul { get; set; }

        [MaxLength(1000, ErrorMessage = "Modul cannot be over 1000 words")]
        public string? description { get; set; }

        [MaxLength(2000, ErrorMessage = "Modul cannot be over 2000 words")]
        public string? url { get; set; }
    }
}