using System;

namespace Notes.Models.Note
{
    public class NoteGetModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        public DateTime? Modified { get; set; }
        public int Version { get; set; }
    }
}
