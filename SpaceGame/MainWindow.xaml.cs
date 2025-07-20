using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using SpaceGame.Classes;
using SpaceGame.Controller;

namespace SpaceGame
{
    public partial class MainWindow : Window
    {
        Player player;
        Controller.Controller controller;
        double width;
        double height;

        public MainWindow()
        {
            InitializeComponent();
            GameCanvas.Loaded += (sender, e) =>
            {
                width = GameCanvas.ActualWidth;
                height = GameCanvas.ActualHeight;

                // Créer le player (modèle)
                player = new Player(@"C:\Users\salah\Desktop\SpaceGame\asserts\player.png", width, height);

                // Créer le controller en lui passant le player et la vue
                controller = new Controller.Controller(player, this);

                // Ajouter le player au canvas
                GameCanvas.Children.Add(player.Sprite);

                // Démarrer le jeu
                CreateEnemy();
                controller.StartCollisionDetection();
            };
            this.KeyDown += MainWindow_KeyDown;
        }

        public async void CreateEnemy()
        {
            Random random = new Random();
            while (true)
            {
                double enemyX = random.Next(50, (int)(width - 50));
                controller.CreateEnemy(@"C:\Users\salah\Desktop\SpaceGame\asserts\E1.png", enemyX);
                await Task.Delay(4000);
            }
        }

        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                controller.CreateBullet();
            }
            if (e.Key == Key.Left)
            {
                controller.MovePlayer(-10, 0);
            }
            else if (e.Key == Key.Right)
            {
                controller.MovePlayer(10, 0);
            }
            else if (e.Key == Key.Up)
            {
                controller.MovePlayer(0, -10);
            }
            else if (e.Key == Key.Down)
            {
                controller.MovePlayer(0, 10);
            }
        }
    }
}