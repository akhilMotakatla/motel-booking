using Microsoft.EntityFrameworkCore;
using MotelBooking.Domain.Entities;

namespace MotelBooking.Infrastructure.Data.Seeding;

/// <summary>
/// Seeds ~3,000 Starry Nights Motel branches across the United States.
/// </summary>
public static class LocationSeeder
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        if (await context.Locations.AnyAsync()) return;

        var locations = new List<Location>();

        // State → Cities → BranchCount
        // Total targets ~3000 branches
        var data = new (string State, string Code, (string City, int Count)[] Cities)[]
        {
            ("Texas", "TX", new[] {
                ("Houston", 28), ("Dallas", 26), ("San Antonio", 22), ("Austin", 20),
                ("Fort Worth", 16), ("El Paso", 12), ("Arlington", 10), ("Corpus Christi", 9),
                ("Plano", 8), ("Frisco", 7), ("Irving", 7), ("Laredo", 6),
                ("Lubbock", 6), ("Garland", 5), ("Amarillo", 5)
            }),
            ("California", "CA", new[] {
                ("Los Angeles", 30), ("San Francisco", 24), ("San Diego", 22), ("San Jose", 16),
                ("Sacramento", 14), ("Fresno", 10), ("Long Beach", 9), ("Oakland", 9),
                ("Bakersfield", 7), ("Anaheim", 7), ("Santa Ana", 6), ("Riverside", 6),
                ("Irvine", 5), ("Stockton", 5)
            }),
            ("Florida", "FL", new[] {
                ("Miami", 26), ("Orlando", 22), ("Tampa", 18), ("Jacksonville", 16),
                ("Fort Lauderdale", 12), ("St. Petersburg", 10), ("Tallahassee", 8),
                ("Hialeah", 7), ("Cape Coral", 6), ("Boca Raton", 6), ("Naples", 5)
            }),
            ("New York", "NY", new[] {
                ("New York City", 30), ("Buffalo", 12), ("Rochester", 10), ("Yonkers", 8),
                ("Syracuse", 7), ("Albany", 7), ("New Rochelle", 5), ("Mount Vernon", 5),
                ("Schenectady", 4), ("Utica", 4)
            }),
            ("Illinois", "IL", new[] {
                ("Chicago", 28), ("Aurora", 10), ("Naperville", 9), ("Rockford", 8),
                ("Elgin", 7), ("Springfield", 7), ("Joliet", 6), ("Peoria", 6)
            }),
            ("Pennsylvania", "PA", new[] {
                ("Philadelphia", 22), ("Pittsburgh", 18), ("Allentown", 10), ("Erie", 8),
                ("Reading", 7), ("Scranton", 6), ("Bethlehem", 6), ("Lancaster", 5)
            }),
            ("Ohio", "OH", new[] {
                ("Columbus", 18), ("Cleveland", 16), ("Cincinnati", 14), ("Toledo", 10),
                ("Akron", 9), ("Dayton", 8), ("Parma", 6), ("Canton", 5)
            }),
            ("Georgia", "GA", new[] {
                ("Atlanta", 22), ("Augusta", 10), ("Columbus", 9), ("Savannah", 9),
                ("Athens", 7), ("Sandy Springs", 6), ("Roswell", 6), ("Macon", 5)
            }),
            ("North Carolina", "NC", new[] {
                ("Charlotte", 18), ("Raleigh", 16), ("Greensboro", 12), ("Durham", 10),
                ("Winston-Salem", 9), ("Fayetteville", 7), ("Cary", 6), ("Wilmington", 5)
            }),
            ("Michigan", "MI", new[] {
                ("Detroit", 18), ("Grand Rapids", 12), ("Warren", 8), ("Sterling Heights", 7),
                ("Ann Arbor", 8), ("Lansing", 6), ("Flint", 6), ("Dearborn", 5)
            }),
            ("New Jersey", "NJ", new[] {
                ("Newark", 12), ("Jersey City", 10), ("Paterson", 8), ("Elizabeth", 7),
                ("Trenton", 6), ("Clifton", 6), ("Camden", 5), ("Passaic", 4)
            }),
            ("Virginia", "VA", new[] {
                ("Virginia Beach", 14), ("Norfolk", 12), ("Chesapeake", 10), ("Richmond", 10),
                ("Newport News", 8), ("Alexandria", 7), ("Hampton", 6), ("Roanoke", 5)
            }),
            ("Washington", "WA", new[] {
                ("Seattle", 20), ("Spokane", 12), ("Tacoma", 10), ("Vancouver", 9),
                ("Bellevue", 8), ("Kirkland", 6), ("Renton", 5), ("Everett", 5)
            }),
            ("Arizona", "AZ", new[] {
                ("Phoenix", 22), ("Tucson", 14), ("Scottsdale", 10), ("Mesa", 10),
                ("Tempe", 8), ("Chandler", 7), ("Gilbert", 6), ("Glendale", 6)
            }),
            ("Massachusetts", "MA", new[] {
                ("Boston", 20), ("Worcester", 10), ("Springfield", 8), ("Lowell", 7),
                ("Cambridge", 7), ("New Bedford", 5), ("Brockton", 5), ("Quincy", 5)
            }),
            ("Tennessee", "TN", new[] {
                ("Nashville", 16), ("Memphis", 14), ("Knoxville", 10), ("Chattanooga", 9),
                ("Clarksville", 7), ("Murfreesboro", 6), ("Jackson", 5)
            }),
            ("Indiana", "IN", new[] {
                ("Indianapolis", 16), ("Fort Wayne", 10), ("Evansville", 8), ("South Bend", 7),
                ("Carmel", 6), ("Fishers", 5), ("Hammond", 5)
            }),
            ("Missouri", "MO", new[] {
                ("St. Louis", 16), ("Kansas City", 16), ("Springfield", 8), ("O'Fallon", 7),
                ("Columbia", 7), ("Independence", 6), ("Lee's Summit", 5)
            }),
            ("Maryland", "MD", new[] {
                ("Baltimore", 16), ("Frederick", 8), ("Rockville", 7), ("Gaithersburg", 7),
                ("Bowie", 6), ("Hagerstown", 5), ("Annapolis", 6)
            }),
            ("Colorado", "CO", new[] {
                ("Denver", 18), ("Colorado Springs", 12), ("Aurora", 9), ("Fort Collins", 8),
                ("Lakewood", 7), ("Boulder", 7), ("Arvada", 5), ("Westminster", 5)
            }),
            ("Wisconsin", "WI", new[] {
                ("Milwaukee", 14), ("Madison", 10), ("Green Bay", 8), ("Kenosha", 6),
                ("Racine", 5), ("Appleton", 5), ("Waukesha", 4)
            }),
            ("Minnesota", "MN", new[] {
                ("Minneapolis", 16), ("Saint Paul", 12), ("Rochester", 8), ("Duluth", 7),
                ("Bloomington", 6), ("Brooklyn Park", 5), ("Plymouth", 4)
            }),
            ("South Carolina", "SC", new[] {
                ("Charleston", 12), ("Columbia", 10), ("North Charleston", 8), ("Mount Pleasant", 7),
                ("Rock Hill", 6), ("Greenville", 7), ("Summerville", 5)
            }),
            ("Alabama", "AL", new[] {
                ("Birmingham", 12), ("Montgomery", 9), ("Huntsville", 9), ("Mobile", 7),
                ("Tuscaloosa", 6), ("Hoover", 5), ("Dothan", 4)
            }),
            ("Louisiana", "LA", new[] {
                ("New Orleans", 14), ("Baton Rouge", 12), ("Shreveport", 8), ("Metairie", 7),
                ("Lafayette", 7), ("Lake Charles", 5), ("Kenner", 5)
            }),
            ("Kentucky", "KY", new[] {
                ("Louisville", 14), ("Lexington", 12), ("Bowling Green", 7), ("Owensboro", 6),
                ("Covington", 5), ("Hopkinsville", 4)
            }),
            ("Oregon", "OR", new[] {
                ("Portland", 16), ("Salem", 8), ("Eugene", 8), ("Gresham", 7),
                ("Hillsboro", 6), ("Beaverton", 5), ("Medford", 5)
            }),
            ("Oklahoma", "OK", new[] {
                ("Oklahoma City", 14), ("Tulsa", 12), ("Norman", 8), ("Broken Arrow", 7),
                ("Edmond", 6), ("Lawton", 5), ("Moore", 4)
            }),
            ("Connecticut", "CT", new[] {
                ("Hartford", 10), ("New Haven", 9), ("Bridgeport", 8), ("Stamford", 7),
                ("Waterbury", 6), ("Norwalk", 5)
            }),
            ("Iowa", "IA", new[] {
                ("Des Moines", 10), ("Cedar Rapids", 8), ("Davenport", 7), ("Sioux City", 6),
                ("Waterloo", 5), ("Iowa City", 5)
            }),
            ("Mississippi", "MS", new[] {
                ("Jackson", 10), ("Gulfport", 7), ("Southaven", 6), ("Hattiesburg", 5),
                ("Biloxi", 6), ("Meridian", 4)
            }),
            ("Arkansas", "AR", new[] {
                ("Little Rock", 10), ("Fort Smith", 7), ("Fayetteville", 7), ("Springdale", 6),
                ("Jonesboro", 5), ("North Little Rock", 4)
            }),
            ("Nevada", "NV", new[] {
                ("Las Vegas", 20), ("Reno", 12), ("Henderson", 10), ("North Las Vegas", 8),
                ("Paradise", 6), ("Sparks", 5)
            }),
            ("Utah", "UT", new[] {
                ("Salt Lake City", 14), ("West Valley City", 8), ("Provo", 8), ("West Jordan", 6),
                ("Orem", 6), ("Sandy", 5), ("Ogden", 5)
            }),
            ("Kansas", "KS", new[] {
                ("Wichita", 10), ("Overland Park", 8), ("Kansas City", 7), ("Topeka", 7),
                ("Olathe", 6), ("Lawrence", 4)
            }),
            ("New Mexico", "NM", new[] {
                ("Albuquerque", 12), ("Santa Fe", 8), ("Las Cruces", 7), ("Rio Rancho", 6),
                ("Roswell", 4), ("Farmington", 4)
            }),
            ("Nebraska", "NE", new[] {
                ("Omaha", 10), ("Lincoln", 8), ("Bellevue", 6), ("Grand Island", 4), ("Kearney", 4)
            }),
            ("West Virginia", "WV", new[] {
                ("Charleston", 8), ("Huntington", 6), ("Morgantown", 6), ("Parkersburg", 4), ("Wheeling", 4)
            }),
            ("Idaho", "ID", new[] {
                ("Boise", 10), ("Nampa", 6), ("Meridian", 6), ("Idaho Falls", 4), ("Pocatello", 4)
            }),
            ("Hawaii", "HI", new[] {
                ("Honolulu", 12), ("Hilo", 6), ("Kailua", 5), ("Pearl City", 4), ("Waipahu", 3)
            }),
            ("New Hampshire", "NH", new[] {
                ("Manchester", 7), ("Nashua", 6), ("Concord", 5), ("Dover", 4)
            }),
            ("Maine", "ME", new[] {
                ("Portland", 7), ("Lewiston", 5), ("Bangor", 4), ("Augusta", 3)
            }),
            ("Montana", "MT", new[] {
                ("Billings", 6), ("Missoula", 5), ("Great Falls", 4), ("Bozeman", 4)
            }),
            ("Rhode Island", "RI", new[] {
                ("Providence", 7), ("Warwick", 5), ("Cranston", 4), ("Pawtucket", 3)
            }),
            ("Delaware", "DE", new[] {
                ("Wilmington", 6), ("Dover", 4), ("Newark", 4), ("Middletown", 3)
            }),
            ("South Dakota", "SD", new[] {
                ("Sioux Falls", 6), ("Rapid City", 5), ("Aberdeen", 3)
            }),
            ("North Dakota", "ND", new[] {
                ("Fargo", 6), ("Bismarck", 5), ("Grand Forks", 3)
            }),
            ("Alaska", "AK", new[] {
                ("Anchorage", 6), ("Fairbanks", 4), ("Juneau", 3)
            }),
            ("Vermont", "VT", new[] {
                ("Burlington", 5), ("South Burlington", 3), ("Rutland", 3)
            }),
            ("Wyoming", "WY", new[] {
                ("Cheyenne", 5), ("Casper", 4), ("Laramie", 3)
            }),
        };

        var streetNames = new[]
        {
            "Main", "Oak", "Pine", "Maple", "Cedar", "Elm", "Washington", "Lake",
            "Hill", "Park", "River", "Sunset", "Highland", "Valley", "Meadow",
            "Lincoln", "Jefferson", "Madison", "Adams", "Monroe", "Commerce",
            "Market", "Central", "Broadway", "Grand", "University", "College"
        };

        int branchNum = 1;
        foreach (var (state, code, cities) in data)
        {
            foreach (var (city, count) in cities)
            {
                var cityKey = city.Replace(" ", "").Replace("'", "").Replace(".", "").ToLower();

                for (int i = 1; i <= count; i++)
                {
                    var streetNum = 100 + (i * 17) % 900;
                    var street = streetNames[(branchNum + i) % streetNames.Length];
                    locations.Add(new Location
                    {
                        State = state,
                        StateCode = code,
                        City = city,
                        BranchName = $"Starry Nights Motel - {city} #{i}",
                        // Use global branchNum for guaranteed uniqueness across all state+city combinations
                        BranchCode = $"SNM{branchNum:D5}",
                        Address = $"{streetNum} {street} Street, {city}, {code}",
                        PhoneNumber = $"+1 ({(500 + branchNum % 400):D3}) {(100 + branchNum % 800):D3}-{(1000 + branchNum % 8999):D4}",
                        Email = $"branch{branchNum}@starrynightsmotel.com",
                        IsActive = true,
                        Latitude = 0,
                        Longitude = 0
                    });
                    branchNum++;
                }
            }
        }

        // Bulk insert in batches of 500 for performance
        const int batchSize = 500;
        for (int i = 0; i < locations.Count; i += batchSize)
        {
            var batch = locations.Skip(i).Take(batchSize).ToList();
            await context.Locations.AddRangeAsync(batch);
            await context.SaveChangesAsync();
        }
    }
}
