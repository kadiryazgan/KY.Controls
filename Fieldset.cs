using Ky.Controls.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Xml;
using System.Xml.XPath;

namespace KY.Controls
{
    [ParseChildren(typeof(FieldSetRow), DefaultProperty = "Rows", ChildrenAsProperties = true)]
    [ToolboxData("<{0}:FieldSet runat=server></{0}:FieldSet>")]
    public class FieldSet : WebControl, INamingContainer
    {
        [PersistenceMode(PersistenceMode.InnerDefaultProperty)]
        public List<FieldSetRow> Rows { get; private set; }

        [PersistenceMode(PersistenceMode.Attribute)]
        public bool Bootstrap { get; set; }

        [PersistenceMode(PersistenceMode.Attribute)]
        public string Title { get; set; }

        [PersistenceMode(PersistenceMode.Attribute)]
        public bool ShowMessageBox { get; set; }

        [PersistenceMode(PersistenceMode.Attribute)]
        public bool ShowSummary { get; set; }

        [PersistenceMode(PersistenceMode.Attribute)]
        public string TitleLinkCssName { get; set; }

        [PersistenceMode(PersistenceMode.Attribute)]
        public string TitleLinkUrl { get; set; }

        [PersistenceMode(PersistenceMode.Attribute)]
        public string TitleLinkText { get; set; }

        [PersistenceMode(PersistenceMode.Attribute)]
        public string TitleLinkOnClick { get; set; }

        private HyperLink _titleLink;
        public HyperLink GetTitleLink { get { return _titleLink; } }

        [PersistenceMode(PersistenceMode.Attribute)]
        public FieldSetRowValidatorPosition DefaultValidatorPosition { get; set; }

        private string _bodyCssName = "body";
        [PersistenceMode(PersistenceMode.Attribute)]
        public string BodyCssName { get { return Bootstrap ? "form-horizontal" : _bodyCssName; } set { _bodyCssName = value; } }

        [PersistenceMode(PersistenceMode.Attribute)]
        public bool ShowValidationTexts { get; set; }

        private string _titleCssName = "title";
        [PersistenceMode(PersistenceMode.Attribute)]
        public string TitleCssName { get { return _titleCssName; } set { _titleCssName = value; } }


        private Dictionary<string, IFieldSetItem> _itemsDic;
        private Dictionary<string, IFieldSetItem> ItemsDic
        {
            get
            {
                if (_itemsDic == null)
                {
                    _itemsDic = new Dictionary<string, IFieldSetItem>();
                    foreach (FieldSetRow row in this.Rows) FillItemDic(row);
                }
                return _itemsDic;
            }
        }

        private void FillItemDic(IFieldSetItem it)
        {
            if (!string.IsNullOrEmpty(it.Name)) _itemsDic.Add(it.Name, it);
            if (it is FieldSetRow) foreach (IFieldSetItem item in (it as FieldSetRow).Items) FillItemDic(item);
        }

        private HtmlGenericControl _hgcFieldset;
        private HtmlGenericControl _hgcLegend;
        private HtmlGenericControl _currentDiv;

        public string ValidationGroup { get; set; }

        public FieldSet()
            : base(HtmlTextWriterTag.Div)
        {
            this.Rows = new List<FieldSetRow>();
        }


        public FieldSetItem this[string name]
        {
            get
            {
                if (ItemsDic.ContainsKey(name) && ItemsDic[name] is FieldSetItem) return ItemsDic[name] as FieldSetItem;
                return null;
            }
        }

        protected override void CreateChildControls()
        {
            #region init panels
            // init container
            //HtmlGenericControl p = GetElement("div", "fieldset");
            //this.Controls.Add(p);
            WebControl p = this;

            if (string.IsNullOrEmpty(this.CssClass)) this.CssClass = "fieldset";
            // init header
            if (!string.IsNullOrEmpty(Title))
            {
                _hgcLegend = GetElement("div", this.TitleCssName);
                Label lbl1 = new Label();
                lbl1.Text = Title;
                _hgcLegend.Controls.Add(lbl1);
                p.Controls.Add(_hgcLegend);

                if (!string.IsNullOrEmpty(TitleLinkText))
                {
                    _titleLink = new HyperLink();
                    _titleLink.Text = TitleLinkText;
                    _titleLink.NavigateUrl = TitleLinkUrl;
                    if (!string.IsNullOrEmpty(TitleLinkOnClick)) _titleLink.Attributes.Add("onclick", TitleLinkOnClick);
                    if (!string.IsNullOrEmpty(TitleLinkCssName)) _titleLink.CssClass = TitleLinkCssName;
                    _hgcLegend.Controls.Add(_titleLink);
                }
            }


            //if (HeaderRow != null && HeaderRow.Count > 0)
            //    foreach (var c in HeaderRow)
            //        _hgcLegend.Controls.Add(c);

            this.ChildControlsCreated = true;

            //init body
            _hgcFieldset = GetElement("div", BodyCssName);
            p.Controls.Add(_hgcFieldset);

            _currentDiv = _hgcFieldset;
            #endregion

            if (this.ShowSummary)
            {
                ValidationSummary vs = new ValidationSummary();
                vs.ForeColor = System.Drawing.Color.Empty;
                vs.ID = "vs" + this.ID;
                vs.CssClass = "validation-summary";
                vs.ShowSummary = ShowSummary;
                vs.ShowMessageBox = ShowMessageBox;
                vs.ValidationGroup = this.ValidationGroup;
                _currentDiv.Controls.Add(vs);
            }

            foreach (var row in Rows)
            {
                row.FieldSet = this;
                _currentDiv.Controls.Add(row.Control);
            }
        }

