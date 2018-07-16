using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Threading;

namespace FixTricks
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class PreLoader : ContentPage
	{
		public PreLoader ()
		{
			InitializeComponent ();
            WaitLoading();
		}

        public async void WaitLoading()
        {
            MainView mV = null;
            await Task.Run(() => {
                mV = new MainView(false);
                mV.LoadPage();
                Thread.Sleep(1000);
            });
            await Task.Run(() => {
                Device.BeginInvokeOnMainThread(() => {
                    App.Current.MainPage = mV;
                });
            });
        }
	}
}