using Npgsql;
using ITDEREPO = MonsterTradingCard.DAL.ITradingdealRepository;
using MonsterTradingCard.Models.TradingDeal;
using MonsterTradingCard.Models.Enums.Element;
using MonsterTradingCard.Models.Enums.Species;
using MonsterTradingCard.Models.Enums.CardType;
using System;
using System.Data;
using System.Collections.Generic;

namespace MonsterTradingCard.DAL.DatabaseTradingdealRepository
{
    class DatabaseTradingdealRepository : ITDEREPO.ITradingdealRepository
    {
        private const string CreateTableCommand =   @"create table if not exists tradingdeals
                                                    (
                                                        trading_id  text not null
                                                            constraint tradingdeals_pk
                                                                primary key,
                                                        cardtotrade text not null
                                                            constraint tradingdeals_cards_card_id_fk
                                                                references cards
                                                                on update cascade on delete cascade,
                                                        mindmg      integer,
                                                        element     information_schema.element,
                                                        cardtype    information_schema.card_type,
                                                        species     information_schema.species,
                                                        usertoken   text not null
                                                            constraint tradingdeals_users_token_fk
                                                                references users (token)
                                                                on update cascade on delete cascade
                                                    );

                                                    alter table tradingdeals
                                                        owner to postgres;

                                                    create unique index if not exists tradingdeals_cardtotrade_uindex
                                                        on tradingdeals (cardtotrade);

                                                    create unique index if not exists tradingdeals_trading_id_uindex
                                                        on tradingdeals (trading_id);
                                                    ";

        private const string SelectTradingdealsCommand = "SELECT * FROM tradingdeals";
        private const string SelectTradingdealByCardIdCommand = "SELECT * FROM tradingdeals WHERE cardtotrade=@card_id";
        private const string InsertTradingdealCommand = "INSERT INTO tradingdeals (trading_id, usertoken, cardtotrade, mindmg, element, cardtype, species) VALUES (@trading_id, @usertoken, @cardtotrade, @mindmg, @element, @cardtype, @species)";
        private const string DeleteTradingdealByTradingIdAndTokenCommand = "DELETE FROM tradingdeals WHERE trading_id=@trading_id AND usertoken=@usertoken";
        private const string SelectTradingdealByTradingIdCommand = "SELECT * FROM tradingdeals WHERE trading_id=@trading_id";

        private readonly NpgsqlConnection _connection;

        public DatabaseTradingdealRepository(NpgsqlConnection connection)
        {
            _connection = connection;
            EnsureTables();
        }

        public IEnumerable<TradingDeal> SelectOpenTradingdeals()
        {
            var tradingDeals = new List<TradingDeal>();

            using (var cmd = new NpgsqlCommand(SelectTradingdealsCommand, _connection))
            {

                // take the first row, if any
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    var tradingDeal = ReadTradingdeal(reader);
                    tradingDeals.Add(tradingDeal);
                }
            }
            return tradingDeals;
        }

        public TradingDeal SelectTradingdealByCardId(string cardId)
        {
            TradingDeal tradingDeal = null;

            using (var cmd = new NpgsqlCommand(SelectTradingdealByCardIdCommand, _connection))
            {
                cmd.Parameters.AddWithValue("card_id", cardId);

                using var reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    tradingDeal = ReadTradingdeal(reader);
                }
            }

