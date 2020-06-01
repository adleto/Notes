using SQLite;
using System;

namespace Notes.Mobile.Models.Database
{
    public class Note
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public int? ExternalId { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        public DateTime Modified { get; set; }
        public int Version { get; set; }
    }
}
