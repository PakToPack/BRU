using System.Collections.Generic;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using FixTricks.scripts;
using FixTricks.constructors;
using FixTricks.Lists;
using System.Linq;

namespace FixTricks.views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class SpecDays : ContentPage
    {
        public ObservableCollection<Grouping<string, ParaTypeX>> ParaGroups { get; set; }

        private int dnumx;
        string[] dayy = { "", "Понедельник", "Вторник", "Среда", "Четверг", "Пятница", "Суббота" };
        public SpecDays (int dnum)
		{
			InitializeComponent ();
            dnumx = dnum;
            Title = dayy[dnum];
            LoadWeeks();
		}

        public async void LoadWeeks()
        {
            int day = dnumx;
            await Task.Run(() =>
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    Spinn.IsVisible = true;
                    SetPara.IsVisible = false;
                });
                GetMyPara gmp = new GetMyPara();
                List<ParaX> UpperWeek = gmp.GetAllDay(1, day);
                List<ParaX> LowerWeek = gmp.GetAllDay(2, day);

                List<ParaTypeX> sDay = new List<ParaTypeX>();
                foreach (ParaX px in UpperWeek)
                {
                    int numb = px.number;
                    string time = gmp.p_time[numb];
                    string[] times = time.Split('_');
                    sDay.Add(new ParaTypeX() { Para = px.name, ParaStart = times[0], ParaEnd = times[1], Week = "Верхняя неделя" });
                }
                foreach (ParaX px in LowerWeek)
                {
                    int numb = px.number;
                    string time = gmp.p_time[numb];
                    string[] times = time.Split('_');
                    sDay.Add(new ParaTypeX() { Para = px.name, ParaStart = times[0], ParaEnd = times[1], Week = "Нижняя неделя" });
                }

                var groups = sDay.GroupBy(p => p.Week).Select(g => new Grouping<string, ParaTypeX>(g.Key, g));

                ParaGroups = new ObservableCollection<Grouping<string, ParaTypeX>>(groups);

                Device.BeginInvokeOnMainThread(() =>
                {
                    this.BindingContext = this;
                    SetPara.ItemsSource = ParaGroups;
                    Spinn.IsVisible = false;
                    SetPara.IsVisible = true;
                });
            });
        }

        public void DeColor(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem == null)
                return;
            ((ListView)sender).SelectedItem = null;
        }
	}
}