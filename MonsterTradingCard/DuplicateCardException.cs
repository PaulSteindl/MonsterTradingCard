using System;

namespace MonsterTradingCard.DuplicateCardException
{
    public class DuplicateCardException : Exception
    {
        public DuplicateCardException()
        {
        }

        public DuplicateCardException(string message) : base(message)
        {
        }

        public DuplicateCardException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}