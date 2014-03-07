using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using DAL.Root;
using BLL.Root.Validation;

namespace BLL.Root
{
    public interface IAccessorService<DTO>
    { 
        DTO Get(int id);
        DTO Get(string expression, params object[] parameters);
        List<DTO> GetMany(string expression, params object[] parameters);

        List<DTO> GetAll();
        List<DTO> GetPage(int pagesize, int page, string sortfield, bool descending, string filter,
                            out int totalcount, out bool hasprev, out bool hasnext,
                            out string positiontext, string positiontextformat);

	}
}
