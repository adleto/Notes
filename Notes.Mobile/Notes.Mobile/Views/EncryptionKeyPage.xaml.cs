using Notes.Mobile.ViewModels;
using System;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Notes.Mobile.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EncryptionKeyPage : ContentPage
    {
        EncryptionKeyViewModel viewModel;
        public EncryptionKeyPage()
        {
            InitializeComponent();

            BindingContext = viewModel = new EncryptionKeyViewModel();
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();

            viewModel.SecretKey = "**********";
            showKeyButton.IsEnabled = true;
            saveKeyButton.IsVisible = false;
            secretKeyField.IsEnabled = false;
        }

        private async void showKeyButton_Clicked(object sender, EventArgs e)
        {
            await viewModel.GetKey();
            showKeyButton.IsEnabled = false;
            saveKeyButton.IsVisible = true;
            secretKeyField.IsEnabled = true;
        }

        private async void saveKeyButton_Clicked(object sender, EventArgs e)
        {
            if (viewModel.SecretKey.Length != 24)
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Key has to be 24 characters long.", "OK");
            }
            else
            {
                Device.BeginInvokeOnMainThread(new Action(async () =>
                {
                    if (await DisplayAlert("Key Changed", "Are you sure you want to change your key? WARNING! If you change encryption key all your notes will be deleted!", "Yes", "No"))
                    {
                        bool finalWarning = await DisplayAlert("WARNING", "All your notes will be deleted. Do you want to continue?", "Yes", "No");
                        if (finalWarning)
                        {
                            await viewModel.SetNewKey();
                        }
                    }
                }));
            }
        }

        private void resetKeyButton_Clicked(object sender, EventArgs e)
        {
            showKeyButton.IsEnabled = false;
            saveKeyButton.IsVisible = true;
            secretKeyField.IsEnabled = true;

            viewModel.ResetKey();
        }
    }
}