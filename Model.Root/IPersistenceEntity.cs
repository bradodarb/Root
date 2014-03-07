using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Model.Root
{
    public interface IPersistenceEntity
    {
        int Id { get; set; }
        bool IsActive { get; set; }
        DateTime Created { get; set; }
        DateTime Modified { get; set; }
        string ModifiedByUserName { get; set; }
    }
}
