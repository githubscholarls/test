using EFAttribute.Domain.Entity.Common;
using EFAttribute.Domain.Entity;
using Microsoft.EntityFrameworkCore;

namespace EFAttribute.MyDbContext
{
    public class TestCpuContext: DbContext
    {
        public TestCpuContext(string connection) : base(GetDbContextOptions(connection))
        {

        }
        private static DbContextOptions<TestCpuContext> GetDbContextOptions(string connectionString)
        {
            var optionsBuilder = new DbContextOptionsBuilder<TestCpuContext>();
            optionsBuilder.UseNpgsql(connectionString);
            return optionsBuilder.Options;
        }
        public TestCpuContext(DbContextOptions<TestCpuContext> options) : base(options)
        {
        }
        public DbSet<pguser> user { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
            base.OnConfiguring(optionsBuilder);
        }
    }
}
