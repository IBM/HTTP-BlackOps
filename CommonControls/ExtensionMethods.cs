using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CommonControls
{
    public static class ExtensionMethods
    {

        public static void InvokeIt<T>(this T ctl, Action<T> action) 
            where T: Control
        {
            if (ctl.InvokeRequired)
                ctl.Invoke(action);
            else
                action(ctl);
        }

        public static bool EqualsIgnoreCase(this string str, string otherString)
        {
            return str.Equals(otherString, StringComparison.InvariantCultureIgnoreCase);
        }

    }
}
