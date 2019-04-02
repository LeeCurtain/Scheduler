using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TasksCommon
{
    public class PaginatedList<T> : List<T> where T : class
    {
        public PaginationBase PaginationBase { get; set; }
        public int TotalItemsCount { get; set; }
        public string Cursor { get; set; }

        public int PageCount => TotalItemsCount / PaginationBase.PageSize + (TotalItemsCount % PaginationBase.PageSize > 0 ? 1 : 0);

        public bool HasPrevious => PaginationBase.Page > 1;
        public bool HasNext => PaginationBase.Page < PageCount;

        public PaginatedList(int page, int pageSize, int totalItemsCount, IEnumerable<T> data, string cursor = "")
        {
            PaginationBase = new PaginationBase
            {
                Page = page,
                PageSize = pageSize
            };
            TotalItemsCount = totalItemsCount;
            Cursor = cursor;
            AddRange(data);
        }

        public static PaginatedList<T> Create(IQueryable<T> source, int page, int pageSize)
        {
            var count = source.Count();
            var items = source.Skip(page * pageSize).Take(pageSize).ToList();
            return new PaginatedList<T>(page, pageSize, count, items);
        }

    }
}
