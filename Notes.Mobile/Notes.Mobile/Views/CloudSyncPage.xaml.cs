using Notes.Mobile.ViewModels;
using System;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Notes.Mobile.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CloudSyncPage : ContentPage
    {
        CloudSyncViewModel viewModel;
        public CloudSyncPage()
        {
            InitializeComponent();
            BindingContext = viewModel = new CloudSyncViewModel();
        }
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            IsBusy = true;
            aiLayout.IsVisible = true;
            await viewModel.LoadData();
            if (viewModel.LoggedIn)
            {
                logoutButton.IsEnabled = true;
            }
            IsBusy = false;
            aiLayout.IsVisible = false;
        }

        private async void loginButton_Clicked(object sender, EventArgs e)
        {
            IsBusy = true;
            aiLayout.IsVisible = true;
            await viewModel.Login();
            if (viewModel.LoggedIn)
            {
                logoutButton.IsEnabled = true;
                MessagingCenter.Send(this, "loggedIn");
            }
            IsBusy = false;
            aiLayout.IsVisible = false;
        }

        private async void registerButton_Clicked(object sender, EventArgs e)
        {
            IsBusy = true;
            aiLayout.IsVisible = true;
            await viewModel.Register();
            IsBusy = false;
            aiLayout.IsVisible = false;
        }

        private async void logoutButton_Clicked(object sender, EventArgs e)
        {
            IsBusy = true;
            aiLayout.IsVisible = true;
            await viewModel.Logout();
            logoutButton.IsEnabled = false;
            IsBusy = false;
            aiLayout.IsVisible = false;
        }
    }
}