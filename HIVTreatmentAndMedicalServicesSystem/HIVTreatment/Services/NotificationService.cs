using HIVTreatment.Services;

public class NotificationService : INotificationService
{
    public void Send(string userId, string message)
    {
        Console.WriteLine($" Gửi tới {userId}: {message}");
        // Có thể lưu vào bảng Notification, gửi email, v.v.
    }
}
