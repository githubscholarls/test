using EFAttribute.Domain.Entity;
using EFAttribute.Domain.Entity.Common;
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
            optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
            base.OnConfiguring(optionsBuilder); 
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<user>(entity =>
            //{
            //    entity.ToTable("user")
            //    .SplitToTable("userposition", tablebuilder =>
            //    {
            //        tablebuilder.Property(usr => usr.id).HasColumnName("userId");
            //        tablebuilder.Property(usr => usr.bornAddress);
            //        tablebuilder.Property(usr => usr.nowAddress);
            //        tablebuilder.Property(usr => usr.schoolAddress);
            //        tablebuilder.Property(usr => usr.homeAddress);
            //        //tablebuilder.Property(usr => usr.bornAddress);
            //        //tablebuilder.Property(usr => usr.nowaddress);
            //        //tablebuilder.Property(usr => usr.homeaddress);
            //        //tablebuilder.Property(usr => usr.schooladdress);
            //    });
            //});

            base.OnModelCreating(modelBuilder); 
        }
        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            var result = await base.SaveChangesAsync(cancellationToken);

            await DispatchEvents();
            return result;
        }


        private async Task DispatchEvents()
        {
            while (true)
            {
                var domainEventEntity = ChangeTracker
                    .Entries<IHasDomainEvent>()
                    .Select(x => x.Entity.DomainEvents)
                    .SelectMany(x => x)
                    .FirstOrDefault(domainEvent => !domainEvent.IsPublished);
                if (domainEventEntity == null) break;

                domainEventEntity.IsPublished = true;
                //await _domainEventService.Publish(domainEventEntity);
            }
        }

    }
}
