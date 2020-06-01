using Xamarin.Forms;
using Notes.Mobile.Views;
using Notes.Mobile.Services.Local;
using Xamarin.Essentials;
using Notes.Mobile.Services.External;

namespace Notes.Mobile
{
    public partial class App : Application
    {

        public App()
        {
            InitializeComponent();
            DependencyService.Register<NoteService>();
            MainPage = new MainPage();
        }

        protected override async void OnStart()
        {
            var token = await SecureStorage.GetAsync("token");
            if (token != null)
            {
                ApiService.Token = token;
            }
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
