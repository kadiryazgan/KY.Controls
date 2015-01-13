using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace KY.Controls
{
    [ParseChildren(typeof(WebControl), DefaultProperty = "Controls", ChildrenAsProperties = true)]
    public class CustomItem : IFieldSetItem
    {
        [PersistenceMode(PersistenceMode.InnerProperty)]
        public List<Control> Controls { get; set; }
    }

}
