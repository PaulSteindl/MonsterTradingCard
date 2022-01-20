using Npgsql;
using IUSERREPO = MonsterTradingCard.DAL.IUserRepository;
using MonsterTradingCard.Models.User;
using MonsterTradingCard.Models.UserData;
using MonsterTradingCard.Models.UserStats;
using System;
using System.Data;
using System.Globalization;
using System.Threading;

namespace MonsterTradingCard.DAL.DatabaseUserRepository
{
    class DatabaseUserRepository : IUSERREPO.IUserRepository
    {
        private const string CreateTableCommand =   @"create table if not exists users
                                                    (
                                                        username varchar                     not null,
                                                        password varchar                     not null,
                                                        token    varchar                     not null,
                                                        user_id  serial
                                                            constraint users_pk
                                                                primary key,
                                                        bio      text,
                                                        image    text    default '-.-'::text not null,
                                                        wins     integer default 0           not null,
                                                        loses    integer default 0           not null,
                                                        coins    integer default 20          not null
                                                    );

                                                    alter table users
                                                        owner to postgres;

                                                    create unique index if not exists users_user_id_uindex
                                                        on users (user_id);

                                                    create unique index if not exists users_token_uindex
                                                        on users (token);

                                                    create unique index if not exists users_username_uindex
                                                        on users (username);
                                                    ";

        private const string TruncateAllAndRestartIdCommand = "TRUNCATE users, cards, decks, highscores, packages, tradingdeals RESTART IDENTITY;";
        private const string InsertUserCommand = "INSERT INTO users(username, password, token) VALUES (@username, @password, @token)";
        private const string SelectUserByTokenCommand = "SELECT username, password FROM users WHERE token=@token";
        private const string SelectUserByCredentialsCommand = "SELECT username, password FROM users WHERE username=@username AND password=@password";
        private const string SelectCoinsByTokenCommand = "SELECT coins FROM users WHERE token=@token";
        private const string UpdateCoinsByMinus5Command = "UPDATE users SET coins=coins - 5 WHERE token=@token";
        private const string SelectUserDataByUsernameCommand = "SELECT name, bio, image FROM users WHERE username=@username";
        private const string UpdateUserDataByUsernameCommand = "UPDATE users SET name=@name, bio=@bio, image=@image WHERE username=@username";
        private const string SelectUserStatsByUsernameCommand = "SELECT username, wins, loses, draws, elo FROM users WHERE token=@token";
        private const string UpdateStatsWinnerByTokenCommand = "UPDATE users SET wins=wins + 1, elo=elo + 3 WHERE token=@token";
        private const string UpdateStatsLoserByTokenCommand = "UPDATE users SET loses=loses + 1, elo=elo - 5 WHERE token=@token";
        private const string UpdateStatsDrawByTokenCommand = "UPDATE users SET draws=draws + 1 WHERE token=@token";

        private readonly NpgsqlConnection _connection;
        private Mutex mDB;

        public DatabaseUserRepository(NpgsqlConnection connection, Mutex mDB)
        {
            _connection = connection;
            this.mDB = mDB;
            EnsureTables();
        }

        public void TruncateAllAndRestartId(string authToken)
        {
            if (authToken == "fYyhAF4Lof#J8zbxfYcCGUDO2IpYy?dkH&1")
            {
                using (var cmd = new NpgsqlCommand(TruncateAllAndRestartIdCommand, _connection))
                {
                    mDB.WaitOne();
                    cmd.ExecuteNonQuery();
                    mDB.ReleaseMutex();
                }
            }
        }

        public User GetUserByAuthToken(string authToken)
        {
            User user = null;
            using (var cmd = new NpgsqlCommand(SelectUserByTokenCommand, _connection))
            {
                cmd.Parameters.AddWithValue("token", authToken);

                // take the first row, if any
                mDB.WaitOne();
                using var reader = cmd.ExecuteReader();
                mDB.ReleaseMutex();
                if (reader.Read())
                {
                    user = ReadUser(reader);
                }
            }
            return user;
        }

        public User GetUserByCredentials(string username, string password)
        {
            User user = null;
            using (var cmd = new NpgsqlCommand(SelectUserByCredentialsCommand, _connection))
            {
                cmd.Parameters.AddWithValue("username", username);
                cmd.Parameters.AddWithValue("password", password);

                // take the first row, if any
                mDB.WaitOne();
                using var reader = cmd.ExecuteReader();
                mDB.ReleaseMutex();
                if (reader.Read())
                {
                    user = ReadUser(reader);
                }
            }
            return user;
        }

        public bool InsertUser(User user)
        {
            var affectedRows = 0;
            try
            {
                using var cmd = new NpgsqlCommand(InsertUserCommand, _connection);
                cmd.Parameters.AddWithValue("username", user.Username);
                cmd.Parameters.AddWithValue("password", user.Password);
                cmd.Parameters.AddWithValue("token", user.Token);
                mDB.WaitOne();
                affectedRows = cmd.ExecuteNonQuery();
            }
            catch (PostgresException)
            {
                // this might happen, if the user already exists (constraint violation)
                // we just catch it an keep affectedRows at zero
            }
            finally
            {
                mDB.ReleaseMutex();
            }
            return affectedRows > 0;
        }

