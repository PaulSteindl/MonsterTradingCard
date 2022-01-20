using System;
using System.Net.Http;
using GUI = Client.Gui;
using GUIOPT = Client.GuiOptions;

namespace Client
{
    class Program
    {
        static GUIOPT.GuiOptions userOption;
        static HttpClient client = new HttpClient();
        static void Main(string[] args)
        {
            var gui = new GUI.Gui();

            gui.Start(client);
        }
    }
}
