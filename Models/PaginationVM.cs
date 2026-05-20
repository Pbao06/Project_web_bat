using System.Collections.Generic;

namespace Getdata1.Models
{
    public class PaginationVM
    {
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public string ActionName { get; set; } = "Index";
        public string? ControllerName { get; set; }
        public string? AreaName { get; set; }
        public Dictionary<string, string> RouteValues { get; set; } = new Dictionary<string, string>();

        public bool HasPreviousPage => CurrentPage > 1;
        public bool HasNextPage => CurrentPage < TotalPages;
    }
}
