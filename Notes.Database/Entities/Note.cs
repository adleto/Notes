using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Notes.Database.Entities
{
    public class Note
    {
        public int Id { get; set; }
        [Required]
        [ForeignKey(nameof(ApplicationUser))]
        public int ApplicationUserId { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        public int Version { get; set; }

        [Required]
        public DateTime Created { get; set; }
        public DateTime? Modified { get; set; }
    }
}