            return tradingDeal;
        }

        public int InsertTradingdeal(TradingDeal tradingDeal, string authToken)
        {
            var affectedRows = 0;

            try
            { 
                using var cmd = new NpgsqlCommand(InsertTradingdealCommand, _connection);
                cmd.Parameters.AddWithValue("trading_id", tradingDeal.Id);
                cmd.Parameters.AddWithValue("cardtotrade", tradingDeal.CardToTrade);
                cmd.Parameters.AddWithValue("usertoken", authToken);

                if (tradingDeal.MinimumDamage.HasValue)
                    cmd.Parameters.AddWithValue("mindmg", tradingDeal.MinimumDamage);
                else
                    cmd.Parameters.AddWithValue("mindmg", DBNull.Value);

                if (tradingDeal.Element.HasValue)
                    cmd.Parameters.AddWithValue("element", tradingDeal.Element);
                else
                    cmd.Parameters.AddWithValue("element", DBNull.Value);

                if (tradingDeal.Type.HasValue)
                    cmd.Parameters.AddWithValue("cardtype", tradingDeal.Type);
                else
                    cmd.Parameters.AddWithValue("cardtype", DBNull.Value);

                if (tradingDeal.Species.HasValue)
                    cmd.Parameters.AddWithValue("species", tradingDeal.Species);
                else
                    cmd.Parameters.AddWithValue("species", DBNull.Value);

                affectedRows = cmd.ExecuteNonQuery();
            }
            catch (PostgresException)
            {

            }
            return affectedRows;
        }

        public int DeleteTradingdealByTradingIdAndToken(string tradingDealId, string authToken)
        {
            var rowsAffected = 0;

            try
            {
                var cmd = new NpgsqlCommand(DeleteTradingdealByTradingIdAndTokenCommand, _connection);
                cmd.Parameters.AddWithValue("trading_id", tradingDealId);
                cmd.Parameters.AddWithValue("usertoken", authToken);
                rowsAffected = cmd.ExecuteNonQuery();
            }
            catch (PostgresException)
            {

            }

            return rowsAffected;
        }

        public TradingDeal SelectTradingdealAndTokenByTradingId(string tradingDealId)
        {
            TradingDeal tradingDeal = null;

            using (var cmd = new NpgsqlCommand(SelectTradingdealByTradingIdCommand, _connection))
            {
                cmd.Parameters.AddWithValue("trading_id", tradingDealId);

                using var reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    tradingDeal = ReadTradingdealWithToken(reader);
                }
            }

            return tradingDeal;
        }

        private void EnsureTables()
        {
            using var cmd = new NpgsqlCommand(CreateTableCommand, _connection);
            cmd.ExecuteNonQuery();
        }

        private TradingDeal ReadTradingdeal(IDataRecord record)
        {
            var tradingdeal = new TradingDeal
            {
                Id = Convert.ToString(record["trading_id"]),
                CardToTrade = Convert.ToString(record["cardtotrade"]),
                MinimumDamage = record["mindmg"] is DBNull ? null : Convert.ToInt32(record["mindmg"]),
                Element = record["element"] is DBNull ? null : (Element)Enum.Parse(typeof(Element), Convert.ToString(record["element"])),
                Type = record["cardtype"] is DBNull ? null : (CardType)Enum.Parse(typeof(CardType), Convert.ToString(record["cardtype"])),
                Species = record["species"] is DBNull ? null : (Species)Enum.Parse(typeof(Species), Convert.ToString(record["species"]))
            };
            return tradingdeal;
        }

        private TradingDeal ReadTradingdealWithToken(IDataRecord record)
        {
            var tradingdeal = new TradingDeal
            {
                Id = Convert.ToString(record["trading_id"]),
                Usertoken = Convert.ToString(record["usertoken"]),
                CardToTrade = Convert.ToString(record["cardtotrade"]),
                MinimumDamage = record["mindmg"] is DBNull ? null : Convert.ToInt32(record["mindmg"]),
                Element = record["element"] is DBNull ? null : (Element)Enum.Parse(typeof(Element), Convert.ToString(record["element"])),
                Type = record["cardtype"] is DBNull ? null : (CardType)Enum.Parse(typeof(CardType), Convert.ToString(record["cardtype"])),
                Species = record["species"] is DBNull ? null : (Species)Enum.Parse(typeof(Species), Convert.ToString(record["species"]))
            };
            return tradingdeal;
        }
    }
}