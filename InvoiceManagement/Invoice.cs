using System;

namespace HotelManagementSystem.InvoiceManagement
{
    public class Invoice
    {
        public int InvoiceID { get; set; }
        public int BookingID { get; set; }
        public int CustomerID { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime IssueDate { get; set; }
        public string? PaymentStatus { get; set; } // "Paid" hoặc "Unpaid"
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int? UpdatedBy { get; set; }
        public string? UpdatedByUsername { get; set; }

        public Invoice() { }

        public Invoice(int invoiceId, int bookingId, int customerId, decimal totalAmount, DateTime issueDate, string paymentStatus, DateTime createdAt, DateTime updatedAt, int? updatedBy, string updatedByUsername)
        {
            InvoiceID = invoiceId;
            BookingID = bookingId;
            CustomerID = customerId;
            TotalAmount = totalAmount;
            IssueDate = issueDate;
            PaymentStatus = paymentStatus;
            CreatedAt = createdAt;
            UpdatedAt = updatedAt;
            UpdatedBy = updatedBy;
            UpdatedByUsername = updatedByUsername;
        }
    }
}
