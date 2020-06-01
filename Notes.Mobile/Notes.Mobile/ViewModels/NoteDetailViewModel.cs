using Notes.Mobile.Models.Requests;

namespace Notes.Mobile.ViewModels
{
    public class NoteDetailViewModel : BaseViewModel
    {
        public NoteUpdateModel Note { get; set; }
        public NoteDetailViewModel(NoteUpdateModel note)
        {
            Title = "Browse Notes";
            Note = note;
        }
    }
}
