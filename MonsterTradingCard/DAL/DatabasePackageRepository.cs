using Npgsql;
using IPACKAGEREPO = MonsterTradingCard.DAL.IPackageRepository;
using MonsterTradingCard.Models.Package;
using System;
using System.Data;
using System.Collections.Generic;

namespace MonsterTradingCard.DAL.DatabasePackageRepository
{
    class DatabasePackageRepository : IPACKAGEREPO.IPackageRepository
    {
        private const string CreateTableCommand =   @"create table if not exists packages
                                                    (
                                                        package_id serial
                                                            constraint packages_pk
                                                                primary key,
                                                        card1_id   text not null
                                                            constraint packages_cards_card_id_fk
                                                                references cards
                                                                on update cascade on delete cascade,
                                                        card2_id   text not null
                                                            constraint packages_cards_card_id_fk_2
                                                                references cards
                                                                on update cascade on delete cascade,
                                                        card3_id   text not null
                                                            constraint packages_cards_card_id_fk_3
                                                                references cards
                                                                on update cascade on delete cascade,
                                                        card4_id   text not null
                                                            constraint packages_cards_card_id_fk_4
                                                                references cards
                                                                on update cascade on delete cascade,
                                                        card5_id   text not null
                                                            constraint packages_cards_card_id_fk_5
                                                                references cards
                                                                on update cascade on delete cascade,
                                                        owner      text
                                                            constraint packages_users_token_fk
                                                                references users (token)
                                                                on update cascade on delete cascade
                                                    );

                                                    alter table packages
                                                        owner to postgres;

                                                    create unique index if not exists packages_card1_uindex
                                                        on packages (card1_id);

                                                    create unique index if not exists packages_card2_uindex
                                                        on packages (card2_id);

                                                    create unique index if not exists packages_card3_uindex
                                                        on packages (card3_id);

                                                    create unique index if not exists packages_card4_uindex
                                                        on packages (card4_id);

                                                    create unique index if not exists packages_card5_uindex
                                                        on packages (card5_id);
                                                    ";

        private const string InsertPackageCommand = "INSERT INTO packages(card1_id, card2_id, card3_id, card4_id, card5_id) VALUES (@card1_id, @card2_id, @card3_id, @card4_id, @card5_id)";
        private const string SelectRandomPackageWithNoOwnerCommand = "SELECT package_id, card1_id, card2_id, card3_id, card4_id, card5_id FROM packages WHERE owner IS NULL";
        private const string UpdatePackageOwnerByTokenCommand = "UPDATE packages SET owner=@owner WHERE package_id=@package_id";

        private readonly NpgsqlConnection _connection;

        public DatabasePackageRepository(NpgsqlConnection connection)
        {
            _connection = connection;
            EnsureTables();
        }

        public void InsertPackage(Package package)
        {
            using var cmd = new NpgsqlCommand(InsertPackageCommand, _connection);
            cmd.Parameters.AddWithValue("card1_id", package.CardIds[0]);
            cmd.Parameters.AddWithValue("card2_id", package.CardIds[1]);
            cmd.Parameters.AddWithValue("card3_id", package.CardIds[2]);
            cmd.Parameters.AddWithValue("card4_id", package.CardIds[3]);
            cmd.Parameters.AddWithValue("card5_id", package.CardIds[4]);
            var result = cmd.ExecuteScalar();
        }

        public Package SelectRandomPackage()
        {
            var packages = new List<Package>();
            Random random = new Random();

            using (var cmd = new NpgsqlCommand(SelectRandomPackageWithNoOwnerCommand, _connection))
            {
                using var reader = cmd.ExecuteReader();
                while(reader.Read())
                {
                    var package = ReadUnClaimedPackage(reader);
                    packages.Add(package);
                }
            }

            return packages.Count > 0 ? packages[random.Next() % packages.Count] : null;
        }

        public void UpdatePackageOwner(int packageId, string authToken)
        {
            using var cmd = new NpgsqlCommand(UpdatePackageOwnerByTokenCommand, _connection);
            cmd.Parameters.AddWithValue("owner", authToken);
            cmd.Parameters.AddWithValue("package_id", packageId);
            cmd.ExecuteNonQuery();
        }

        private void EnsureTables()
        {
            using var cmd = new NpgsqlCommand(CreateTableCommand, _connection);
            cmd.ExecuteNonQuery();
        }

        private Package ReadClaimedPackage(IDataRecord record)
        {
            var cardIds = new List<string>();

            cardIds.Add(Convert.ToString(record["card1_id"]));
            cardIds.Add(Convert.ToString(record["card2_id"]));
            cardIds.Add(Convert.ToString(record["card3_id"]));
            cardIds.Add(Convert.ToString(record["card4_id"]));
            cardIds.Add(Convert.ToString(record["card5_id"]));

            var package = new Package
            {
                Id = Convert.ToInt32(record["package_id"]),
                Owner = Convert.ToString(record["owner"]),
                CardIds = cardIds
            };
            return package;
        }

        private Package ReadUnClaimedPackage(IDataRecord record)
        {
            var cardIds = new List<string>();

            cardIds.Add(Convert.ToString(record["card1_id"]));
            cardIds.Add(Convert.ToString(record["card2_id"]));
            cardIds.Add(Convert.ToString(record["card3_id"]));
            cardIds.Add(Convert.ToString(record["card4_id"]));
            cardIds.Add(Convert.ToString(record["card5_id"]));

            var package = new Package
            {
                Id = Convert.ToInt32(record["package_id"]),
                CardIds = cardIds
            };
            return package;
        }
    }
}
