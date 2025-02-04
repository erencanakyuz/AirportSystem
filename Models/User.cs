using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.DataAnnotations;

namespace AirportDemo.Models
{
    public class User : IdentityUser<int> // IdentityUser zaten UserName içeriyor
    {
        [DataType(DataType.DateTime)]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // Kullanýcý kayýt tarihi

        [DataType(DataType.DateTime)]
        public DateTime? LastLogin { get; set; } // Son giriþ zamaný (Nullable)

        public string Role { get; set; } = "User"; // Kullanýcýnýn rolü (Default: User)

        public string? ProfilePictureUrl { get; set; } // Profil resmi linki (Opsiyonel)

        public bool IsActive { get; set; } = true; // Kullanýcý aktif mi? (Soft delete için)
    }
}
