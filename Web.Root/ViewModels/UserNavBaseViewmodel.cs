using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Web.Root.ViewModels
{
    public class UserNavBaseViewmodel
    {
        public List<UserNavItemViewModel> UserNavItems { get; set; }

        public UserNavBaseViewmodel()
        {
            UserNavItems = new List<UserNavItemViewModel>();
        }

      
        public void AddNavItem(string title, string url, bool active)
        {
            UserNavItemViewModel item = new UserNavItemViewModel();
            item.Title = title;
            item.Url = url;
            item.Active = active;
            UserNavItems.Add(item);
        }

        public void AddNavItem(UserNavItemViewModel item)
        {
            UserNavItems.Add(item);
        }
    }

    public class UserNavItemViewModel
    {
        public string Title { get; set; }
        public string Url { get; set; }
        public bool Active { get; set; }
		public string IconUrl { get; set; }

        public void CheckActive(string match)
        {
            if (Title != null && match != null)
            {
                Active = Title.ToLower() == match.ToLower();
            }
            else
            {
                Active = false;
            }
        }
    }
}
