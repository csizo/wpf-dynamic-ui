using System;
using System.Linq;
using System.Linq.Expressions;
using Csizmazia.Tracing;
using LinqKit;

namespace Csizmazia.Collections
{
    internal static class PagingExtensions
    {
        private static readonly Tracer<TraceSource> Tracer = Tracer<TraceSource>.Instance;

        public static IOrderedQueryable<T> ApplyFilter<T>(this IOrderedQueryable<T> query,
                                                          Expression<Func<T, bool>> condition)
        {
            Tracer.Verbose(() => "applying query filter");

            if (condition == null)
            {
                Tracer.Verbose(() => "no filter has been applied");
                return query;
            }

            Tracer.Verbose(() => "applying filter");
            return (IOrderedQueryable<T>)query.Where(condition.Expand());
        }

        public static IOrderedQueryable<T> ApplyOrder<T>(this IOrderedQueryable<T> query, string orderColumn,
                                                         SortDirection sortDirection)
        {
            Tracer.Verbose(() => "applying query order");

            if (string.IsNullOrEmpty(orderColumn))
                return query;

            switch (sortDirection)
            {
                case SortDirection.Ascending:
                    Tracer.Verbose(() => "ascending order");
                    return query.OrderBy(orderColumn);

                case SortDirection.Descending:
                    Tracer.Verbose(() => "descending order");
                    return query.OrderByDescending(orderColumn);

                default:
                    Tracer.Verbose(() => "no ordering applied");
                    return query;
            }
        }

        public static IQueryable<T> ApplyPaging<T>(this IOrderedQueryable<T> query, int currentPage, int pageSize,
                                                   out int pageCount, out int queryCount)
        {
            Tracer.Verbose(() => "applying query paging");

            pageCount = query.CalculatePageCount(pageSize, out queryCount);

            if (pageSize == 0)
            {
                Tracer.Verbose(() => "paging is disabled");
                return query;
            }

            Tracer.Verbose(() => "building paged query");
            return query.Skip((currentPage - 1) * pageSize).Take(pageSize);
        }

        internal static int CalculatePageCount<T>(this IOrderedQueryable<T> query, int pageSize, out int queryCount)
        {
            Tracer.Verbose(() => "calculating pagecount");

            if (pageSize <= 0)
                throw new ArgumentOutOfRangeException("pageSize", "pageSize cannot be less than zero");

            Tracer.Verbose(() => "getting the query count");
            queryCount = query.Count();


            Tracer.Verbose(() => "check for pageSize");
            if (pageSize == 0)
            {
                Tracer.Verbose(() => "paging is disabled, pageCount set to 1");
                return 1;
            }


            Tracer.Verbose(() => "calculating pageCount");
            int remainder;

            int pageCount = Math.DivRem(queryCount, pageSize, out remainder);

            if (remainder > 0)
            {
                Tracer.Verbose(() => "adding 1 extra page for the remaining items");
                pageCount++;
            }

            //if query returns zero rows adjust the pageCount
            pageCount = Math.Max(1, pageCount);

            Tracer.Verbose(() => "PageCount is " + pageCount);
            return pageCount;
        }

        #region Nested type: TraceSource

        private abstract class TraceSource
        {
        }

        #endregion
    }
}