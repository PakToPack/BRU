using SQLite;
using FixTricks.constructors;

namespace FixTricks.scripts
{
    class DBControl
    {
        SQLiteConnection db = new SQLiteConnection(SysPath.DBPath);

        public AppSettings appSet()
        {
            return db.Table<AppSettings>().First();
        }

        public void UpdateAppLink(string link)
        {
            AppSettings apps = appSet();
            apps.excelLink = link;
            db.Update(apps);
        }

        public void ReloadExcel()
        {
            db.DropTable<ParaX>();
            db.DropTable<WeekSelect>();
            db.CreateTable<ParaX>();
            db.CreateTable<WeekSelect>();
        }

        public void ReloadRates()
        {
            db.DropTable<AllMyRates>();
            db.DropTable<DBDisciplines>();
            db.CreateTable<AllMyRates>();
            db.CreateTable<DBDisciplines>();
        }

        public void CreateDB()
        {
            db.CreateTable<AllMyRates>();
            db.CreateTable<AppSettings>();
            db.CreateTable<DBDisciplines>();
            db.CreateTable<ParaX>();
            db.CreateTable<Users>();
            db.CreateTable<WeekSelect>();
        }
        public void DestroyDB()
        {
            db.DropTable<AllMyRates>();
            db.DropTable<AppSettings>();
            db.DropTable<DBDisciplines>();
            db.DropTable<ParaX>();
            db.DropTable<Users>();
            db.DropTable<WeekSelect>();
        }
    }
}
