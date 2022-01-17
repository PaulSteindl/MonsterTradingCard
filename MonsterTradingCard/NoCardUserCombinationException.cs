﻿using System;

namespace MonsterTradingCard.NoCardUserCombinationException
{
    [Serializable]
    internal class NoCardUserCombinationException : Exception
    {
        public NoCardUserCombinationException()
        {
        }

        public NoCardUserCombinationException(string message) : base(message)
        {
        }

        public NoCardUserCombinationException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}