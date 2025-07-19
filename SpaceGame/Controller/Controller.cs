using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using SpaceGame.Classes;

namespace SpaceGame.Controller
{
    internal class Controller
    {
        public Player Player { get; private set; }
        private List<Enemy> enemies = new List<Enemy>();

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
            return bullet;
        }
        public async void UpdateBulletPosition(Bullet bullet, Canvas canvas)
        {
            while (bullet.MoveUp()) // Continue tant que MoveUp retourne true
            {
                Application.Current.Dispatcher.Invoke(() => { }, DispatcherPriority.Render);
                await Task.Delay(50);
            }

            canvas.Children.Remove(bullet.Shape);
            bullet.ClearShape();
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
    }
}
