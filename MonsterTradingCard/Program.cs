using System;
using System.Net;
using Newtonsoft.Json;
using System.Threading;
using MSGMANAGER = MonsterTradingCard.MessageManager;
using ID_ROUTE_PARSER = MonsterTradingCard.IdRouteParser;
using IMESSAGE_MANAGER = MonsterTradingCard.IMessageManager;
using MSG_ID_PROVIDER = MonsterTradingCard.MessageIdentityProvider;
using MonsterTradingCard.RouteCommands.Users.RegisterCommand;
using MonsterTradingCard.RouteCommands.Users.LoginCommand;
using MonsterTradingCard.RouteCommands.Users.ShowUserDataCommand;
using MonsterTradingCard.RouteCommands.Users.UpdateUserDataCommand;
using MonsterTradingCard.RouteCommands.Users.ShowUserStatsCommand;
using MonsterTradingCard.RouteCommands.Packages.CreatePackageCommand;
using MonsterTradingCard.RouteCommands.Packages.AcquirePackageCommand;
using MonsterTradingCard.RouteCommands.Cards.ShowAcquiredCardsCommand;
using MonsterTradingCard.RouteCommands.Decks.ShowDeckCommand;
using MonsterTradingCard.RouteCommands.Decks.ConfigureDeckCommand;
using MonsterTradingCard.RouteCommands.Decks.ShowDeckPlainCommand;
using MonsterTradingCard.RouteCommands.Battle.ShowScoreCommand;
using MonsterTradingCard.RouteCommands.Trading.CreateTradingDealCommand;
using MonsterTradingCard.RouteCommands.Trading.DeleteTradingDealCommand;
using MonsterTradingCard.RouteCommands.Trading.ShowTradingDealsCommand;
using MonsterTradingCard.RouteCommands.Trading.TradeCommand;
using MonsterTradingCard.Models.Credentials;
using MonsterTradingCard.Models.Card;
using MonsterTradingCard.Models.UserData;
using MonsterTradingCard.Models.TradingDeal;
using MonsterTradingCard.DAL.Database;
using HTTPServerCore.Routing.Router;
using HTTPServerCore.Request.MethodUtilities;
using HTTPServerCore.Server.HttpServer;
using System.Collections.Generic;

//Stripe - paymant gateway
namespace MonsterTradingCard
{
    class Program
    {
        static void Main(string[] args)
        {
            var db = new Database("Host=localhost;Port=5432;Username=postgres;Password=123;Database=swe1messagedb");
            var messageManager = new MSGMANAGER.MessageManager(db.UserRepository, db.CardRepository, db.PackageRepository, db.DeckRepository, db.HighscoreRepositroy, db.TradingdealRepository);

            var identityProvider = new MSG_ID_PROVIDER.MessageIdentityProvider(db.UserRepository);
            var routeParser = new ID_ROUTE_PARSER.IdRouteParser();

            var router = new Router(routeParser, identityProvider);
            RegisterRoutes(router, messageManager);

            var httpServer = new HttpServer(IPAddress.Any, 10001, router);
            httpServer.Start();

            //Thread.CurrentThread.Join();

            Console.WriteLine("HI");
        }

        private static void RegisterRoutes(Router router, IMESSAGE_MANAGER.IMessageManager messageManager)
        {
            // public routes
            router.AddRoute(HttpMethod.Post, "/sessions", (r, p) => new LoginCommand(messageManager, Deserialize<Credentials>(r.Payload)));
            router.AddRoute(HttpMethod.Post, "/users", (r, p) => new RegisterCommand(messageManager, Deserialize<Credentials>(r.Payload)));

            // protected routes
            router.AddProtectedRoute(HttpMethod.Post, "/packages", (r, p) => new CreatePackageCommand(messageManager, Deserialize<List<Card>>(r.Payload)));
            router.AddProtectedRoute(HttpMethod.Post, "/transactions/packages", (r, p) => new AcquirePackageCommand(messageManager));
            router.AddProtectedRoute(HttpMethod.Get, "/cards", (r, p) => new ShowAcquiredCardsCommand(messageManager));
            router.AddProtectedRoute(HttpMethod.Get, "/deck", (r, p) => new ShowDeckCommand(messageManager));
            router.AddProtectedRoute(HttpMethod.Get, "/deck\\?format=plain", (r, p) => new ShowDeckPlainCommand(messageManager));
            router.AddProtectedRoute(HttpMethod.Put, "/deck", (r, p) => new ConfigureDeckCommand(messageManager, Deserialize<List<string>>(r.Payload)));
            router.AddProtectedRoute(HttpMethod.Get, "/users/{id}", (r, p) => new ShowUserDataCommand(messageManager, p["id"]));
            router.AddProtectedRoute(HttpMethod.Put, "/users/{id}", (r, p) => new UpdateUserDataCommand(messageManager, p["id"], Deserialize<UserData>(r.Payload)));
            router.AddProtectedRoute(HttpMethod.Get, "/stats", (r, p) => new ShowUserStatsCommand(messageManager));
            router.AddProtectedRoute(HttpMethod.Get, "/score", (r, p) => new ShowScoreCommand(messageManager));
            router.AddProtectedRoute(HttpMethod.Get, "/tradings", (r, p) => new ShowTradingDealsCommand(messageManager));
            router.AddProtectedRoute(HttpMethod.Post, "/tradings", (r, p) => new CreateTradingDealCommand(messageManager, Deserialize<TradingDeal>(r.Payload)));
            router.AddProtectedRoute(HttpMethod.Delete, "/tradings/{id}", (r, p) => new DeleteTradingDealCommand(messageManager, p["id"]));
            router.AddProtectedRoute(HttpMethod.Post, "/tradings/{id}", (r, p) => new TradeCommand(messageManager, p["id"], Deserialize<string>(r.Payload)));
        }

        private static T Deserialize<T>(string payload) where T : class
        {
            var deserializedData = JsonConvert.DeserializeObject<T>(payload);
            return deserializedData;
        }
    }
}
