using Npgsql;
using DAFE = MonsterTradingCard.DAL.DataAccessFailedException;
using IMESSAGE = MonsterTradingCard.DAL.IMessageRepository;
using IUSER = MonsterTradingCard.DAL.IUserRepository;
using DATA_USER_REPO = MonsterTradingCard.DAL.DatabaseUserRepository;
using DATA_MESSAGE_REPO = MonsterTradingCard.DAL.DatabaseMessageRepository;

namespace MonsterTradingCard.DAL.Database
{
    class Database
    {
        private readonly NpgsqlConnection _connection;

        public IMESSAGE.IMessageRepository MessageRepository { get; private set; }
        public IUSER.IUserRepository UserRepository { get; private set; }
        public ICardRepository CardRepository { get; private set; }
        public IDeckRepository DeckRepository { get; private set; }
        public IHighscoreRepository HighscoreRepository { get; private set; }
        public IPackageRepository PackageRepository { get; private set;  }

        public Database(string connectionString)
        {
            try
            {
                _connection = new NpgsqlConnection(connectionString);
                _connection.Open();

                // first users, then messages
                // we need this special order since messages has a foreign key to users
                UserRepository = new DATA_USER_REPO.DatabaseUserRepository(_connection);
                MessageRepository = new DATA_MESSAGE_REPO.DatabaseMessageRepository(_connection);
            }
            catch (NpgsqlException e)
            {
                // provide our own custom exception
                throw new DAFE.DataAccessFailedException("Could not connect to or initialize database", e);
            }
        }
    }
}
