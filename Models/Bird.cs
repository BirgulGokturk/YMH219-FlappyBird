using System.Drawing;
using System.Drawing.Drawing2D;

namespace YMH219_FlappyBird.Models
{
    /// <summary>
    /// Flappy Bird karakterini, fiziğini ve görsel sunumunu yöneten sınıf.
    /// </summary>
    public class Bird
    {
        public float X { get; set; }
        public float Y { get; set; }
        public int Width { get; private set; } = 34;
        public int Height { get; private set; } = 26;

        public float VelocityY { get; private set; } = 0f;
        public float Gravity { get; set; } = 0.55f;
        public float JumpStrength { get; set; } = -8.5f;

        private readonly float _initialX;
        private readonly float _initialY;

        private int _wingFrame = 0;
        private int _wingCounter = 0;

        public Bird(float startX, float startY)
        {
            _initialX = startX;
            _initialY = startY;
            Reset();
        }

        /// <summary>
        /// Kuşun konumunu ve hızını başlangıç durumuna döndürür.
        /// </summary>
        public void Reset()
        {
            X = _initialX;
            Y = _initialY;
            VelocityY = 0f;
            _wingFrame = 0;
            _wingCounter = 0;
        }

        /// <summary>
        /// Kuşun zıplamasını sağlar (hızı yukarı yönlü ayarlar).
        /// </summary>
        public void Flap()
        {
            VelocityY = JumpStrength;
        }

        /// <summary>
        /// Her karede yerçekimini ve konumu günceller.
        /// </summary>
        public void Update()
        {
            VelocityY += Gravity;
            Y += VelocityY;

            // Kanat çırpma animasyon sayacı
            _wingCounter++;
            if (_wingCounter % 4 == 0)
            {
                _wingFrame = (_wingFrame + 1) % 3;
            }
        }

        /// <summary>
        /// Çarpışma tespiti için sınırlayıcı dikdörtgeni döndürür.
        /// </summary>
        public RectangleF GetBounds()
        {
            // Çarpışmanın adil olması için biraz daraltılmış sınır kutusu
            return new RectangleF(X + 3, Y + 3, Width - 6, Height - 6);
        }

        /// <summary>
        /// Kuşu GDI+ ile estetik bir biçimde çizer.
        /// </summary>
        public void Draw(Graphics g)
        {
            GraphicsState state = g.Save();

            // Kuşun hareket hızına göre eğilme açısı (-25 ile +65 derece arası)
            float angle = Math.Clamp(VelocityY * 3.5f, -25f, 65f);

            // Kuşun merkezine göre döndürme
            float centerX = X + Width / 2f;
            float centerY = Y + Height / 2f;
            g.TranslateTransform(centerX, centerY);
            g.RotateTransform(angle);
            g.TranslateTransform(-centerX, -centerY);

            // Yumuşak kenarlar
            g.SmoothingMode = SmoothingMode.AntiAlias;

            // 1. Gövde (Canlı Sarı)
            using (Brush bodyBrush = new SolidBrush(Color.FromArgb(254, 215, 0)))
            using (Pen borderPen = new Pen(Color.FromArgb(60, 40, 10), 2f))
            {
                g.FillEllipse(bodyBrush, X, Y, Width, Height);
                g.DrawEllipse(borderPen, X, Y, Width, Height);
            }

            // 2. Karın bölgesi (Açık sarı ton)
            using (Brush bellyBrush = new SolidBrush(Color.FromArgb(255, 238, 130)))
            {
                g.FillEllipse(bellyBrush, X + 2, Y + Height / 3f, Width * 0.6f, Height * 0.55f);
            }

            // 3. Kanat (Animasyonlu)
            float wingYOffset = _wingFrame switch
            {
                0 => 2f,
                1 => 0f,
                _ => -3f
            };
            using (Brush wingBrush = new SolidBrush(Color.FromArgb(240, 180, 0)))
            using (Pen wingPen = new Pen(Color.FromArgb(60, 40, 10), 1.5f))
            {
                RectangleF wingRect = new RectangleF(X + 4, Y + 6 + wingYOffset, 14, 10);
                g.FillEllipse(wingBrush, wingRect);
                g.DrawEllipse(wingPen, wingRect);
            }

            // 4. Göz (Beyaz + Siyah gözbebeği)
            using (Brush whiteBrush = new SolidBrush(Color.White))
            using (Brush pupilBrush = new SolidBrush(Color.Black))
            using (Pen eyePen = new Pen(Color.FromArgb(60, 40, 10), 1.2f))
            {
                float eyeX = X + Width - 13;
                float eyeY = Y + 3;
                g.FillEllipse(whiteBrush, eyeX, eyeY, 9, 10);
                g.DrawEllipse(eyePen, eyeX, eyeY, 9, 10);
                g.FillEllipse(pupilBrush, eyeX + 4, eyeY + 3, 4, 4);
            }

            // 5. Gaga (Turuncu)
            using (Brush beakBrush = new SolidBrush(Color.FromArgb(255, 100, 30)))
            using (Pen beakPen = new Pen(Color.FromArgb(120, 40, 0), 1.5f))
            {
                PointF[] beakPoints =
                {
                    new PointF(X + Width - 4, Y + Height / 2f - 3),
                    new PointF(X + Width + 8, Y + Height / 2f + 1),
                    new PointF(X + Width - 4, Y + Height / 2f + 6)
                };
                g.FillPolygon(beakBrush, beakPoints);
                g.DrawPolygon(beakPen, beakPoints);
            }

            g.Restore(state);
        }
    }
}
