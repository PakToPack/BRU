using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Xamarin.Forms;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Widget;

namespace FixTricks.Droid.Receivers
{
    [BroadcastReceiver]
    public class MessagingReceiver : BroadcastReceiver
    {
        public override void OnReceive(Context context, Intent intent)
        {
            
            MessagingCenter.Send<object, string>(this, "UpdateLabel", "Hello from Android");
        }
    }
}