using System;

namespace MonsterTradingCard.CardIsAvailabelToTrade
{
    [Serializable]
    internal class CardIsAvailabelToTrade : Exception
    {
        public CardIsAvailabelToTrade()
        {
        }

        public CardIsAvailabelToTrade(string message) : base(message)
        {
        }

        public CardIsAvailabelToTrade(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}