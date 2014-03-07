using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using DAL.Root;
using BLL.Root.Validation;

namespace BLL.Root
{
    public interface IAdminService<DTO>
    {
        string User { get; set; }

        DTO New(DTO modelstart);
        bool Update(DTO model, out int id);
        bool Patch(object source, int id);
        bool Remove(int id);

        bool Validate(DTO model, List<ValidationInfo> info);
    }
}
