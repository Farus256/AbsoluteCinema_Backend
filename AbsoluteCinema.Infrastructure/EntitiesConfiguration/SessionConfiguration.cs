using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AbsoluteCinema.Domain.Entities;

namespace AbsoluteCinema.Infrastructure.EntitiesConfiguration
{
    public class SessionConfiguration : IEntityTypeConfiguration<Session>
    {
        public void Configure(EntityTypeBuilder<Session> builder) 
        {
            builder.HasKey(s => s.Id);
            builder.Property(s => s.Date)
                .HasColumnType("timestamp without time zone");

            builder.HasOne(s => s.Movie)
                .WithMany(m => m.Sessions)
                .HasForeignKey(s => s.MovieId);

            builder.HasMany(s => s.Tickets)
                .WithOne(t => t.Session)
                .HasForeignKey(t => t.SessionId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(s => s.Hall)
                .WithMany(h => h.Sessions)
                .HasForeignKey(s => s.HallId);
        }
    }
}
