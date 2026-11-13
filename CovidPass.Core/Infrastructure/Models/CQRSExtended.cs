// © 2020 Partners HealthCare Technology & Innovation Department
using CovidPass.Common.Models;

namespace CovidPass.Core.Infrastructure.Models
{
    public class PageQuery<T> : Query<Page<T>>
    {
        public int Page { get; set; } = 1;
        public string SortBy { get; set; }
        public string OrderBy { get; set; }
    }

    public class ItemQuery<T> : Query<T>
    {
        public int Id { get; set; }
    }

    public class ItemCommand : Command
    {
        public int Id { get; set; }
    }
}