        public FieldSetRow GetRowByName(string name)
        {
            if (ItemsDic.ContainsKey(name))
            {
                return ItemsDic[name] as FieldSetRow;
            }
            return null;
        }

        static internal HtmlGenericControl GetElement(string tag, string cssClass)
        {
            HtmlGenericControl ctrl = new HtmlGenericControl(tag);
            if (!string.IsNullOrEmpty(cssClass)) ctrl.Attributes.Add("class", cssClass);
            return ctrl;
        }

        public void Bind(object obj)
        {
            if (obj != null)
            {
                foreach (var row in Rows)
                {
                    Bind(obj, row);
                }
            }
        }

        private void Bind(object obj, IFieldSetItem it)
        {
            if (it is FieldSetItem)
            {
                FieldSetItem item = it as FieldSetItem;
                if (!string.IsNullOrEmpty(item.Name))
                {
                    string fieldName = item.Name;

                    object val = TypeUtil.GetPropertyValue(obj, fieldName);
                    if (val != null)
                    {
                        if (val is DateTime)
                        {
                            DateTime dt = (DateTime)val;
                            if (dt.TimeOfDay.Ticks == 0)
                            {
                                item.Value = dt.ToShortDateString();
                            }
                            else
                            {
                                item.Value = dt.ToString();
                            }
                        }
                        else
                        {
                            if (val is string[])
                            {
                                item.Value = string.Join(",", val as string[]);
                            }
                            else
                            {
                                item.Value = val.ToString();
                            }
                        }
                    }
                }
            }
            else if (it is FieldSetRow)
            {
                foreach (var it2 in (it as FieldSetRow).Items)
                {
                    Bind(obj, it2);
                }
            }
        }


        /// <summary>
        /// Kod kismindan row eklendigi durumda, Item dictionary'sinin 
        /// yeniden olusturulmasini saglar
        /// </summary>
        public void RefreshDictionary()
        {
            _itemsDic = null;
        }

        public object UpdateObject(object obj)
        {
            if (obj != null)
            {
                foreach (var row in Rows)
                {
                    UpdateObject(obj, row);
                }
            }
            return obj;
        }

        public void UpdateObject(object obj, IFieldSetItem it)
        {
            if (it is FieldSetItem)
            {
                FieldSetItem item = it as FieldSetItem;
                if (item.Value != null && !string.IsNullOrEmpty(item.Name))
                {
                    try
                    {
                        string fieldName = item.Name;
                        object val = TypeUtil.SetPropertyValue(obj, fieldName, item.Value);
                    }
                    catch (Exception ex) { Debug.WriteLine("[ky:FieldSet " + item.Name + "]: " + ex.Message); }
                }
            }
            else if (it is FieldSetRow)
            {
                foreach (var it2 in (it as FieldSetRow).Items)
                {
                    UpdateObject(obj, it2);
                }
            }
        }

        public void BindXML(string xmldata)
        {
            FieldSet fs = this;
            XPathDocument xpdoc = new XPathDocument(new StringReader(xmldata));
            XPathNavigator xnav = xpdoc.CreateNavigator();

            foreach (FieldSetRow row in fs.Rows)
            {
                BindXML(xnav, row);
            }
        }

        private void BindXML(XPathNavigator xnav, IFieldSetItem it)
        {
            if (it is FieldSetItem)
            {
                if (it is FieldSetItem)
                {
                    FieldSetItem item = it as FieldSetItem;
                    if (!string.IsNullOrEmpty(item.Name))
                    {
                        string xpath = item.Name.Replace('.', '/');
                        var node = xnav.SelectSingleNode("*/" + xpath);
                        if (node != null) item.Value = node.Value;
                    }
                }
            }
            else if (it is FieldSetRow)
            {
                foreach (IFieldSetItem item in (it as FieldSetRow).Items)
                {
                    BindXML(xnav, item);
                }
            }
        }


        public XmlDocument GetXML(string rootNodeName)
        {
            return GetXML(rootNodeName, null);
        }

        public XmlDocument GetXML(string rootNodeName, string prefix)
        {
            FieldSet fs = this;

            XmlDocument xdoc = new XmlDocument();
            XmlNode root = xdoc.CreateElement(rootNodeName);
            xdoc.AppendChild(root);

            foreach (FieldSetRow row in fs.Rows)
            {
                BuildXML(xdoc, row, prefix);
            }
            return xdoc;
        }

        private void BuildXML(XmlDocument xdoc, IFieldSetItem it, string prefix)
        {
            if (it is FieldSetRow)
            {
                FieldSetRow row = it as FieldSetRow;
                foreach (IFieldSetItem it2 in row.Items)
                {
                    BuildXML(xdoc, it2, prefix);
                }
            }
            else if (it is FieldSetItem)
            {
                FieldSetItem item = it as FieldSetItem;
                if (!string.IsNullOrEmpty(item.Name))
                {
                    if (string.IsNullOrEmpty(prefix) || item.Name.StartsWith(prefix))
                        SetNodeValue(xdoc, item.Name, item.Value);
                }
            }
        }

        static void SetNodeValue(XmlDocument xdoc, string path, string value)
        {
            XmlNode root = xdoc.FirstChild;
            string[] nodeNames = path.Split('.', '/');
            foreach (string nn in nodeNames)
            {
                root = GetOrCreateNode(xdoc, root, nn);
            }
            root.AppendChild(xdoc.CreateTextNode(value));
        }

        static XmlNode GetOrCreateNode(XmlDocument xdoc, XmlNode parent, string nodeName)
        {
            foreach (XmlNode node in parent.ChildNodes)
            {
                if (node.Name == nodeName) return node;
            }
            return parent.AppendChild(xdoc.CreateElement(nodeName));
        }
    }
}
