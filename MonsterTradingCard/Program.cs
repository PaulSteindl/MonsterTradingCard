using System;
using System.Net;
using Newtonsoft.Json;
using System.Threading;
using MSGMANAGER = MonsterTradingCard.MessageManager;
using MSG_ID_PROVIDER = MonsterTradingCard.MessageIdentityProvider;
using ID_ROUTE_PARSER = MonsterTradingCard.IdRouteParser;
using IMESSAGE_MANAGER = MonsterTradingCard.IMessageManager;
using MonsterTradingCard.RouteCommands.Messages.AddMessageCommand;
using MonsterTradingCard.RouteCommands.Messages.ListMessagesCommand;
using MonsterTradingCard.RouteCommands.Messages.RemoveMessageCommand;
using MonsterTradingCard.RouteCommands.Messages.ShowMessageCommand;
using MonsterTradingCard.RouteCommands.Messages.UpdateMessageCommand;
using MonsterTradingCard.RouteCommands.Users.RegisterCommand;
using MonsterTradingCard.RouteCommands.Users.LoginCommand;
using MonsterTradingCard.Models.Credentials;
using MonsterTradingCard.DAL.Database;
using HTTPServerCore.Routing.Router;
using HTTPServerCore.Request.MethodUtilities;
using HTTPServerCore.Server.HttpServer;

//Stripe - paymant gateway
namespace MonsterTradingCard
{
    class Program
    {
        static void Main(string[] args)
        {
            var db = new Database("Host=localhost;Port=5432;Username=postgres;Password=123;Database=swe1messagedb");
            var messageManager = new MSGMANAGER.MessageManager(db.MessageRepository, db.UserRepository);

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
            router.AddProtectedRoute(HttpMethod.Get, "/messages", (r, p) => new ListMessagesCommand(messageManager));
            router.AddProtectedRoute(HttpMethod.Post, "/messages", (r, p) => new AddMessageCommand(messageManager, r.Payload));
            router.AddProtectedRoute(HttpMethod.Get, "/messages/{id}", (r, p) => new ShowMessageCommand(messageManager, int.Parse(p["id"])));
            router.AddProtectedRoute(HttpMethod.Put, "/messages/{id}", (r, p) => new UpdateMessageCommand(messageManager, int.Parse(p["id"]), r.Payload));
            router.AddProtectedRoute(HttpMethod.Delete, "/messages/{id}", (r, p) => new RemoveMessageCommand(messageManager, int.Parse(p["id"])));
        }

        private static T Deserialize<T>(string payload) where T : class
        {
            var deserializedData = JsonConvert.DeserializeObject<T>(payload);
            return deserializedData;
        }
    }
}
