using System;
using System.Collections.Generic;
using System.Text;
using SQLite;

namespace FixTricks.constructors
{
    class WeekSelect
    {
        [PrimaryKey, AutoIncrement, Column("_id")]
        public int Id { get; set; }
        public int month { get; set; }
        public string days { get; set; }
        public int week_t { get; set; }
    }
}
