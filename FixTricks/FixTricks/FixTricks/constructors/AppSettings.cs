using SQLite;

namespace FixTricks.constructors
{
    class AppSettings
    {
        [PrimaryKey, AutoIncrement, Column("_id")]
        public int Id { get; set; }
        public string excelLink { get; set; }
        public string userCourse { get; set; }
        public bool UpdateCheck { get; set; } = false;
        public bool AlarmOn5 { get; set; } = false;
    }
}
