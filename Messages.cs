namespace Menadżer_3
{
    public class RefreshMessage : MessagesBase
    {
        public RefreshMessage(object source, string data) : base(source) => Data = data;
    }
}