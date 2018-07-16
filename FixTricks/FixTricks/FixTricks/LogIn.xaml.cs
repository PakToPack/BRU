using System;
using System.Threading.Tasks;
using SQLite;
using FixTricks.scripts;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using FixTricks.views;
using FixTricks.constructors;

namespace FixTricks
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class LogIn : ContentPage
	{
        private string[,] _courses;
		public LogIn ()
		{
			InitializeComponent ();
            LoadCourses();
		}

        public async void LoadCourses()
        {
            TextX.Text = "Подождите! Мы загружаем базы!";
            CoursePick.IsVisible = false;
            CourseText.IsVisible = false;
            Zachetka.IsVisible = false;
            Label1.IsVisible = false;
            GroupPick.IsVisible = false;
            PressButton.IsVisible = false;
            ActivityInd.IsVisible = true;
            await Task.Run(() => {
                ReloadData relData = new ReloadData();
                _courses = relData.LoadCourses();
                Device.BeginInvokeOnMainThread(() =>
                {
                    try
                    {
                        for (int i = 0; i < _courses.Length/2; i++)
                            CoursePick.Items.Add(_courses[i, 0]);
                    }
                    catch
                    {

                    }
                });
            });
            TextX.Text = "Введите номер зачётки";
            CoursePick.IsVisible = true;
            CourseText.IsVisible = true;
            Zachetka.IsVisible = true;
            Label1.IsVisible = true;
            GroupPick.IsVisible = true;
            PressButton.IsVisible = true;
            ActivityInd.IsVisible = false;
        }

        public async void TaskClick()
        {
            if (!int.TryParse(Zachetka.Text, out int k) || GroupPick.SelectedItem == null || CoursePick.SelectedItem == null)
            {
                MessagingCenter.Send<object, string>(this, "SendToast", "Все поля должны быть заполнены!");
                return;
            }
            CoursePick.IsVisible = false;
            CourseText.IsVisible = false;
            Zachetka.IsVisible = false;
            Label1.IsVisible = false;
            GroupPick.IsVisible = false;
            PressButton.IsVisible = false;
            ActivityInd.IsVisible = true;
            ReloadData reldata = new ReloadData();
            TextX.Text = "Узнаём про вас в деканате!";
            reldata.LoadDB();
            bool set_user = await Task.Run(() => reldata.ReloadRate(Zachetka.Text, int.Parse(GroupPick.Items[GroupPick.SelectedIndex])));
            string _link = null;
            for (int i = 0; i < _courses.Length/2; i++)
            {
                if(_courses[i,0] == CoursePick.Items[CoursePick.SelectedIndex])
                {
                    _link = _courses[i, 1];
                    break;
                }
            }
            
            SQLiteConnection db = new SQLiteConnection(SysPath.DBPath);
            db.Insert(new AppSettings() { excelLink = _link, userCourse = CoursePick.Items[CoursePick.SelectedIndex] });

            if (set_user)
                Application.Current.MainPage = new SelectGroupByCourse(_link);

            TextX.Text = "Введите номер зачётки";
            CoursePick.IsVisible = true;
            CourseText.IsVisible = true;
            Zachetka.IsVisible = true;
            Label1.IsVisible = true;
            GroupPick.IsVisible = true;
            PressButton.IsVisible = true;
            ActivityInd.IsVisible = false;
        }
	}
}