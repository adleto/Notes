namespace Notes.Mobile.Models
{
    public enum MenuItemType
    {
        BrowseNotes,
        EncryptionKey,
        CloudSync,
        About
    }
    public class HomeMenuItem
    {
        public MenuItemType Id { get; set; }

        public string Title { get; set; }
    }
}
