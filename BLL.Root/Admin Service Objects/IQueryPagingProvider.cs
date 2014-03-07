using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.Root.Dto;

namespace BLL.Root.Admin_Service_Objects
{
    interface IQueryPagingProvider<T> where T : DtoBase
    {
        List<DtoBase> GetQueryPage(T queryobject, int pagesize, int page, string sortfield, bool descending, string filter,
                            out int totalcount, out bool hasprev, out bool hasnext, out string positiontext, string positiontextformat);
    }
}
