using System.Collections.Generic;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using FixTricks.Lists;
using FixTricks.scripts;
using FixTricks.constructors;
using System.Windows.Input;

namespace FixTricks.views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class AllDays : ContentPage
	{
		public AllDays(bool needload = true)
		{
			InitializeComponent ();
            BindingContext = this;
            if (needload)
                LoadPage();
        }

        public async void LoadPage()
        {
            await Task.Run(() => {
                List<AllDaysList> aldList = new List<AllDaysList>();
                GetMyPara gmp = new GetMyPara();
                if (gmp.ExistsDay(1))
                    aldList.Add(new AllDaysList() { Day = "Понедельник", DayNum = 1 });
                if (gmp.ExistsDay(2))
                    aldList.Add(new AllDaysList() { Day = "Вторник", DayNum = 2 });
                if (gmp.ExistsDay(3))
                    aldList.Add(new AllDaysList() { Day = "Среда", DayNum = 3 });
                if (gmp.ExistsDay(4))
                    aldList.Add(new AllDaysList() { Day = "Четверг", DayNum = 4 });
                if (gmp.ExistsDay(5))
                    aldList.Add(new AllDaysList() { Day = "Пятница", DayNum = 5 });
                if (gmp.ExistsDay(6))
                    aldList.Add(new AllDaysList() { Day = "Суббота", DayNum = 6 });
                Device.BeginInvokeOnMainThread(() => {
                    SetDays.ItemsSource = aldList;
                });
            });
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
                            SetDays.IsVisible = false;
                        });

                        Setting sett = new Setting();
                        string exc_link = sett.CheckExcelUpdate();
                        if (exc_link == null)
                        {
                            IsRefreshing = false;
                            Device.BeginInvokeOnMainThread(() =>
                            {
                                modd.IsVisible = false;
                                SetDays.IsVisible = true;
                            });
                        }
                        else
                            UpdateExcel(exc_link);
                    });
                });
            }
        }

        public async void UpdateExcel(string link)
        {
            await Task.Run(() => {
                ReloadData relData = new ReloadData();
                relData.ReloadExcel(link);
                Device.BeginInvokeOnMainThread(() =>
                {
                    modd.IsVisible = false;
                    SetDays.IsVisible = true;
                    IsRefreshing = false;
                });
            });
        }

        public async void SetPage(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem == null)
                return;
            var item = (AllDaysList)e.SelectedItem;
            await Navigation.PushAsync(new SpecDays(item.DayNum));
            ((ListView)sender).SelectedItem = null;
        }
	}
}