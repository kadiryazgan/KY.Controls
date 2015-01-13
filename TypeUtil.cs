using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Collections;
using System.Globalization;
using System.IO;
using System.Xml;

namespace Ky.Controls.Utils
{
    internal class TypeUtil
    {
        public static object GetPropertyValue(object obj, string propertyName)
        {
            if (obj == null) return null;

            string[] p = propertyName.Split(char.Parse("."));
            string[] k = p[0].Split(char.Parse("#"));
            propertyName = k[0];
            int idx = (k.Length > 1) ? int.Parse(k[1]) : -1;

            Type objType = obj.GetType();
            PropertyInfo pi = objType.GetProperty(propertyName);

            if (pi == null) return null;
            object objVal;
            if (idx > -1)
            {
                if (pi.GetValue(obj, null) is IList)
                {
                    IList list = (IList)pi.GetValue(obj, null);
                    if (list.Count > idx) return list[idx]; else return null;
                }
                else
                {
                    objVal = pi.GetValue(obj, null);
                }
            }
            else objVal = pi.GetValue(obj, null);

            if (p.Length == 1)
            {
                return objVal;
            }
            else
            {
                StringBuilder sb = new StringBuilder();
                for (int i = 1; i < p.Length; i++) { sb.Append((i > 1 ? "." : "") + p[i]); }
                return GetPropertyValue(objVal, sb.ToString());
            }
        }

        public static object SetPropertyValue(object obj, string propertyName, string value)
        {
            string[] p = propertyName.Split(char.Parse("."));
            string[] k = p[0].Split(char.Parse("#"));
            propertyName = k[0];
            int idx = (k.Length > 1) ? int.Parse(k[1]) : -1;

            Type objType = obj.GetType();
            PropertyInfo pi = objType.GetProperty(propertyName);

            if (pi == null) throw new Exception(objType.Name + " class does not have a property named '" + propertyName + "'");
            object objVal = null;
            if (idx > -1)
            {
                if (pi.GetValue(obj, null) is IList)
                {
                    IList list = (IList)pi.GetValue(obj, null);
                    Type innerType = list.GetType().GetGenericArguments()[0];
                    if (p.Length == 1)
                    {
                        if (list.Count > idx)
                        {
                            list[idx] = ConvertType(value, innerType);
                            return list[idx];
                        }
                        else if (list.Count == idx)
                        {
                            list.Add(ConvertType(value, innerType));
                            return list[idx];
                        }
                        else
                        {
                            throw new IndexOutOfRangeException("TypeUtil: " + objType.Name + "." + propertyName);
                        }
                    }
                    else
                    {
                        if (list.Count > idx)
                        {
                            objVal = list[idx];
                        }
                        else if (list.Count == idx)
                        {
                            list.Add(innerType.GetConstructor(Type.EmptyTypes).Invoke(new object[] { }));
                            objVal = list[idx];
                        }
                        else
                        {
                            throw new IndexOutOfRangeException("TypeUtil: " + objType.Name + "." + propertyName);
                        }
                    }
                }
                else
                {
                    objVal = pi.GetValue(obj, null);
                }
            }
            else objVal = pi.GetValue(obj, null);

            if (p.Length == 1)
            {
                SetPropertyValue(pi, obj, value);
                return objVal;
            }
            else
            {
                StringBuilder sb = new StringBuilder();
                for (int i = 1; i < p.Length; i++) { sb.Append((i > 1 ? "." : "") + p[i]); }
                return SetPropertyValue(objVal, sb.ToString(), value);
            }
        }

        internal static void SetPropertyValue(PropertyInfo pi, object obj, object value)
        {
            if (pi != null)
            {
                object o = ConvertType(value, pi.PropertyType);
                if (o != null)
                {
                    pi.SetValue(obj, o, null);
                }
                else
                {
                    throw new InvalidCastException("Type " + pi.Name + " requires a constructor or a parse method which accepts a parameter of type " + value.GetType().ToString());
                }
            }
        }

        internal static object ConvertType(object value, Type type)
        {
            if (type.IsEnum)
            {
                return Enum.Parse(type, value.ToString());
            }
            else if (type.Namespace == "System")
            {
                if (type.Name == "DateTime" && string.IsNullOrEmpty(value.ToString()))
                {
                    return DateTime.MinValue;
                }
                else
                {
                    if (type.Name != "String" && value is string && string.IsNullOrEmpty(value.ToString())) value = "0";
                    return Convert.ChangeType(value, type);
                }
            }
            else
            {
                if (value is string)
                {
                    MethodInfo mi = type.GetMethod("Parse");
                    if (mi != null) return mi.Invoke(null, new object[] { value });
                }

                ConstructorInfo ci = type.GetConstructor(new Type[] { value.GetType() });
                if (ci != null)
                {
                    return ci.Invoke(new object[] { value });
                }
                else
                {
                    return null;
                }
            }
        }

