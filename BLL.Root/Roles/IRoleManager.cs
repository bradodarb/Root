using System;
namespace BLL.Root.Roles
{
    public interface IRoleManager<T>
    {
        bool CheckUserRole(string user, T authorizedroles);
        T GetRole(string user);
    }
}
