using System;

namespace HotelManagementSystem
{
    public class Service
    {
        public int ServiceID { get; set; }
        public string ServiceName { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty; // Food, Laundry, Spa, Other
        public decimal Price { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int? UpdatedBy { get; set; }
        public string? UpdatedByUsername { get; set; }
    }
}
