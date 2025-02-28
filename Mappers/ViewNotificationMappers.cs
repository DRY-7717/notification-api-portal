using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using notificationapi.Dtos.Notification;
using notificationapi.Models;

namespace notificationapi.Mappers
{
    public static class ViewNotificationMappers
    {
        public static ViewNotificationDto ToViewNotificationDto(this ViewNotification viewNotificationModel)
        {
            return new ViewNotificationDto
            {
                id = viewNotificationModel.id,
                ndate = viewNotificationModel.ndate,
                fromuser = viewNotificationModel.fromuser,
                touser = viewNotificationModel.touser,
                title = viewNotificationModel.title,
                sub_title = viewNotificationModel.sub_title,
                body = viewNotificationModel.body,
                type = viewNotificationModel.type,
                isread = viewNotificationModel.isread,
                modul = viewNotificationModel.modul,
                description = viewNotificationModel.description,
                url = viewNotificationModel.url,
                namalengkappengirim = viewNotificationModel.namalengkappengirim,
                namalengkappenerima = viewNotificationModel.namalengkappenerima
            };
        }

    }
}