using System;

namespace HotelManagementSystem
{
    public class Customer
    {
        public int CustomerID { get; set; }
        public string Name { get; set; }
        public string IDCard { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Nationality { get; set; }

        public Customer(int customerId, string name, string idCard, string phone, string email, string nationality)
        {
            if (customerId <= 0 || !customerId.ToString().All(char.IsDigit))
            {
                Console.WriteLine("Customer ID must be a positive integer consisting only of digits.");
                return;
            }
            if (string.IsNullOrWhiteSpace(name) || !name.Any(char.IsLetter) || !name.Any(c => c == ' ' || char.IsLetter(c)))
            {
                Console.WriteLine("Name must contain letters with diacritics and may include spaces.");
                return;
            }
            if (string.IsNullOrWhiteSpace(idCard) || idCard.Length != 12 || !idCard.All(char.IsDigit))
            {
                Console.WriteLine("IDCard must be exactly 12 digits.");
                return;
            }
            if (string.IsNullOrWhiteSpace(phone) || phone.Length != 10 || !phone.All(char.IsDigit))
            {
                Console.WriteLine("Phone number must be exactly 10 digits.");
                return;
            }
            if (string.IsNullOrWhiteSpace(nationality) || !nationality.Any(char.IsLetter) || !nationality.Any(c => c == ' ' || char.IsLetter(c)))
            {
                Console.WriteLine("Nationality must contain letters with diacritics and may include spaces.");
                return;
            }

            CustomerID = customerId;
            Name = name.Trim();
            IDCard = idCard.Trim();
            Phone = phone.Trim();
            Email = string.IsNullOrWhiteSpace(email) ? null : email.Trim();
            Nationality = nationality.Trim();
        }

        public override string ToString()
        {
            return $"CustomerID: {CustomerID}, Name: {Name}, IDCard: {IDCard}, Phone: {Phone}, Email: {Email ?? "N/A"}, Nationality: {Nationality}";
        }
    }
}
