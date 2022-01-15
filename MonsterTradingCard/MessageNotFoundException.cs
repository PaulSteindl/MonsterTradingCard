using System;

namespace MonsterTradingCard.MessageNotFoundException
{
    [Serializable]
    public class MessageNotFoundException : Exception
    {
        public MessageNotFoundException()
        {
        }

        public MessageNotFoundException(string message) : base(message)
        {
        }

        public MessageNotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}