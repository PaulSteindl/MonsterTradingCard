using Npgsql;
using IUSERREPO = MonsterTradingCard.DAL.IUserRepository;
using MonsterTradingCard.Models.User;
using System;
using System.Data;

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
                                                        winrate  real    default 0           not null
                                                    );

                                                            alter table users
                                                                owner to postgres;

                                                            create unique index if not exists users_user_id_uindex
                                                                on users(user_id);

                                                            create unique index if not exists users_token_uindex
                                                                on users(token);

                                                            create unique index if not exists users_username_uindex
                                                                on users(username);

                                                    ";

        private const string InsertUserCommand = "INSERT INTO users(username, password, token) VALUES (@username, @password, @token)";
        private const string SelectUserByTokenCommand = "SELECT username, password FROM users WHERE token=@token";
        private const string SelectUserByCredentialsCommand = "SELECT username, password FROM users WHERE username=@username AND password=@password";
        private const string SelectUserStatsByTokenCommand = "SELECT username, bio, image, wins, loses, winrate FROM users WHERE token=@token";

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
    }
}
