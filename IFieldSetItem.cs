using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KY.Controls
{
    public abstract class IFieldSetItem
    {
        public string Name { get; set; }
        internal FieldSet FieldSet { get; set; }
    }
}
