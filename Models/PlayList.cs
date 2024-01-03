namespace MusicBoxServer.Models
{
    public class PlayList
    {
        public int PlayListID { get; set; }
        public int UserID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime DateCreated { get; set; }
    }

}
