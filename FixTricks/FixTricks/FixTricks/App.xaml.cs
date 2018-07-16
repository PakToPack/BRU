using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SQLite;
using FixTricks.constructors;
using FixTricks.scripts;
using Xamarin.Forms;
using System.Threading.Tasks;
using FixTricks.views;

namespace FixTricks
{
	public partial class App : Application
	{
		public App ()
		{
			InitializeComponent();
            Load();
		}

        private async void Load()
        {
            await Task.Run(() => {
                SQLiteConnection db = new SQLiteConnection(SysPath.DBPath);
                DBControl dbc = new DBControl();
                Setting _Setting = new Setting();
                Device.BeginInvokeOnMainThread(() =>
                {
                    try
                    {
                        if (db.Table<Users>().Count() != 0)
                        {
                            if (db.Table<ParaX>().Count() == 0 || db.Table<AllMyRates>().Count() == 0 || db.Table<AppSettings>().Count() == 0)
                            {
                                dbc.DestroyDB();
                                throw new ArgumentException("Error! Uncompleted steps found!");
                            }
                            else
                                MainPage = new PreLoader();
                        }
                        else
                        {
                            throw new ArgumentException("Error! 0 users found!");
                        }
                    }
                    catch
                    {
                        if (_Setting.CheckForInternetConnection())
                            MainPage = new LogIn();
                        else
                            MainPage = new ErrorStart();
                    }
                });
            });
        }

        protected override void OnStart ()
		{
			// Handle when your app starts
		}

		protected override void OnSleep ()
		{
			// Handle when your app sleeps
		}

		protected override void OnResume ()
		{
            MessagingCenter.Send<object, string>(this, "UpdateLabel", "Hello from App");
        }
	}
}
