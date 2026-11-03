using System.Drawing;
using System.Drawing.Drawing2D;

namespace YMH219_FlappyBird.Models
{
    /// <summary>
    /// Üst ve alt borudan oluşan bir boru çiftini temsil eder.
    /// </summary>
    public class PipePair
    {
        public float X { get; set; }
        public int Width { get; private set; } = 65;
        public int Gap { get; set; } = 145; // Kuşun geçeceği dikey boşluk
        public float TopHeight { get; set; }
        public float GroundY { get; set; }

        public bool IsScored { get; set; } = false;

        private static readonly Random _rng = new Random();

        public PipePair(float startX, float groundY, int minHeight = 60, int maxHeight = 300)
        {
            X = startX;
            GroundY = groundY;
            Reposition(startX, minHeight, maxHeight);
        }

        /// <summary>
        /// Borunun yüksekliğini rastgele belirleyerek yeni bir konuma taşır.
        /// </summary>
        public void Reposition(float newX, int minHeight = 60, int maxHeight = 300)
        {
            X = newX;
            IsScored = false;
            TopHeight = _rng.Next(minHeight, maxHeight);
        }

        /// <summary>
        /// Boruyu sola doğru hareket ettirir.
        /// </summary>
        public void Update(float speed)
        {
            X -= speed;
        }

        /// <summary>
        /// Üst borunun sınırlayıcı dikdörtgeni.
        /// </summary>
        public RectangleF GetTopBounds()
        {
            return new RectangleF(X, 0, Width, TopHeight);
        }

        /// <summary>
        /// Alt borunun sınırlayıcı dikdörtgeni.
        /// </summary>
        public RectangleF GetBottomBounds()
        {
            float bottomY = TopHeight + Gap;
            float bottomHeight = Math.Max(0, GroundY - bottomY);
            return new RectangleF(X, bottomY, Width, bottomHeight);
        }

        /// <summary>
        /// Kuş ile borulardan herhangi birinin çarpışıp çarpışmadığını kontrol eder.
        /// </summary>
        public bool CollidesWith(RectangleF birdBounds)
        {
            return birdBounds.IntersectsWith(GetTopBounds()) ||
                   birdBounds.IntersectsWith(GetBottomBounds());
        }

        /// <summary>
        /// Boruları klasik ve gölgeli yeşil Flappy Bird tarzında çizer.
        /// </summary>
        public void Draw(Graphics g)
        {
            // Renk paleti
            Color pipeGreen = Color.FromArgb(115, 191, 46);
            Color pipeHighlight = Color.FromArgb(160, 225, 75);
            Color pipeShadow = Color.FromArgb(85, 140, 32);
            Color pipeBorder = Color.FromArgb(40, 75, 15);

            using (Pen borderPen = new Pen(pipeBorder, 2f))
            using (Brush mainBrush = new SolidBrush(pipeGreen))
            using (Brush highlightBrush = new SolidBrush(pipeHighlight))
            using (Brush shadowBrush = new SolidBrush(pipeShadow))
            {
                int capHeight = 26;
                int capOverhang = 4;

                // ===== 1. ÜST BORU =====
                // Gövde
                RectangleF topBody = new RectangleF(X, 0, Width, Math.Max(0, TopHeight - capHeight));
                g.FillRectangle(mainBrush, topBody);
                // Vurgu (açık yeşil dikey şerit)
                g.FillRectangle(highlightBrush, X + 5, 0, 7, topBody.Height);
                // Gölge (koyu yeşil dikey şerit)
                g.FillRectangle(shadowBrush, X + Width - 10, 0, 7, topBody.Height);
                g.DrawRectangle(borderPen, topBody.X, topBody.Y, topBody.Width, topBody.Height);

                // Üst boru başlığı (uç kısım)
                RectangleF topCap = new RectangleF(X - capOverhang, TopHeight - capHeight, Width + (capOverhang * 2), capHeight);
                g.FillRectangle(mainBrush, topCap);
                g.FillRectangle(highlightBrush, topCap.X + 5, topCap.Y, 7, topCap.Height);
                g.FillRectangle(shadowBrush, topCap.X + topCap.Width - 10, topCap.Y, 7, topCap.Height);
                g.DrawRectangle(borderPen, topCap.X, topCap.Y, topCap.Width, topCap.Height);

                // ===== 2. ALT BORU =====
                float bottomY = TopHeight + Gap;
                float bottomHeight = Math.Max(0, GroundY - bottomY);

                // Alt boru başlığı
                RectangleF bottomCap = new RectangleF(X - capOverhang, bottomY, Width + (capOverhang * 2), capHeight);
                g.FillRectangle(mainBrush, bottomCap);
                g.FillRectangle(highlightBrush, bottomCap.X + 5, bottomCap.Y, 7, bottomCap.Height);
                g.FillRectangle(shadowBrush, bottomCap.X + bottomCap.Width - 10, bottomCap.Y, 7, bottomCap.Height);
                g.DrawRectangle(borderPen, bottomCap.X, bottomCap.Y, bottomCap.Width, bottomCap.Height);

                // Alt boru gövdesi
                RectangleF bottomBody = new RectangleF(X, bottomY + capHeight, Width, Math.Max(0, bottomHeight - capHeight));
                g.FillRectangle(mainBrush, bottomBody);
                g.FillRectangle(highlightBrush, X + 5, bottomBody.Y, 7, bottomBody.Height);
                g.FillRectangle(shadowBrush, X + Width - 10, bottomBody.Y, 7, bottomBody.Height);
                g.DrawRectangle(borderPen, bottomBody.X, bottomBody.Y, bottomBody.Width, bottomBody.Height);
            }
        }
    }
}
