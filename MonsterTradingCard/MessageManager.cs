using MonsterTradingCard.DAL.IUserRepository;
using MonsterTradingCard.DAL.ICardRepository;
using MonsterTradingCard.DAL.IPackageRepository;
using MonsterTradingCard.DAL.IDeckRepository;
using IMSGMANAGER = MonsterTradingCard.IMessageManager;
using USER_NOT_FOUND = MonsterTradingCard.UserNotFoundException;
using DUPUSER = MonsterTradingCard.DuplicateUserException;
using DUPCARD = MonsterTradingCard.DuplicateCardException;
using INVALIDDECK = MonsterTradingCard.DeckNot4CardsException;
using MonsterTradingCard.Models.User;
using MonsterTradingCard.Models.Credentials;
using MonsterTradingCard.Models.Package;
using MonsterTradingCard.Models.Card;
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

        public MessageManager(IUserRepository userRepository, ICardRepository cardRepository, IPackageRepository packageRepository, IDeckRepository deckRepository)
        {
            this.userRepository = userRepository;
            this.cardRepository = cardRepository;
            this.packageRepository = packageRepository;
            this.deckRepository = deckRepository;
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

        public Package SelectRandomPackage()
        {
            return packageRepository.SelectRandomPackage();
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
                if (cardRepository.SelectCardById(card.Id) == null)
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
            return cardRepository.GetCardsByToken(authToken);
        }

        public List<Card> GetDeck(string authToken)
        {
            var deck = deckRepository.GetDeckByToken(authToken);
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
    }
}
