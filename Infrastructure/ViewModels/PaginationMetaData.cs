using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.ViewModels
{
    public class PaginationMetaData
    {
        public int PageSize { get; set; }
        public int CurrentPage { get; set; }
        public int TotalItemCount { get; set; }
        public int TotalPageCount { get; set; }
        public PaginationMetaData(int totalItemCount, int pageSize, int currentPage)
        {
            TotalItemCount = totalItemCount;
            PageSize = pageSize;
            CurrentPage = currentPage;
            TotalPageCount = (int)Math.Ceiling(totalItemCount / (double)pageSize);
        }
    }
}
