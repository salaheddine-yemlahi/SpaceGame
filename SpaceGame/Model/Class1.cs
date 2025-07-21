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

namespace SpaceGame.Classes
{
    // Classe mère commune avec barre de vie
    public abstract class GameObject
    {
        public Image Sprite { get; protected set; }
        public Rectangle HealthBar { get; protected set; }

        protected GameObject(string imagePath, double x, double y, double width = 100, double height = 80)
        {
            // Créer le sprite principal
            Sprite = new Image
            {
                Width = width,
                Height = height,
                Source = new BitmapImage(new Uri(imagePath, UriKind.Absolute))
            };

            // Créer la barre de vie
            HealthBar = new Rectangle
            {
                Width = width,
                Height = 8, // Hauteur de la barre de vie
                Fill = Brushes.Green,
                Stroke = Brushes.Black,
                StrokeThickness = 1
            };

            // Positionner les éléments
            Canvas.SetLeft(Sprite, x);
            Canvas.SetTop(Sprite, y);
        }

        public virtual void Clear()
        {
            Sprite = null;
            HealthBar = null;
        }
    }

    // Classe Player héritant de GameObject
    public class Player : GameObject
    {
        public int ScoreEnemiesKilled { get; set; } = 0;

        public Player(string imagePath, double x, double y)
            : base(imagePath, (x - 100) / 2, y - 80)
        {
            double left = Canvas.GetLeft(Sprite);
            double top = Canvas.GetTop(Sprite);
            Canvas.SetLeft(HealthBar, left);
            Canvas.SetTop(HealthBar, top + Sprite.Height + 2); // 2px d'espacement
                                                             // Positionner la barre de vie sous le sprite

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

            // Vérifier limites horizontales
            if (newLeft < 0) newLeft = 0;
            if (newLeft + Sprite.Width > maxWidth) newLeft = maxWidth - Sprite.Width;

            // Vérifier limites verticales
            if (newTop < 0) newTop = 0;
            if (newTop + Sprite.Height > maxHeight) newTop = maxHeight - Sprite.Height;

            // Appliquer les nouvelles positions au sprite
            Canvas.SetLeft(Sprite, newLeft);
            Canvas.SetTop(Sprite, newTop);

            // Déplacer aussi la barre de vie
            Canvas.SetLeft(HealthBar, newLeft);
            Canvas.SetTop(HealthBar, newTop + Sprite.Height + 2);
        }
    }

    // Classe Enemy héritant de GameObject
    public class Enemy : GameObject
    {
        public Enemy(string imagePath, double x)
            : base(imagePath, x, 0)
        {
            Canvas.SetLeft(HealthBar, x);
            Canvas.SetTop(HealthBar, -10); // 2px d'espacement
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
                Width = 4, // 10 pixels de largeur  
                Height = 25, // 10 pixels de hauteur  
                Fill = Brushes.LightYellow // Couleur rouge pour la balle
            };
            Canvas.SetLeft(Shape, x+47); // Centrer la balle sur le joueur
            Canvas.SetTop(Shape, y-20); // Centrer la balle sur le joueur
            Speed = 10; // Vitesse de la balle
        }
        public bool MoveUp()
        {
            double newTop = Canvas.GetTop(Shape) - Speed;
            if (newTop >= 0)
            {
                Canvas.SetTop(Shape, newTop);
                return true; // Encore visible
            }
            return false; // Hors écran
        }
        public void ClearShape()
        {
            Shape = null;
        }
    }
}