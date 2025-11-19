using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AbsoluteCinema.Domain.Entities;
using System.Reflection.Emit;
using AbsoluteCinema.Infrastructure.Identity.Data;
using AbsoluteCinema.Domain.Entities.Interfaces;

namespace AbsoluteCinema.Infrastructure.EntitiesConfiguration
{
    public class TicketConfiguration : IEntityTypeConfiguration<Ticket>
    {
        public void Configure(EntityTypeBuilder<Ticket> builder)
        {
            builder.HasKey(t => t.Id);
            builder.Property(t => t.Row);
            builder.Property(t => t.Place);
            builder.Property(t => t.Price).IsRequired();

            builder.HasIndex(t => new { t.SessionId, t.Row, t.Place })
                .IsUnique()
                .HasDatabaseName("IX_Ticket_SessionId_Row_Place_Unique");

            builder.HasOne(t => t.Session)
                .WithMany(s => s.Tickets)
                .HasForeignKey(t => t.SessionId)
                .IsRequired();

            builder.HasOne(t => t.Status)
                .WithMany(ts => ts.Tickets)
                .HasForeignKey(t => t.StatusId)
                .IsRequired();
        }
    }
}