        static public string GetJSON(object obj, params string[] propertyNames)
        {
            if (obj == null) return "null";
            if (obj is IList)
            {
                IList array = (IList)obj;

                StringBuilder sb = new StringBuilder("[");
                foreach (object item in array)
                {
                    if (sb.Length > 1) sb.Append(",");
                    if (item is int || item is double || item is float || item is decimal || item is bool || item is string || item is byte || item is DateTime || item == null)
                    {
                        sb.Append(ConvertToJSON(item));
                    }
                    else
                    {
                        sb.Append(GetJSON(item, propertyNames));
                    }
                }
                sb.Append("]");
                return sb.ToString();
            }
            else
            {
                StringBuilder sb = new StringBuilder("{");

                foreach (string prop in propertyNames)
                {
                    if (sb.Length > 1) sb.Append(",");

                    int di = prop.IndexOf(":");
                    if (di > -1)
                    {
                        string propName = prop.Substring(0, di);
                        string[] propProps = prop.Substring(di + 1).Split(',');
                        sb.Append(propName + ":" + GetJSON(GetPropertyValue(obj, propName), propProps));
                    }
                    else
                    {
                        object val = GetPropertyValue(obj, prop);

                        sb.Append("\"" + prop + "\":" + ConvertToJSON(val));
                    }
                }
                sb.Append("}");
                return sb.ToString();
            }
        }

        static private string ConvertToJSON(object val)
        {
            if (val is DateTime)
            {
                DateTime baseDate = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                return "\"\\/Date(" + ((DateTime)val).Subtract(baseDate).TotalMilliseconds + ")\\/\"";

                //return "\"/Date(" + ((DateTime)val).+ ")/\"";

                //return "\"new Date(" + ((DateTime)val).ToString("yyyy,") + (((DateTime)val).Month - 1).ToString() + ((DateTime)val).ToString(",d,h,m,s") + ")\"";
            }
            else if (val is bool)
            {
                return val.ToString().ToLower();
            }
            else if (val == null)
            {
                return "null";
            }
            else if (val is Array)
            {
                return GetJSON(val);
            }
            else if (val is int || val is long || val is byte)
            {
                return val.ToString();
            }
            else if (val is double)
            {
                return ((double)val).ToString(new CultureInfo("en-US"));
            }
            else
            {
                return "\"" + val.ToString().Replace(@"\", @"\\").Replace("\"", "\\\"").Replace("\r\n", "\\n").Replace("\n", "\\n").Replace("/", "\\/") + "\"";
            }
        }

        static public string GetXML(object obj)
        {
            return GetXML(obj, null, null);
        }

        static public string GetXML(object obj, string[] includeProperties)
        {
            return GetXML(obj, includeProperties, null);
        }

        static public string GetXML(object obj, string[] includeProperties, string[] excludeProperties)
        {
            using (StringWriter sw = new StringWriter())
            {
                XmlTextWriter xtw = new XmlTextWriter(sw);
                GetXML(obj, includeProperties, excludeProperties, xtw, 0);
                xtw.Flush();
                return sw.ToString();
            }
        }

        static internal void GetXML(object obj, string[] includeProperties, string[] excludeProperties, XmlTextWriter xtw, int depth)
        {
            if (depth > 3 || obj == null) return;
            System.Type objType = obj.GetType();

            if (depth == 0)
            {
                xtw.WriteStartElement(objType.Name);
            }
            else
            {
                xtw.WriteAttributeString("Type", objType.FullName);
            }
            PropertyInfo[] pis = objType.GetProperties();
            foreach (var pi in pis)
            {
                object val = pi.GetValue(obj, null);
                bool ok = false;

                if ((includeProperties != null))
                {
                    ok = Array.IndexOf(includeProperties, pi.Name) != -1;
                }
                else if (excludeProperties != null)
                {
                    ok = Array.IndexOf(excludeProperties, pi.Name) == -1;
                }
                else
                {
                    ok = true;
                }

                if (ok)
                {
                    if (pi.PropertyType.IsVisible)
                    {
                        xtw.WriteStartElement(pi.Name);
                        if (pi.PropertyType.IsSerializable)
                        {
                            xtw.WriteAttributeString("Type", pi.PropertyType.FullName.StartsWith("System.") ? pi.PropertyType.Name : pi.PropertyType.FullName);
                            if (val != null)
                            {
                                xtw.WriteString(val.ToString());
                            }
                        }
                        else
                        {
                            GetXML(val, FilterArray(includeProperties, pi.Name + "."), FilterArray(excludeProperties, pi.Name + "."), xtw, depth + 1);
                        }
                        xtw.WriteEndElement();
                    }
                }
            }
            if (depth == 0) xtw.WriteEndElement();
        }


        static public void SetXML(object obj, string xml)
        {
            XmlDocument xdoc = new XmlDocument();
            xdoc.InnerXml = xml;
            SetXML(obj, xdoc.FirstChild);
        }

        static public void SetXML(object obj, XmlNode xn)
        {
            if (obj == null || xn == null) return;
            System.Type type = obj.GetType();

            foreach (XmlNode n in xn)
            {
                if (n.HasChildNodes)
                {
                    if (n.FirstChild.NodeType == XmlNodeType.Text)
                    {
                        try { TypeUtil.SetPropertyValue(obj, n.Name, n.FirstChild.Value); }
                        catch { }
                    }
                    else
                    {
                        PropertyInfo pi = type.GetProperty(n.Name);
                        if (pi != null) SetXML(pi.GetValue(obj, null), n);
                    }
                }
            }
        }

        static internal string[] FilterArray(string[] input, string prefix)
        {
            if (input == null) return null;
            if (prefix == null) return input;
            return Array.ConvertAll<string, string>(Array.FindAll<string>(input, delegate(string a) { return a.StartsWith(prefix); }), delegate(string a) { return a.Substring(prefix.Length); });
        }
    }
}