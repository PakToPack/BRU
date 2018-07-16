using System;
using System.Collections.Generic;
using FixTricks.constructors;
using SQLite;
using FixTricks.scripts;


namespace FixTricks.consts
{
    public class GetParaNum
    {

        public bool IsAlarming(int hour, int minute)
        {
            Setting set = new Setting();
            if(set.IsAlarm)
                if ((hour == 8 && minute == 25) || (hour == 10 && minute == 20) || (hour == 12 && minute == 25) || (hour == 14 && minute == 15) || (hour == 16 && minute == 0))
                    return true;
            return false;
        }

        public int GetParaByTime(int hour, int minute)
        {
            if ((hour == 8 && minute >= 30) || (hour == 9) || (hour == 10 && minute < 25))
            {
                return 1;
            }
            else if ((hour == 10 && minute >= 25) || (hour == 11) || (hour == 12 && minute < 30))
            {
                return 2;
            }
            else if ((hour == 12 && minute >= 30) || (hour == 13) || (hour == 14 && minute < 20))
            {
                return 3;
            }
            else if ((hour == 14 && minute >= 20) || (hour == 15) || (hour == 16 && minute < 5))
            {
                return 4;
            }
            else if ((hour == 16 && minute >= 5) || (hour == 16) || (hour == 17 && minute < 20))
            {
                return 5;
            }
            else if ((hour == 17 && minute >= 20) || (hour > 17))
                return -1;
            else
                return 0;
        }

        public int GetWeek()
        {
            SQLiteConnection db = new SQLiteConnection(SysPath.DBPath);
            DateTime dTime = DateTime.Now;
            int month = dTime.Month;
            string[] DaysOfWheek = { "Monday", "Friday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday" };
            int dayOfWeek = (int)(dTime.DayOfWeek + 6) % 7;
            int day = dTime.Day - dayOfWeek;
            if (day < 0)
                day += 14;

            List<WeekSelect> wSelect1 = db.Query<WeekSelect>("SELECT * FROM WeekSelect WHERE month=" + month + " AND week_t=1");
            List<WeekSelect> wSelect2 = db.Query<WeekSelect>("SELECT * FROM WeekSelect WHERE month=" + month + " AND week_t=2");
            int week_t = 0;
            foreach (WeekSelect w1 in wSelect1)
            {
                string[] days = w1.days.Split(' ');
                foreach (string k in days)
                {
                    int x = 0;
                    int.TryParse(k, out x);
                    if (x == day)
                    {
                        week_t = 1;
                        break;
                    }
                }
                if (week_t != 0)
                    break;
            }
            if(week_t == 0)
            {
                foreach (WeekSelect w2 in wSelect2)
                {
                    string[] days = w2.days.Split(' ');
                    foreach (string k in days)
                    {
                        int x = 0;
                        int.TryParse(k, out x);
                        if (x == day)
                        {
                            week_t = 2;
                            break;
                        }
                    }
                }
            }

            return week_t;
        }
    }
}
