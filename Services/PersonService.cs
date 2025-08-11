using HotelReservationAgentChatBot.Models;

namespace HotelReservationAgentChatBot.Services;

public interface IPersonService
{
    Task<Person> CreatePersonAsync(string firstName, string lastName, string? email = null, string? phone = null);
    Task<Person?> GetPersonAsync(int personId);
    Task<List<Person>> GetAllPersonsAsync();
}

public class PersonService : IPersonService
{
    private readonly List<Person> _people;
    private readonly IReservationService _reservationService;
    private int _nextPersonId = 1;

    public PersonService(IReservationService reservationService)
    {
        _people = new List<Person>();
        _reservationService = reservationService;
    }

    public async Task<Person> CreatePersonAsync(string firstName, string lastName, string? email = null, string? phone = null)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            throw new ArgumentException("First name is required.", nameof(firstName));

        if (string.IsNullOrWhiteSpace(lastName))
            throw new ArgumentException("Last name is required.", nameof(lastName));

        // Email benzersizlik kontrolü
        if (!string.IsNullOrWhiteSpace(email) && _people.Any(p => p.Email?.ToLower() == email.ToLower()))
        {
            throw new InvalidOperationException($"A person with email '{email}' already exists.");
        }

        var person = new Person
        {
            Id = _nextPersonId++,
            FirstName = firstName.Trim(),
            LastName = lastName.Trim(),
            Email = email?.Trim(),
            Phone = phone?.Trim(),
            LoyaltyPoints = 0 // Yeni üyeler 0 puan ile başlar
        };

        _people.Add(person);

        // ReservationService'e de ekle (dependency)
        if (_reservationService is ReservationService reservationService)
        {
            reservationService.AddPerson(person);
        }

        return await Task.FromResult(person);
    }

    public async Task<Person?> GetPersonAsync(int personId)
    {
        var person = _people.FirstOrDefault(p => p.Id == personId);
        return await Task.FromResult(person);
    }

    public async Task<List<Person>> GetAllPersonsAsync()
    {
        return await Task.FromResult(_people.ToList());
    }
}