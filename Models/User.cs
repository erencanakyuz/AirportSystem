using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.DataAnnotations;

namespace AirportDemo.Models
{
    public class User : IdentityUser<int> // IdentityUser zaten UserName i�eriyor
    {
        [DataType(DataType.DateTime)]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // Kullan�c� kay�t tarihi

        [DataType(DataType.DateTime)]
        public DateTime? LastLogin { get; set; } // Son giri� zaman� (Nullable)

        public string Role { get; set; } = "User"; // Kullan�c�n�n rol� (Default: User)

        public string? ProfilePictureUrl { get; set; } // Profil resmi linki (Opsiyonel)

        public bool IsActive { get; set; } = true; // Kullan�c� aktif mi? (Soft delete i�in)
    }
}
