using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace KY.Controls
{
    public class FieldSetItem : IFieldSetItem
    {
        public FieldSetItemType Type { get; set; }
        public bool Required { get; set; }
        public string Regex { get; set; }
        public RegexName RegexName { get; set; }
        public string ControlToCompare { get; set; }
        public string Placeholder { get; set; }

        public string CssClass
        {
            get
            {
                if (Control is WebControl)
                {
                    return (Control as WebControl).CssClass;
                }
                return null;
            }
            set
            {
                if (Control is WebControl)
                {
                    (Control as WebControl).CssClass = value;
                }
            }
        }

        public string Style
        {
            get
            {
                if (Control is WebControl)
                {
                    return (Control as WebControl).Style.Value;
                }
                return null;
            }
            set
            {
                if (Control is WebControl)
                {
                    (Control as WebControl).Style.Value = value;
                }
            }
        }

        public string RequiredErrorText { get; set; }
        public string RegexErrorText { get; set; }
        public string CompareErrorText { get; set; }

        public string Value
        {
            #region get;
            get
            {
                switch (Type)
                {
                    case FieldSetItemType.TextBox:
                        return (Control as TextBox).Text;

                    case FieldSetItemType.DropDownList:
                        return (Control as DropDownList).SelectedValue;

                    case FieldSetItemType.CheckBoxList:
                        StringBuilder sb = new StringBuilder();
                        foreach (ListItem li in (this.Control as CheckBoxList).Items)
                        {
                            if (li.Selected)
                            {
                                if (sb.Length > 0) sb.Append(",");
                                sb.Append(li.Value);
                            }
                        }
                        return sb.ToString();

                    case FieldSetItemType.RadioButtonList:
                        return (Control as RadioButtonList).SelectedValue;

                    case FieldSetItemType.Button:
                        return null;

                    case FieldSetItemType.CheckBox:
                        return (Control as CheckBox).Checked.ToString();

                    case FieldSetItemType.Label:
                        return null;

                    case FieldSetItemType.LinkButton:
                        return null;

                    case FieldSetItemType.TextBox_MultiLine:
                        return (Control as TextBox).Text;

                    case FieldSetItemType.TextBox_Password:
                        return (Control as TextBox).Text;

                    case FieldSetItemType.FileUpload:
                        return null;

                    case FieldSetItemType.Image:
                        return null;

                    case FieldSetItemType.ListBox:
                        return (Control as ListBox).SelectedValue;

                    case FieldSetItemType.HiddenField:
                        return (Control as HiddenField).Value;

                    case FieldSetItemType.HyperLink:
                        return (Control as HyperLink).NavigateUrl;

                    default:
                        return null;
                }
            }
            #endregion
            #region set;
            set
            {
                switch (Type)
                {
                    case FieldSetItemType.TextBox:
                        (Control as TextBox).Text = value;
                        break;

                    case FieldSetItemType.DropDownList:
                        (Control as DropDownList).SelectedValue = value;
                        break;

                    case FieldSetItemType.CheckBoxList:
                        if ((Control as CheckBoxList).Items.FindByValue(value) != null)
                        {
                            (Control as CheckBoxList).SelectedValue = value;
                        }
                        else if (value.IndexOf(",") > -1)
                        {
                            string[] values = value.Split(char.Parse(","));
                            foreach (string val in values)
                            {
                                ListItem li = (Control as CheckBoxList).Items.FindByValue(val);
                                if (li != null) li.Selected = true;
                            }
                        }
                        break;

                    case FieldSetItemType.RadioButtonList:
                        (Control as RadioButtonList).SelectedValue = value;
                        break;

                    case FieldSetItemType.Button:
                        (Control as Button).Text = value;
                        break;

                    case FieldSetItemType.CheckBox:
                        (Control as CheckBox).Checked = bool.Parse(value);
                        break;

                    case FieldSetItemType.Label:
                        (Control as Label).Text = value;
                        break;

                    case FieldSetItemType.LinkButton:
                        (Control as LinkButton).Text = value;
                        break;

                    case FieldSetItemType.TextBox_MultiLine:
                        (Control as TextBox).Text = value;
                        break;

                    case FieldSetItemType.TextBox_Password:
                        (Control as TextBox).Text = value;
                        break;

                    case FieldSetItemType.FileUpload:
                        break;

                    case FieldSetItemType.Image:
                        (Control as Image).ImageUrl = value;
                        break;

                    case FieldSetItemType.ListBox:
                        (Control as ListBox).SelectedValue = value;
                        break;

                    case FieldSetItemType.HiddenField:
                        (Control as HiddenField).Value = value;
                        break;

                    case FieldSetItemType.HyperLink:
                        (Control as HyperLink).NavigateUrl = value;
                        break;

                    default:
                        break;
                }
            }
            #endregion
        }

        public string Text
        {
            #region get;
            get
            {
                switch (Type)
                {
                    case FieldSetItemType.TextBox:
                        return (Control as TextBox).Text;

                    case FieldSetItemType.TextBox_MultiLine:
                        return (Control as TextBox).Text;

                    case FieldSetItemType.TextBox_Password:
                        return (Control as TextBox).Text;

                    case FieldSetItemType.Button:
                        return (Control as Button).Text;

                    case FieldSetItemType.CheckBox:
                        return (Control as CheckBox).Text;

                    case FieldSetItemType.Label:
                        return (Control as Label).Text;

                    case FieldSetItemType.LinkButton:
                        return (Control as LinkButton).Text;

                    case FieldSetItemType.Image:
                        return (Control as Image).ImageUrl;

                    case FieldSetItemType.HyperLink:
                        return (Control as HyperLink).Text;

                    default:
                        return null;
                }
            }
            #endregion
            #region set;
            set
            {
                switch (Type)
                {
                    case FieldSetItemType.TextBox:
                        (Control as TextBox).Text = value;
                        break;

                    case FieldSetItemType.TextBox_MultiLine:
                        (Control as TextBox).Text = value;
                        break;

                    case FieldSetItemType.TextBox_Password:
                        (Control as TextBox).Text = value;
                        break;

                    case FieldSetItemType.DropDownList:
                        (Control as DropDownList).DataSource = value.Split(new string[] { "," }, StringSplitOptions.None);
                        (Control as DropDownList).DataBind();
                        break;

                    case FieldSetItemType.CheckBoxList:
                        (Control as CheckBoxList).DataSource = value.Split(new string[] { "," }, StringSplitOptions.None);
                        (Control as CheckBoxList).DataBind();
                        break;

                    case FieldSetItemType.RadioButtonList:
                        (Control as RadioButtonList).DataSource = value.Split(new string[] { "," }, StringSplitOptions.None);
                        (Control as RadioButtonList).DataBind();
                        break;

                    case FieldSetItemType.Button:
                        (Control as Button).Text = value;
                        break;

                    case FieldSetItemType.CheckBox:
                        (Control as CheckBox).Text = value;
                        break;

                    case FieldSetItemType.Label:
                        (Control as Label).Text = value;
                        break;

                    case FieldSetItemType.LinkButton:
                        (Control as LinkButton).Text = value;
                        break;

                    case FieldSetItemType.Image:
                        (Control as Image).ImageUrl = value;
                        break;

                    case FieldSetItemType.ListBox:
                        (Control as ListBox).DataSource = value.Split(new string[] { "," }, StringSplitOptions.None);
                        (Control as ListBox).DataBind();
                        break;

                    case FieldSetItemType.HyperLink:
                        (Control as HyperLink).Text = value;
                        break;

                    default:
                        break;
                }
            }
            #endregion
        }

        private Control control;
        public Control Control
        {
            #region get;
            get
            {
                if (control == null)
                {
                    #region switch control types
                    switch (Type)
                    {
                        case FieldSetItemType.TextBox:
                            control = new TextBox();
                            break;

                        case FieldSetItemType.DropDownList:
                            control = new DropDownList();
                            break;

                        case FieldSetItemType.CheckBoxList:
                            control = new CheckBoxList();
                            break;

                        case FieldSetItemType.RadioButtonList:
                            control = new RadioButtonList();
                            break;

                        case FieldSetItemType.Button:
                            control = new Button();
                            break;

                        case FieldSetItemType.CheckBox:
                            control = new CheckBox();
                            break;

                        case FieldSetItemType.Label:
                            control = new Label();
                            break;

                        case FieldSetItemType.LinkButton:
                            control = new LinkButton();
                            break;

                        case FieldSetItemType.TextBox_MultiLine:
                            control = new TextBox();
                            ((TextBox)control).TextMode = TextBoxMode.MultiLine;
                            break;

                        case FieldSetItemType.TextBox_Password:
                            control = new TextBox();
                            ((TextBox)control).TextMode = TextBoxMode.Password;
                            break;

                        case FieldSetItemType.FileUpload:
                            control = new FileUpload();
                            break;

                        //case FieldSetItemType.HtmlEditor:
                        //    control = new FredCK.FCKeditorV2.FCKeditor();
                        //    break;

                        case FieldSetItemType.Image:
                            control = new Image();
                            break;

                        case FieldSetItemType.ListBox:
                            control = new ListBox();
                            break;

                        case FieldSetItemType.HiddenField:
                            control = new HiddenField();
                            break;

                        case FieldSetItemType.HyperLink:
                            control = new HyperLink();
                            break;

                        default:
                            control = new TextBox();
                            break;
                    }
                    #endregion
                    if (!string.IsNullOrEmpty(this.Name))
                        control.ID = this.Name.Replace(".", string.Empty);
                    if (control is WebControl)
                    {
                        if (!string.IsNullOrEmpty(CssClass)) (control as WebControl).CssClass = CssClass;
                        if (!string.IsNullOrEmpty(Style)) (control as WebControl).Style.Value = Style;
                        if (!string.IsNullOrEmpty(Placeholder))
                        {
                            (control as WebControl).Attributes["placeholder"] = Placeholder;
                        }
                    }
                }
                return control;
            }
            #endregion
        }

        public FieldSetItem()
        {
        }

        public FieldSetItem(FieldSetItemType type, string name)
        {
            this.Type = type;
            this.Name = name;
        }

        public FieldSetItem(FieldSetItemType type, string name, string text)
        {
            this.Type = type;
            this.Name = name;
            this.Text = text;
        }
    }
}
