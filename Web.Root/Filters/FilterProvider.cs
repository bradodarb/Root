using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Microsoft.Practices.Unity;

namespace Web.Root.Filters
{
    public class FilterProvider : IFilterProvider
    {
        private IUnityContainer container;

        public FilterProvider(IUnityContainer container) {
            this.container = container;
        }

        public IEnumerable<Filter> GetFilters(ControllerContext controllerContext, ActionDescriptor actionDescriptor)
        {
            foreach (IActionFilter actionFilter in this.container.ResolveAll<IActionFilter>()) {
                yield return new Filter(actionFilter, FilterScope.First, null);
            }
        }


    }
}
