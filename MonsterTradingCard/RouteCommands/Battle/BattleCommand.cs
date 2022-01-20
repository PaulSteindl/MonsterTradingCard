using Newtonsoft.Json;
using HTTPServerCore.Response.Response;
using HTTPServerCore.Response.StatusCode;
using PROT_ROUTE_COM = MonsterTradingCard.RouteCommands.ProtectedRouteCommand;
using INVALIDDECK = MonsterTradingCard.DeckNot4CardsException;
using IMSGMANAGER = MonsterTradingCard.IMessageManager;
using NOSTATSUPDATE = MonsterTradingCard.NoStatsUpdateException;
using BATTLEMANAGER = MonsterTradingCard.BattleManager;
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
        private static Queue<(User, Deck, ManualResetEvent, Action<FightLog>)> queue = new();

        public BattleCommand(IMSGMANAGER.IMessageManager messageManager)
        {
            this.messageManager = messageManager;
        }

        public override Response Execute()
        {
            var response = new Response();
            Deck playerDeck;

            try
            {
                if ((playerDeck = messageManager.GetDeck(User.Token)) == null)
                    throw new INVALIDDECK.DeckNot4CardsException();

                if (queue.Count == 0)
                {
                    ManualResetEvent manualResetEvent = new ManualResetEvent(false);
                    FightLog message = new FightLog();
                    Action<FightLog> log = new Action<FightLog>(a =>
                    {
                        message = a;
                    });

                    queue.Enqueue((User, playerDeck, manualResetEvent, log));
                    manualResetEvent.WaitOne();
                    return new Response()
                    {
                        Payload = JsonConvert.SerializeObject(message),
                        StatusCode = StatusCode.Ok
                    };
                }
                else
                {
                    (User opponent, Deck opponentDeck, ManualResetEvent manualResetEvent, Action<FightLog> log) = queue.Dequeue();

                    FightLog l = new BATTLEMANAGER.BattleManager(messageManager, playerDeck, User.Username, opponentDeck, opponent.Username).Startbattle();
                    log(l);
                    manualResetEvent.Set();

                    return new Response()
                    {
                        Payload = JsonConvert.SerializeObject(l),
                        StatusCode = StatusCode.Ok
                    };
                }
            }
            catch(INVALIDDECK.DeckNot4CardsException)
            {
                return new Response()
                {
                    Payload = "User hat kein Deck",
                    StatusCode = StatusCode.Conflict
                };
            }
            catch(NOSTATSUPDATE.NoStatsUpdateException)
            {
                return new Response()
                {
                    Payload = "Fehler beim Updaten der Stats/Scoreboard",
                    StatusCode = StatusCode.InternalServerError
                };
            }
        }
    }
}
