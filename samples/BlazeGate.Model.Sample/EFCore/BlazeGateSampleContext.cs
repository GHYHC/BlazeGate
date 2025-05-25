using Microsoft.EntityFrameworkCore;

namespace BlazeGate.Model.Sample.EFCore
{
    public class BlazeGateSampleContext : DbContext
    {
        public BlazeGateSampleContext(DbContextOptions<BlazeGateSampleContext> options) : base(options)
        {
        }

        public DbSet<TB_Dictionary> TB_Dictionaries { get; set; }
    }
}