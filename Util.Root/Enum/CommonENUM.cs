using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

/*Put several enums here because I'm lazy
 We should break it out if it gets too busy
 */
namespace Util.Root
{
    public enum ContentType : int
    {
        None,
        HTML,
        Content,
        JSON,
        XML
    }

    public enum UsageTrackingType : int
    {
        None,
        View,
        Click,
        Hover,
        Print,
        SearchResult
    }

    public enum ValidationInfoType : int
    {
        Field,
        Property,
        Method,
        Class
    }

    public enum CRUDDispatchType : int
    {
        [Description("dispatchCreate")]
        Create,
        [Description("dispatchRead")]
        Read,
        [Description("dispatchUpdate")]
        Update,
        [Description("dispatchDelete")]
        Delete
    }

    public enum JsonConvertType : int
    {
        PhoneFormatStandard,
        EnumToDescription
    }
}
