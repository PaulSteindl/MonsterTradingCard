using Npgsql;
using IUSERREPO = MonsterTradingCard.DAL.IUserRepository;
using MonsterTradingCard.Models.User;
using System;
using System.Data;
using System.Globalization;

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
                                                        winrate  real    default 0           not null,
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

        private const string InsertUserCommand = "INSERT INTO users(username, password, token) VALUES (@username, @password, @token)";
        private const string SelectUserByTokenCommand = "SELECT username, password FROM users WHERE token=@token";
        private const string SelectUserByCredentialsCommand = "SELECT username, password FROM users WHERE username=@username AND password=@password";
        private const string SelectUserStatsByTokenCommand = "SELECT username, bio, image, wins, loses, winrate, coins FROM users WHERE token=@token";
        private const string SelectCoinsByTokenCommand = "SELECT coins FROM users WHERE token=@token";
        private const string UpdateCoinsByMinus5Command = "UPDATE users SET coins=coins - 5 WHERE token=@token";

        private readonly NpgsqlConnection _connection;

        public DatabaseUserRepository(NpgsqlConnection connection)
        {
            _connection = connection;
            EnsureTables();
        }

        public User GetUserByAuthToken(string authToken)
        {
            User user = null;
            using (var cmd = new NpgsqlCommand(SelectUserByTokenCommand, _connection))
            {
                cmd.Parameters.AddWithValue("token", authToken);

                // take the first row, if any
                using var reader = cmd.ExecuteReader();
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
                using var reader = cmd.ExecuteReader();
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
                affectedRows = cmd.ExecuteNonQuery();
            }
            catch (PostgresException)
            {
                // this might happen, if the user already exists (constraint violation)
                // we just catch it an keep affectedRows at zero
            }
            return affectedRows > 0;
        }
        
        //public User SelectUserStatsByToken(string token)
        //{
        //    User user = null;
        //    using (var cmd = new NpgsqlCommand(SelectUserStatsByTokenCommand, _connection))
        //    {
        //        cmd.Parameters.AddWithValue("username", username);
        //        cmd.Parameters.AddWithValue("password", password);

        //        // take the first row, if any
        //        using var reader = cmd.ExecuteReader();
        //        if (reader.Read())
        //        {
        //            user = ReadUser(reader);
        //        }
        //    }
        //    return user;
        //}

        public int SelectCoinsByToken(string authToken)
        {
            int coins = 0;

            using (var cmd = new NpgsqlCommand(SelectCoinsByTokenCommand, _connection))
            {
                cmd.Parameters.AddWithValue("token", authToken);

                using var reader = cmd.ExecuteReader();
                if(reader.Read())
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
                cmd.ExecuteNonQuery();
            }
        }

        private void EnsureTables()
        {
            using var cmd = new NpgsqlCommand(CreateTableCommand, _connection);
            cmd.ExecuteNonQuery();
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
                //Username = Convert.ToString(record["username"]),
                //Password = Convert.ToString(record["password"]),
                //Bio = Convert.ToString(record["bio"]),
                //Image = Convert.ToString(record["image"]),
                //Wins = Convert.ToInt32(record["wins"]),
                //Loses = Convert.ToInt32(record["loses"]),
                //Winrate = float.Parse(Convert.ToString(record["winrate"]), CultureInfo.InvariantCulture.NumberFormat),
                Coins = Convert.ToInt32(record["coins"])
            };
            return user;
        }
    }
}
