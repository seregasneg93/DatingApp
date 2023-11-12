using Microsoft.EntityFrameworkCore;

namespace DatingApp.Helpers
{
    public class PagesList<T> : List<T>
    {
        public PagesList(IEnumerable<T> items, int count, int pageNumber, int pageZise)
        {
            CurrentPage = pageNumber;
            TotalPage = (int)Math.Ceiling(count / (double)pageZise);
            PageZise = pageZise;
            TotalCount = count;
            AddRange(items);
        }

        public int CurrentPage { get; set; }
        public int TotalPage { get; set; }
        public int PageZise { get; set; }
        public int TotalCount { get; set; }

        public static async Task<PagesList<T>> CreateAsync(IQueryable<T> source,int pageNumber, int pageSize)
        {
            var count = await source.CountAsync();
            var items = await source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
            return new PagesList<T>(items,count,pageNumber,pageSize);
        }
    }
}
