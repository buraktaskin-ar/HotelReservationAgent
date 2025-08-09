

using HotelReservationAgentChatBot.Models;
namespace HotelReservationAgentChatBot.Data;


public class HotelDataSeeder
{
    public IEnumerable<Hotel> GetHotels()
    {
        yield return CreateGrandPlazaHotel();
        yield return CreateSeasideResort();
        yield return CreateMountainLodge();
        yield return CreateBusinessInnExpress();
        yield return CreateBoutiqueArtHotel();
        yield return CreateFamilyParadiseResort();
        yield return CreateEcoGreenHotel();
        yield return CreateAirportTransitHotel();
        yield return CreateLuxuryDesertOasis();
        yield return CreateHistoricDowntownInn();
    }

    private Hotel CreateGrandPlazaHotel()
    {
        return new Hotel
        {
            HotelId = "1",
            HotelName = "Grand Plaza Hotel",
            City = "New York",
            Country = "USA",
            Address = "123 5th Avenue, Manhattan, NY 10001",
            StarRating = 5,
            PricePerNight = 450.00,
            Description = "Luxury hotel in the heart of Manhattan with stunning city views. Perfect for business travelers and tourists. Near Central Park and Times Square.",
            Amenities = "24/7 concierge, Business center, Conference rooms, Rooftop restaurant, Bar/Lounge, Room service, Laundry service, Currency exchange, Luggage storage",
            RoomTypes = "Standard, Deluxe, Suite, Presidential Suite",
            CancellationPolicy = "Free cancellation up to 48 hours before check-in. 50% charge for cancellations within 48 hours. No refund for no-shows or cancellations within 24 hours.",
            CheckInCheckOut = "Check-in: 3:00 PM, Check-out: 11:00 AM. Early check-in and late check-out available upon request with additional charges.",
            HouseRules = "No smoking in rooms. Quiet hours 10 PM - 8 AM. Maximum 4 guests per room. ID required at check-in.",
            NearbyAttractions = "Central Park (0.5 miles), Times Square (0.8 miles), Empire State Building (1.2 miles), Broadway Theaters (0.6 miles)",
            HasPool = true,
            HasGym = true,
            HasSpa = true,
            PetFriendly = false,
            HasParking = true,
            HasWifi = true
        };
    }

    private Hotel CreateSeasideResort()
    {
        return new Hotel
        {
            HotelId = "2",
            HotelName = "Seaside Resort & Spa",
            City = "Miami",
            Country = "USA",
            Address = "456 Ocean Drive, Miami Beach, FL 33139",
            StarRating = 4,
            PricePerNight = 320.00,
            Description = "Beachfront resort with private beach access. Family-friendly with kids club and activities. Perfect for relaxation and water sports.",
            Amenities = "Private beach, Kids club, Water sports equipment, Beach bar, 3 restaurants, Poolside service, Babysitting service, Gift shop, ATM",
            RoomTypes = "Ocean View, Garden View, Family Suite, Honeymoon Suite",
            CancellationPolicy = "Free cancellation up to 72 hours before arrival. 30% charge for cancellations within 72 hours. Full charge for cancellations within 24 hours.",
            CheckInCheckOut = "Check-in: 4:00 PM, Check-out: 12:00 PM. Express check-out available.",
            HouseRules = "Children welcome. Pets allowed with deposit. No parties. Beach towels provided. Safety deposit boxes available.",
            NearbyAttractions = "South Beach (0 miles), Art Deco District (1 mile), Lincoln Road Mall (0.5 miles), Jungle Island (3 miles)",
            HasPool = true,
            HasGym = true,
            HasSpa = true,
            PetFriendly = true,
            HasParking = true,
            HasWifi = true
        };
    }

    private Hotel CreateMountainLodge()
    {
        return new Hotel
        {
            HotelId = "3",
            HotelName = "Mountain Lodge Retreat",
            City = "Aspen",
            Country = "USA",
            Address = "789 Mountain View Road, Aspen, CO 81611",
            StarRating = 4,
            PricePerNight = 280.00,
            Description = "Cozy mountain lodge with ski-in/ski-out access. Rustic charm with modern amenities. Fireplace in every room. Perfect for winter sports enthusiasts.",
            Amenities = "Ski storage, Ski equipment rental, Hot tub, Sauna, Fireplace lounge, Restaurant with local cuisine, Shuttle service to town, Hiking guides",
            RoomTypes = "Mountain View Room, Slope Side Suite, Cabin, Penthouse",
            CancellationPolicy = "Flexible cancellation up to 7 days before arrival with full refund. 50% refund for cancellations within 7 days. No refund within 48 hours.",
            CheckInCheckOut = "Check-in: 4:00 PM, Check-out: 11:00 AM. Ski valet service available.",
            HouseRules = "No smoking property. Ski equipment must be stored in designated areas. Quiet hours after 10 PM. Firewood provided.",
            NearbyAttractions = "Aspen Mountain Ski Area (0 miles), Maroon Bells (10 miles), Downtown Aspen (2 miles), Snowmass Village (8 miles)",
            HasPool = false,
            HasGym = true,
            HasSpa = false,
            PetFriendly = true,
            HasParking = true,
            HasWifi = true
        };
    }

