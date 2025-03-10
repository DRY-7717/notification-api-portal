using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using notificationapi.Data;
using notificationapi.Dtos.Notification.Request;
using notificationapi.Dtos.Notification.Response;
using notificationapi.Interfaces;
using notificationapi.Mappers;
using notificationapi.Models;

namespace notificationapi.Controllers
{
    [ApiController]
    [Route("api/notification")]
    public class NotificationAPIController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly INotificationRepository _notificationRepository;
        private readonly NotificationResponse _response;
        private static readonly List<TextWriter> _clients = new();

        public NotificationAPIController(ApplicationDbContext context, INotificationRepository notificationRepository)
        {
            _context = context;
            _notificationRepository = notificationRepository;
            _response = new NotificationResponse();
        }


        [HttpGet]
        [Route("getallnotification/{userLoginId}")]
        public async Task<IActionResult> GetAllNotification([FromRoute] string userLoginId)
        {

            var notifications = await _notificationRepository.GetAllNotificationsAsync(userLoginId);
            var notifDto = notifications.Select(n => n.ToViewNotificationDto());

            _response.status = 200;
            _response.message = "Success get all notification";
            _response.data = notifications;

            return new JsonResult(_response);
        }

        [HttpGet]
        [Route("unreadnotifications/{userLoginId}")]
        public async Task<IActionResult> UnreadNotifications([FromRoute] string userLoginId)
        {

            var notifications = await _notificationRepository.GetUnreadNotificationsAsync(userLoginId);
            var notifDto = notifications.Select(n => n.ToViewNotificationDto());

            _response.status = 200;
            _response.message = "Success get all notification";
            _response.data = notifications;

            return new JsonResult(_response);
        }


        [HttpGet]
        [Route("getnotificationbyid/{userLoginId}/{id:int}")]
        public async Task<IActionResult> GetNotificaitonById([FromRoute] string userLoginId, [FromRoute] int id)
        {

            var notification = await _notificationRepository.GetNotificationByIdAsync(userLoginId, id);

            if (notification == null)
            {
                return NotFound();
            }

            _response.status = 200;
            _response.message = "Success get notification";
            _response.data = notification;

            return new JsonResult(_response);
        }

