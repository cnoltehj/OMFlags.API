namespace OMFlags.API.Models.Common
{
    public class MessageAndID
    {
        public MessageAndID()
        {
            Id = 0;
            Text = "";
        }

        public int Id { get; set; }
        public string Text { get; set; }
    }
}
