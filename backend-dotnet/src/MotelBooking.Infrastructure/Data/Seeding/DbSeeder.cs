using Microsoft.EntityFrameworkCore;
using MotelBooking.Domain.Entities;
using MotelBooking.Domain.Enums;

namespace MotelBooking.Infrastructure.Data.Seeding;

public static class DbSeeder
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        // MigrateAsync applies all pending migrations and creates the database if it doesn't exist.
        await context.Database.MigrateAsync();

        if (!await context.RoomTypes.AnyAsync())
        {
            var roomTypes = new[]
            {
                new RoomType { Name = "Standard", Description = "Comfortable standard rooms with essential amenities" },
                new RoomType { Name = "Deluxe", Description = "Spacious rooms with premium furnishings and city views" },
                new RoomType { Name = "Suite", Description = "Luxurious suites with separate living areas and premium amenities" },
                new RoomType { Name = "Executive", Description = "Business-class rooms with work desk and premium bedding" },
                new RoomType { Name = "Family", Description = "Spacious rooms ideal for families with multiple beds" }
            };
            context.RoomTypes.AddRange(roomTypes);
            await context.SaveChangesAsync();
        }

        if (!await context.Amenities.AnyAsync())
        {
            var amenities = new[]
            {
                new Amenity { Name = "Free WiFi", Icon = "wifi", Category = "Technology" },
                new Amenity { Name = "Smart TV", Icon = "tv", Category = "Technology" },
                new Amenity { Name = "Air Conditioning", Icon = "ac_unit", Category = "Comfort" },
                new Amenity { Name = "Mini Bar", Icon = "local_bar", Category = "Food & Drink" },
                new Amenity { Name = "Coffee Maker", Icon = "coffee", Category = "Food & Drink" },
                new Amenity { Name = "Private Balcony", Icon = "balcony", Category = "Views" },
                new Amenity { Name = "City View", Icon = "location_city", Category = "Views" },
                new Amenity { Name = "King Bed", Icon = "king_bed", Category = "Sleeping" },
                new Amenity { Name = "Queen Bed", Icon = "bed", Category = "Sleeping" },
                new Amenity { Name = "Jacuzzi", Icon = "hot_tub", Category = "Bathroom" },
                new Amenity { Name = "Work Desk", Icon = "desk", Category = "Business" },
                new Amenity { Name = "Safe", Icon = "lock", Category = "Security" },
                new Amenity { Name = "Parking", Icon = "local_parking", Category = "Transport" },
                new Amenity { Name = "Room Service", Icon = "room_service", Category = "Services" }
            };
            context.Amenities.AddRange(amenities);
            await context.SaveChangesAsync();
        }

        if (!await context.Rooms.AnyAsync())
        {
            var standardTypeId = (await context.RoomTypes.FirstAsync(rt => rt.Name == "Standard")).Id;
            var deluxeTypeId = (await context.RoomTypes.FirstAsync(rt => rt.Name == "Deluxe")).Id;
            var suiteTypeId = (await context.RoomTypes.FirstAsync(rt => rt.Name == "Suite")).Id;
            var execTypeId = (await context.RoomTypes.FirstAsync(rt => rt.Name == "Executive")).Id;
            var familyTypeId = (await context.RoomTypes.FirstAsync(rt => rt.Name == "Family")).Id;

            var wifiId = (await context.Amenities.FirstAsync(a => a.Name == "Free WiFi")).Id;
            var tvId = (await context.Amenities.FirstAsync(a => a.Name == "Smart TV")).Id;
            var acId = (await context.Amenities.FirstAsync(a => a.Name == "Air Conditioning")).Id;
            var miniBarId = (await context.Amenities.FirstAsync(a => a.Name == "Mini Bar")).Id;
            var balconyId = (await context.Amenities.FirstAsync(a => a.Name == "Private Balcony")).Id;
            var cityViewId = (await context.Amenities.FirstAsync(a => a.Name == "City View")).Id;
            var kingBedId = (await context.Amenities.FirstAsync(a => a.Name == "King Bed")).Id;
            var queenBedId = (await context.Amenities.FirstAsync(a => a.Name == "Queen Bed")).Id;
            var jacuzziId = (await context.Amenities.FirstAsync(a => a.Name == "Jacuzzi")).Id;
            var deskId = (await context.Amenities.FirstAsync(a => a.Name == "Work Desk")).Id;
            var coffeeId = (await context.Amenities.FirstAsync(a => a.Name == "Coffee Maker")).Id;
            var roomServiceId = (await context.Amenities.FirstAsync(a => a.Name == "Room Service")).Id;

            var rooms = new[]
            {
                new Room
                {
                    Name = "Classic Standard Room", RoomNumber = "101", FloorNumber = 1,
                    Description = "A cozy, well-appointed room perfect for solo travelers or couples seeking comfortable accommodation at an unbeatable value.",
                    PricePerNight = 79.00m, MaxOccupancy = 2, SizeInSqFt = 280,
                    RoomTypeId = standardTypeId, IsFeatured = false,
                    ThumbnailUrl = "https://images.unsplash.com/photo-1631049307264-da0ec9d70304?w=800",
                    AverageRating = 4.2, TotalReviews = 48,
                    Images = new List<RoomImage>
                    {
                        new() { ImageUrl = "https://images.unsplash.com/photo-1631049307264-da0ec9d70304?w=1200", IsPrimary = true, SortOrder = 0 },
                        new() { ImageUrl = "https://images.unsplash.com/photo-1522771739844-6a9f6d5f14af?w=1200", IsPrimary = false, SortOrder = 1 }
                    },
                    RoomAmenities = new List<RoomAmenity>
                    {
                        new() { AmenityId = wifiId }, new() { AmenityId = tvId },
                        new() { AmenityId = acId }, new() { AmenityId = queenBedId }, new() { AmenityId = coffeeId }
                    }
                },
                new Room
                {
                    Name = "Deluxe King Room", RoomNumber = "201", FloorNumber = 2,
                    Description = "Indulge in elevated comfort with stunning city views, premium king-sized bedding, and a range of luxury amenities to make your stay unforgettable.",
                    PricePerNight = 129.00m, MaxOccupancy = 2, SizeInSqFt = 420,
                    RoomTypeId = deluxeTypeId, IsFeatured = true,
                    ThumbnailUrl = "https://images.unsplash.com/photo-1590490360182-c33d57733427?w=800",
                    AverageRating = 4.7, TotalReviews = 134,
                    Images = new List<RoomImage>
                    {
                        new() { ImageUrl = "https://images.unsplash.com/photo-1590490360182-c33d57733427?w=1200", IsPrimary = true, SortOrder = 0 },
                        new() { ImageUrl = "https://images.unsplash.com/photo-1571003123894-1f0594d2b5d9?w=1200", IsPrimary = false, SortOrder = 1 }
                    },
                    RoomAmenities = new List<RoomAmenity>
                    {
                        new() { AmenityId = wifiId }, new() { AmenityId = tvId }, new() { AmenityId = acId },
                        new() { AmenityId = kingBedId }, new() { AmenityId = cityViewId },
                        new() { AmenityId = miniBarId }, new() { AmenityId = coffeeId }, new() { AmenityId = roomServiceId }
                    }
                },
                new Room
                {
                    Name = "Executive Suite", RoomNumber = "301", FloorNumber = 3,
                    Description = "Our flagship Executive Suite blends business functionality with luxury comfort. Features a separate living area, private balcony, jacuzzi, and panoramic city views.",
                    PricePerNight = 229.00m, MaxOccupancy = 3, SizeInSqFt = 680,
                    RoomTypeId = suiteTypeId, IsFeatured = true,
                    ThumbnailUrl = "https://images.unsplash.com/photo-1582719478250-c89cae4dc85b?w=800",
                    AverageRating = 4.9, TotalReviews = 89,
                    Images = new List<RoomImage>
                    {
                        new() { ImageUrl = "https://images.unsplash.com/photo-1582719478250-c89cae4dc85b?w=1200", IsPrimary = true, SortOrder = 0 },
                        new() { ImageUrl = "https://images.unsplash.com/photo-1560185127-6ed189bf02f4?w=1200", IsPrimary = false, SortOrder = 1 }
                    },
                    RoomAmenities = new List<RoomAmenity>
                    {
                        new() { AmenityId = wifiId }, new() { AmenityId = tvId }, new() { AmenityId = acId },
                        new() { AmenityId = kingBedId }, new() { AmenityId = balconyId }, new() { AmenityId = cityViewId },
                        new() { AmenityId = miniBarId }, new() { AmenityId = jacuzziId },
                        new() { AmenityId = deskId }, new() { AmenityId = roomServiceId }
                    }
                },
                new Room
                {
                    Name = "Business Executive Room", RoomNumber = "401", FloorNumber = 4,
                    Description = "Designed for the discerning business traveler. High-speed internet, an ergonomic work desk, and premium amenities ensure maximum productivity and comfort.",
                    PricePerNight = 159.00m, MaxOccupancy = 2, SizeInSqFt = 480,
                    RoomTypeId = execTypeId, IsFeatured = true,
                    ThumbnailUrl = "https://images.unsplash.com/photo-1611892440504-42a792e24d32?w=800",
                    AverageRating = 4.6, TotalReviews = 72,
                    Images = new List<RoomImage>
                    {
                        new() { ImageUrl = "https://images.unsplash.com/photo-1611892440504-42a792e24d32?w=1200", IsPrimary = true, SortOrder = 0 }
                    },
                    RoomAmenities = new List<RoomAmenity>
                    {
                        new() { AmenityId = wifiId }, new() { AmenityId = tvId }, new() { AmenityId = acId },
                        new() { AmenityId = kingBedId }, new() { AmenityId = deskId },
                        new() { AmenityId = coffeeId }, new() { AmenityId = roomServiceId }
                    }
                },
                new Room
                {
                    Name = "Family Suite", RoomNumber = "501", FloorNumber = 5,
                    Description = "Spacious and welcoming, our Family Suite features two bedrooms, a shared living space, and everything your family needs for a memorable stay.",
                    PricePerNight = 189.00m, MaxOccupancy = 5, SizeInSqFt = 720,
                    RoomTypeId = familyTypeId, IsFeatured = false,
                    ThumbnailUrl = "https://images.unsplash.com/photo-1566665797739-1674de7a421a?w=800",
                    AverageRating = 4.5, TotalReviews = 56,
                    Images = new List<RoomImage>
                    {
                        new() { ImageUrl = "https://images.unsplash.com/photo-1566665797739-1674de7a421a?w=1200", IsPrimary = true, SortOrder = 0 }
                    },
                    RoomAmenities = new List<RoomAmenity>
                    {
                        new() { AmenityId = wifiId }, new() { AmenityId = tvId }, new() { AmenityId = acId },
                        new() { AmenityId = coffeeId }, new() { AmenityId = roomServiceId }
                    }
                },
                new Room
                {
                    Name = "Luxury Penthouse Suite", RoomNumber = "601", FloorNumber = 6,
                    Description = "The pinnacle of luxury. Our Penthouse Suite offers breathtaking 360° views, a private rooftop terrace, full butler service, and world-class interior design.",
                    PricePerNight = 499.00m, MaxOccupancy = 4, SizeInSqFt = 1200,
                    RoomTypeId = suiteTypeId, IsFeatured = true,
                    ThumbnailUrl = "https://images.unsplash.com/photo-1613490493576-7fde63acd811?w=800",
                    AverageRating = 5.0, TotalReviews = 23,
                    Images = new List<RoomImage>
                    {
                        new() { ImageUrl = "https://images.unsplash.com/photo-1613490493576-7fde63acd811?w=1200", IsPrimary = true, SortOrder = 0 },
                        new() { ImageUrl = "https://images.unsplash.com/photo-1519974719765-e6559eac2575?w=1200", IsPrimary = false, SortOrder = 1 }
                    },
                    RoomAmenities = new List<RoomAmenity>
                    {
                        new() { AmenityId = wifiId }, new() { AmenityId = tvId }, new() { AmenityId = acId },
                        new() { AmenityId = kingBedId }, new() { AmenityId = balconyId }, new() { AmenityId = cityViewId },
                        new() { AmenityId = miniBarId }, new() { AmenityId = jacuzziId },
                        new() { AmenityId = deskId }, new() { AmenityId = coffeeId }, new() { AmenityId = roomServiceId }
                    }
                }
            };

            context.Rooms.AddRange(rooms);
            await context.SaveChangesAsync();
        }

        if (!await context.Users.AnyAsync(u => u.Role == "Admin"))
        {
            var adminPasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123!", workFactor: 12);
            context.Users.Add(new User
            {
                FullName = "System Administrator",
                Email = "admin@motelbooking.com",
                PasswordHash = adminPasswordHash,
                Role = "Admin",
                IsEmailVerified = true
            });
            await context.SaveChangesAsync();
        }

        // Seed 3,000 branches across the USA
        await LocationSeeder.SeedAsync(context);
    }
}
