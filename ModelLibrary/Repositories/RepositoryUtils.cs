using EntityLib.Entities.AbstractClasses;
using Microsoft.EntityFrameworkCore;
using ModelLib.ApiDTOs.Pagination;
using ModelLib.DTOs.AbstractDTOs;
using ModelLib.Utils;
using NetTopologySuite.Geometries;
using NpgsqlTypes;

namespace ModelLib.Repositories
{
    public class RepositoryUtils
    {
        public static async Task<(PaginationResultNoList, IQueryable<T>)> GetPaginationQuery<T>(IQueryable<T>? query, Func<IQueryable<T>, IOrderedQueryable<T>> orderByFunc, Func<IQueryable<T>, IQueryable<T>> keyOffsetFunc, PaginationRequest pagReq)
        {

            var startIndex = pagReq.Page * pagReq.ItemsPerPage;
            var total = await query.CountAsync();
            var hasNext = startIndex + pagReq.ItemsPerPage < total;

            query = orderByFunc(query);
            query = keyOffsetFunc(query);
            query = query.Take(pagReq.ItemsPerPage);

            var paginationResult = new PaginationResultNoList()
            {
                Total = total,
                CurrentPage = pagReq.Page,
                HasNext = hasNext,
            };

            return (paginationResult, query);
        }
        public static async Task<PaginationResult<U>> PaginateAsync<T, U>(IQueryable<T>? query, Func<IQueryable<T>, IQueryable<U>> select, PaginationRequest pagReq)
            where T : IntegerId
            where U : IntegerId
        {
            var paginationInfo = await GetPaginationBaseInfo(query, pagReq);

            query = query.OrderByDescending(p => p.Id);
            query = query.Where(p => p.Id < pagReq.LastId);
            query = query.Take(pagReq.ItemsPerPage);
            var result = await select(query).ToListAsync();
            var paginationResult = new PaginationResult<U>()
            {
                Total = paginationInfo.Total,
                CurrentPage = pagReq.Page,
                HasNext = paginationInfo.HasNext,
                Result = result,
                LastId = result.LastOrDefault()?.Id ?? -1
            };

            return paginationResult;
        }

        public static async Task<PaginationResult<U>> PaginateByStringAsync<T, U>(IQueryable<T> query, Func<IQueryable<T>, IQueryable<T>> orderBy, Func<IQueryable<T>, IQueryable<T>> keyOffset, Func<IQueryable<T>, IQueryable<U>> select, StringPaginationRequest pagReq)
    where T : IntegerId
    where U : IntegerId
        {
            var paginationInfo = await GetPaginationBaseInfo(query, pagReq);

            query = orderBy(query);
            query = keyOffset(query);
            query = query.Take(pagReq.ItemsPerPage);
            var result = await select(query).ToListAsync();
            var paginationResult = new PaginationResult<U>()
            {
                Total = paginationInfo.Total,
                CurrentPage = pagReq.Page,
                HasNext = paginationInfo.HasNext,
                Result = result,
                LastId = result.LastOrDefault()?.Id ?? -1
            };

            return paginationResult;
        }

        public static async Task<DateTimePaginationResult<U>> PaginateByDateAsync<T, U>(IQueryable<T>? query, Func<IQueryable<T>, IQueryable<U>> select, DateTimePaginationRequest pagReq) where T : DateAndIntegerId where U : DateAndIntegerId
        {
            var paginationInfo = await GetPaginationBaseInfo(query, pagReq);

            query = query.OrderByDescending(r => r.DateTime).ThenBy(r => r.Id);
            query = query.Where(r => r.DateTime < pagReq.LastDate || (r.DateTime == pagReq.LastDate && r.Id > pagReq.LastId));
            query = query.Take(pagReq.ItemsPerPage);
            var result = await select(query).ToListAsync();
            var paginationResult = new DateTimePaginationResult<U>()
            {
                Total = paginationInfo.Total,
                CurrentPage = pagReq.Page,
                HasNext = paginationInfo.HasNext,
                Result = result,
                LastId = result.LastOrDefault()?.Id ?? -1,
                LastDate = result.LastOrDefault()?.DateTime ?? DateTime.MinValue
            };
            return paginationResult;
        }

        public static async Task<DistancePaginationResult<U>> PaginateByDistanceAsync<T, U>(IQueryable<T>? query, Func<IQueryable<T>, IQueryable<U>> select, DistancePaginationRequest pagReq) where T : EntityWithLocation where U : DTOWithLocation
        {
            var paginationInfo = await GetPaginationBaseInfo(query, pagReq);
            var point = new Point(x: pagReq.SearchArea.Center.Lng, y: pagReq.SearchArea.Center.Lat);

            query = query.OrderBy(r => EF.Functions.Distance(r.Location, point, true));
            query = query.Where(r => EF.Functions.Distance(r.Location, point, true) > pagReq.PreviousDistance);
            query = query.Take(pagReq.ItemsPerPage);
            var result = await select(query).ToListAsync();

            var lastPlace = result.LastOrDefault();
            var lastDistance = -1f;
            if (lastPlace is not null)
            {
                lastDistance = (float)point.DistanceTo(new Point(x: lastPlace.Location.X, y:lastPlace.Location.Y), Geographics.DistanceUnit.Meters);
            }

            var paginationResult = new DistancePaginationResult<U>()
            {
                Total = paginationInfo.Total,
                CurrentPage = pagReq.Page,
                HasNext = paginationInfo.HasNext,
                Result = result,
                LastId = result.LastOrDefault()?.Id ?? -1,
                LastDistance = lastDistance,
            };
            return paginationResult;
        }


        private static async Task<PaginationBaseInfo> GetPaginationBaseInfo<T>(IQueryable<T>? query, PaginationRequest pagReq)
        {
            var startIndex = pagReq.Page * pagReq.ItemsPerPage;
            var total = await query.CountAsync();
            var hasNext = startIndex + pagReq.ItemsPerPage < total;
            return new PaginationBaseInfo
            {
                Total = total,
                HasNext = hasNext
            };
        }
        private class PaginationBaseInfo
        {
            public int Total { get; set; }
            public bool HasNext { get; set; }
        }
    }

}
