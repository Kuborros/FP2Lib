using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FP2Lib.Saves
{
    public class SaveRedirectHandler
    {
        public static string CurrentModRedirectProfile { get; internal set; } = "";
        internal Dictionary<int,string> modRedirects = new Dictionary<int,string>();


    }
}
