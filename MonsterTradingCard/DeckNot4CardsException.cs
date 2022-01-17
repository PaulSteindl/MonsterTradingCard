using System;

namespace MonsterTradingCard.DeckNot4CardsException
{
    [Serializable]
    internal class DeckNot4CardsException : Exception
    {
        public DeckNot4CardsException()
        {
        }

        public DeckNot4CardsException(string message) : base(message)
        {
        }

        public DeckNot4CardsException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
