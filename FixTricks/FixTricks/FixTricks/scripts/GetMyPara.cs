using System;
using System.Collections.Generic;
using FixTricks.constructors;
using SQLite;
using FixTricks.consts;

namespace FixTricks.scripts
{
    class GetMyPara
    {
        public string[] p_time { get; set; } = { "", "8:30_10:05", "10:25_12:00", "12:30_14:05", "14:20_15:55", "16:05_17:40" };
        public ParaX GetParaByNum(int para)
        {
            SQLiteConnection db = new SQLiteConnection(SysPath.DBPath);

            GetParaNum gpn = new GetParaNum();
            int week_t = gpn.GetWeek();
            DateTime dTime = DateTime.Now;
            int dayOfWeek = (int)(dTime.DayOfWeek + 6) % 7 + 1;

            return db.Query<ParaX>("SELECT * FROM ParaX WHERE week=" + week_t + " AND day=" + dayOfWeek + " AND number=" + para)[0];
        }

        public List<ParaX> GetAllDay(int week, int day)
        {
            SQLiteConnection db = new SQLiteConnection(SysPath.DBPath);

            return db.Query<ParaX>("SELECT * FROM ParaX WHERE week=" + week + " AND day=" + day);
        }
        public bool ExistsDay(int day)
        {
            SQLiteConnection db = new SQLiteConnection(SysPath.DBPath);
            if (db.Query<ParaX>("SELECT * FROM ParaX WHERE day=" + day).Count != 0)
                return true;
            return false;
        }
        public bool ExistsPara(int para)
        {
            DateTime dTime = DateTime.Now;
            int day = (int)(dTime.DayOfWeek + 6) % 7 + 1;

            GetParaNum gpn = new GetParaNum();
            int week_t = gpn.GetWeek();
            SQLiteConnection db = new SQLiteConnection(SysPath.DBPath);
            if (db.Query<ParaX>("SELECT * FROM ParaX WHERE day=" + day + " AND number="+para + " AND week="+week_t).Count != 0)
                return true;
            return false;
        }
    }
}
