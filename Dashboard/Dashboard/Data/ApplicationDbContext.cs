using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Dashboard.Data;

internal class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser>(options)
{
    /// <summary>
    /// Gets or sets the incidents logged in the system.
    /// </summary>
    public DbSet<Incident> Incidents { get; set; }

    /// <summary>
    /// Gets or sets the key dates.
    /// </summary>
    public DbSet<KeyDates> KeyDates { get; set; }

    /// <summary>
    /// Gets or sets the vehicles logged in the system.
    /// </summary>
    public DbSet<Vehicle> Vehicles { get; set; }

}
