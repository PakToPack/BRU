using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using FixTricks.constructors;
using SQLite;

namespace FixTricks.scripts
{
    public class Setting
    {
        public bool CheckForInternetConnection()
        {
            try
            {
                using (var client = new WebClient())
                using (var stream = client.OpenRead("http://clients3.google.com/generate_204"))
                {
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        public bool IsAlarm
        {
            get
            {
                SQLiteConnection db = new SQLiteConnection(SysPath.DBPath);
                AppSettings appSettings = db.Table<AppSettings>().First();
                return appSettings.AlarmOn5;
            }
            set
            {
                SQLiteConnection db = new SQLiteConnection(SysPath.DBPath);
                AppSettings appSettings = db.Table<AppSettings>().First();
                appSettings.AlarmOn5 = value;
                db.Update(appSettings);
            }
        }

        public bool IsUpdate
        {
            get
            {
                SQLiteConnection db = new SQLiteConnection(SysPath.DBPath);
                AppSettings appSettings = db.Table<AppSettings>().First();
                return appSettings.UpdateCheck;
            }
            set
            {
                SQLiteConnection db = new SQLiteConnection(SysPath.DBPath);
                AppSettings appSettings = db.Table<AppSettings>().First();
                appSettings.AlarmOn5 = value;
                db.Update(appSettings);
            }
        }

        public string CheckExcelUpdate()
        {
            if (!CheckForInternetConnection())
                return null;
            ReloadData relData = new ReloadData();
            string[,] _courses = relData.LoadCourses();
            DBControl dbc = new DBControl();
            AppSettings appSet = dbc.appSet();

            for (int i = 0; i < _courses.Length / 2; i++)
            {
                if (_courses[i, 0] == appSet.userCourse)
                {
                    if (_courses[i, 1] != appSet.excelLink)
                        return _courses[i, 1];
                    break;
                }
            }

            return null;
        }
    }
}
