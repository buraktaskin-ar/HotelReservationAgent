using Microsoft.SemanticKernel;
using System.ComponentModel;
using System.Text.Json;
using HotelReservationAgentChatBot.Services;

namespace HotelReservationAgentChatBot.Plugins;

public class PersonPlugin
{
    private readonly IPersonService _personService;
    private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true };

    public PersonPlugin(IPersonService personService)
    {
        _personService = personService;
    }

    [KernelFunction, Description("Create a new person/customer in the system. This should be done before making a reservation. Phone number is MANDATORY.")]
    public async Task<string> CreatePerson(
        [Description("The person's first name")] string firstName,
        [Description("The person's last name")] string lastName,
        [Description("The person's phone number (MANDATORY - cannot be empty)")] string phone,
        [Description("The person's email address (optional)")] string? email = null)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(phone))
                throw new ArgumentException("Phone number is required and cannot be empty.", nameof(phone));

            var person = await _personService.CreatePersonAsync(firstName, lastName, phone, email);

            return JsonSerializer.Serialize(new
            {
                success = true,
                message = "Müşteri başarıyla oluşturuldu!",
                person = new
                {
                    id = person.Id,
                    firstName = person.FirstName,
                    lastName = person.LastName,
                    fullName = $"{person.FirstName} {person.LastName}",
                    email = person.Email,
                    phone = person.Phone,
                    loyaltyPoints = person.LoyaltyPoints
                }
            }, JsonOptions);
        }
        catch (ArgumentException ex)
        {
            return JsonSerializer.Serialize(new
            {
                success = false,
                error = ex.Message,
                field = ex.ParamName,
                requiredInfo = "İsim, soyisim ve telefon numarası zorunludur."
            }, JsonOptions);
        }
        catch (InvalidOperationException ex)
        {
            return JsonSerializer.Serialize(new
            {
                success = false,
                error = ex.Message
            }, JsonOptions);
        }
        catch (Exception ex)
        {
            return JsonSerializer.Serialize(new
            {
                success = false,
                error = "Müşteri oluşturulurken beklenmeyen bir hata oluştu.",
                details = ex.Message
            }, JsonOptions);
        }
    }

    [KernelFunction, Description("Get information about a specific person by their ID")]
    public async Task<string> GetPersonInfo(
        [Description("The ID of the person to retrieve information for")] int personId)
    {
        try
        {
            var person = await _personService.GetPersonAsync(personId);

            if (person == null)
            {
                return JsonSerializer.Serialize(new
                {
                    success = false,
                    error = $"Person with ID {personId} not found.",
                    personId = personId
                }, JsonOptions);
            }

            return JsonSerializer.Serialize(new
            {
                success = true,
                message = "Person information retrieved successfully.",
                person = new
                {
                    id = person.Id,
                    firstName = person.FirstName,
                    lastName = person.LastName,
                    fullName = $"{person.FirstName} {person.LastName}",
                    email = person.Email,
                    phone = person.Phone,
                    loyaltyPoints = person.LoyaltyPoints
                }
            }, JsonOptions);
        }
        catch (Exception ex)
        {
            return JsonSerializer.Serialize(new
            {
                success = false,
                error = "An error occurred while retrieving person information.",
                details = ex.Message
            }, JsonOptions);
        }
    }

    [KernelFunction, Description("Get all persons/customers in the system")]
    public async Task<string> GetAllPersons()
    {
        try
        {
            var persons = await _personService.GetAllPersonsAsync();

            if (!persons.Any())
            {
                return JsonSerializer.Serialize(new
                {
                    message = "No persons found in the system.",
                    totalPersons = 0,
                    persons = new List<object>()
                }, JsonOptions);
            }

            var personsList = persons.Select(p => new
            {
                id = p.Id,
                firstName = p.FirstName,
                lastName = p.LastName,
                fullName = $"{p.FirstName} {p.LastName}",
                email = p.Email,
                phone = p.Phone,
                loyaltyPoints = p.LoyaltyPoints
            }).ToList();

            return JsonSerializer.Serialize(new
            {
                message = "All persons retrieved successfully.",
                totalPersons = persons.Count,
                persons = personsList
            }, JsonOptions);
        }
        catch (Exception ex)
        {
            return JsonSerializer.Serialize(new
            {
                error = "An error occurred while retrieving persons.",
                details = ex.Message
            }, JsonOptions);
        }
    }
}