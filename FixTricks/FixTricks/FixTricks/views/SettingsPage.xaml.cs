using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using FixTricks.scripts;

namespace FixTricks.views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class SettingsPage : ContentPage
    {
        Setting set = new Setting();

		public SettingsPage ()
		{
			InitializeComponent ();
            AlarmSwitcher.On = set.IsAlarm;
            this.BindingContext = this;
		}

        public void Setter()
        {
            set.IsAlarm = AlarmSwitcher.On;
        }
	}
}