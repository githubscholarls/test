using EFAttribute.Domain.Entity;
using Microsoft.EntityFrameworkCore;

namespace EFAttribute.MyDbContext
{
    public class TestDbContext:DbContext
    {
        public TestDbContext(string connection):base(GetDbContextOptions(connection))
        {
            
        }
        private static DbContextOptions<TestDbContext> GetDbContextOptions(string connectionString)
        {
            var optionsBuilder = new DbContextOptionsBuilder<TestDbContext>();
            optionsBuilder.UseSqlite(connectionString);
            return optionsBuilder.Options;
        }
        public TestDbContext(DbContextOptions<TestDbContext> options):base(options)
        {
        }
        public DbSet<user> user { get; set; }
        public DbSet<wechat> wechat { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder); 
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<user>(entity =>
            {
                entity.ToTable("user")
                .SplitToTable("userposition", tablebuilder =>
                {
                    tablebuilder.Property(usr => usr.id).HasColumnName("userId");
                    tablebuilder.Property(usr => usr.bornAddress);
                    tablebuilder.Property(usr => usr.nowAddress);
                    tablebuilder.Property(usr => usr.schoolAddress);
                    tablebuilder.Property(usr => usr.homeAddress);
                    //tablebuilder.Property(usr => usr.bornAddress);
                    //tablebuilder.Property(usr => usr.nowaddress);
                    //tablebuilder.Property(usr => usr.homeaddress);
                    //tablebuilder.Property(usr => usr.schooladdress);
                });
            });

            base.OnModelCreating(modelBuilder); 
        }
    }
}
