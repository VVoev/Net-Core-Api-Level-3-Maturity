using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Library.API.Helpers
{
    public class AuthorsResourceParameters
    {
        const int maximumPageSize = 20;
        private int pagesize = 10;

        public int PageNumber { get; set; } = 1;

        public int PageSize
        {
            get
            {
                return this.pagesize;
            }
            set
            {
                this.pagesize = pagesize > maximumPageSize ? maximumPageSize : value;
            }
        }

        public string Genre { get; set; }

        public string SearchQuery { get; set; }
    }
}
