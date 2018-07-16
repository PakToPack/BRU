using System.Collections.Generic;
using System.Threading.Tasks;
using SQLite;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using FixTricks.constructors;
using FixTricks.Lists;

namespace FixTricks.views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class SpecRate : ContentPage
	{
        public string discx { get; set; }
		public SpecRate (string item)
		{
			InitializeComponent ();
            this.discx = item;
            LoadSpecRate();
		}

        public async void LoadSpecRate()
        {
            await Task.Run(() => {
                Device.BeginInvokeOnMainThread(() =>
                {
                    Title = this.discx;
                    SQLiteConnection db = new SQLiteConnection(SysPath.DBPath);
                    RateBuilding xrb;
                    List<RateBuilding> rb = new List<RateBuilding>();

                    var db1 = db.Query<AllMyRates>("SELECT * FROM AllMyRates WHERE discipline='" + discx + "'");
                    foreach (AllMyRates dx in db1)
                    {
                        if (dx.mark != "-" && dx.mark != "" && dx.mark != "Не изучает")
                        {
                            xrb = new RateBuilding() { Mark = dx.mark, Title = dx.rate };
                            rb.Add(xrb);
                        }
                    }
                    if (rb.Count == 0)
                        rb.Add(new RateBuilding() { Title = "Нет данных" });
                    myProgress.ItemsSource = rb;
                });
            });
        }

        public void deselect(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem == null)
                return;
            ((ListView)sender).SelectedItem = null;
        }
    }
}