using MonsterTradingCard.Models.TradingDeal;
using System.Collections.Generic;

namespace MonsterTradingCard.DAL.ITradingdealRepository
{
    public interface ITradingdealRepository
    {
        IEnumerable<TradingDeal> SelectOpenTradingdeals();
        TradingDeal SelectTradingdealByCardId(string cardId);
        int InsertTradingdeal(TradingDeal tradingDeal, string authToken);
        int DeleteTradingdealByTradingIdAndToken(string tradingDealId, string authToken);
    }
}