        public int SelectCoinsByToken(string authToken)
        {
            int coins = 0;

            using (var cmd = new NpgsqlCommand(SelectCoinsByTokenCommand, _connection))
            {
                cmd.Parameters.AddWithValue("token", authToken);
                mDB.WaitOne();
                using var reader = cmd.ExecuteReader();
                mDB.ReleaseMutex();
                if (reader.Read())
                {
                    coins = ReadCoins(reader).Coins;
                }
            }

            return coins;
        }

        public void UpdateCoinsByMinus5(string authToken)
        {
            using (var cmd = new NpgsqlCommand(UpdateCoinsByMinus5Command, _connection))
            {
                cmd.Parameters.AddWithValue("token", authToken);
                mDB.WaitOne();
                cmd.ExecuteNonQuery();
                mDB.ReleaseMutex();
            }
        }

        public UserData SelectUserDataByUsername(string username)
        {
            UserData userData = null;
            using (var cmd = new NpgsqlCommand(SelectUserDataByUsernameCommand, _connection))
            {
                cmd.Parameters.AddWithValue("username", username);

                // take the first row, if any
                mDB.WaitOne();
                using var reader = cmd.ExecuteReader();
                mDB.ReleaseMutex();
                if (reader.Read())
                {
                    userData = ReadUserData(reader);
                }
            }
            return userData;
        }

        public void UpdateUserDataByUsername(string username, UserData userData)
        {
            using (var cmd = new NpgsqlCommand(UpdateUserDataByUsernameCommand, _connection))
            {
                cmd.Parameters.AddWithValue("name", userData.Name);
                cmd.Parameters.AddWithValue("bio", userData.Bio);
                cmd.Parameters.AddWithValue("image", userData.Image);
                cmd.Parameters.AddWithValue("username", username);
                mDB.WaitOne();
                cmd.ExecuteNonQuery();
                mDB.ReleaseMutex();
            }
        }

        public UserStats SelectUserStatsByToken(string authToken)
        {
            UserStats userStats = null;
            using (var cmd = new NpgsqlCommand(SelectUserStatsByUsernameCommand, _connection))
            {
                cmd.Parameters.AddWithValue("token", authToken);

                // take the first row, if any
                mDB.WaitOne();
                using var reader = cmd.ExecuteReader();
                mDB.ReleaseMutex();
                if (reader.Read())
                {
                    userStats = ReadUserStats(reader);
                }
            }
            return userStats;
        }

        public int UpdateStatsWinnerByToken(string authToken)
        {
            int affectedRows = 0;

            try
            {
                using var cmd = new NpgsqlCommand(UpdateStatsWinnerByTokenCommand, _connection);
                cmd.Parameters.AddWithValue("token", authToken);
                mDB.WaitOne();
                affectedRows = cmd.ExecuteNonQuery();
            }
            catch (PostgresException)
            {

            }
            finally
            {
                mDB.ReleaseMutex();
            }
            return affectedRows;
        }

        public int UpdateStatsLoserByToken(string authToken)
        {
            int affectedRows = 0;

            try
            {
                using var cmd = new NpgsqlCommand(UpdateStatsLoserByTokenCommand, _connection);
                cmd.Parameters.AddWithValue("token", authToken);
                mDB.WaitOne();
                affectedRows = cmd.ExecuteNonQuery();
            }
            catch (PostgresException)
            {

            }
            finally
            {
                mDB.ReleaseMutex();
            }
            return affectedRows;
        }

        public int UpdateStatsDrawByToken(string authToken)
        {
            int affectedRows = 0;

            try
            {
                using var cmd = new NpgsqlCommand(UpdateStatsDrawByTokenCommand, _connection);
                cmd.Parameters.AddWithValue("token", authToken);
                mDB.WaitOne();
                affectedRows = cmd.ExecuteNonQuery();
            }
            catch (PostgresException)
            {

            }
            finally
            {
                mDB.ReleaseMutex();
            }
            return affectedRows;
        }

        private void EnsureTables()
        {
            using var cmd = new NpgsqlCommand(CreateTableCommand, _connection);
            mDB.WaitOne();
            cmd.ExecuteNonQuery();
            mDB.ReleaseMutex();
        }

        private User ReadUser(IDataRecord record)
        {
            var user = new User
            {
                Username = Convert.ToString(record["username"]),
                Password = Convert.ToString(record["password"])
            };

            return user;
        }

        private User ReadCoins(IDataRecord record)
        {
            var user = new User
            {                
                Coins = Convert.ToInt32(record["coins"])
            };
            return user;
        }

        private UserData ReadUserData(IDataRecord record)
        {
            var userData = new UserData
            {
                Name = Convert.ToString(record["name"]),
                Bio = Convert.ToString(record["bio"]),
                Image = Convert.ToString(record["image"])
            };

            return userData;
        }

        private UserStats ReadUserStats(IDataRecord record)
        {
            var userData = new UserStats
            {
                Username = Convert.ToString(record["username"]),
                Wins = Convert.ToInt32(record["wins"]),
                Loses = Convert.ToInt32(record["loses"]),
                Draws = Convert.ToInt32(record["draws"]),
                Elo = Convert.ToInt32(record["elo"])
            };

            return userData;
        }
    }
}
