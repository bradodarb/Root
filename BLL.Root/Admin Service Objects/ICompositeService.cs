using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using DAL.Root;
using BLL.Root.Validation;

namespace BLL.Root
{
    public interface ICompositeService<DTO> : IAccessorService<DTO>, IAdminService<DTO>
    { 
    }
}
