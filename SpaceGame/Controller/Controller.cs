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
            player.Move(dx, dy, view.GameCanvas.ActualWidth, view.GameCanvas.ActualHeight - 15);
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
                    if (Application.Current.Dispatcher.CheckAccess())
                    {
                        SafeRemoveBullet(bullet);
                    }
                    else
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            SafeRemoveBullet(bullet);
                        });
                    }
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
                view.GameCanvas.Children.Add(enemy.healthBar.Bar);
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

                double enemyCurrentTop = Canvas.GetTop(enemy.Sprite);
                double enemyNewTop = enemyCurrentTop + 2;
                double healthBarCurrentTop = Canvas.GetTop(enemy.healthBar.Bar);
                double healthBarNewTop = healthBarCurrentTop + 2;

                Canvas.SetTop(enemy.Sprite, enemyNewTop);
                Canvas.SetTop(enemy.healthBar.Bar, healthBarNewTop);

                if (enemyNewTop > view.GameCanvas.ActualHeight)
                {
                    if (Application.Current.Dispatcher.CheckAccess())
                    {
                        SafeRemoveEnemy(enemy);
                    }
                    else
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            SafeRemoveEnemy(enemy);
                        });
                    }
                    break;
                }

                await Task.Delay(100 / player.Level);
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

                var currentBullets = bullets.ToList();
                var currentEnemies = enemies.ToList();

                foreach (Bullet bullet in currentBullets)
                {
                    if (bullet?.Shape == null || !bullets.Contains(bullet)) continue;

                    double bulletTop = Canvas.GetTop(bullet.Shape);
                    double bulletLeft = Canvas.GetLeft(bullet.Shape);

                    foreach (Enemy enemy in currentEnemies)
                    {
                        if (enemy?.Sprite == null || !enemies.Contains(enemy)) continue;

                        double enemyTop = Canvas.GetTop(enemy.Sprite);
                        double enemyLeft = Canvas.GetLeft(enemy.Sprite);

                        // Collision balle-ennemi  
                        if (bulletTop <= enemyTop + enemy.Sprite.Height &&
                            bulletTop + bullet.Shape.Height >= enemyTop &&
                            bulletLeft + bullet.Shape.Width >= enemyLeft &&
                            bulletLeft <= enemyLeft + enemy.Sprite.Width)
                        {
                            if (!bulletsToRemove.Contains(bullet))
                                bulletsToRemove.Add(bullet);
                            if (enemy.health > 25)
                            {
                                enemy.UpdateHealthBar();
                                enemy.healthBar.UpdateHealthBarre(enemyLeft, enemyTop, enemy.Sprite.Height);
                            }
                            else
                            {
                                if (!enemiesToRemove.Contains(enemy))
                                    enemiesToRemove.Add(enemy);

                                player.IncrementScore();

                                if (Application.Current.Dispatcher.CheckAccess())
                                {
                                    view.scoreEnemiesKilled.Text = $"{player.ScoreEnemiesKilled}";
                                    if (player.ScoreEnemiesKilled % 10 == 0)
                                    {
                                        player.IncrementLevel();
                                        view.LevelTextBlock.Text = $"{player.Level}";
                                    }
                                }
                                else
                                {
                                    Application.Current.Dispatcher.Invoke(() =>
                                    {
                                        view.scoreEnemiesKilled.Text = $"{player.ScoreEnemiesKilled}";
                                        if (player.ScoreEnemiesKilled % 5 == 0)
                                        {
                                            player.IncrementLevel();
                                            
                                        }
                                        view.LevelTextBlock.Text = $"{player.Level}";
                                    });
                                }
                            }
                        }
                    }
                }

                // Vérifier collision joueur-ennemi (NOUVELLE FONCTION)  
                await CheckPlayerEnemyCollision(enemiesToRemove);

                if (Application.Current.Dispatcher.CheckAccess())
                {
                    RemoveGameObjects(bulletsToRemove, enemiesToRemove);
                }
                else
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        RemoveGameObjects(bulletsToRemove, enemiesToRemove);
                    });
                }

                await Task.Delay(50 / player.Level);
            }
        }

        // NOUVELLE FONCTION : Vérifier collision joueur-ennemi avec mort des deux
        private async Task CheckPlayerEnemyCollision(List<Enemy> enemiesToRemove)
        {
            if (player?.Sprite == null) return;

            var currentEnemies = enemies.ToList();

            foreach (var enemy in currentEnemies)
            {
                if (enemy?.Sprite == null || !enemies.Contains(enemy)) continue;

                double enemyTop = Canvas.GetTop(enemy.Sprite);
                double enemyLeft = Canvas.GetLeft(enemy.Sprite);
                double playerTop = Canvas.GetTop(player.Sprite);
                double playerLeft = Canvas.GetLeft(player.Sprite);

                // Détection de collision
                if (playerTop <= enemyTop + enemy.Sprite.Height &&
                    playerTop + player.Sprite.Height >= enemyTop &&
                    playerLeft + player.Sprite.Width >= enemyLeft &&
                    playerLeft <= enemyLeft + enemy.Sprite.Width)
                {
                    // Collision détectée - les deux meurent
                    if (!enemiesToRemove.Contains(enemy))
                        enemiesToRemove.Add(enemy);

                    // Tuer le joueur et arrêter le jeu
                    if (player.health > 25)
                    {
                        player.UpdateHealthBar();
                    }
                    else
                    {
                        view.RemoveElementFromCanvas(player);
                        return; // Sortir de la boucle car le joueur est mort
                    }

                }
            }
        }

        // MÉTHODE CORRIGÉE : Suppression sécurisée des objets
        private void RemoveGameObjects(List<Bullet> bulletsToRemove, List<Enemy> enemiesToRemove)
        {
            try
            {
                // Supprimer les balles de manière sécurisée
                if (bulletsToRemove != null)
                {
                    foreach (var bullet in bulletsToRemove.ToList())
                    {
                        SafeRemoveBullet(bullet);
                    }
                }

                // Supprimer les ennemis de manière sécurisée
                if (enemiesToRemove != null)
                {
                    foreach (var enemy in enemiesToRemove.ToList())
                    {
                        SafeRemoveEnemy(enemy);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erreur dans RemoveGameObjects: {ex.Message}");
            }
        }

        // NOUVELLE MÉTHODE : Suppression sécurisée d'une balle
        private void SafeRemoveBullet(Bullet bullet)
        {
            try
            {
                if (bullet != null)
                {
                    if (bullets.Contains(bullet))
                        bullets.Remove(bullet);

                    if (bullet.Shape != null && view.GameCanvas.Children.Contains(bullet.Shape))
                        view.GameCanvas.Children.Remove(bullet.Shape);

                    bullet.ClearShape();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erreur lors de la suppression de balle: {ex.Message}");
            }
        }

        // NOUVELLE MÉTHODE : Suppression sécurisée d'un ennemi
        private void SafeRemoveEnemy(Enemy enemy)
        {
            try
            {
                if (enemy != null)
                {
                    if (enemies.Contains(enemy))
                        enemies.Remove(enemy);

                    if (enemy.Sprite != null && view.GameCanvas.Children.Contains(enemy.Sprite))
                        view.GameCanvas.Children.Remove(enemy.Sprite);

                    if (enemy.healthBar?.Bar != null && view.GameCanvas.Children.Contains(enemy.healthBar.Bar))
                        view.GameCanvas.Children.Remove(enemy.healthBar.Bar);

                    enemy.ClearEnemy();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erreur lors de la suppression d'ennemi: {ex.Message}");
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

        // NOUVELLE MÉTHODE : Redémarrer le jeu
        public void RestartGame()
        {
            isGameRunning = false;

            // Nettoyer tous les objets existants
            bullets.Clear();
            enemies.Clear();

            // Nettoyer le canvas
            var objectsToRemove = view.GameCanvas.Children.OfType<UIElement>().ToList();
            foreach (var obj in objectsToRemove)
            {
                if (obj != player.Sprite && obj != player.healthBar.Bar)
                {
                    view.GameCanvas.Children.Remove(obj);
                }
            }

            // Réinitialiser le joueur
            player.ScoreEnemiesKilled = 0;
            player.health = 100; // Assumant que la santé max est 100

            // Redémarrer la détection de collision
            StartCollisionDetection();
        }
    }
}