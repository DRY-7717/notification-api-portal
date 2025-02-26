using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using notificationapi.Dtos.Notification;
using notificationapi.Dtos.Notification.Request;
using notificationapi.Models;

namespace notificationapi.Mappers
{
    public static class NotificationMappers
    {

        public static NotificationDto ToNotificationDto(this Notification notificationModel)
        {
            return new NotificationDto
            {
                id = notificationModel.id,
                ndate = notificationModel.ndate,
                fromuser = notificationModel.fromuser,
                touser = notificationModel.touser,
                title = notificationModel.title,
                sub_title = notificationModel.sub_title,
                body = notificationModel.body,
                type = notificationModel.type,
                isread = notificationModel.isread,
                modul = notificationModel.modul,
                description = notificationModel.description,
                url = notificationModel.url
            };
        }

        public static List<Notification> ToNotificationFromCreateDtoRequest(this List<CreateNotificationRequestDto> requestNotifDto)
        {
            return requestNotifDto.Select(request => new Notification
            {
                fromuser = request.fromuser,
                touser = request.touser,
                title = request.title,
                sub_title = request.sub_title,
                body = request.body,
                type = request.type,
                isread = request.isread,
                modul = request.modul,
                description = request.description,
                url = request.url,
            }).ToList();
        }

      

    }
}