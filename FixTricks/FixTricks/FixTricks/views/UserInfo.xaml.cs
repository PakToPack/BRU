using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using FixTricks.scripts;
using FixTricks.constructors;
using SQLite;

namespace FixTricks.views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class UserInfo : ContentPage
	{
        private SQLiteConnection db = new SQLiteConnection(SysPath.DBPath);
        public UserInfo (bool needload = true)
		{
			InitializeComponent();
            if(needload)
                LoadStat();
		}

        public async void LoadStat()
        {
            await Task.Run(() => {
                Users usr = db.Table<Users>().First();

                Name_Surname.Text = usr.name + " " + usr.surname;
                Zachetka.Text = "Номер зачётки: " + usr.studak;
                Group.Text = "Группа: " + usr.group;
                Podgroup.Text = "Подгруппа: " + usr.podgroup;
            });
        }

        public async void ExitApp()
        {
            DBControl dbc = new DBControl();
            dbc.DestroyDB();
            Setting setting = new Setting();
            if (!setting.CheckForInternetConnection())
            {
                MessagingCenter.Send<object, string>(this, "SendToast", "Сначала подключитесь к интернету!");
                return;
            }
            MessagingCenter.Send<object, string>(this, "ControlService", "stop");
            await Task.Run(() =>
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    Application.Current.MainPage = new LogIn();
                });
            });
        }
	}
}