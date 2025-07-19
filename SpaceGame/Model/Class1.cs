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

namespace SpaceGame.Classes
{
    public class Player
    {
        public Image Sprite { get; private set; }
        public int ScoreEnemiesKilled { get; set; } = 0; // Compteur de score pour les ennemis tués

        public Player(String imagePath, double x, double y)
        {
            Sprite = new Image
            {
                Width = 100, // 100 pixels de largeur  
                Height = 80, // 80 pixels de longeur  
                Source = new BitmapImage(new Uri(imagePath, UriKind.Absolute))
                /*  
                 * BitmapImage - classe WPF pour afficher des images dans l'interface utilisateur  
                 * Uri - crée une URI absolue depuis le chemin d'image  
                */
            };
            // Positionnement en BAS AU MILIEU du Canvas  
            Canvas.SetLeft(Sprite, (x - Sprite.Width) / 2);
            Canvas.SetTop(Sprite, y - Sprite.Height);
            /*  
             * (0,0) = coin supérieur gauche du Canvas  
             * X augmente vers la droite  
             * Y augmente vers le bas  
            */
        }
        public void IncrementScore()
        {
            ScoreEnemiesKilled++;
        }
        public void Move(double dx, double dy, double maxWidth, double maxHeight)
        {
            double left = Canvas.GetLeft(Sprite); // récupère la position horizontale actuelle du sprite  
            double top = Canvas.GetTop(Sprite); // récupère la position verticale actuelle du sprite  


            double newLeft = left + dx;
            double newTop = top + dy;

            // Vérifier limites horizontales
            if (newLeft < 0) newLeft = 0;
            if (newLeft + Sprite.Width > maxWidth) newLeft = maxWidth - Sprite.Width;

            // Vérifier limites verticales
            if (newTop < 0) newTop = 0;
            if (newTop + Sprite.Height > maxHeight) newTop = maxHeight - Sprite.Height;

            // Appliquer les nouvelles positions
            Canvas.SetLeft(Sprite, newLeft);
            Canvas.SetTop(Sprite, newTop);
        }
    }

    public class Enemy
    {
        public Image Sprite { get; private set; }

        public Enemy(String imagePath, double x)
        {
            Sprite = new Image
            {
                Width = 100, // 100 pixels de largeur  
                Height = 80, // 80 pixels de longeur  
                Source = new BitmapImage(new Uri(imagePath, UriKind.Absolute))
            };
            Canvas.SetLeft(Sprite, x);
            Canvas.SetTop(Sprite, 0);
        }
        public void ClearEnemy()
        {
            Sprite = null;
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