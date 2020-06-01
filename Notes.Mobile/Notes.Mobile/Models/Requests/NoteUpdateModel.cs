using System;

namespace Notes.Mobile.Models.Requests
{
    public class NoteUpdateModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        public DateTime? Modified { get; set; }
    }
}
