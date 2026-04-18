using System;
using System.Collections;
using System.Reflection;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class WebUtility : Utility
{
    public class MasterPageHelper
    {
        static readonly Type _masterType = typeof(MasterPage);
        static readonly PropertyInfo _contentTemplatesProp = _masterType.GetProperty("ContentTemplates", BindingFlags.GetProperty | BindingFlags.NonPublic | BindingFlags.Instance);

        public static bool IsDefinedOrHasContent(ContentPlaceHolder cph)
        {
            return IsDefined(cph) || HasContent(cph);
        }

        public static bool IsDefined(ContentPlaceHolder cph)
        {
            IDictionary templates = null;
            MasterPage master = cph.Page.Master;

            while (templates == null && master != null)
            {
                templates = (IDictionary)_contentTemplatesProp.GetValue(master, null);
                master = master.Master;
            }

            if (templates == null)
                return false;

            bool isSpecified = false;

            foreach (string key in templates.Keys)
            {
                if (key == cph.ID)
                {
                    isSpecified = true;

                    break;
                }
            }

            return isSpecified;
        }

        public static bool HasContent(ContentPlaceHolder cph)
        {
            if (cph.Controls.Count == 0)
            {
                return false;
            }
            else if (cph.Controls.Count == 1)
            {
                if (cph.Controls[0] is Literal)
                {
                    //if this is cms dynamic literal control
                    Literal c = cph.Controls[0] as Literal;
                    if (c == null || string.IsNullOrEmpty(c.Text) || IsWhiteSpace(c.Text))
                        return false;
                }
                else
                {
                    //if this is static html
                    LiteralControl c = cph.Controls[0] as LiteralControl;
                    if (c == null || string.IsNullOrEmpty(c.Text) || IsWhiteSpace(c.Text))
                        return false;
                }
            }
            else if (cph.Controls.Count == 2)
            {
                return ControlHasContent(cph.Controls[0]) || ControlHasContent(cph.Controls[1]);
            }

            return true;
        }

        private static bool ControlHasContent(Control objControl)
        {
            if (objControl is Literal)
            {
                //if this is cms dynamic literal control
                Literal c = objControl as Literal;
                if (c == null || string.IsNullOrEmpty(c.Text) || IsWhiteSpace(c.Text))
                    return false;
            }
            else if (objControl is LiteralControl)
            {
                //if this is static html
                LiteralControl c = objControl as LiteralControl;
                if (c == null || string.IsNullOrEmpty(c.Text) || IsWhiteSpace(c.Text))
                    return false;
            }
            return true;
        }

        static bool IsWhiteSpace(string s)
        {
            for (int i = 0; i < s.Length; i++)
                if (!char.IsWhiteSpace(s[i]))
                    return false;

            return true;
        }
    }
}
