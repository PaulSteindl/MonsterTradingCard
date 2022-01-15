using System;
using GUIOPT = Client.GuiOptions;

namespace Client.Gui
{
    class Gui
    {
        string input;
        public GUIOPT.GuiOptions StartMenu()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("Willkommen zum MonsterTradingCard-Game 2022\n");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("(1)Login\n(2)Registrieren");

            while(((input = Console.ReadLine()) != "1") && input != "2")
            {
                Console.Write("Ungültige Eingabe, versuche es erneut: ");
            }

            switch(input)
            {
                case "1":
                    return GUIOPT.GuiOptions.login;

                case "2":
                    return GUIOPT.GuiOptions.register;

                default:
                    throw new ArgumentException();
            }
        }
    }
}
