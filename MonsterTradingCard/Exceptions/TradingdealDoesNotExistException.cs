using System;

namespace MonsterTradingCard.Exceptions.TradingdealDoesNotExistException
{
    [Serializable]
    public class TradingdealDoesNotExistException : Exception
    {
        public TradingdealDoesNotExistException()
        {
        }

        public TradingdealDoesNotExistException(string message) : base(message)
        {
        }

        public TradingdealDoesNotExistException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}