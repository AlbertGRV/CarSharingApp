using System.ComponentModel.DataAnnotations;

namespace CarSharingApp.Models
{
    public class Booking
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required]
        public int CarId { get; set; }

        [Display(Name = "Время начала")]
        public DateTime? StartTime { get; set; }

        [Display(Name = "Время окончания")]
        public DateTime? EndTime { get; set; }

        [Display(Name = "Планируемая продолжительность (минуты)")]
        public int? PlannedDurationMinutes { get; set; }

        [Display(Name = "Фактическая продолжительность (минуты)")]
        public int? ActualDurationMinutes { get; set; }

        [Display(Name = "Начальный пробег")]
        public int? StartMileage { get; set; }

        [Display(Name = "Конечный пробег")]
        public int? EndMileage { get; set; }

        [Display(Name = "Начальный уровень топлива")]
        public int? StartFuelLevel { get; set; }

        [Display(Name = "Конечный уровень топлива")]
        public int? EndFuelLevel { get; set; }

        [Display(Name = "Общая стоимость")]
        [DataType(DataType.Currency)]
        public decimal? TotalCost { get; set; }

        [Display(Name = "Статус")]
        public BookingStatus Status { get; set; } = BookingStatus.Active;

        [Display(Name = "Причина отмены")]
        [StringLength(500)]
        public string? CancellationReason { get; set; }

        [Display(Name = "Дата создания")]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        [Display(Name = "Дата создания")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Display(Name = "Комментарий")]
        public string? Notes { get; set; }

        // Navigation properties
        public virtual ApplicationUser User { get; set; } = null!;
        public virtual Car Car { get; set; } = null!;
    }

    public enum BookingStatus
    {
        Active,
        Completed,
        Cancelled
    }
}