    private Hotel CreateBusinessInnExpress()
    {
        return new Hotel
        {
            HotelId = "4",
            HotelName = "Business Inn Express",
            City = "Chicago",
            Country = "USA",
            Address = "321 Business Park Drive, Chicago, IL 60601",
            StarRating = 3,
            PricePerNight = 150.00,
            Description = "Affordable business hotel near airport and convention center. Free airport shuttle. Ideal for business travelers on a budget.",
            Amenities = "Free breakfast, Business center, Meeting rooms, Airport shuttle, Printing services, Express laundry, Convenience store, 24-hour front desk",
            RoomTypes = "Standard, Executive, Junior Suite",
            CancellationPolicy = "Free cancellation up to 24 hours before check-in. No refund for late cancellations or no-shows.",
            CheckInCheckOut = "Check-in: 2:00 PM, Check-out: 12:00 PM. 24-hour check-in available.",
            HouseRules = "No smoking. No pets. Valid credit card required. Government ID required.",
            NearbyAttractions = "O'Hare Airport (5 miles), McCormick Place Convention Center (3 miles), Downtown Chicago (8 miles)",
            HasPool = true,
            HasGym = true,
            HasSpa = false,
            PetFriendly = false,
            HasParking = true,
            HasWifi = true
        };
    }

    private Hotel CreateBoutiqueArtHotel()
    {
        return new Hotel
        {
            HotelId = "5",
            HotelName = "Boutique Art Hotel",
            City = "Paris",
            Country = "France",
            Address = "15 Rue de Rivoli, 75001 Paris",
            StarRating = 4,
            PricePerNight = 350.00,
            Description = "Stylish boutique hotel in central Paris featuring local art exhibitions. Walking distance to Louvre. Romantic atmosphere with personalized service.",
            Amenities = "Art gallery, Wine bar, Michelin-starred restaurant, Terrace cafe, Valet parking, Personal shopping service, Museum tickets booking, Bike rental",
            RoomTypes = "Classic, Superior, Deluxe, Artist Suite",
            CancellationPolicy = "Non-refundable rate available with 20% discount. Standard rate: free cancellation up to 48 hours. 50% charge within 48 hours.",
            CheckInCheckOut = "Check-in: 3:00 PM, Check-out: 12:00 PM. Luggage storage available.",
            HouseRules = "No smoking. Small pets welcome (under 10kg). Dress code for restaurant. Art pieces are for display only.",
            NearbyAttractions = "Louvre Museum (0.3 miles), Notre-Dame (0.5 miles), Eiffel Tower (2 miles), Champs-Élysées (1 mile)",
            HasPool = false,
            HasGym = false,
            HasSpa = true,
            PetFriendly = true,
            HasParking = true,
            HasWifi = true
        };
    }

    private Hotel CreateFamilyParadiseResort()
    {
        return new Hotel
        {
            HotelId = "6",
            HotelName = "Family Paradise Resort",
            City = "Orlando",
            Country = "USA",
            Address = "999 Theme Park Boulevard, Orlando, FL 32830",
            StarRating = 4,
            PricePerNight = 275.00,
            Description = "Ultimate family resort with water park and entertainment. Free shuttle to major theme parks. Kids eat free program. Spacious family suites.",
            Amenities = "Water park, Kids club, Teen lounge, Game arcade, Mini golf, 5 restaurants, Character breakfast, Babysitting, Laundry facilities, Gift shops",
            RoomTypes = "Standard, Family Suite, Kids Theme Suite, Villa",
            CancellationPolicy = "Free cancellation up to 5 days before arrival. 25% penalty within 5 days. 50% penalty within 48 hours. No refund day of arrival.",
            CheckInCheckOut = "Check-in: 4:00 PM, Check-out: 11:00 AM. Online check-in available.",
            HouseRules = "Wristbands required for water park. Children must be supervised. No outside food at pool. Maximum 6 guests per room.",
            NearbyAttractions = "Disney World (3 miles), Universal Studios (5 miles), SeaWorld (7 miles), Legoland (45 minutes)",
            HasPool = true,
            HasGym = true,
            HasSpa = false,
            PetFriendly = false,
            HasParking = true,
            HasWifi = true
        };
    }

