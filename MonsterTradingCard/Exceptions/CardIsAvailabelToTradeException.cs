using System;

namespace MonsterTradingCard.Exceptions.CardIsAvailabelToTradeException
{
    [Serializable]
    internal class CardIsAvailabelToTradeException : Exception
    {
        public CardIsAvailabelToTradeException()
        {
        }

        public CardIsAvailabelToTradeException(string message) : base(message)
        {
        }

        public CardIsAvailabelToTradeException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}