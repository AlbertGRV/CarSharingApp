using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace CarSharingApp.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        [Display(Name = "Имя")]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Фамилия")]
        public string LastName { get; set; } = string.Empty;

        [Display(Name = "Дата рождения")]
        [DataType(DataType.Date)]
        public DateTime? DateOfBirth { get; set; }

        [Display(Name = "Номер водительского удостоверения")]
        public string? DriverLicenseNumber { get; set; }

        [Display(Name = "Дата регистрации")]
        public DateTime RegistrationDate { get; set; } = DateTime.Now;

        [Display(Name = "Статус")]
        public UserStatus Status { get; set; } = UserStatus.Active;

        // Navigation properties
        public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    }

    public enum UserStatus
    {
        Active,
        Suspended,
        Blocked
    }
}