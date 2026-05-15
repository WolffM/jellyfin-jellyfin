## Steps to reproduce

1. Add several movies to a Jellyfin library, giving them diverse but overlapping genres and tags (e.g., two Action/Drama films and several Action-only films).
2. Navigate to the detail page for one of the Action/Drama movies.
3. Scroll to the "More Like This" section and observe which items appear.
4. Refresh the page multiple times and note that the recommendations change on every load and frequently include movies that share only one genre instead of the most closely matching movies.
5. Alternatively, call the API directly: `GET /Items/{itemId}/Similar?limit=20` and observe that results differ on each call and are not ordered by similarity score.

## Observed

After PR #14918 introduced `OrderBy = [(ItemSortBy.Random, SortOrder.Ascending)]` in the `GetSimilarItems` method of `LibraryController`, all candidate items (every item sharing any genre or tag with the source) are shuffled randomly before the `limit` is applied. This means the top-N results are drawn from the entire matching pool at random, so a movie sharing only one genre out of five can appear before a movie sharing all five genres. Recommendations are therefore inaccurate and change on every page load, regardless of how closely an item actually matches the source.

## Expected

The `GetSimilarItems` endpoint should rank candidate items by a similarity score — the number of genres and tags the candidate shares with the source item — and return the highest-scoring items first. This ensures that "More Like This" always surfaces the most relevant content. The previous behaviour (before PR #14918) computed a similarity score and returned results ordered by that score, giving stable and accurate recommendations.
