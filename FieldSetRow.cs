using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace KY.Controls
{
    [ParseChildren(typeof(IFieldSetItem), DefaultProperty = "Items", ChildrenAsProperties = true)]
    public class FieldSetRow : IFieldSetItem
    {
        [PersistenceMode(PersistenceMode.InnerDefaultProperty)]
        public List<IFieldSetItem> Items { get; private set; }

        public IFieldSetItem this[string name]
        {
            get
            {
                foreach (IFieldSetItem item in this.Items)
                {
                    if (item.Name == name)
                    {
                        return item;
                    }
                    else if (item is FieldSetRow)
                    {
                        var it = (item as FieldSetRow)[name];
                        if (it != null) return it;
                    }
                }
                return null;
            }
        }

        private string _label;
        public string Label
        {
            get { return _label; }
            set
            {
                _label = value;
                if (_labelControl != null) _labelControl.Text = value;
            }
        }
        public string CssClass { get; set; }
        public string Style { get; set; }
        protected bool DivOnly { get; set; }
        private bool _visible = true;
        public bool Visible
        {
            get { return _visible; }
            set
            {
                _visible = value;
                if (divContainer != null) divContainer.Visible = value;
            }
        }
        public FieldSetRowValidatorPosition ValidatorPosition { get; set; }
        internal string ValidationGroup { get { return FieldSet.ValidationGroup; } }

        private HtmlGenericControl divContainer;
        private Label _labelControl;

        public FieldSetRow()
        {
            this.Items = new List<IFieldSetItem>();
        }

        public FieldSetRow(string label, string cssClass)
        {
            this.Items = new List<IFieldSetItem>();
            this.Label = label;
            this.CssClass = cssClass;
        }

        private string _controlsCssName = "controls";
        public string ControlsCssName { get { return _controlsCssName; } set { _controlsCssName = value; } }

        private string _validatorsCssName = "validators";
        public string ValidatorsCssName { get { return _validatorsCssName; } set { _validatorsCssName = value; } }

        public string LabelCssName { get; set; }

        internal Control Control
        {
            get
            {
                divContainer = FieldSet.GetElement("div", CssClass ?? (FieldSet.Bootstrap ? "control-group" : null));
                if (!string.IsNullOrEmpty(Style)) divContainer.Attributes.Add("style", Style);
                if (!string.IsNullOrEmpty(Name)) divContainer.Attributes.Add("id", Name);

                if (Items.Count > 0)
                {
                    DivOnly = true;
                    foreach (var it2 in Items)
                        if (!(it2 is FieldSetRow))
                        {
                            DivOnly = false;
                            break;
                        }
                }
                else
                {
                    DivOnly = true;
                }
                HtmlGenericControl divValidators = DivOnly ? divContainer : FieldSet.GetElement("span", ValidatorsCssName);
                HtmlGenericControl divControls = DivOnly ? divContainer : FieldSet.GetElement(FieldSet.Bootstrap ? "div" : "span", ControlsCssName);

                if (!string.IsNullOrEmpty(Label))
                {
                    _labelControl = new Label();
                    _labelControl.Text = Label;
                    if (Items.Count > 0 && Items[0] is FieldSetItem)
                    {
                        _labelControl.AssociatedControlID = (Items[0] as FieldSetItem).Control.ID;
                    }
                    if (!string.IsNullOrEmpty(LabelCssName)) _labelControl.CssClass = LabelCssName;
                    else if (this.FieldSet.Bootstrap) _labelControl.CssClass = "control-label";

                    divContainer.Controls.Add(_labelControl);
                }
                foreach (var it in Items)
                {
                    if (it is CustomItem)
                    {
                        if ((it as CustomItem).Controls != null)
                        {
                            foreach (var c in (it as CustomItem).Controls)
                                divControls.Controls.Add(c);
                        }
                    }
                    else if (it is FieldSetItem)
                    {
                        var item = it as FieldSetItem;
                        divControls.Controls.Add(item.Control);

                        var pi = item.Control.GetType().GetProperty("ValidationGroup");
                        if (pi != null)
                        {
                            pi.SetValue(item.Control, this.ValidationGroup, null);
                        }

                        if (item.Required)
                        {
                            RequiredFieldValidator rfv = new RequiredFieldValidator();
                            InitValidator(rfv, item.Control, "rfv",
                                item.RequiredErrorText ?? Strings.this_field_is_required,
                                "*",
                                item.RequiredErrorText ?? string.Format(Strings.x_is_required, this.Label));
                            divValidators.Controls.Add(rfv);
                        }
                        if (!string.IsNullOrEmpty(item.ControlToCompare))
                        {
                            CompareValidator cv = new CompareValidator();
                            cv.ControlToCompare = item.ControlToCompare;
                            InitValidator(cv, item.Control, "cv", item.CompareErrorText ?? Strings.fields_dont_match, "!", item.CompareErrorText ?? string.Format(Strings.x_dont_match, this.Label));
                            divValidators.Controls.Add(cv);
                        }
                        if (item.RegexName != RegexName.None)
                        {
                            item.Regex = "_" + item.RegexName.ToString();
                        }

                        if (!string.IsNullOrEmpty(item.Regex))
                        {
                            RegularExpressionValidator rev = new RegularExpressionValidator();
                            if (item.Regex.StartsWith("_"))
                            {
                                rev.ValidationExpression = typeof(RegularExpressions).GetField(item.Regex.Substring(1)).GetValue(null).ToString();
                            }
                            else
                            {
                                rev.ValidationExpression = item.Regex;
                            }
                            InitValidator(rev, item.Control, "rev", item.RegexErrorText ?? Strings.invalid_input, "!", item.RegexErrorText ?? string.Format(Strings.invalid_input_for_x, this.Label));
                            divValidators.Controls.Add(rev);
                        }

                        if (FieldSet.Bootstrap && item.Type == FieldSetItemType.Button && string.IsNullOrEmpty(item.CssClass))
                        {
                            (item.Control as Button).CssClass = "btn";
                        }
                    }
                    else if (it is FieldSetRow)
                    {
                        var item = it as FieldSetRow;
                        (it as FieldSetRow).FieldSet = this.FieldSet;
                        divControls.Controls.Add(item.Control);
                    }
                }
                if (!DivOnly)
                {
                    if (divValidators.Controls.Count > 0)
                    {
                        if (ValidatorPosition == FieldSetRowValidatorPosition.Default)
                        {
                            ValidatorPosition = this.FieldSet.DefaultValidatorPosition;
                        }

                        if (ValidatorPosition == FieldSetRowValidatorPosition.BeforeControl || ValidatorPosition == FieldSetRowValidatorPosition.Default)
                        {
                            divContainer.Controls.Add(divValidators);
                            divContainer.Controls.Add(divControls);
                        }
                        else
                        {
                            divContainer.Controls.Add(divControls);
                            divContainer.Controls.Add(divValidators);
                        }
                    }
                    else
                    {
                        divContainer.Controls.Add(divControls);
                    }
                }

                divContainer.Visible = Visible;
                return divContainer;
            }
        }

        private void InitValidator(BaseValidator validator, Control control, string type, string tooltip, string text, string errormsg)
        {
            validator.ControlToValidate = control.ID;
            validator.ID = type + control.ID;
            validator.ValidationGroup = this.ValidationGroup;
            validator.ForeColor = System.Drawing.Color.Empty;
            validator.Display = ValidatorDisplay.Dynamic;
            validator.ToolTip = tooltip;
            validator.Text = this.FieldSet != null && this.FieldSet.ShowValidationTexts ? tooltip : text;
            validator.ErrorMessage = errormsg;
        }
    }
}
