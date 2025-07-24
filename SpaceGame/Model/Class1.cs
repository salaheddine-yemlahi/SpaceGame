using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using SpaceGame;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Security.Policy;

namespace SpaceGame.Classes
{
    public class HealthBar
    {
        public Rectangle Bar { get; private set; }
        public Brush Color { get; set; } = Brushes.Green;
        public Brush TmpColor { get; set; }
        public HealthBar(double x, double y, double width = 100, double height = 10)
        {
            Bar = new Rectangle
            {
                Width = width,
                Height = height,
                Fill = Color,
                Stroke = Brushes.Black,
                StrokeThickness = 1
            };
            Canvas.SetLeft(Bar, x);
            Canvas.SetTop(Bar, y);
        }
        public void UpdateHealthBarre(double newLeft, double newTop, double height)
        {
            Canvas.SetLeft(Bar, newLeft);
            Canvas.SetTop(Bar, newTop + height + 2);
        }
        public virtual void Clear()
        {
            Bar = null;
        }
    }

    public abstract class GameObject
    {
        public Image Sprite { get; protected set; }
        public HealthBar healthBar { get; protected set; }
        const int MaxHealth = 100;
        public int health { get; set; } = MaxHealth;

        protected GameObject(string imagePath, double x, double y, double width = 100, double height = 80)
        {
            string fullPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, imagePath);

            Sprite = new Image
            {
                Width = width,
                Height = height,
                Source = new BitmapImage(new Uri(fullPath, UriKind.Absolute))
            };


            Canvas.SetLeft(Sprite, x);
            Canvas.SetTop(Sprite, y);


            healthBar = new HealthBar(x, y + height + 2, width, 10);
        }

        public virtual void Clear()
        {
            Sprite = null;
            healthBar?.Clear();
            healthBar = null;
        }

        public void UpdateHealthBar()
        {
            health -= MaxHealth / 4;
            healthBar.Bar.Width = health;
            switch (health)
            {
                case var h when h > 75:
                    healthBar.Bar.Fill = Brushes.Green;
                    break;
                case var h when h > 25:
                    healthBar.Bar.Fill = Brushes.Yellow;
                    break;
                case var h when h > 0:
                    healthBar.Bar.Fill = Brushes.Red;
                    break;
            }
        }
    }

    public class Player : GameObject
    {
        public int ScoreEnemiesKilled { get; set; } = 0;
        public int Level { get; set; } = 1;
        public bool OneShotModeActive { get; set; } = false;


        public Player(string imagePath, double canvasWidth, double canvasHeight)
            : base(imagePath, (canvasWidth - 100) / 2, canvasHeight - 100)
        {
        }

        public void IncrementScore()
        {
            ScoreEnemiesKilled++;
        }

        public void Move(double dx, double dy, double maxWidth, double maxHeight)
        {
            double left = Canvas.GetLeft(Sprite);
            double top = Canvas.GetTop(Sprite);
            double newLeft = left + dx;
            double newTop = top + dy;


            if (newLeft < 0) newLeft = 0;
            if (newLeft + Sprite.Width > maxWidth) newLeft = maxWidth - Sprite.Width;


            if (newTop < 0) newTop = 0;
            if (newTop + Sprite.Height > maxHeight) newTop = maxHeight - Sprite.Height;

            Canvas.SetLeft(Sprite, newLeft);
            Canvas.SetTop(Sprite, newTop);

            healthBar.UpdateHealthBarre(newLeft, newTop, Sprite.Height);
        }

        public void IncrementLevel()
        {
            Level++;
        }

        public async void ApplyEffect(Player player)
        {
            OneShotModeActive = true;
            await Task.Delay(10000);
            OneShotModeActive = false;
            player.healthBar.Bar.Fill = player.healthBar.TmpColor;
        }

        public bool IsOneShotModeActive()
        {
            return OneShotModeActive;
        }
    }

    public class Enemy : GameObject
    {
        public Enemy(string imagePath, double x)
            : base(imagePath, x, 0)
        {
        }

        public void ClearEnemy()
        {
            Clear();
        }
    }

    public class Bullet
    {
        public Ellipse Shape { get; private set; }
        public double Speed { get; private set; }

        public Bullet(double x, double y)
        {
            Shape = new Ellipse
            {
                Width = 4,
                Height = 25,
                Fill = Brushes.LightYellow
            };
            Canvas.SetLeft(Shape, x + 47);
            Canvas.SetTop(Shape, y - 20);
            Speed = 10;
        }

        public bool MoveUp()
        {
            double newTop = Canvas.GetTop(Shape) - Speed;
            if (newTop >= 0)
            {
                Canvas.SetTop(Shape, newTop);
                return true;
            }
            return false;
        }

        public void ClearShape()
        {
            Shape = null;
        }
    }


    public abstract class Star : GameObject
    {
        public bool IsCollected { get; protected set; } = false;

        protected Star(string imagePath, double x, double y)
            : base(imagePath, x, y, 40, 40)
        {

            if (healthBar != null)
            {
                healthBar.Clear();
                healthBar = null;
            }
        }

        public abstract void ApplyEffect(Player player);

        public virtual void Collect()
        {
            IsCollected = true;
        }

        public bool CheckCollisionWithPlayer(Player player)
        {
            if (IsCollected || player?.Sprite == null || Sprite == null) return false;

            double starLeft = Canvas.GetLeft(Sprite);
            double starTop = Canvas.GetTop(Sprite);
            double playerLeft = Canvas.GetLeft(player.Sprite);
            double playerTop = Canvas.GetTop(player.Sprite);

            return starLeft < playerLeft + player.Sprite.Width &&
                   starLeft + Sprite.Width > playerLeft &&
                   starTop < playerTop + player.Sprite.Height &&
                   starTop + Sprite.Height > playerTop;
        }

        public void ClearStar()
        {
            Clear();
        }
    }


    public class StarYellow : Star
    {
        public StarYellow(double x, double y)
            : base(@"asserts\star2.png", x, y)
        {
        }

        public override void ApplyEffect(Player player)
        {
            player.health = 100;
            player.healthBar.Bar.Width = 100;
            player.healthBar.Bar.Fill = System.Windows.Media.Brushes.Green;
        }
    }


    public class StarBlue : Star
    {
        public StarBlue(double x, double y)
            : base(@"asserts\star1.png", x, y)
        {
        }

        public override void ApplyEffect(Player player)
        {
            player.ApplyEffect(player);
            player.healthBar.TmpColor = player.healthBar.Bar.Fill;
            player.healthBar.Bar.Fill = System.Windows.Media.Brushes.Blue;
        }
    }
}