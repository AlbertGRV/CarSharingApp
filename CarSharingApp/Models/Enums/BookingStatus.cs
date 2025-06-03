namespace CarSharingApp.Models.Enums
{
    public enum BookingStatus
    {
        Pending,        // Ожидает подтверждения
        Confirmed,      // Подтверждено
        Active,         // Активно
        Completed,      // Завершено
        Cancelled,      // Отменено
        NoShow          // Не явился
    }
}