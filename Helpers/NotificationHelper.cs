using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System.Text.Json;

namespace Getdata1.Helpers
{
    public static class NotificationHelper
    {
        /// <summary>
        /// Thiết lập thông báo hiển thị cho người dùng sau khi redirect
        /// </summary>
        /// <param name="tempData">TempData của Controller</param>
        /// <param name="message">Nội dung thông báo</param>
        /// <param name="type">Loại: success, error, warning</param>
        /// <param name="showLoginLink">Hiển thị nút Đăng nhập nếu cần</param>
        public static void SetNotification(ITempDataDictionary tempData, string message, string type = "success", bool showLoginLink = false)
        {
            var notification = new
            {
                Message = message,
                Type = type.ToLower(),
                ShowLoginLink = showLoginLink
            };

            tempData["Notification"] = JsonSerializer.Serialize(notification);
        }
    }
}