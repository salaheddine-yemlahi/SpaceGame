using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Threading;
using SpaceGame.Classes;

namespace SpaceGame.Controller
{
    internal class Controller
    {
        public Player Player { get; private set; }
        private List<Enemy> enemies = new List<Enemy>();
        private List<Bullet> bullets = new List<Bullet>();

        public Controller(string imagePath, double x, double y)
        {
            Player = new Player(imagePath, x, y);
        }
        public void MovePlayer(double dx, double dy, double maxWidth, double maxHeight)
        {
            Player.Move(dx, dy, maxWidth, maxHeight);
        }
        public Bullet CreateBullet(double x, double y)
        {
            Bullet bullet = new Bullet(x, y);
            if (bullet.Shape == null)
            {
                throw new InvalidOperationException("Bullet.Shape n'a pas été initialisé");
            }
            bullets.Add(bullet);
            return bullet;
        }
        public async void UpdateBulletPosition(Bullet bullet, Canvas canvas)
        {
            while (true)
            {
                foreach (Bullet existingBullet in bullets)
                {
                    double newTop = Canvas.GetTop(existingBullet.Shape) - existingBullet.Speed;
                    if (newTop < 0)
                    {
                        canvas.Children.Remove(existingBullet.Shape);
                        existingBullet.ClearShape();
                        bullets.Remove(existingBullet);
                        return;
                    }
                    else
                    {
                        Canvas.SetTop(existingBullet.Shape, newTop);
                    }
                }
                Application.Current.Dispatcher.Invoke(() => { }, DispatcherPriority.Render);
                await Task.Delay(100);
            }
        }
        // le model doit returner la nouvelle position du bullet pour le comparer avec la position de l'enemy. a chaque eteration.
        public Enemy CreateEnemy(string imagePath, double x)
        {
            bool canCreateEnemy = true;
            Enemy enemy = null; // Initialize the variable to null to avoid CS0165  

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
                enemy = new Enemy(imagePath, x);
                enemies.Add(enemy);
            }

            return enemy;
        }

        public async void CheckBulletEnemy(Canvas GameCanvas)
        {
            while (true)
            {
                // Utiliser une liste temporaire pour éviter la modification pendant l'itération
                var bulletsToRemove = new List<Bullet>();
                var enemiesToRemove = new List<Enemy>();

                // Vérifications de sécurité
                /*if (bullets == null || enemies == null)
                {
                    await Task.Delay(100);
                    continue;
                }*/

                foreach (Bullet bullet in bullets.ToList()) // Copie de la liste
                {
                    //if (bullet?.Shape == null) continue;

                    double bulletTop = Canvas.GetTop(bullet.Shape);
                    double bulletLeft = Canvas.GetLeft(bullet.Shape);

                    foreach (Enemy enemy in enemies.ToList()) // Copie de la liste
                    {
                        if (enemy?.Sprite == null) continue;

                        double enemyTop = Canvas.GetTop(enemy.Sprite);
                        double enemyLeft = Canvas.GetLeft(enemy.Sprite);

                        // Vérifier si la balle touche l'ennemi
                        if (bulletTop <= enemyTop + enemy.Sprite.ActualHeight &&
                            bulletTop + bullet.Shape.ActualHeight >= enemyTop &&
                            bulletLeft + bullet.Shape.ActualWidth >= enemyLeft &&
                            bulletLeft <= enemyLeft + enemy.Sprite.ActualWidth)
                        {
                            // Marquer pour suppression
                            if (!bulletsToRemove.Contains(bullet))
                                bulletsToRemove.Add(bullet);
                            if (!enemiesToRemove.Contains(enemy))
                                enemiesToRemove.Add(enemy);
                        }
                    }
                }

                // Supprimer les objets marqués
                foreach (var bullet in bulletsToRemove)
                {
                    bullets.Remove(bullet);
                    GameCanvas.Children.Remove(bullet.Shape);
                    bullet.ClearShape();
                }

                foreach (var enemy in enemiesToRemove)
                {
                    enemies.Remove(enemy);
                    GameCanvas.Children.Remove(enemy.Sprite);
                    enemy.ClearEnemy();
                }
                await Task.Delay(50);
            }
        }
    }
}
