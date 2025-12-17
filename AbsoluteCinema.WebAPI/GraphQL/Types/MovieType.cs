using AbsoluteCinema.Application.DTO.Entities;
using HotChocolate.Types;

namespace AbsoluteCinema.WebAPI.GraphQL.Types
{
    public class MovieType : ObjectType<MovieDto>
    {
        protected override void Configure(IObjectTypeDescriptor<MovieDto> descriptor)
        {
            descriptor.Description("Represents a movie in the cinema");
            
            descriptor
                .Field(m => m.Id)
                .Description("The unique identifier of the movie");
                
            descriptor
                .Field(m => m.Title)
                .Description("The title of the movie");
                
            descriptor
                .Field(m => m.Discription)
                .Name("description")
                .Description("The description of the movie");
                
            descriptor
                .Field(m => m.ReleaseDate)
                .Description("The release date of the movie");
                
            descriptor
                .Field(m => m.Score)
                .Description("The score/rating of the movie");

            descriptor
                .Field(m => m.Adult)
                .Description("Whether the movie is marked as adult");

            descriptor
                .Field(m => m.PosterPath)
                .Description("Poster path");

            descriptor
                .Field(m => m.Language)
                .Description("Movie language");

            descriptor
                .Field(m => m.TrailerPath)
                .Description("Trailer path");

            /*descriptor
                .Field(m => m.MovieGenres)
                .Description("Genres linked to the movie");

            descriptor
                .Field(m => m.MovieActors)
                .Description("Actors linked to the movie");*/
        }
    }
}
