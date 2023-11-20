using LightMessageBus.Interfaces;
using System.Threading;

namespace Menadżer_3
{
    public abstract class MessagesBase : IMessage
    {
        public string Data { get; set; }
        public int Id => _id;
        public object Source { get; set; }
        private static int _id;

        protected MessagesBase(object source) : this(source, null)
        {
        }

        protected MessagesBase(object source, string data)
        {
            Source = source;
            Data = data;
            _id = Interlocked.Increment(ref _id);
        }
    }
}