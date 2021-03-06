using MonsterTradingCard.DAL.IUserRepository;
using MonsterTradingCard.DAL.ICardRepository;
using MonsterTradingCard.DAL.IPackageRepository;
using MonsterTradingCard.DAL.IDeckRepository;
using MonsterTradingCard.DAL.IHighscoreRepository;
using MonsterTradingCard.DAL.ITradingdealRepository;
using IMSGMANAGER = MonsterTradingCard.IMessageManager;
using USER_NOT_FOUND = MonsterTradingCard.Exceptions.UserNotFoundException;
using DUPUSER = MonsterTradingCard.Exceptions.DuplicateUserException;
using DUPCARD = MonsterTradingCard.Exceptions.DuplicateCardException;
using INVALIDDECK = MonsterTradingCard.Exceptions.DeckNot4CardsException;
using INVALIDDEAL = MonsterTradingCard.Exceptions.TradingdealDoesNotExistException;
using NOSTATSUPDATE = MonsterTradingCard.Exceptions.NoStatsUpdateException;
using MonsterTradingCard.Models.User;
using MonsterTradingCard.Models.UserData;
using MonsterTradingCard.Models.UserStats;
using MonsterTradingCard.Models.Credentials;
using MonsterTradingCard.Models.Package;
using MonsterTradingCard.Models.Card;
using MonsterTradingCard.Models.Highscore;
using MonsterTradingCard.Models.TradingDeal;
using MonsterTradingCard.Models.Deck;
using System.Collections.Generic;
using System;

namespace MonsterTradingCard.MessageManager
{
    public class MessageManager : IMSGMANAGER.IMessageManager
    {
        private readonly IUserRepository userRepository;
        private readonly ICardRepository cardRepository;
        private readonly IPackageRepository packageRepository;
        private readonly IDeckRepository deckRepository;
        private readonly IHighscoreRepository highscoreRepository;
        private readonly ITradingdealRepository tradingdealRepository;

        public MessageManager(IUserRepository userRepository, ICardRepository cardRepository, IPackageRepository packageRepository, IDeckRepository deckRepository, IHighscoreRepository highscoreRepository, ITradingdealRepository tradingdealRepository)
        {
            this.userRepository = userRepository;
            this.cardRepository = cardRepository;
            this.packageRepository = packageRepository;
            this.deckRepository = deckRepository;
            this.highscoreRepository = highscoreRepository;
            this.tradingdealRepository = tradingdealRepository;
        }

        public User LoginUser(Credentials credentials)
        {
            var user = userRepository.GetUserByCredentials(credentials.Username, credentials.Password);
            return user ?? throw new USER_NOT_FOUND.UserNotFoundException();
        }

        public void RegisterUser(Credentials credentials)
        {
            var user = new User()
            {
                Username = credentials.Username,
                Password = credentials.Password
            };
            if (userRepository.InsertUser(user) == false)
            {
                throw new DUPUSER.DuplicateUserException();
            }
        }

        public void AddCard(Card cardIn)
        {
            var card = new Card()
            {
                Id = cardIn.Id,
                Name = cardIn.Name,
                Damage = Convert.ToInt32(cardIn.Damage)
            };

            if(cardRepository.InsertCard(card) == false)
            {
                throw new DUPCARD.DuplicateCardException(card.Id);
            }
        }

        public void CreatePackage(List<Card> cards)
        {
            var package = new Package();
            var cardIds = new List<string>();

            foreach(Card card in cards)
            {
                cardIds.Add(card.Id);
            }

            package.CardIds = cardIds;

            packageRepository.InsertPackage(package);
        }

        public Package SelectFirstPackage()
        {
            return packageRepository.SelectFirstPackage();
        }

        public bool CheckCoins(string authToken)
        {
            int coinsAvailabel = 0;

            coinsAvailabel = userRepository.SelectCoinsByToken(authToken);

            return coinsAvailabel >= 5;
        }

        public void CardExistence(List<Card> cards)
        {
            foreach (Card card in cards)
                if (cardRepository.SelectCardById(card.Id) != null)
                    throw new DUPCARD.DuplicateCardException(card.Id);
        }

        public void AcquirePackage(Package package, string authToken)
        {
            packageRepository.UpdatePackageOwner(package.Id, authToken);
            userRepository.UpdateCoinsByMinus5(authToken);
            foreach (string cardId in package.CardIds)
                cardRepository.UpdateCardOwner(cardId, authToken);
        }

