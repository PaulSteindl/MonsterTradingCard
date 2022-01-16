using System;
using System.Net.Http;
using GUIOPT = Client.GuiOptions;
using HTTPServerCore.PwdHash.PasswordHash;
using System.Text.RegularExpressions;

namespace Client.Gui
{
    class Gui
    {
        string input;
        string username;
        string password;

        private GUIOPT.GuiOptions StartMenu()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("Willkommen zum MonsterTradingCard-Game 2022\n");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("(1)Anmelden\n(2)Registrieren");

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

        public void Start(HttpClient client)
        {
            FormUrlEncodedContent requestContent;

            switch (StartMenu())
            {
                case GUIOPT.GuiOptions.login:
                    ShowLogin();
                    break;

                case GUIOPT.GuiOptions.register:
                    ShowRegister();
                    break;
            }


        }

        private void ShowLogin()
        {
            Console.Write("Username: ");
            username = Console.ReadLine();
            while (!CheckForNoSymbols(username))
            {
                Console.Write("Keine Sonderzeichen, versuche es erneut: ");
                username = Console.ReadLine();
            }

            System.Console.Write("Passwort: ");
            ReadPassword();
            Console.WriteLine("\nMelde an...");
        }

        private void ShowRegister()
        {
            Console.Write("Username (keine Sonderzeichen): ");
            username = Console.ReadLine();
            while (!CheckForNoSymbols(username))
            {
                Console.Write("Keine Sonderzeichen, versuche es erneut: ");
                username = Console.ReadLine();
            }

            do
            {
                Console.Write("Passwort:");
                ReadPassword();
            } while (!ComparePwd());
        }

        private void ReadPassword()
        {
            while (true)
            {
                var key = System.Console.ReadKey(true);
                if (key.Key == ConsoleKey.Enter)
                    break;
                password += key.KeyChar;
            }
            password = PasswordHash.GetHashString(password);
            Console.Write("\n");
        }

        public bool CheckForNoSymbols(string checkString)
        {
            string pattern = @"^[a-zA-Z0-9]+\z";
            Match match = Regex.Match(checkString, pattern, RegexOptions.IgnoreCase);
            if (!string.IsNullOrEmpty(checkString) && match.Success)
                return true;
            return false;
        }

        private bool ComparePwd()
        {
            string tmp = password;
            Console.Write("Bestätigen: ");
            ReadPassword();

            if(tmp == password)
            {
                return true;
            }

            Console.WriteLine("Stimmen nicht überein!");
            return false;
        }
    }
}
