using Newtonsoft.Json;
using HTTPServerCore.Response.Response;
using HTTPServerCore.Response.StatusCode;
using PROT_ROUTE_COM = MonsterTradingCard.RouteCommands.ProtectedRouteCommand;
using IMSGMANAGER = MonsterTradingCard.IMessageManager;
using System.Collections.Generic;
using MonsterTradingCard.Models.Deck;
using MonsterTradingCard.Models.FightLog;
using MonsterTradingCard.Models.User;
using System.Threading;
using System;

namespace MonsterTradingCard.RouteCommands.Battle.BattleCommand
{
    class BattleCommand : PROT_ROUTE_COM.ProtectedRouteCommand
    {
        private readonly IMSGMANAGER.IMessageManager messageManager;
        private static Queue<(User, ManualResetEvent, Action<FightLog>)> queue = new();

        public BattleCommand(IMSGMANAGER.IMessageManager messageManager)
        {
            this.messageManager = messageManager;
        }

        public override Response Execute()
        {
            var response = new Response();
            Deck playerDeck, opponentDeck;

            if(queue.Count == 0)
            {
                ManualResetEvent manualResetEvent = new ManualResetEvent(false);
                FightLog message = new FightLog();
                Action<FightLog> log = new Action<FightLog>(a =>
                {
                    message = a;
                });

                queue.Enqueue((User, manualResetEvent, log));
                manualResetEvent.WaitOne();
                return new Response()
                {
                    Payload = JsonConvert.SerializeObject(message),
                    StatusCode = StatusCode.Ok
                };
            }
            else
            {
                (User opponent, ManualResetEvent manualResetEvent, Action<FightLog> log) = queue.Dequeue();

                playerDeck = messageManager.GetDeck(User.Token);
                opponentDeck = messageManager.GetDeck(opponent.Token);

                FightLog l = new BattleManager(messageManager, playerDeck, User.Username, opponentDeck, opponent.Username).Startbattle();
                log(l);
                manualResetEvent.Set();

                return new Response()
                {
                    Payload = JsonConvert.SerializeObject(l),
                    StatusCode = StatusCode.Ok
                };
            }
        }
    }
}