        public IEnumerable<Card> GetCards(string authToken)
        {
            return cardRepository.SelectCardsByToken(authToken);
        }

        public List<Card> GetDeckReturnCardList(string authToken)
        {
            var deck = deckRepository.SelectDeckByToken(authToken);
            var cardList = new List<Card>();

            if (deck != null && deck.CardIds.Count == 4)
            {
                foreach (string cardId in deck.CardIds)
                    cardList.Add(cardRepository.SelectCardById(cardId)); 
            }
            else if(deck == null)
            {
                return cardList;
            }
            else
                throw new INVALIDDECK.DeckNot4CardsException();

            return cardList;
        }

        public bool CheckCardAndUser(string cardId, string authToken)
        {
            return cardRepository.SelectCardByIdAndToken(cardId, authToken) == null ? false : true;
        }

        public bool UserDeckExists(string authToken)
        {
            return deckRepository.SelectDeckByToken(authToken) == null ? false : true;
        }

        public void UpdateDeck(string authToken, List<string> cardIds)
        {
            deckRepository.UpdateDeckByToken(authToken, cardIds);
        }

        public int CreateDeck(string authToken, List<string> cardIds)
        {
            return deckRepository.InsertDeck(authToken, cardIds);
        }

        public UserData GetUserData(string username)
        {
            return userRepository.SelectUserDataByUsername(username);
        }

        public void UpdateUserData(string username, UserData userData)
        {
            userRepository.UpdateUserDataByUsername(username, userData);
        }

        public UserStats GetUserStats(string authToken)
        {
            return userRepository.SelectUserStatsByToken(authToken);
        }

        public IEnumerable<Highscore> GetScore()
        {
            return highscoreRepository.SelectHighscoreTop50();
        }

        public IEnumerable<TradingDeal> GetTradingDeals()
        {
            return tradingdealRepository.SelectOpenTradingdeals();
        }

        public bool CheckCardForTrade(string cardId)
        {
            return tradingdealRepository.SelectTradingdealByCardId(cardId) == null ? false : true;
        }

        public bool CreateTradingdeal(TradingDeal tradingDeal, string authToken)
        {
            return tradingdealRepository.InsertTradingdeal(tradingDeal, authToken) > 0;
        }

        public bool DeleteTradingdeal(string tradingDealId, string authToken)
        {
            return tradingdealRepository.DeleteTradingdealByTradingIdAndToken(tradingDealId, authToken) > 0;
        }

        public TradingDeal CheckTradingdealExistsReturnDeal(string tradingDealId)
        {
            return tradingdealRepository.SelectTradingdealAndTokenByTradingId(tradingDealId) ?? throw new INVALIDDEAL.TradingdealDoesNotExistException();
        }

        public Card GetCardByIdAndToken(string cardId, string authToken)
        {
            return cardRepository.SelectCardByIdAndToken(cardId, authToken);
        }

        public void UpdateCardOwnerById(string cardId, string authToken)
        {
            cardRepository.UpdateCardOwner(cardId, authToken);
        }

        public Deck GetDeck(string authToken)
        {
            var deck = deckRepository.SelectDeckByToken(authToken);

            if (deck != null && deck.CardIds.Count == 4)
            {
                return deck;
            }
            else if (deck == null)
            {
                return deck;
            }
            else
                throw new INVALIDDECK.DeckNot4CardsException();
        }

        public void UpdateStatsScoreWinner(string authToken, string username)
        {
            if (userRepository.UpdateStatsWinnerByToken(authToken) > 0)
            {
                if (highscoreRepository.SelectHighscoreByUsername(authToken) != null)
                {
                    if (highscoreRepository.UpdateWinByOneByUsername(authToken) <= 0)
                        throw new NOSTATSUPDATE.NoStatsUpdateException();
                }
                else
                {
                    if (highscoreRepository.InsertWinOneByUsername(authToken) <= 0)
                        throw new NOSTATSUPDATE.NoStatsUpdateException();
                }
            }
            else
                throw new NOSTATSUPDATE.NoStatsUpdateException();
        }

        public void UpdateStatsLoser(string authToken)
        {
            if (userRepository.UpdateStatsLoserByToken(authToken) <= 0)
                throw new NOSTATSUPDATE.NoStatsUpdateException();
        }

        public void UpdateStatsDraw(string authToken)
        {
            if (userRepository.UpdateStatsDrawByToken(authToken) <= 0)
                throw new NOSTATSUPDATE.NoStatsUpdateException();
        }
    }
}
