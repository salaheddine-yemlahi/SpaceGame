using System;
using System.Windows;
using SpaceGame;
using SpaceGame;
namespace SpaceGame
{
    public class Program
    {
        [STAThread]  // Important pour WPF
        public static void Main(string[] args)
        {
            Application app = new Application();
            MainWindow mainWindow = new MainWindow();
            app.Run(mainWindow);  // Lance l'application
        }
    }
}