using System;
using System.Windows;
using SpaceGame.Classes;
using SpaceGame.Controller;

namespace SpaceGame
{
    public class Program
    {
        [STAThread]  // Important pour WPF
        public static void Main(string[] args)
        {
            Application app = new Application();

            // Créer la vue
            MainWindow view = new MainWindow();

            // Créer le modèle (Player sera initialisé avec des dimensions par défaut)
            Player player = new Player(@"C:\Users\salah\Desktop\SpaceGame\asserts\player.png", 800, 600);

            // Créer le controller en lui passant le modèle et la vue
            Controller.Controller controller = new Controller.Controller(player, view);

            app.Run(view);  // Lance l'application
        }
    }
}