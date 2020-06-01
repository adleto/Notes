using Notes.Mobile.ViewModels;
using System;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Notes.Mobile.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NoteDetailPage : ContentPage
    {
        NoteDetailViewModel viewModel;
        public NoteDetailPage(NoteDetailViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = this.viewModel = viewModel;
        }
        async void Save_Clicked(object sender, EventArgs e)
        {
            MessagingCenter.Send(this, "UpdateNote", viewModel.Note);
            await Navigation.PopModalAsync();
        }

        async void Cancel_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }
    }
}