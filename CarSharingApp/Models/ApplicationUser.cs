using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace CarSharingApp.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Display(Name = "Имя")]
        [StringLength(50)]
        public string? FirstName { get; set; }

        [Display(Name = "Фамилия")]
        [StringLength(50)]
        public string? LastName { get; set; }

        [Display(Name = "Дата рождения")]
        [DataType(DataType.Date)]
        public DateTime? DateOfBirth { get; set; }

        [Display(Name = "Адрес")]
        [StringLength(200)]
        public string? Address { get; set; }

        [Display(Name = "Город")]
        [StringLength(100)]
        public string? City { get; set; }

        [Display(Name = "Почтовый индекс")]
        [StringLength(20)]
        public string? PostalCode { get; set; }

        [Display(Name = "Страна")]
        [StringLength(100)]
        public string? Country { get; set; }

        [Display(Name = "Номер водительских прав")]
        [StringLength(20)]
        public string? DriverLicenseNumber { get; set; }

        [Display(Name = "Дата выдачи ВУ")]
        [DataType(DataType.Date)]
        public DateTime? DriverLicenseIssueDate { get; set; }


        [Display(Name = "Дата окончания ВУ")]
        [DataType(DataType.Date)]
        public DateTime? DriverLicenseExpiryDate { get; set; }

        [Display(Name = "Дата создания")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Display(Name = "Последний вход")]
        public DateTime? LastLoginAt { get; set; }

        [Display(Name = "Подтвержден")]
        public bool IsVerified { get; set; } = false;

        [Display(Name = "Активен")]
        public bool IsActive { get; set; } = true;

        // Навигационные свойства
        public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    }
}