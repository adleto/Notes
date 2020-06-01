using Notes.Mobile.Models.Requests;
using System;
using System.ComponentModel;
using Xamarin.Forms;


namespace Notes.Mobile.Views
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class NewNotePage : ContentPage
    {
        public NoteInsertModel Note { get; set; }

        public NewNotePage()
        {
            InitializeComponent();

            Note = new NoteInsertModel
            {
                Text = "",
                Title = ""
            };

            BindingContext = this;
        }

        async void Save_Clicked(object sender, EventArgs e)
        {
            if(!string.IsNullOrEmpty(Note.Title) || !string.IsNullOrEmpty(Note.Text)) {
                MessagingCenter.Send(this, "AddNote", Note);
                await Navigation.PopModalAsync();
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Warning", "Can not add note without text or title.", "OK");
            }
        }

        async void Cancel_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }
    }
}