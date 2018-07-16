using System;
using System.IO;

namespace FixTricks
{
    class SysPath
    {
        public static string DBPath
        {
            get
            {
                return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "database.db3");
            }
        }
        public static string ExcelPath
        {
            get
            {
                return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "ief.xlsx");
            }
        }
    }
}