    private Hotel CreateEcoGreenHotel()
    {
        return new Hotel
        {
            HotelId = "7",
            HotelName = "Eco Green Hotel",
            City = "Portland",
            Country = "USA",
            Address = "567 Sustainable Way, Portland, OR 97201",
            StarRating = 3,
            PricePerNight = 180.00,
            Description = "Environmentally friendly hotel with LEED certification. Organic restaurant. Solar powered. Perfect for eco-conscious travelers.",
            Amenities = "Organic restaurant, Electric car charging, Bike sharing program, Recycling program, Organic toiletries, Farmers market shuttle, Yoga studio",
            RoomTypes = "Eco Standard, Eco Deluxe, Green Suite",
            CancellationPolicy = "Flexible green rate: free cancellation up to 24 hours. 10% of cancellation fees go to environmental charities.",
            CheckInCheckOut = "Check-in: 3:00 PM, Check-out: 11:00 AM. Paperless check-in/out available.",
            HouseRules = "No smoking. Composting required. Reusable water bottles provided. Energy saving card key system.",
            NearbyAttractions = "Powell's Books (0.5 miles), Portland Art Museum (1 mile), Washington Park (2 miles), Food Truck Pods (0.3 miles)",
            HasPool = false,
            HasGym = true,
            HasSpa = false,
            PetFriendly = true,
            HasParking = true,
            HasWifi = true
        };
    }

    private Hotel CreateAirportTransitHotel()
    {
        return new Hotel
        {
            HotelId = "8",
            HotelName = "Airport Transit Hotel",
            City = "Atlanta",
            Country = "USA",
            Address = "100 Airport Terminal Road, Atlanta, GA 30320",
            StarRating = 3,
            PricePerNight = 120.00,
            Description = "Convenient airport hotel inside terminal. Perfect for layovers and early flights. Soundproof rooms. 24-hour operations.",
            Amenities = "24-hour restaurant, Business center, Day rooms available, Shower facilities, Flight information displays, Wake-up service, Express breakfast",
            RoomTypes = "Standard, Day Room (hourly), Overnight, Executive",
            CancellationPolicy = "Free cancellation up to 6 hours before check-in. No refund within 6 hours. Day rooms: no refund within 2 hours.",
            CheckInCheckOut = "Check-in: Any time, Check-out: Flexible. Day rooms available in 4-hour blocks.",
            HouseRules = "No smoking. Quiet zones enforced. Luggage must be with owner. Airport security rules apply.",
            NearbyAttractions = "Inside Hartsfield-Jackson Airport, Downtown Atlanta (10 miles), Georgia Aquarium (11 miles)",
            HasPool = false,
            HasGym = false,
            HasSpa = false,
            PetFriendly = false,
            HasParking = false,
            HasWifi = true
        };
    }

    private Hotel CreateLuxuryDesertOasis()
    {
        return new Hotel
        {
            HotelId = "9",
            HotelName = "Luxury Desert Oasis",
            City = "Scottsdale",
            Country = "USA",
            Address = "789 Desert Bloom Road, Scottsdale, AZ 85260",
            StarRating = 5,
            PricePerNight = 550.00,
            Description = "Exclusive desert resort with championship golf courses. World-class spa with indigenous treatments. Stunning sunset views.",
            Amenities = "3 Golf courses, Full-service spa, 6 pools, Tennis courts, 4 restaurants, Poolside cabanas, Helicopter tours, Personal butler service",
            RoomTypes = "Casita, Villa, Presidential Suite, Penthouse",
            CancellationPolicy = "Cancellation requires 14 days notice for full refund. 50% refund within 14 days. No refund within 72 hours. Holidays require 30 days notice.",
            CheckInCheckOut = "Check-in: 4:00 PM, Check-out: 12:00 PM. Private jet arrivals accommodated.",
            HouseRules = "Resort casual dress code. No children under 16 in spa. Golf course dress code enforced. Reservation required for restaurants.",
            NearbyAttractions = "Camelback Mountain (5 miles), Old Town Scottsdale (3 miles), Desert Botanical Garden (7 miles)",
            HasPool = true,
            HasGym = true,
            HasSpa = true,
            PetFriendly = false,
            HasParking = true,
            HasWifi = true
        };
    }

    private Hotel CreateHistoricDowntownInn()
    {
        return new Hotel
        {
            HotelId = "10",
            HotelName = "Historic Downtown Inn",
            City = "Boston",
            Country = "USA",
            Address = "42 Heritage Street, Boston, MA 02109",
            StarRating = 4,
            PricePerNight = 295.00,
            Description = "Restored 19th-century building in historic district. Walking distance to Freedom Trail. Combines historic charm with modern comfort.",
            Amenities = "Library lounge, Historic tours, Afternoon tea service, Antique furnishings, Modern business center, Curated art collection, Wine cellar",
            RoomTypes = "Classic, Heritage Suite, Governor's Suite",
            CancellationPolicy = "Standard rate: 48-hour cancellation policy. Advanced purchase rate: non-refundable with 15% discount.",
            CheckInCheckOut = "Check-in: 3:00 PM, Check-out: 11:00 AM. Complimentary luggage storage.",
            HouseRules = "No smoking in historic building. Respect historic artifacts. Quiet hours 10 PM - 7 AM. Professional photography requires permission.",
            NearbyAttractions = "Freedom Trail (0 miles), Boston Common (0.3 miles), Faneuil Hall (0.5 miles), North End (0.4 miles)",
            HasPool = false,
            HasGym = true,
            HasSpa = false,
            PetFriendly = true,
            HasParking = true,
            HasWifi = true
        };
    }
}
