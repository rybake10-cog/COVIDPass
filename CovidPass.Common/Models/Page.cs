// © 2020 Partners HealthCare Technology & Innovation Department
using System;
using System.Collections.Generic;
using CovidPass.Common.Constants;

namespace CovidPass.Common.Models
{
    public class Page<T>
    {
        public List<T> Items { get; set; }

        public int TotalPages { get; set; }

        public int TotalItems { get; set; }

        public int CurrentPage { get; set; }

        public int PageSize { get; set; }

        public Page(List<T> items, int totalItems, int currentPage, int pageSize = CommonConstants.PageSize)
        {
            CurrentPage = currentPage;
            PageSize = pageSize;
            TotalItems = totalItems;
            Items = items;

            TotalPages = PageSize != 0 ? (int)Math.Ceiling(TotalItems / (double)PageSize) : 0;
        }

    }
}
