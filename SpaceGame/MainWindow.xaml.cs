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
                height = GameCanvas.ActualHeight - 25;

                // CORRECTION: Passer width et height au constructeur Player
                player = new Player(@"C:\Users\salah\Desktop\SpaceGame\asserts\player.png", width, height);
                controller = new Controller.Controller(player, this);

                // CORRECTION: Ajouter dans le bon ordre
                GameCanvas.Children.Add(player.Sprite);
                GameCanvas.Children.Add(player.healthBar.Bar);

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
                double enemyX = random.Next(50, (int)(width - 100));
                controller.CreateEnemy(@"C:\Users\salah\Desktop\SpaceGame\asserts\E1.png", enemyX);
                await Task.Delay(3000 / player.Level);
            }
        }

        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            // CORRECTION: Utiliser switch pour plus de clarté
            switch (e.Key)
            {
                case Key.Space:
                    controller.CreateBullet();
                    break;
                case Key.Left:
                    controller.MovePlayer(-10, 0);
                    break;
                case Key.Right:
                    controller.MovePlayer(10, 0);
                    break;
                case Key.Up:
                    controller.MovePlayer(0, -10);
                    break;
                case Key.Down:
                    controller.MovePlayer(0, 10);
                    break;
            }
        }
        public void RemoveElementFromCanvas(Player player)
        {
            GameCanvas.Children.Remove(player.Sprite);
            GameCanvas.Children.Remove(player.healthBar.Bar);
        }

        public void printLevel(int level)
        {
            LevelTextBlock.Text = $"{level}";
        }
    }
}
