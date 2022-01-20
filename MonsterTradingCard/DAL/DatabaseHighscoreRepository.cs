using Npgsql;
using ISCOREREPO = MonsterTradingCard.DAL.IHighscoreRepository;
using MonsterTradingCard.Models.Highscore;
using System;
using System.Data;
using System.Collections.Generic;
using System.Threading;

namespace MonsterTradingCard.DAL.DatabaseHighscoreRepository
{
    class DatabaseHighscoreRepository : ISCOREREPO.IHighscoreRepository
    {
        private const string CreateTableCommand =   @"create table if not exists highscores
                                                    (
                                                        highscore_id serial
                                                            constraint highscores_pk
                                                                primary key,
                                                        username     text              not null
                                                            constraint highscores_users_username_fk
                                                                references users (username)
                                                                on update cascade on delete cascade,
                                                        score        integer default 0 not null
                                                    );

                                                    alter table highscores
                                                        owner to postgres;

                                                    create unique index if not exists highscores_username_uindex
                                                        on highscores (username);
                                                    ";

        private const string SelectHighscoresTop50Command = "SELECT username, score FROM highscores ORDER BY score DESC LIMIT 50";
        private const string SelectHighscoreByTokenCommand = "SELECT username, score FROM highscores WHERE username=@username";
        private const string UpdateWinByOneByTokenCommand = "UPDATE highscores SET score=score + 1 WHERE username=@username";
        private const string InsertWinOneByTokenCommand = "INSERT INTO highscores(username) VALUES (@username)";

        private readonly NpgsqlConnection _connection;
        private Mutex mDB;

        public DatabaseHighscoreRepository(NpgsqlConnection connection, Mutex mDB)
        {
            _connection = connection;
            this.mDB = mDB;
            EnsureTables();
        }

        public IEnumerable<Highscore> SelectHighscoreTop50()
        {
            var highscore = new List<Highscore>();

            using (var cmd = new NpgsqlCommand(SelectHighscoresTop50Command, _connection))
            {
                mDB.WaitOne();
                using var reader = cmd.ExecuteReader();
                mDB.ReleaseMutex();
                while (reader.Read())
                {
                    var card = ReadHighscore(reader);
                    highscore.Add(card);
                }
            }
            return highscore;
        }

        public Highscore SelectHighscoreByUsername(string username)
        {
            Highscore userStats = null;
            using (var cmd = new NpgsqlCommand(SelectHighscoreByTokenCommand, _connection))
            {
                cmd.Parameters.AddWithValue("username", username);

                // take the first row, if any
                mDB.WaitOne();
                using var reader = cmd.ExecuteReader();
                mDB.ReleaseMutex();
                if (reader.Read())
                {
                    userStats = ReadHighscore(reader);
                }
            }
            return userStats;
        }

        public int UpdateWinByOneByUsername(string username)
        {
            int affectedRows = 0;

            try
            {
                using var cmd = new NpgsqlCommand(UpdateWinByOneByTokenCommand, _connection);
                cmd.Parameters.AddWithValue("username", username);
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

        public int InsertWinOneByUsername(string username)
        {
            var affectedRows = 0;
            try
            {
                using var cmd = new NpgsqlCommand(InsertWinOneByTokenCommand, _connection);
                cmd.Parameters.AddWithValue("username", username);
;
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

        private Highscore ReadHighscore(IDataRecord record)
        {
            var highscore = new Highscore
            {
                Username = Convert.ToString(record["username"]),
                Score = Convert.ToInt32(record["score"])
            };

            return highscore;
        }
    }
}
