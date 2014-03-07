using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using DAL.Root;
using Model.Root;
using Util.Root;

namespace BLL.Root
{
    public static class AdminServiceProvider
    {
        public static IRepositoryFactory RepositoryFactory { get; set; }
 
        public static string GetCurrentUserName()
        {
            return HttpContext.Current.User.Identity.Name;  
        }

        public static IRepository<T> GetRepository<T>() where T : class, IPersistenceEntity
        {
            return RepositoryFactory.GetRepository<T>();
        }
    }
}
