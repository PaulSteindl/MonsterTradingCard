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
using MonsterTradingCard.RouteCommands.Packages.CreatePackageCommand;
using MonsterTradingCard.RouteCommands.Packages.AcquirePackageCommand;
using MonsterTradingCard.Models.Credentials;
using MonsterTradingCard.Models.Card;
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
            var messageManager = new MSGMANAGER.MessageManager(db.UserRepository, db.CardRepository, db.PackageRepository);

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
            //router.AddProtectedRoute(HttpMethod.Post, "/cards", (r, p) => new ShowAqcuiredCards(messageManager));
        }

        private static T Deserialize<T>(string payload) where T : class
        {
            var deserializedData = JsonConvert.DeserializeObject<T>(payload);
            return deserializedData;
        }
    }
}
