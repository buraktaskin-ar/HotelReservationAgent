using HotelReservationAgentChatBot.Models;

namespace HotelReservationAgentChatBot.Data;

public class PersonDataSeeder
{
    public List<Person> GetPersons()
    {
        return new List<Person>
        {
            new Person
            {
                Id = 1,
                FirstName = "Ahmet",
                LastName = "Yılmaz",
                Email = "ahmet.yilmaz@email.com",
                Phone = "+90 532 123 4567",
                LoyaltyPoints = 1250
            },
            new Person
            {
                Id = 2,
                FirstName = "Fatma",
                LastName = "Kaya",
                Email = "fatma.kaya@email.com",
                Phone = "+90 533 234 5678",
                LoyaltyPoints = 850
            },
            new Person
            {
                Id = 3,
                FirstName = "Mehmet",
                LastName = "Demir",
                Email = "mehmet.demir@email.com",
                Phone = "+90 534 345 6789",
                LoyaltyPoints = 2100
            },
            new Person
            {
                Id = 4,
                FirstName = "Ayşe",
                LastName = "Öz",
                Email = "ayse.oz@email.com",
                Phone = "+90 535 456 7890",
                LoyaltyPoints = 0
            },
            new Person
            {
                Id = 5,
                FirstName = "Can",
                LastName = "Şen",
                Email = "can.sen@email.com",
                Phone = "+90 536 567 8901",
                LoyaltyPoints = 3200
            },
            new Person
            {
                Id = 6,
                FirstName = "Emily",
                LastName = "Johnson",
                Email = "emily.johnson@email.com",
                Phone = "+1 555 123 4567",
                LoyaltyPoints = 1800
            },
            new Person
            {
                Id = 7,
                FirstName = "Marco",
                LastName = "Rossi",
                Email = "marco.rossi@email.com",
                Phone = "+39 333 123 4567",
                LoyaltyPoints = 950
            },
            new Person
            {
                Id = 8,
                FirstName = "Sophie",
                LastName = "Mueller",
                Email = "sophie.mueller@email.com",
                Phone = "+49 174 123 4567",
                LoyaltyPoints = 2750
            },
            new Person
            {
                Id = 9,
                FirstName = "Ali",
                LastName = "Çelik",
                Email = "ali.celik@email.com",
                Phone = "+90 537 678 9012",
                LoyaltyPoints = 1450
            },
            new Person
            {
                Id = 10,
                FirstName = "Zeynep",
                LastName = "Aydın",
                Email = "zeynep.aydin@email.com",
                Phone = "+90 538 789 0123",
                LoyaltyPoints = 2850
            },
            new Person
            {
                Id = 11,
                FirstName = "James",
                LastName = "Wilson",
                Email = "james.wilson@email.com",
                Phone = "+44 20 7123 4567",
                LoyaltyPoints = 3100
            },
            new Person
            {
                Id = 12,
                FirstName = "Maria",
                LastName = "Garcia",
                Email = "maria.garcia@email.com",
                Phone = "+34 91 123 4567",
                LoyaltyPoints = 1650
            }
        };
    }
}