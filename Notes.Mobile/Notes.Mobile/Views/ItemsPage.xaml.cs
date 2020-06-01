using System;
using System.ComponentModel;
using Xamarin.Forms;
using Notes.Mobile.ViewModels;
using Notes.Mobile.Models.Requests;
using Xamarin.Essentials;

namespace Notes.Mobile.Views
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class ItemsPage : ContentPage
    {
        ItemsViewModel viewModel;

        public ItemsPage()
        {
            InitializeComponent();
            BindingContext = viewModel = new ItemsViewModel();
        }
        void OnItemSwiped(object sender, EventArgs args)
        {
            IsBusy = true;
            aiLayout.IsVisible = true;

            var layout = (BindableObject)sender;
            var note = (NoteGetModel)layout.BindingContext;

            Device.BeginInvokeOnMainThread(new Action(async () =>
            {
                if (await DisplayAlert("Delete note", "Do you want to delete this note?", "Yes", "No"))
                {
                    await viewModel.DeleteNote(note);
                }
            }));

            aiLayout.IsVisible = false;
            IsBusy = false;
        }

        async void OnItemSelected(object sender, EventArgs args)
        {
            var layout = (BindableObject)sender;
            var note = (NoteGetModel)layout.BindingContext;
            await Navigation.PushModalAsync(new NavigationPage(new NoteDetailPage(new NoteDetailViewModel(new NoteUpdateModel { 
                Id = note.Id,
                Text = note.Text,
                Title = note.Title,
                Modified = note.Modified
            }))));
        }

        async void AddItem_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new NavigationPage(new NewNotePage()));
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            await viewModel.ExecuteLoadNotesCommand();
        }

        private async void Pull_Clicked(object sender, EventArgs e)
        {
            if (Preferences.ContainsKey("loggedIn"))
            {
                IsBusy = true;
                aiLayout.IsVisible = true;
                await viewModel.Pull();
                await viewModel.ExecuteLoadNotesCommand();
                aiLayout.IsVisible = false;
                IsBusy = false;
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Error", "You are not logged in.", "OK");
            }
        }

        private async void Push_Clicked(object sender, EventArgs e)
        {
            if (Preferences.ContainsKey("loggedIn"))
            {
                IsBusy = true;
                aiLayout.IsVisible = true;
                await viewModel.Push();
                await viewModel.ExecuteLoadNotesCommand();
                aiLayout.IsVisible = false;
                IsBusy = false;
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Error", "You are not logged in.", "OK");
            }
        }

        //private async void Sync_Clicked(object sender, EventArgs e)
        //{
        //    if (Preferences.ContainsKey("loggedIn"))
        //    {
        //        IsBusy = true;
        //        aiLayout.IsVisible = true;
        //        await viewModel.Sync();
        //        await viewModel.ExecuteLoadNotesCommand();
        //        aiLayout.IsVisible = false;
        //        IsBusy = false;
        //    }
        //    else
        //    {
        //        await Application.Current.MainPage.DisplayAlert("Error", "You are not logged in.", "OK");
        //    }
        //}
    }
}