using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RBAC_CoreMVC.Models
{
    /// <summary>
    /// 分类类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PagedList<T> : List<T>
    {
        public int pageIndex { get; private set; }
        public int totalPages { get; private set; }

        public PagedList(List<T> items, int count, int index, int size)
        {
            pageIndex = index;
            totalPages = (int)Math.Ceiling(count / (double)size);

            this.AddRange(items);
        }

        public bool hasPreviousPage => pageIndex > 1;

        public bool hasNextPage => pageIndex < totalPages;

        public static async Task<PagedList<T>> CreateAsync(IQueryable<T> source, int index, int size)
        {
            var count = await source.CountAsync();
            var items = await source.Skip((index - 1) * size).Take(size).ToListAsync();
            return new PagedList<T>(items, count, index, size);
        }
    }
}
