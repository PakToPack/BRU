using System;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using FixTricks.Lists;
using FixTricks.consts;
using FixTricks.scripts;
using FixTricks.constructors;
using System.Collections.ObjectModel;

namespace FixTricks.views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class Today : ContentPage
	{
        private int week_type { get; set; }
        private ObservableCollection<ParaTypeX> pType = new ObservableCollection<ParaTypeX>();
        private bool _IsBusy = true;

        public Today(bool needload = true)
		{
			InitializeComponent ();
            MessagingCenter.Send<object, string>(this, "ControlService", "start");
            MessagingCenter.Subscribe<object, string>(this, "ControlService", (s, e) => {
                if (e == "stop")
                    MessagingCenter.Unsubscribe<object, string>(this, "UpdateLabel");
                else
                    _IsBusy = true;
            });

            MessagingCenter.Subscribe<object, string>(this, "UpdateLabel", async (s, e) => {
                await Task.Run(() => {
                    if (_IsBusy)
                    {
                        Device.BeginInvokeOnMainThread(() =>
                        {
                            pType = new ObservableCollection<ParaTypeX>();
                            SetPage();
                        });
                    }
                });
            });
            if(needload)
                LoadPage();
        }

        public async void LoadPage()
        {
            await Task.Run(() => {
                SetPage();
            });
        }

        private void SetPage()
        {
            GetParaNum gpn = new GetParaNum();
            GetMyPara gpm = new GetMyPara();
            DateTime dTime = DateTime.Now;
            Setting set = new Setting();
            int para = gpn.GetParaByTime(dTime.Hour, dTime.Minute);

            int not_para = gpn.GetParaByTime(dTime.Hour, dTime.Minute + 5);

            if (gpn.IsAlarming(dTime.Hour, dTime.Minute) && gpm.ExistsPara(not_para))
            {
                ParaX pX = gpm.GetParaByNum(not_para);
                MessagingCenter.Send<object, string>(this, "SendNotify", pX.name+" через 5 минут!");
            }

            string time1 = "";
            if (para > 0 && gpm.ExistsPara(para))
                time1 = gpm.p_time[para];
            string time2 = null;
            if (gpm.ExistsPara(para + 1))
                time2 = gpm.p_time[para + 1];
            ParaTypeX paraNext;
            ParaTypeX paraNow;
            if (para > 0 && gpm.ExistsPara(para))
            {
                string[] times = time1.Split('_');
                ParaX pX = gpm.GetParaByNum(para);
                paraNow = new ParaTypeX() { Para = pX.name, Title = "Сейчас", ParaStart = times[0], ParaEnd = times[1] };
                pType.Add(paraNow);
            }
            if (time2 != null)
            {
                string[] times = time2.Split('_');
                ParaX pX = gpm.GetParaByNum(para + 1);
                paraNext = new ParaTypeX() { Para = pX.name, Title = "Следующая", ParaStart = times[0], ParaEnd = times[1] };
                pType.Add(paraNext);
            }
            if ((para <= 0 || !gpm.ExistsPara(para)) && time2 == null)
            {
                paraNext = new ParaTypeX() { Para = "Занятия закончились", Title = "Сейчас" };
                pType.Add(paraNext);
            }
            SetPara.ItemsSource = null;
            SetPara.ItemsSource = pType;
        }

        public void DeColor(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem == null)
                return;
            ((ListView)sender).SelectedItem = null;
        }
    }
}