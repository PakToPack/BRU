using System.Threading.Tasks;
using FixTricks.views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace FixTricks
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainView : TabbedPage
    {
        public MainView (bool needload = true)
        {
            InitializeComponent();
            if(needload)
                LoadPage();
        }

        public async void LoadPage()
        {
            await Task.Run(() => {
                Today td = new Today(false);
                td.LoadPage();
                AllDays ad = new AllDays(false);
                ad.LoadPage();
                MainRate mr = new MainRate(false);
                mr.LoadRates();
                UserInfo ui = new UserInfo(false);
                ui.LoadStat();
                NavigationPage navToday = new NavigationPage(td);
                
                navToday.Icon = "today.png";
                NavigationPage allDays = new NavigationPage(ad);
                allDays.Icon = "calendar.png";
                NavigationPage rate = new NavigationPage(mr);
                rate.Icon = "rates.png";
                NavigationPage usr = new NavigationPage(ui);
                usr.Icon = "profile.png";
                Device.BeginInvokeOnMainThread(() =>
                {
                    Children.Add(navToday);
                    Children.Add(allDays);
                    Children.Add(rate);
                    Children.Add(usr);
                });
            });
        }
    }
}