using System.Dynamic;
using Microsoft.EntityFrameworkCore;
using PRN232.LMS.Services.QueryParams;
using PRN232.LMS.Services.Shared;

namespace PRN232.LMS.Services.Helpers;

public static class QueryHelper
{
    /// <summary>Apply sort from "field1,-field2" syntax to an IQueryable.</summary>
    public static IQueryable<T> ApplySort<T>(IQueryable<T> source, string? sort)
    {
        if (string.IsNullOrWhiteSpace(sort)) return source;

        var fields = sort.Split(',', StringSplitOptions.RemoveEmptyEntries);
        IOrderedQueryable<T>? ordered = null;

        foreach (var raw in fields)
        {
            var desc = raw.StartsWith('-');
            var name = raw.TrimStart('-').Trim();

            if (ordered == null)
                ordered = desc
                    ? source.OrderByDescending(e => EF.Property<object>(e!, name))
                    : source.OrderBy(e => EF.Property<object>(e!, name));
            else
                ordered = desc
                    ? ordered.ThenByDescending(e => EF.Property<object>(e!, name))
                    : ordered.ThenBy(e => EF.Property<object>(e!, name));
        }

        return ordered ?? source;
    }

    /// <summary>Paginate a queryable and return PagedResult with mapped items.</summary>
    public static async Task<PagedResult<object>> PaginateAsync<T>(
        IQueryable<T> source,
        BaseQueryParams query,
        Func<T, object> mapItem)
    {
        var totalItems = await source.CountAsync();
        var totalPages = (int)Math.Ceiling(totalItems / (double)query.Size);
        var entities   = await source.Skip((query.Page - 1) * query.Size).Take(query.Size).ToListAsync();

        var items = string.IsNullOrWhiteSpace(query.Fields)
            ? entities.Select(e => mapItem(e))
            : entities.Select(e => SelectFields(mapItem(e), query.Fields));

        return new PagedResult<object>
        {
            Items = items,
            Pagination = new PaginationMeta
            {
                Page       = query.Page,
                PageSize   = query.Size,
                TotalItems = totalItems,
                TotalPages = totalPages
            }
        };
    }

    /// <summary>Dynamic field selection using ExpandoObject.</summary>
    public static object SelectFields(object obj, string fields)
    {
        var props  = fields.Split(',').Select(f => f.Trim()).ToHashSet(StringComparer.OrdinalIgnoreCase);
        var expando = (IDictionary<string, object?>)new ExpandoObject();
        foreach (var prop in obj.GetType().GetProperties())
            if (props.Contains(prop.Name))
                expando[prop.Name] = prop.GetValue(obj);
        return expando;
    }
}
