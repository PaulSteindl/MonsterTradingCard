using Npgsql;
using IDECKREPO = MonsterTradingCard.DAL.IDeckRepository;
using MonsterTradingCard.Models.Deck;
using System;
using System.Data;
using System.Collections.Generic;

namespace MonsterTradingCard.DAL.DatabaseDeckRepository
{
    class DatabaseDeckRepository : IDECKREPO.IDeckRepository
    {
        private const string CreateTableCommand =   @"create table if not exists decks
                                                    (
                                                        deck_id  serial
                                                            constraint decks_pk
                                                                primary key,
                                                        token    text not null
                                                            constraint decks_users_token_fk
                                                                references users (token)
                                                                on update cascade on delete cascade,
                                                        card1_id text
                                                            constraint decks_cards_card_id_fk
                                                                references cards
                                                                on update cascade on delete cascade,
                                                        card2_id text
                                                            constraint decks_cards_card_id_fk_2
                                                                references cards
                                                                on update cascade on delete cascade,
                                                        card3_id text
                                                            constraint decks_cards_card_id_fk_3
                                                                references cards
                                                                on update cascade on delete cascade,
                                                        card4_id text
                                                            constraint decks_cards_card_id_fk_4
                                                                references cards
                                                                on update cascade on delete cascade
                                                    );

                                                    alter table decks
                                                        owner to postgres;

                                                    create unique index if not exists decks_card1_id_uindex
                                                        on decks (card1_id);

                                                    create unique index if not exists decks_card2_id_uindex
                                                        on decks (card2_id);

                                                    create unique index if not exists decks_card3_id_uindex
                                                        on decks (card3_id);

                                                    create unique index if not exists decks_card4_id_uindex
                                                        on decks (card4_id);

                                                    create unique index if not exists decks_deck_id_uindex
                                                        on decks (deck_id);

                                                    create unique index if not exists decks_user_id_uindex
                                                        on decks (token);
                                                    ";

        private const string SelectDeckByTokenCommand = "SELECT * FROM decks WHERE token=@token";

        private readonly NpgsqlConnection _connection;

        public DatabaseDeckRepository(NpgsqlConnection connection)
        {
            _connection = connection;
            EnsureTables();
        }

        public Deck GetDeckByToken(string authToken)
        {
            Deck deck = null;

            using (var cmd = new NpgsqlCommand(SelectDeckByTokenCommand, _connection))
            {
                cmd.Parameters.AddWithValue("token", authToken);

                using var reader = cmd.ExecuteReader();
                if(reader.Read())
                {
                    deck = ReadDeck(reader);
                }
            }

            return deck;
        }

        private void EnsureTables()
        {
            using var cmd = new NpgsqlCommand(CreateTableCommand, _connection);
            cmd.ExecuteNonQuery();
        }

        private Deck ReadDeck(IDataRecord record)
        {
            var cardIds = new List<string>();

            cardIds.Add(Convert.ToString(record["card1_id"]));
            cardIds.Add(Convert.ToString(record["card2_id"]));
            cardIds.Add(Convert.ToString(record["card3_id"]));
            cardIds.Add(Convert.ToString(record["card4_id"]));

            var deck = new Deck
            {
                Id = Convert.ToInt32(record["deck_id"]),
                Token = Convert.ToString(record["token"]),
                CardIds = cardIds
            };
            return deck;
        }
    }
}
