using BoxedSoftware.Models;
using Microsoft.EntityFrameworkCore;
using static BoxedSoftware.Provider;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

builder.Services.AddDbContext<VehiclesContext>(options =>
{
    var provider = config.GetValue("provider", Sqlite.Name);

    if (provider == Sqlite.Name)
    {
        options.UseSqlite(
            config.GetConnectionString(Sqlite.Name)!,
            x => x.MigrationsAssembly(Sqlite.Assembly)
        );
    }

    if (provider == Postgres.Name) {
        options.UseNpgsql(
            config.GetConnectionString(Postgres.Name)!,
            x => x.MigrationsAssembly(Postgres.Assembly)
        );
    }
});

var app = builder.Build();

// initialize database
using (var scope = app.Services.CreateScope()) {
    var db = scope.ServiceProvider.GetRequiredService<VehiclesContext>();
    
    db.Database.Migrate();
    
    await VehiclesContext.InitializeAsync(db);
}

// please don't do this, proof of concept
// the VehiclesContext will be tied to the Provider
// passed in at the start of the application lifetime
app.MapGet("/", (VehiclesContext db) => 
    db.Vehicles.OrderBy(x => x.Id).Take(10).ToList()
);

app.Run();

