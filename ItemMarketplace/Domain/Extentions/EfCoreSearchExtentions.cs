using Domain.Helpers;
using Microsoft.EntityFrameworkCore;

namespace Domain.Extentions
{
    public static class EfCoreSearchExtentions
    {
        public static IQueryable<TItem> BuildSearchTextQuery<TItem>(this IQueryable<TItem> query, Func<IQueryable<TItem>, string, IQueryable<TItem>> expression, string searchString)
        {
            string[] searchTerms = searchString.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string item in searchTerms)
            {
                query = expression(query, item);
            }

            return query;
        }

        public static async Task<(IQueryable<TItem> query, int totalCount)> BuildPageAsync<TItem>(this IQueryable<TItem> query, int pageNumber, int limit)
        {
            int countElements = await query.CountAsync();
            if (countElements < 1)
            {
                return (Enumerable.Empty<TItem>().AsQueryable(), 0);
            }

            int totalCount = PageHelper.CalculateTotalPagesCount(countElements, limit);

            return (query.BuildPage(pageNumber, totalCount, limit), totalCount);
        }

        public static IQueryable<TItem> BuildPage<TItem>(this IQueryable<TItem> query, int pageNumber, int totalCount, int limit) => 
            query.Skip((pageNumber > totalCount ? totalCount - 1 : pageNumber - 1) * limit)
                .Take(limit);
    }
}
