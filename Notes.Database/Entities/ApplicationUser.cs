using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Notes.Database.Entities
{
    public class ApplicationUser
    {
        public int Id { get; set; }
        [Required]
        public string Email { get; set; }

        [Required]
        public string Username { get; set; }
        public string PasswordSalt { get; set; }
        public string PasswordHash { get; set; }
        public virtual List<Note> Notes { get; set; }
        public bool Active { get; set; }
    }
}
