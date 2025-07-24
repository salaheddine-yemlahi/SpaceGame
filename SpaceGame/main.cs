using System;
using System.Windows;
using SpaceGame.Classes;
using SpaceGame.Controller;

namespace SpaceGame
{
    public class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            Application app = new Application();


            MainWindow view = new MainWindow();


            Player player = new Player(@"C:\Users\salah\Desktop\SpaceGame\asserts\player.png", 800, 600);


            Controller.Controller controller = new Controller.Controller(player, view);

            app.Run(view);
        }
    }
}