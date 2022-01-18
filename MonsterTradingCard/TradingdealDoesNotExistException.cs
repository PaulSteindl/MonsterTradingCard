using System;

namespace MonsterTradingCard.TradingdealDoesNotExistException
{
    [Serializable]
    internal class TradingdealDoesNotExistException : Exception
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