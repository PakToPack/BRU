using System;
using System.Collections.Generic;
using System.Text;
using SQLite;

namespace FixTricks.constructors
{
    class DBDisciplines
    {
        [PrimaryKey, AutoIncrement, Column("_id")]
        public int Id { get; set; }
        public string name { get; set; }
    }
}
