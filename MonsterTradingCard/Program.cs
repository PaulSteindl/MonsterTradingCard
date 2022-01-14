using System;
using MonsterTradingCard.DAL.Database;

//Stripe - paymant gateway
namespace MonsterTradingCard
{
    class Program
    {
        static void Main(string[] args)
        {
            var db = new Database("Host=localhost;Port=5432;Username=postgres;Password=123;Database=swe1messagedb");

            Console.WriteLine("HI");
        }
    }
}
