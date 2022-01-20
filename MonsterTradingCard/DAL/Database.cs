using Npgsql;
using DAFE = MonsterTradingCard.DAL.DataAccessFailedException;
using IUSER = MonsterTradingCard.DAL.IUserRepository;
using ICARD = MonsterTradingCard.DAL.ICardRepository;
using IPACK = MonsterTradingCard.DAL.IPackageRepository;
using IDECK = MonsterTradingCard.DAL.IDeckRepository;
using ISCORE = MonsterTradingCard.DAL.IHighscoreRepository;
using ITRADE = MonsterTradingCard.DAL.ITradingdealRepository;
using DATA_USER_REPO = MonsterTradingCard.DAL.DatabaseUserRepository;
using DATA_CARD_REPO = MonsterTradingCard.DAL.DatabaseCardRepository;
using DATA_PACK_REPO = MonsterTradingCard.DAL.DatabasePackageRepository;
using DATA_DECK_REPO = MonsterTradingCard.DAL.DatabaseDeckRepository;
using DATA_SCORE_REPO = MonsterTradingCard.DAL.DatabaseHighscoreRepository;
using DATA_TRADE_REPO = MonsterTradingCard.DAL.DatabaseTradingdealRepository;
using System.Threading;

namespace MonsterTradingCard.DAL.Database
{
    public class Database
    {
        private readonly NpgsqlConnection _connection;
        public IUSER.IUserRepository UserRepository { get; private set; }
        public ICARD.ICardRepository CardRepository { get; private set; }
        public IPACK.IPackageRepository PackageRepository { get; private set; }
        public IDECK.IDeckRepository DeckRepository { get; private set; }
        public ISCORE.IHighscoreRepository HighscoreRepositroy { get; private set; }
        public ITRADE.ITradingdealRepository TradingdealRepository { get; private set; }
        private static Mutex mDB = new();

        public Database(string connectionString)
        {
            try
            {
                _connection = new NpgsqlConnection(connectionString);
                _connection.Open();

                // first users, then messages
                // we need this special order since messages has a foreign key to users
                UserRepository = new DATA_USER_REPO.DatabaseUserRepository(_connection, mDB);
                CardRepository = new DATA_CARD_REPO.DatabaseCardRepository(_connection, mDB);
                PackageRepository = new DATA_PACK_REPO.DatabasePackageRepository(_connection, mDB);
                DeckRepository = new DATA_DECK_REPO.DatabaseDeckRepository(_connection, mDB);
                HighscoreRepositroy = new DATA_SCORE_REPO.DatabaseHighscoreRepository(_connection, mDB);
                TradingdealRepository = new DATA_TRADE_REPO.DatabaseTradingdealRepository(_connection, mDB);
            }
            catch (NpgsqlException e)
            {
                // provide our own custom exception
                throw new DAFE.DataAccessFailedException("Could not connect to or initialize database", e);
            }
        }
    }
}
