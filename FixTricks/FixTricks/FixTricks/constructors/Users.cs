using System;
using System.Collections.Generic;
using System.Text;
using SQLite;

namespace FixTricks.constructors
{
    class Users
    {
        [PrimaryKey, AutoIncrement, Column("_id")]
        public int Id { get; set; }
        public string studak { get; set; }
        public string name { get; set; }
        public string surname { get; set; }
        public string group { get; set; }
        public int podgroup { get; set; }
    }
}
