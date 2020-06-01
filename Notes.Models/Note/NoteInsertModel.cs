using System;
using System.ComponentModel.DataAnnotations;

namespace Notes.Models.Note
{
    public class NoteInsertModel
    {
        [MaxLength(500)]
        public string Title { get; set; }
        public string Text { get; set; }
        public DateTime? Modified { get; set; }
        public int Version { get; set; }
    }
}
