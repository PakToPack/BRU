using System;
using System.Collections.Generic;
using System.Text;
using SQLite;

namespace FixTricks.constructors
{
    class AllMyRates
    {
        [PrimaryKey, AutoIncrement, Column("_id")]
        public int Id { get; set; }
        public string discipline { get; set; }
        public string rate { get; set; }
        [MaxLength(2)]
        public string mark { get; set; }
    }
}
