﻿using System;

namespace MonsterTradingCard.NoStatsUpdateException
{
    [Serializable]
    internal class NoStatsUpdateException : Exception
    {
        public NoStatsUpdateException()
        {
        }

        public NoStatsUpdateException(string message) : base(message)
        {
        }

        public NoStatsUpdateException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}