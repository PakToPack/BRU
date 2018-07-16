using System.Collections.Generic;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using FixTricks.Lists;
using SQLite;
using FixTricks.constructors;
using System.Windows.Input;
using FixTricks.scripts;

namespace FixTricks.views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class MainRate : ContentPage
	{
		public MainRate(bool needload = true)
		{
			InitializeComponent ();
            BindingContext = this;
            if(needload)
                LoadRates();
		}

        private bool _isRefreshing = false;
        public bool IsRefreshing
        {
            get { return _isRefreshing; }
            set
            {
                _isRefreshing = value;
                OnPropertyChanged(nameof(IsRefreshing));
            }
        }

        public ICommand RefreshCommand
        {
            get
            {
                return new Command(async () =>
                {
                    await Task.Run(() =>
                    {
                        Setting set = new Setting();
                        if (!set.CheckForInternetConnection())
                        {
                            IsRefreshing = false;
                            return;
                        }
                        MessagingCenter.Send<object, string>(this, "ControlService", "stop");
                        Device.BeginInvokeOnMainThread(() =>
                        {
                            modd.IsVisible = true;
                            ratingDrawer.IsVisible = false;
                        });
                        ReloadData relData = new ReloadData();
                        SQLiteConnection db = new SQLiteConnection(SysPath.DBPath);
                        Users usr = db.Table<Users>().First();
                        relData.ReloadRate(usr.studak, usr.podgroup, usr.group);
                    });
                    LoadRates();
                });
            }
        }

        public async void LoadRates()
        {
            await Task.Run(() => {
                Device.BeginInvokeOnMainThread(() =>
                {
                    modd.IsVisible = true;
                    ratingDrawer.IsVisible = false;
                });
                List<RatingTypeX> rtx = new List<RatingTypeX>();
                SQLiteConnection db = new SQLiteConnection(SysPath.DBPath);
                var dbd = db.Query<DBDisciplines>("SELECT * FROM DBDisciplines");
                for (int i = 0; i < dbd.Count; i++)
                {
                    RatingTypeX rtt = new RatingTypeX() { Title = dbd[i].name };
                    rtx.Add(rtt);
                }
                Device.BeginInvokeOnMainThread(() =>
                {
                    ratingDrawer.ItemsSource = null;
                    ratingDrawer.ItemsSource = rtx;
                    modd.IsVisible = false;
                    ratingDrawer.IsVisible = true;
                    IsRefreshing = false;
                    MessagingCenter.Send<object, string>(this, "ControlService", "start");
                });
            });
        }

        public async void OnMenuSelect(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem == null)
                return;
            var item = (RatingTypeX)e.SelectedItem;
            await Navigation.PushAsync(new SpecRate(item.Title));
            ((ListView)sender).SelectedItem = null;
        }

    }
}