using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using notificationapi.Data;
using notificationapi.Dtos.Notification.Request;
using notificationapi.Mappers;

namespace notificationapi.Controllers
{
    [ApiController]
    [Route("api/notification")]
    public class NotificationAPIController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public NotificationAPIController(ApplicationDbContext context)
        {
            _context = context;
        }


        [HttpGet]
        [Route("getallnotification/{userLoginId}")]
        public async Task<IActionResult> GetAllNotification([FromRoute] string userLoginId)
        {

            var notifications = await _context.Notifications.Where(s => s.touser == userLoginId).OrderByDescending(s => s.ndate).ToListAsync();
            var notifDto = notifications.Select(s => s.ToNotificationDto());

            return Ok(notifications);
        }


        [HttpPost]
        [Route("createnotification")]
        public async Task<IActionResult> CreateNotification([FromBody] List<CreateNotificationRequestDto> createDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var notifModel = createDto.ToNotificationFromCreateDtoRequest();
                await _context.Notifications.AddRangeAsync(notifModel);
                await _context.SaveChangesAsync();

                return Ok(notifModel.Select(n => n.ToNotificationDto()));
            }
            catch (Exception e)
            {
                return BadRequest(new
                {
                    status = 400,
                    message = e.Message
                });
            }

        }

        [HttpPut]
        [Route("updatenotification/{id:int}")]
        public async Task<IActionResult> UpdateNotification([FromRoute] int id, [FromBody] UpdateNotificationRequestDto updateDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var existingNotification = await _context.Notifications.FirstOrDefaultAsync(s => s.id == id);

                if (existingNotification == null)
                {
                    return NotFound();
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

                return Ok(existingNotification.ToNotificationDto());


            }
            catch (Exception e)
            {
                return BadRequest(new
                {
                    status = 400,
                    message = e.Message
                });
            }

        }
        [HttpPut]
        [Route("markallasread/{userLoginId}")]
        public async Task<IActionResult> BulkUpdateNotification([FromRoute] string userLoginId, [FromBody] string isread)
        {
            try
            {
                var existingNotification = await _context.Notifications.Where(notif => notif.touser == userLoginId).ToListAsync();

                if (existingNotification == null)
                {
                    return NotFound();
                }

                existingNotification.ForEach(notif => notif.isread = isread);
                await _context.SaveChangesAsync();
                return Ok(existingNotification.Select(n => n.ToNotificationDto()));
            }
            catch (Exception e)
            {
                return BadRequest(new
                {
                    status = 400,
                    message = e.Message
                });
            }

        }

    }
}