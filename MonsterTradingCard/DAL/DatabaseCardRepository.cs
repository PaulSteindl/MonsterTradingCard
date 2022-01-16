using Npgsql;
using ICARDREPO = MonsterTradingCard.DAL.ICardRepository;
using MonsterTradingCard.Models.Card;
using System;
using System.Data;
using System.Collections.Generic;

namespace MonsterTradingCard.DAL.DatabaseCardRepository
{
    class DatabaseCardRepository : ICARDREPO.ICardRepository
    {
        private const string CreateTableCommand =   @"create table if not exists cards
                                                    (
                                                        card_id     text                  not null
                                                            constraint cards_pk
                                                                primary key,
                                                        name        text                  not null,
                                                        dmg         integer               not null,
                                                        token       text
                                                            constraint cards_users_token_fk
                                                                references users(token)
                                                                on update cascade on delete cascade,
                                                        ""tradeOpen"" boolean default false not null
                                                    );

                                                            alter table cards
                                                                owner to postgres;

                                                            create unique index if not exists cards_card_id_uindex
                                                                on cards(card_id);

                                                            create unique index if not exists cards_user_id_uindex
                                                                on cards(token);

                                                    ";

        private const string InsertCardCommand = "INSERT INTO cards(card_id, name, dmg) VALUES (@card_id, @name, @dmg)";
        private const string SelectCardsByTokenCommand = "SELECT name, dmg, \"tradeOpen\" FROM cards WHERE token=@token";

        private readonly NpgsqlConnection _connection;

        public DatabaseCardRepository(NpgsqlConnection connection)
        {
            _connection = connection;
            EnsureTables();
        }

        public IEnumerable<Card> GetCards(string token)
        {
            var cards = new List<Card>();

            using (var cmd = new NpgsqlCommand(SelectCardsByTokenCommand, _connection))
            {
                cmd.Parameters.AddWithValue("token", token);

                // take the first row, if any
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    var card = ReadCard(reader);
                    cards.Add(card);
                }
            }
            return cards;
        }

        public void InsertCard(Card card)
        {
            using var cmd = new NpgsqlCommand(InsertCardCommand, _connection);
            cmd.Parameters.AddWithValue("card_id", card.Id);
            cmd.Parameters.AddWithValue("name", card.Name);
            cmd.Parameters.AddWithValue("dmg", card.Dmg);
            var result = cmd.ExecuteScalar();

            card.Dmg = Convert.ToInt32(result);
        }

        private void EnsureTables()
        {
            using var cmd = new NpgsqlCommand(CreateTableCommand, _connection);
            cmd.ExecuteNonQuery();
        }

        private Card ReadCard(IDataRecord record)
        {
            var message = new Card
            {
                Id = Convert.ToString(record["id"]),
                Name = Convert.ToString(record["name"]),
                Dmg = Convert.ToInt32(record["dmg"])
            };

            return message;
        }
    }
}
