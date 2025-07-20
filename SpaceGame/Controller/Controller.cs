using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using SpaceGame.Classes;

namespace SpaceGame.Controller
{
    internal class Controller
    {
        private Player player;
        private MainWindow view;
        private List<Enemy> enemies = new List<Enemy>();
        private List<Bullet> bullets = new List<Bullet>();
        private bool isGameRunning = false;

        public Controller(Player player, MainWindow mainWindow)
        {
            this.player = player;
            this.view = mainWindow;
        }

        public Player GetPlayer()
        {
            return player;
        }

        public void MovePlayer(double dx, double dy)
        {
            player.Move(dx, dy, view.GameCanvas.ActualWidth, view.GameCanvas.ActualHeight);
        }

        public Bullet CreateBullet()
        {
            double playerX = Canvas.GetLeft(player.Sprite);
            double playerY = Canvas.GetTop(player.Sprite);

            Bullet bullet = new Bullet(playerX, playerY);
            bullets.Add(bullet);
            view.GameCanvas.Children.Add(bullet.Shape);

            UpdateBulletPosition(bullet);
            return bullet;
        }

        private async void UpdateBulletPosition(Bullet bullet)
        {
            while (bullets.Contains(bullet))
            {
                if (bullet.Shape == null) break;

                bool stillVisible = bullet.MoveUp();

                if (!stillVisible)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        view.GameCanvas.Children.Remove(bullet.Shape);
                        bullets.Remove(bullet);
                        bullet.ClearShape();
                    });
                    break;
                }

                await Task.Delay(50);
            }
        }

        public Enemy CreateEnemy(string imagePath, double x)
        {
            bool canCreateEnemy = true;

            foreach (Enemy existingEnemy in enemies)
            {
                double existingCenterX = Canvas.GetLeft(existingEnemy.Sprite);
                if (Math.Abs(existingCenterX - x) < 100)
                {
                    canCreateEnemy = false;
                    break;
                }
            }

            if (canCreateEnemy)
            {
                Enemy enemy = new Enemy(imagePath, x);
                enemies.Add(enemy);
                view.GameCanvas.Children.Add(enemy.Sprite);
                MoveEnemyDown(enemy);
                return enemy;
            }

            return null;
        }

        private async void MoveEnemyDown(Enemy enemy)
        {
            while (enemies.Contains(enemy))
            {
                if (enemy.Sprite == null) break;

                double currentTop = Canvas.GetTop(enemy.Sprite);
                double newTop = currentTop + 2;

                Canvas.SetTop(enemy.Sprite, newTop);

                // Si l'ennemi sort de l'écran par le bas, le supprimer
                if (newTop > view.GameCanvas.ActualHeight)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        view.GameCanvas.Children.Remove(enemy.Sprite);
                        enemies.Remove(enemy);
                        enemy.ClearEnemy();
                    });
                    break;
                }

                await Task.Delay(100);
            }
        }

        public void StartCollisionDetection()
        {
            CheckBulletEnemy();
        }

        private async void CheckBulletEnemy()
        {
            isGameRunning = true;

            while (isGameRunning)
            {
                var bulletsToRemove = new List<Bullet>();
                var enemiesToRemove = new List<Enemy>();

                foreach (Bullet bullet in bullets.ToList())
                {
                    if (bullet?.Shape == null) continue;

                    double bulletTop = Canvas.GetTop(bullet.Shape);
                    double bulletLeft = Canvas.GetLeft(bullet.Shape);

                    foreach (Enemy enemy in enemies.ToList())
                    {
                        if (enemy?.Sprite == null) continue;

                        double enemyTop = Canvas.GetTop(enemy.Sprite);
                        double enemyLeft = Canvas.GetLeft(enemy.Sprite);

                        // Détection de collision
                        if (bulletTop <= enemyTop + enemy.Sprite.Height &&
                            bulletTop + bullet.Shape.Height >= enemyTop &&
                            bulletLeft + bullet.Shape.Width >= enemyLeft &&
                            bulletLeft <= enemyLeft + enemy.Sprite.Width)
                        {
                            if (!bulletsToRemove.Contains(bullet))
                                bulletsToRemove.Add(bullet);
                            if (!enemiesToRemove.Contains(enemy))
                                enemiesToRemove.Add(enemy);

                            player.IncrementScore();

                            Application.Current.Dispatcher.Invoke(() =>
                            {
                                view.scoreEnemiesKilled.Text = $"{player.ScoreEnemiesKilled}";
                            });
                        }
                    }
                }

                // Supprimer les objets touchés
                Application.Current.Dispatcher.Invoke(() =>
                {
                    foreach (var bullet in bulletsToRemove)
                    {
                        bullets.Remove(bullet);
                        view.GameCanvas.Children.Remove(bullet.Shape);
                        bullet.ClearShape();
                    }

                    foreach (var enemy in enemiesToRemove)
                    {
                        enemies.Remove(enemy);
                        view.GameCanvas.Children.Remove(enemy.Sprite);
                        enemy.ClearEnemy();
                    }
                });

                await Task.Delay(50);
            }
        }

        public int GetScore()
        {
            return player.ScoreEnemiesKilled;
        }

        public void StopGame()
        {
            isGameRunning = false;
        }
    }
}