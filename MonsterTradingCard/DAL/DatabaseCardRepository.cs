using Npgsql;
using ICARDREPO = MonsterTradingCard.DAL.ICardRepository;
using MonsterTradingCard.Models.Card;
using System;
using System.Data;
using System.Collections.Generic;
using System.Threading;

namespace MonsterTradingCard.DAL.DatabaseCardRepository
{
    class DatabaseCardRepository : ICARDREPO.ICardRepository
    {
        private const string CreateTableCommand =   @"create table if not exists cards
                                                    (
                                                        card_id text    not null
                                                            constraint cards_pk
                                                                primary key,
                                                        name    text    not null,
                                                        dmg     integer not null,
                                                        token   text
                                                            constraint cards_users_token_fk
                                                                references users (token)
                                                                on update cascade on delete cascade
                                                                deferrable initially deferred
                                                    );
                                                    ";

        private const string InsertCardCommand = "INSERT INTO cards(card_id, name, dmg) VALUES (@card_id, @name, @dmg)";
        private const string SelectCardsByTokenCommand = "SELECT * FROM cards WHERE token=@token";
        private const string SelectCardByIdCommand = "SELECT * FROM cards WHERE card_id=@card_id";
        private const string UpdateCardOwnerByTokenCommand = "UPDATE cards SET token=@token WHERE card_id=@card_id";
        private const string SelectCardByIdAndTokenCommand = "SELECT * FROM cards WHERE card_id=@card_id AND token=@token";

        private readonly NpgsqlConnection _connection;
        private Mutex mDB;

        public DatabaseCardRepository(NpgsqlConnection connection, Mutex mDB)
        {
            _connection = connection;
            this.mDB = mDB;
            EnsureTables();
        }

        public IEnumerable<Card> SelectCardsByToken(string token)
        {
            var cards = new List<Card>();

            using (var cmd = new NpgsqlCommand(SelectCardsByTokenCommand, _connection))
            {
                cmd.Parameters.AddWithValue("token", token);

                // take the first row, if any
                mDB.WaitOne();
                using var reader = cmd.ExecuteReader();
                mDB.ReleaseMutex();
                while (reader.Read())
                {
                    var card = ReadCard(reader);
                    cards.Add(card);
                }
            }
            return cards;
        }

        public bool InsertCard(Card card)
        {
            var affectedRows = 0;
            try
            {
                using var cmd = new NpgsqlCommand(InsertCardCommand, _connection);
                cmd.Parameters.AddWithValue("card_id", card.Id);
                cmd.Parameters.AddWithValue("name", card.Name);
                cmd.Parameters.AddWithValue("dmg", card.Damage);
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
            return affectedRows > 0;
        }

        public Card SelectCardById(string cardId)
        {
            Card card = null;

            using (var cmd = new NpgsqlCommand(SelectCardByIdCommand, _connection))
            {
                cmd.Parameters.AddWithValue("card_id", cardId);
                mDB.WaitOne();
                using var reader = cmd.ExecuteReader();
                mDB.ReleaseMutex();
                if (reader.Read())
                {
                    card = ReadCard(reader);
                }
            }
            return card;
        }

        public void UpdateCardOwner(string cardId, string authToken)
        {
            using var cmd = new NpgsqlCommand(UpdateCardOwnerByTokenCommand, _connection);
            cmd.Parameters.AddWithValue("token", authToken);
            cmd.Parameters.AddWithValue("card_id", cardId);
            mDB.WaitOne();
            cmd.ExecuteNonQuery();
            mDB.ReleaseMutex();
        }

        public Card SelectCardByIdAndToken(string cardId, string authToken)
        {
            Card card = null;

            using (var cmd = new NpgsqlCommand(SelectCardByIdAndTokenCommand, _connection))
            {
                cmd.Parameters.AddWithValue("card_id", cardId);
                cmd.Parameters.AddWithValue("token", authToken);
                mDB.WaitOne();
                using var reader = cmd.ExecuteReader();
                mDB.ReleaseMutex();
                if (reader.Read())
                {
                    card = ReadCard(reader);
                }
            }
            return card;
        }

        private void EnsureTables()
        {
            using var cmd = new NpgsqlCommand(CreateTableCommand, _connection);
            mDB.WaitOne();
            cmd.ExecuteNonQuery();
            mDB.ReleaseMutex();
        }

        private Card ReadCard(IDataRecord record)
        {
            var message = new Card
            {
                Id = Convert.ToString(record["card_id"]),
                Name = Convert.ToString(record["name"]),
                Damage = Convert.ToInt32(record["dmg"])
            };

            return message;
        }
    }
}
