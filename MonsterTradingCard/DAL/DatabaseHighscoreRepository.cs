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
