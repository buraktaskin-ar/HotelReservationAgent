

using HotelReservationAgentChatBot.Models;
namespace HotelReservationAgentChatBot.Data;


public class HotelDataSeeder
{
    public IEnumerable<Hotel> GetHotels()
    {
        yield return CreateGrandPlazaHotel();
        yield return CreateSeasideResort();
        yield return CreateMountainLodge();
       // yield return CreateBusinessInnExpress();
        yield return CreateBoutiqueArtHotel();
        yield return CreateFamilyParadiseResort();
        yield return CreateEcoGreenHotel();
        //yield return CreateAirportTransitHotel();
        //yield return CreateLuxuryDesertOasis();
        //yield return CreateHistoricDowntownInn();
        yield return CreateFamilyOtel();
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

    //private Hotel CreateBusinessInnExpress()
    //{
    //    return new Hotel
    //    {
    //        HotelId = "4",
    //        HotelName = "Antalya Nightlife Resort & Casino",
    //        City = "Antalya",
    //        Country = "Turkey",
    //        Address = "Konyaaltı Beach Boulevard, Hurma Mahallesi, 07070 Antalya, Turkey",
    //        StarRating = 5,
    //        PricePerNight = 280.00,
    //        Description = "StarRating: 4. Exclusive adults-only resort designed for sophisticated travelers seeking vibrant nightlife and luxury experiences. Premium beachfront location with world-class entertainment and casino facilities.",
    //        Amenities = "Rooftop nightclub with international DJs and panoramic Mediterranean views, Casino with poker tables, slot machines, blackjack and roulette, Premium all-inclusive bars featuring craft cocktails and premium spirits, Beach club with VIP cabanas and day-bed service, Infinity pool with swim-up bar and underwater music system, Adults-only spa with couples massage suites and Turkish hammam, Fine dining restaurants with celebrity chef concepts, Wine cellar with sommelier-guided tastings, Cigar lounge with premium tobacco selection, Private yacht charter services, Water sports including jet skiing and parasailing, Tennis court with professional coaching, Fitness center with personal training services, 24-hour concierge and room service, Valet parking and luxury car rental, Shopping boutique with designer brands, Beauty salon and barber services, Business center with meeting facilities for corporate events",
    //        RoomTypes = "Penthouse Suite, VIP Ocean View Suite, Premium Balcony Room, Deluxe Adult Room",
    //        CancellationPolicy = "Premium flexibility for discerning guests: Complimentary cancellation up to 7 days before arrival for all suite bookings and 48 hours for standard rooms. Cancellations made between 7-3 days prior to check-in for suites incur 30% of total booking value. Standard room cancellations between 48-24 hours incur 50% penalty of first night rate. All cancellations less than 24 hours or no-shows are charged 100% of booking value. VIP members enjoy extended cancellation periods and reduced penalties. Special packages including casino credits and nightclub reservations have separate cancellation terms. Travel insurance is highly recommended for international guests. Corporate and group bookings have customized cancellation policies based on event size and booking value.",
    //        CheckInCheckOut = "Check-in: 3:00 PM, Check-out: 1:00 PM. VIP check-in available. Late check-out until 6:00 PM for suite guests.",
    //        HouseRules = "Adults-only policy strictly enforced (minimum age 21 years): Valid government-issued photo identification required for all guests at check-in and verification of age for access to gaming and alcohol services. Smart casual dress code required in all restaurants and bars after 6:00 PM - no swimwear, flip-flops, or athletic wear permitted in dining venues. Formal attire required in casino and nightclub areas - jackets recommended for men, elegant attire for ladies. Smoking permitted only in designated outdoor areas and cigar lounge - all indoor areas are non-smoking including guest rooms and balconies. Responsible gaming and drinking policies strictly enforced - management reserves right to refuse service or ask guests to leave premises. Noise levels must be respectful of other guests, especially between midnight and 8:00 AM in accommodation areas. Maximum occupancy limits enforced in all room categories. Credit card authorization required for incidental charges including casino credits, spa services, and premium dining experiences.",
    //        NearbyAttractions = "Konyaaltı Beach (direct access) - pristine Mediterranean coastline with beach clubs, water sports, and sunset cocktail venues perfect for romantic evenings, Old Town Antalya (Kaleiçi) (15 minutes) - historic quarter featuring rooftop bars, boutique nightclubs, traditional Turkish restaurants, and atmospheric cobblestone streets ideal for evening strolls, Antalya Marina (20 minutes) - upscale waterfront district with luxury yacht charters, sophisticated dining establishments, designer shopping, and vibrant nightlife scene, Club Inferno Mega Disco (10 minutes) - one of Turkey's largest nightclubs featuring international DJs, VIP bottle service, and themed party nights, Olympos Cable Car (45 minutes) - scenic mountain railway offering breathtaking sunset views and romantic dinner experiences at 2,365 meters altitude, Düden Waterfalls Upper Falls (30 minutes) - spectacular natural attraction with evening illumination and nearby wine bars for romantic sunset viewing, Antalya Archaeological Museum (25 minutes) - world-class collection of ancient artifacts with evening cultural events and wine receptions, Phaselis Ancient City (60 minutes) - romantic beachside ruins perfect for day trips combined with exclusive beach club experiences and private yacht access",
    //        HasPool = true,
    //        HasGym = true,
    //        HasSpa = true,
    //        PetFriendly = false,
    //        HasParking = true,
    //        HasWifi = true
    //    };
    //}

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

    //private Hotel CreateAirportTransitHotel()
    //{
    //    return new Hotel
    //    {
    //        HotelId = "8",
    //        HotelName = "Airport Transit Hotel",
    //        City = "Atlanta",
    //        Country = "USA",
    //        Address = "100 Airport Terminal Road, Atlanta, GA 30320",
    //        StarRating = 3,
    //        PricePerNight = 120.00,
    //        Description = "Convenient airport hotel inside terminal. Perfect for layovers and early flights. Soundproof rooms. 24-hour operations.",
    //        Amenities = "24-hour restaurant, Business center, Day rooms available, Shower facilities, Flight information displays, Wake-up service, Express breakfast",
    //        RoomTypes = "Standard, Day Room (hourly), Overnight, Executive",
    //        CancellationPolicy = "Free cancellation up to 6 hours before check-in. No refund within 6 hours. Day rooms: no refund within 2 hours.",
    //        CheckInCheckOut = "Check-in: Any time, Check-out: Flexible. Day rooms available in 4-hour blocks.",
    //        HouseRules = "No smoking. Quiet zones enforced. Luggage must be with owner. Airport security rules apply.",
    //        NearbyAttractions = "Inside Hartsfield-Jackson Airport, Downtown Atlanta (10 miles), Georgia Aquarium (11 miles)",
    //        HasPool = false,
    //        HasGym = false,
    //        HasSpa = false,
    //        PetFriendly = false,
    //        HasParking = false,
    //        HasWifi = true
    //    };
    //}

    //private Hotel CreateLuxuryDesertOasis()
    //{
    //    return new Hotel
    //    {
    //        HotelId = "9",
    //        HotelName = "Luxury Desert Oasis",
    //        City = "Scottsdale",
    //        Country = "USA",
    //        Address = "789 Desert Bloom Road, Scottsdale, AZ 85260",
    //        StarRating = 5,
    //        PricePerNight = 550.00,
    //        Description = "Exclusive desert resort with championship golf courses. World-class spa with indigenous treatments. Stunning sunset views.",
    //        Amenities = "3 Golf courses, Full-service spa, 6 pools, Tennis courts, 4 restaurants, Poolside cabanas, Helicopter tours, Personal butler service",
    //        RoomTypes = "Casita, Villa, Presidential Suite, Penthouse",
    //        CancellationPolicy = "Cancellation requires 14 days notice for full refund. 50% refund within 14 days. No refund within 72 hours. Holidays require 30 days notice.",
    //        CheckInCheckOut = "Check-in: 4:00 PM, Check-out: 12:00 PM. Private jet arrivals accommodated.",
    //        HouseRules = "Resort casual dress code. No children under 16 in spa. Golf course dress code enforced. Reservation required for restaurants.",
    //        NearbyAttractions = "Camelback Mountain (5 miles), Old Town Scottsdale (3 miles), Desert Botanical Garden (7 miles)",
    //        HasPool = true,
    //        HasGym = true,
    //        HasSpa = true,
    //        PetFriendly = false,
    //        HasParking = true,
    //        HasWifi = true
    //    };
    //}

    //private Hotel CreateHistoricDowntownInn()
    //{
    //    return new Hotel
    //    {
    //        HotelId = "10",
    //        HotelName = "Historic Downtown Inn",
    //        City = "Boston",
    //        Country = "USA",
    //        Address = "42 Heritage Street, Boston, MA 02109",
    //        StarRating = 4,
    //        PricePerNight = 295.00,
    //        Description = "Restored 19th-century building in historic district. Walking distance to Freedom Trail. Combines historic charm with modern comfort.",
    //        Amenities = "Library lounge, Historic tours, Afternoon tea service, Antique furnishings, Modern business center, Curated art collection, Wine cellar",
    //        RoomTypes = "Classic, Heritage Suite, Governor's Suite",
    //        CancellationPolicy = "Standard rate: 48-hour cancellation policy. Advanced purchase rate: non-refundable with 15% discount.",
    //        CheckInCheckOut = "Check-in: 3:00 PM, Check-out: 11:00 AM. Complimentary luggage storage.",
    //        HouseRules = "No smoking in historic building. Respect historic artifacts. Quiet hours 10 PM - 7 AM. Professional photography requires permission.",
    //        NearbyAttractions = "Freedom Trail (0 miles), Boston Common (0.3 miles), Faneuil Hall (0.5 miles), North End (0.4 miles)",
    //        HasPool = false,
    //        HasGym = true,
    //        HasSpa = false,
    //        PetFriendly = true,
    //        HasParking = true,
    //        HasWifi = true
    //    };
    //}

    private Hotel CreateFamilyOtel()
    {
        return new Hotel
        {
            HotelId = "11",
            HotelName = "Antalya Kids Paradise Family Resort",
            City = "Antalya",
            Country = "Turkey",
            Address = "Belek Tourism Center, Serik District, 07506 Antalya, Turkey",
            StarRating = 4,
            PricePerNight = 220.00,
            Description = "12/12/2025 ve 15/12/2025 arası musait 404 numaralı oda.  StarRating: 4.  Ultimate family destination specifically designed for children's happiness and family bonding. Every corner of our resort celebrates childhood with magical experiences and unforgettable memories for the whole family.",
            Amenities = "Massive children's water park with 15 different slides including toddler-safe mini slides and thrilling tube slides for older kids, Dedicated baby pool with temperature control and soft play features for infants aged 6-24 months, Kids club with age-specific programs: Mini Club (4-7 years), Junior Club (8-12 years), and Teen Zone (13-17 years) with qualified childcare professionals, Interactive playground village with climbing frames, zip lines, trampolines, and adventure courses suitable for all ages, Indoor soft play center with ball pits, foam blocks, and sensory play areas for rainy days, Children's cinema showing latest family movies with comfortable bean bag seating and popcorn service, Arts and crafts workshop with pottery classes, painting sessions, jewelry making, and seasonal holiday crafts, Educational mini farm with friendly animals including rabbits, goats, chickens, and ponies for petting and feeding experiences, Professional animation team organizing treasure hunts, magic shows, puppet theaters, face painting, and themed costume parties, Family entertainment including circus performances, acrobatic shows, and interactive musical concerts, Shallow lagoon-style family pool with water fountains, mushroom showers, and gentle water features, Children's buffet restaurant with kid-friendly international cuisine, healthy options, and fun presentation, Baby care center with high chairs, bottle warmers, baby food preparation area, changing stations, and sterilization facilities, Complimentary stroller and baby equipment rental including cribs, car seats, and baby monitors",
            RoomTypes = "Family Connecting Rooms, Kids Themed Suites, Baby-Safe Family Rooms, Grand Family Suites",
            CancellationPolicy = "Family-first flexible cancellation designed for unexpected changes in family plans: Completely free cancellation up to 14 days before arrival for all family bookings regardless of room type or season. Cancellations made between 14-7 days prior to check-in incur only 20% penalty of total booking value, understanding that family schedules can change unexpectedly. Cancellations between 7-3 days before arrival incur 40% penalty of total reservation cost. Last-minute cancellations within 72 hours or no-shows are charged 75% of booking value rather than full amount. Special emergency provisions for child illness, family medical emergencies, or school-related conflicts with doctor's note or official documentation result in full refund regardless of timing. Travel insurance partnerships available at discounted rates for families. Group family bookings for reunions, celebrations, or multiple families traveling together have extended cancellation periods and reduced penalty structures. Seasonal promotions and package deals may have modified cancellation terms clearly stated at time of booking.",
            CheckInCheckOut = "Family-friendly timing: Check-in starts at 1:00 PM to accommodate family travel schedules, Check-out extended until 1:00 PM to allow relaxed morning routines with children.",
            HouseRules = "Child-centered environment with comprehensive family safety policies: Children under 16 years of age stay completely free when sharing parents' room with existing bedding arrangements. All pool and water park areas have certified lifeguards on duty during operating hours from 8:00 AM to sunset with additional safety equipment and first aid stations strategically located. Parents and guardians are required to supervise children under 8 years at all times in water areas and playground facilities for safety reasons. Children's wristbands provided at check-in with parent contact information and room numbers for quick identification and safety. Quiet hours respectfully observed between 9:00 PM and 8:00 AM in accommodation areas to ensure all families can maintain healthy sleep schedules for children. Baby-proofing services available upon request including outlet covers, cabinet locks, and corner guards for families with toddlers. No smoking policy throughout entire resort property including all outdoor areas to maintain healthy environment for children and families. Valid identification required for all adults at check-in along with emergency contact information for each child. Resort reserves right to organize rooms to ensure families with similar-aged children are grouped together when possible for enhanced social experiences. All staff members undergo child safety training and background checks to ensure secure environment for young guests.",
            NearbyAttractions = "Belek Public Beach (5 minutes walk) - safe shallow sandy beach with family lifeguard services, beach toys rental, and children's sand castle building competitions organized daily, The Land of Legends Theme Park (15 minutes) - Turkey's largest theme park featuring roller coasters for all age groups, dolphin shows, aquarium experiences, shopping village, and dedicated areas for toddlers with gentle rides, Antalya Aquarium (35 minutes) - world's largest tunnel aquarium with interactive touch pools, educational programs for children, feeding demonstrations, and 4D cinema experiences featuring ocean adventures, Köprülü Canyon National Park (45 minutes) - family rafting adventures with specially designed safe routes for children over 6, nature walks, picnic areas, and wildlife spotting opportunities perfect for active families, Fire of Anatolia Cultural Show (40 minutes) - spectacular family-friendly cultural performance featuring traditional Turkish dances, costumes, and music suitable for children over 5 years, Düden Waterfalls (30 minutes) - natural wonder with easy walking paths suitable for strollers, picnic areas, playground facilities, and boat trips to see waterfalls from sea perfect for family photos, Antalya Mini City (25 minutes) - miniature replica of Turkey's famous landmarks where children can learn history while having fun, with interactive exhibits and educational treasure hunts, Sandland Sand Sculpture Festival (20 minutes) - incredible sand art exhibition that amazes children and adults alike, with workshops where kids can try creating their own sand sculptures",
            HasPool = true,
            HasGym = false,
            HasSpa = false,
            PetFriendly = true,
            HasParking = true,
            HasWifi = true
        };
    }


}
