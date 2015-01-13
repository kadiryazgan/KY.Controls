using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KY.Controls
{
    public enum FieldSetItemType
    {
        TextBox,
        DropDownList,
        CheckBox,
        CheckBoxList,
        RadioButtonList,
        Button,
        Label,
        LinkButton,
        TextBox_MultiLine,
        TextBox_Password,
        FileUpload,
        Image,
        ListBox,
        HiddenField,
        HyperLink
    }

    public enum RegexName
    {
        None,
        Currency,
        CurrencySigned,
        Date,
        DateTime,
        Email,
        GsmNumber,
        Hour,
        Integer,
        LoginName,
        Password,
        PersonName,
        PhoneNumber,
        Url
    }

    public enum FieldSetRowValidatorPosition
    {
        Default,
        BeforeControl,
        AfterControl
    }
}
