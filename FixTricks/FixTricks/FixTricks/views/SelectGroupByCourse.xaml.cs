using System;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using FixTricks.scripts;
using FixTricks.constructors;
using SQLite;

namespace FixTricks.views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class SelectGroupByCourse : ContentPage
	{
        private string _link;
		public SelectGroupByCourse (string link)
		{
			InitializeComponent ();
            _link = link;
            SetPicker();
		}

        ReloadData relData = new ReloadData();
        public async void SetPicker()
        {
            TextX.IsVisible = true;
            TextX.Text = "Подождите, мы строим коммунизм!";
            string[] _groups = await Task.Run(() => relData.LoadGroups(_link));
            await Task.Run(() =>
            {
                try
                {
                    if (_groups.Length <= 0)
                        throw new ArgumentException("No _groups detected! (SelectGroupByCourse:31)");

                    Device.BeginInvokeOnMainThread(() =>
                    {
                        for (int i = 0; i < _groups.Length; i++)
                        {
                            GroupPick.Items.Add(_groups[i]);
                        }
                    });
                }
                catch
                {

                }
            });
            TextX.Text = "Выберите вашу группу!";
            GroupPick.IsVisible = true;
            Btn.IsVisible = true;
            ActInd.IsVisible = false;
        }

        public async void LoadExcel()
        {
            if (GroupPick.SelectedItem == null || GroupPick.Items[GroupPick.SelectedIndex] == "")
            {
                MessagingCenter.Send<object, string>(this, "SendToast", "Все поля должны быть заполнены!");
                return;
            }
            
            GroupPick.IsVisible = false;
            Btn.IsVisible = false;
            ActInd.IsVisible = true;
            await Task.Run(() =>
            {
                SQLiteConnection db = new SQLiteConnection(SysPath.DBPath);
                Users usr = db.Table<Users>().First();
                usr.group = GroupPick.Items[GroupPick.SelectedIndex];
                db.Update(usr);
            });

            TextX.Text = "Бежим на кафедру за расписанием!";
            bool set_excel = await Task.Run(() => relData.ReloadExcel(_link));
            await Task.Run(() =>
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    if (set_excel)
                        App.Current.MainPage = new MainView();
                });
            });
            ActInd.IsVisible = false;
            GroupPick.IsVisible = true;
        }
	}
}