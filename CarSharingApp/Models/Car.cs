using System.ComponentModel.DataAnnotations;

namespace CarSharingApp.Models
{
    public class Car
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Марка")]
        public string Brand { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Модель")]
        public string Model { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Год выпуска")]
        [Range(2000, 2030)]
        public int Year { get; set; }

        [Required]
        [Display(Name = "Цвет")]
        public string Color { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Номерной знак")]
        public string LicensePlate { get; set; } = string.Empty;

        [Display(Name = "VIN номер")]
        public string? VinNumber { get; set; }

        [Required]
        [Display(Name = "Тип топлива")]
        public FuelType FuelType { get; set; }

        [Required]
        [Display(Name = "Тип трансмиссии")]
        public TransmissionType TransmissionType { get; set; }

        [Required]
        [Display(Name = "Количество мест")]
        [Range(2, 8)]
        public int SeatsCount { get; set; }

        [Required]
        [Display(Name = "Цена за минуту")]
        [Range(0.01, 1000)]
        public decimal PricePerMinute { get; set; }

        [Display(Name = "Уровень топлива (%)")]
        [Range(0, 100)]
        public int FuelLevel { get; set; } = 100;

        [Display(Name = "Пробег (км)")]
        public int Mileage { get; set; }

        [Display(Name = "Широта")]
        public double? Latitude { get; set; }

        [Display(Name = "Долгота")]
        public double? Longitude { get; set; }

        [Display(Name = "Адрес")]
        public string? Address { get; set; }

        [Display(Name = "Статус")]
        public CarStatus Status { get; set; } = CarStatus.Available;

        [Display(Name = "Изображение")]
        public string? ImageUrl { get; set; }

        [Display(Name = "Описание")]
        public string? Description { get; set; }

        [Display(Name = "Дата добавления")]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        // Navigation properties
        public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    }

    public enum FuelType
    {
        Gasoline,
        Diesel,
        Electric,
        Hybrid
    }

    public enum TransmissionType
    {
        Manual,
        Automatic
    }

    public enum CarStatus
    {
        Available,
        Booked,
        InUse,
        Maintenance,
        OutOfService
    }
}