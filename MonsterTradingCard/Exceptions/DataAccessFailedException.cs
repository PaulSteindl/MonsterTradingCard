using System;

namespace MonsterTradingCard.Exceptions.DataAccessFailedException
{
    class DataAccessFailedException : Exception
    {
        public DataAccessFailedException()
        {
        }

        public DataAccessFailedException(string message) : base(message)
        {
        }

        public DataAccessFailedException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}