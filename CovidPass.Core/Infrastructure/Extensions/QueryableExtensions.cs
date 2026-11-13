// © 2020 Partners HealthCare Technology & Innovation Department
using System.Linq;
using CovidPass.Common.Constants;
using CovidPass.Core.Infrastructure.Services;

namespace CovidPass.Core.Infrastructure.Extensions
{
    public static class QueryableExtensions
    {
        public static FilterService<TModel> Filter<TModel>(this IQueryable<TModel> query)
        {
            return new FilterService<TModel>(query);
        }

        public static OrderService<T> Order<T>(this IQueryable<T> query, string orderBy, string sortBy)
        {
            return new OrderService<T>(query, orderBy, sortBy);
        }

        public static IQueryable<T> Page<T>(this IQueryable<T> query, int page, int pageSize = CommonConstants.PageSize)
        {
            return query.Skip((page - 1) * pageSize).Take(pageSize);
        }

    }
}
