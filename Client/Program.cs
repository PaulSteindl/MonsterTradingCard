using System;
using GUI = Client.Gui;
using GUIOPT = Client.GuiOptions;

namespace Client
{
    class Program
    {
        static GUIOPT.GuiOptions userOption;

        static void Main(string[] args)
        {
            var gui = new GUI.Gui();

            userOption = gui.StartMenu();
        }
    }
}
