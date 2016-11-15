using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CNTK_FastRCNN_Sample
{
    public static class Setting
    {
        /// <summary>
        /// Program base path,with "\" at the last.
        /// </summary>
        public static readonly string BasePath = System.AppDomain.CurrentDomain.BaseDirectory;

        public static string labelSet = ConfigurationManager.AppSettings["LabelSet"];
        public static void Reload()
        {
            ConfigurationManager.RefreshSection("appSettings");
        }
    }
}
