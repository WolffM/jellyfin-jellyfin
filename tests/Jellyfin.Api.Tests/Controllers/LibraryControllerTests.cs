using System.Linq;
using Jellyfin.Api.Controllers;
using MediaBrowser.Controller.Entities.Movies;
using Xunit;

namespace Jellyfin.Api.Tests.Controllers
{
    public class LibraryControllerTests
    {
        [Fact]
        public void RankBySimilarity_OrdersByDescendingScore()
        {
            // Arrange: three movies with different degrees of overlap with source genres/tags
            var lowMatch = new Movie { Genres = ["Action"], Tags = [] };          // score 1
            var highMatch = new Movie { Genres = ["Action", "Drama"], Tags = ["thriller"] }; // score 3
            var midMatch = new Movie { Genres = ["Drama"], Tags = ["thriller"] };  // score 2

            var candidates = new[] { lowMatch, highMatch, midMatch };
            var sourceGenres = new[] { "Action", "Drama" };
            var sourceTags = new[] { "thriller" };

            // Act
            var ranked = LibraryController.RankBySimilarity(candidates, sourceGenres, sourceTags).ToList();

            // Assert: most similar item should be first
            Assert.Equal(highMatch, ranked[0]);
            Assert.Equal(midMatch, ranked[1]);
            Assert.Equal(lowMatch, ranked[2]);
        }

        [Fact]
        public void RankBySimilarity_EmptySourceGenresAndTags_AllScoresAreZero()
        {
            // When source has no genres or tags every candidate scores zero; all items are returned
            var item1 = new Movie { Genres = ["Action"], Tags = ["thriller"] };
            var item2 = new Movie { Genres = ["Drama"], Tags = [] };

            var candidates = new[] { item1, item2 };

            var ranked = LibraryController.RankBySimilarity(candidates, [], []).ToList();

            Assert.Equal(2, ranked.Count);
            // All scores are 0 so both items must be present (order is implementation-defined)
            Assert.Contains(item1, ranked);
            Assert.Contains(item2, ranked);
        }

        [Fact]
        public void RankBySimilarity_IsCaseInsensitive()
        {
            // Genre matching should be case-insensitive
            var goodMatch = new Movie { Genres = ["action", "DRAMA"], Tags = [] }; // score 2
            var noMatch = new Movie { Genres = ["Comedy"], Tags = [] };             // score 0

            var candidates = new[] { noMatch, goodMatch };
            var sourceGenres = new[] { "Action", "Drama" };

            var ranked = LibraryController.RankBySimilarity(candidates, sourceGenres, []).ToList();

            Assert.Equal(goodMatch, ranked[0]);
            Assert.Equal(noMatch, ranked[1]);
        }
    }
}
