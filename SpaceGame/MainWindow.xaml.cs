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
        Bullet bullet;
        double width;
        double height;
       

        public MainWindow()
        {
            InitializeComponent();
            GameCanvas.Loaded += (sender, e) =>
            {
                width = GameCanvas.ActualWidth;
                height = GameCanvas.ActualHeight;
                controller = new Controller.Controller(@"C:\Users\salah\Desktop\SpaceGame\asserts\player.png", width, height);
                player = controller.Player;
                GameCanvas.Children.Add(player.Sprite);
                CreateEnemy();
                controller.CheckBulletEnemy(GameCanvas, scoreEnemiesKilled);
            };
            this.KeyDown += MainWindow_KeyDown;
            
        }

        public async void CreateEnemy()
        {
            Random random = new Random();
            while (true)
            {
                double enemyX = random.Next(50, (int)(width - 50));
                Enemy enemy = controller.CreateEnemy(@"C:\Users\salah\Desktop\SpaceGame\asserts\E1.png", enemyX);
                if(enemy != null)
                {
                    GameCanvas.Children.Add(enemy.Sprite);
                }
                await Task.Delay(4000);
            }
        }

        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                bullet = controller.CreateBullet(Canvas.GetLeft(player.Sprite), Canvas.GetTop(player.Sprite));
                GameCanvas.Children.Add(bullet.Shape);
                controller.UpdateBulletPosition(bullet, GameCanvas);
            }
            if (e.Key == Key.Left)
            {
                controller.MovePlayer(-10, 0, GameCanvas.ActualWidth, GameCanvas.ActualHeight);
                
            }
            else if (e.Key == Key.Right)
            {
                controller.MovePlayer(10, 0, GameCanvas.ActualWidth, GameCanvas.ActualHeight);
            }
            else if (e.Key == Key.Up)
            {
                controller.MovePlayer(0, -10, GameCanvas.ActualWidth, GameCanvas.ActualHeight);
            }
            else if (e.Key == Key.Down)
            {
                controller.MovePlayer(0, 10, GameCanvas.ActualWidth, GameCanvas.ActualHeight);
            }
        }
        
    }
}