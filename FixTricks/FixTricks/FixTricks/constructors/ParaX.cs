using System;
using System.Collections.Generic;
using System.Text;
using SQLite;

namespace FixTricks.constructors
{
    class ParaX
    {
        [PrimaryKey, AutoIncrement, Column("_id")]
        public int Id { get; set; }
        public string name { get; set; }
        public int day { get; set; }
        public int week { get; set; }
        public int number { get; set; }
    }
}
