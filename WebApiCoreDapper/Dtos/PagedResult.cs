using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApiCoreDapper.Models;

namespace WebApiCoreDapper.Dtos
{
    public class PagedResult<T> where T:class
    {
        public List<T> Items{ get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int TotalRow { get; set; }
    }
}
