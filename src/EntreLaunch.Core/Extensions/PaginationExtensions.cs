using AutoMapper.QueryableExtensions;
using EntreLaunch.DTOs.BaseDtos;

namespace EntreLaunch.Extensions
{
    public static class PaginationExtensions
    {
        public static async Task<PaginatedResult<T>> ToPagedResultAsync<T>(this IQueryable<T> query, PaginationParams pagination)
        {
            var totalCount = await query.CountAsync();
            var items = await query
                .Skip((pagination.Page - 1) * pagination.PageSize)
                .Take(pagination.PageSize)
                .ToListAsync();

            return new PaginatedResult<T>
            {
                Items = items,
                TotalCount = totalCount,
                Page = pagination.Page,
                PageSize = pagination.PageSize
            };
        }

        public static async Task<PaginatedResult<TDestination>> ToPagedResultAsync<TSource, TDestination>(
            this IQueryable<TSource> query, PaginationParams pagination, AutoMapper.IConfigurationProvider mapperConfiguration)
        {
            var totalCount = await query.CountAsync();
            var items = await query
                .Skip((pagination.Page - 1) * pagination.PageSize)
                .Take(pagination.PageSize)
                .ProjectTo<TDestination>(mapperConfiguration)
                .ToListAsync();

            return new PaginatedResult<TDestination>
            {
                Items = items,
                TotalCount = totalCount,
                Page = pagination.Page,
                PageSize = pagination.PageSize
            };
        }

        public static async Task<PaginatedResult<T>> ToPagedResultAsync<T>(
                this IQueryable<T> query,
                PaginationParams pagination,
                CancellationToken cancellationToken)
        {
            var totalCount = await query.CountAsync(cancellationToken);
            var items = await query
                .Skip((pagination.Page - 1) * pagination.PageSize)
                .Take(pagination.PageSize)
                .ToListAsync(cancellationToken);

            return new PaginatedResult<T>
            {
                Items = items,
                TotalCount = totalCount,
                Page = pagination.Page,
                PageSize = pagination.PageSize
            };
        }
    }
}