        [HttpPost]
        [Route("createnotification")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(NotificationResponse), 201)]
        public async Task<IActionResult> CreateNotification([FromBody] List<CreateNotificationRequestDto> createDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }


                createDto.ForEach((request) =>
                {
                    if (request.isread != "1" && request.isread != "0")
                    {
                        throw new Exception("Is read must be 1 (read) or 0 (unread)");

                    }

                    if (request.type != "1" &&
                        request.type != "0" &&
                        request.type != "")
                    {
                        throw new Exception("Type must be 1, 0, null, or an empty string.");

                    }
                });

                var notifModel = createDto.ToNotificationFromCreateDtoRequest();
                await _notificationRepository.CreateNotificationAsync(notifModel);

                _response.status = 201;
                _response.message = "Notification has been added!";
                _response.data = notifModel.Select(n => n.ToNotificationDto());

                return Created(string.Empty, _response);


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


        [HttpGet("stream/{userLoginId}")]
        public async Task StreamNotifications(string userLoginId)
        {
            Response.Headers.Append("Content-Type", "text/event-stream");
            Response.Headers.Append("Cache-Control", "no-cache");
            // Hapus header Connection yang tidak valid untuk HTTP/2 dan HTTP/3
            // Response.Headers.Append("Connection", "keep-alive");

            var clientStream = Response.BodyWriter.AsStream();
            var writer = new StreamWriter(clientStream) { AutoFlush = true };

            lock (_clients)
            {
                _clients.Add(writer);
            }

            try
            {
                string lastDataHash = string.Empty;

                while (!HttpContext.RequestAborted.IsCancellationRequested)
                {
                    // Mengambil data langsung dari database
                    var currentNotifications = await _notificationRepository.GetAllNotificationsAsync(userLoginId) ?? new List<ViewNotification>();

                    // Serialize data dan buat hash untuk perbandingan efisien
                    var jsonData = System.Text.Json.JsonSerializer.Serialize(currentNotifications);
                    var currentHash = ComputeHash(jsonData);

                    // Hanya kirim jika data berubah (hash berbeda)
                    if (currentHash != lastDataHash)
                    {
                        await writer.WriteLineAsync($"data: {jsonData}\n");
                        await writer.FlushAsync();
                        lastDataHash = currentHash;
                    }

                    await Task.Delay(5000);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[SSE] Error: {ex.Message}");
            }
            finally
            {
                lock (_clients)
                {
                    _clients.Remove(writer);
                }
            }
        }

        [HttpGet("streamunread/{userLoginId}")]
        public async Task StreamNotificationsUnread(string userLoginId)
        {
            Response.Headers.Append("Content-Type", "text/event-stream");
            Response.Headers.Append("Cache-Control", "no-cache");
            // Hapus header Connection yang tidak valid untuk HTTP/2 dan HTTP/3
            // Response.Headers.Append("Connection", "keep-alive");

            var clientStream = Response.BodyWriter.AsStream();
            var writer = new StreamWriter(clientStream) { AutoFlush = true };

            lock (_clients)
            {
                _clients.Add(writer);
            }

            try
            {
                string lastDataHash = string.Empty;

                while (!HttpContext.RequestAborted.IsCancellationRequested)
                {
                    // Mengambil data langsung dari database
                    var unreadNotifications = await _notificationRepository.GetUnreadNotificationsAsync(userLoginId) ?? new List<ViewNotification>();

                    // Serialize data dan buat hash untuk perbandingan efisien
                    var jsonData = System.Text.Json.JsonSerializer.Serialize(unreadNotifications);
                    var currentHash = ComputeHash(jsonData);

                    // Hanya kirim jika data berubah (hash berbeda)
                    if (currentHash != lastDataHash)
                    {
                        await writer.WriteLineAsync($"data: {jsonData}\n");
                        await writer.FlushAsync();
                        lastDataHash = currentHash;
                    }

                    await Task.Delay(5000);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[SSE] Error: {ex.Message}");
            }
            finally
            {
                lock (_clients)
                {
                    _clients.Remove(writer);
                }
            }
        }

        // Helper method untuk menghitung hash dari string
        private string ComputeHash(string input)
        {
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                var bytes = System.Text.Encoding.UTF8.GetBytes(input);
                var hash = sha256.ComputeHash(bytes);
                return Convert.ToBase64String(hash);
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


                if (updateDto.isread != "1" && updateDto.isread != "0")
                {
                    throw new Exception("Is read must be 1 (read) or 0 (unread)");

                }

                if (updateDto.type != "1" &&
                    updateDto.type != "0" &&
                    updateDto.type != "")
                {
                    throw new Exception("Type must be 1, 0, null, or an empty string.");

                }

                var notifData = await _notificationRepository.UpdateNotificationAsync(id, updateDto);

                if (notifData == null)
                {
                    _response.status = 201;
                    _response.message = "Notification has been added!";
                    return NotFound(_response);
                }


                _response.status = 200;
                _response.message = "Notification has been added!";
                _response.data = notifData.ToNotificationDto();

                return new JsonResult(_response);

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


                if (isread != "1" && isread != "0")
                {
                    throw new Exception("Is read must be 1 (read) or 0 (unread)");
                }

                var existingNotification = await _notificationRepository.BulkUpdateNotificationAsync(userLoginId, isread);

                if (!existingNotification.Any())
                {
                    return NotFound();
                }

                _response.status = 200;
                _response.message = "Notification has been updated!";
                _response.data = existingNotification.Select(n => n.ToNotificationDto());

                return new JsonResult(_response);
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