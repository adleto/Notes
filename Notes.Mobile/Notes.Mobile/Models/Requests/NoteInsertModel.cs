using System;

namespace Notes.Mobile.Models.Requests
{
    public class NoteInsertModel
    {
        public string Title { get; set; }
        public string Text { get; set; }
        public int? ExternalId { get; set; }
        public DateTime? Modified { get; set; }
    }
}
