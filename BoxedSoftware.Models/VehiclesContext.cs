using Bogus;
using Microsoft.EntityFrameworkCore;

namespace BoxedSoftware.Models;

public class VehiclesContext : DbContext
{
    public VehiclesContext(DbContextOptions<VehiclesContext> options)
        : base(options)
    {
    }
    
    public DbSet<Vehicle> Vehicles { get; set; }

    public static async Task InitializeAsync(VehiclesContext db)
    {
        // already seeded
        if (db.Vehicles.Any())
            return;
        
        // sample data will be different due
        // to the nature of generating data
        var fake = new Faker<Vehicle>()
            .Rules((f, v) => v.VehicleIdentificationNumber = f.Vehicle.Vin())
            .Rules((f, v) => v.Model = f.Vehicle.Model())
            .Rules((f, v) => v.Type = f.Vehicle.Type())
            .Rules((f, v) => v.Fuel = f.Vehicle.Fuel());

        var vehicles = fake.Generate(100);
        
        db.Vehicles.AddRange(vehicles);
        await db.SaveChangesAsync();
    }
}

public class Vehicle
{
    public int Id { get; set; }
    public string VehicleIdentificationNumber { get; set; } = "";
    public string Model { get; set; } = "";
    public string Type { get; set; } = "";
    public string Fuel { get; set; } = "";
}