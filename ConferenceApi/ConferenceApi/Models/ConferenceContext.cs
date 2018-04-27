using Microsoft.EntityFrameworkCore;

namespace ConferenceApi.Models
{
    /// <summary>
    /// Db context options.
    /// </summary>
    public class ConferenceContext : DbContext
    {
        public ConferenceContext(DbContextOptions<ConferenceContext> options)
            : base(options)
        {
        }

        public DbSet<ConferenceItem> ConferenceItems { get; set; }
    }
}