using System.Collections.Generic;

namespace UniversityManagementSystem.Application.DTOs.Common
{
    public class PagedResult<T>
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalRecords { get; set; }
        public int TotalPages => (int)System.Math.Ceiling((double)TotalRecords / PageSize);
        public List<T> Data { get; set; } = new List<T>();
    }
}