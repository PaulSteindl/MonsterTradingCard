using Npgsql;
using DAFE = MonsterTradingCard.DAL.DataAccessFailedException;
using IUSER = MonsterTradingCard.DAL.IUserRepository;
using ICARD = MonsterTradingCard.DAL.ICardRepository;
using DATA_USER_REPO = MonsterTradingCard.DAL.DatabaseUserRepository;
using DATA_CARD_REPO = MonsterTradingCard.DAL.DatabaseCardRepository;

namespace MonsterTradingCard.DAL.Database
{
    class Database
    {
        private readonly NpgsqlConnection _connection;
        public IUSER.IUserRepository UserRepository { get; private set; }

        public ICARD.ICardRepository CardRepository { get; private set; }

        public Database(string connectionString)
        {
            try
            {
                _connection = new NpgsqlConnection(connectionString);
                _connection.Open();

                // first users, then messages
                // we need this special order since messages has a foreign key to users
                UserRepository = new DATA_USER_REPO.DatabaseUserRepository(_connection);
                CardRepository = new DATA_CARD_REPO.DatabaseCardRepository(_connection);
            }
            catch (NpgsqlException e)
            {
                // provide our own custom exception
                throw new DAFE.DataAccessFailedException("Could not connect to or initialize database", e);
            }
        }
    }
}
