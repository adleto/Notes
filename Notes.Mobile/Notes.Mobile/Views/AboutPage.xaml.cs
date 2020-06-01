using System;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Notes.Mobile.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AboutPage : ContentPage
    {
        public string Url { get; set; } = "https://github.com/adleto/Notes";
        public AboutPage()
        {
            InitializeComponent();
            BindingContext = this;
        }
        public async void Handle_Tapped(object sender, EventArgs e)
        {
            await Browser.OpenAsync(new Uri(Url), BrowserLaunchMode.SystemPreferred);
        }
    }
}