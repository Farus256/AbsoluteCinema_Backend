using AbsoluteCinema.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AbsoluteCinema.Infrastructure.EntitiesConfiguration
{
    public class MovieActorConfiguration : IEntityTypeConfiguration<MovieActor>
    {
        public void Configure(EntityTypeBuilder<MovieActor> builder)
        {
            builder.HasKey(ma => new { ma.MovieId, ma.ActorId });

            builder.HasOne(ma => ma.Movie)
                .WithMany(m => m.MovieActor)
                .HasForeignKey(ma => ma.MovieId)
                .IsRequired();

            builder.HasOne(ma => ma.Actor)
                .WithMany(a => a.MovieActor)
                .HasForeignKey(ma => ma.ActorId)
                .IsRequired();
        }
    }
}
