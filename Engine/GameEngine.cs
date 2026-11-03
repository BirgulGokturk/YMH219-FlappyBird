using System.Drawing;
using System.Drawing.Drawing2D;
using YMH219_FlappyBird.Models;
using YMH219_FlappyBird.Services;

namespace YMH219_FlappyBird.Engine
{
    /// <summary>
    /// Flappy Bird oyun mekaniklerini, nesnelerini, çarpışmalarını ve durumunu yöneten ana motor sınıfı.
    /// </summary>
    public class GameEngine
    {
        public Bird Bird { get; private set; }
        public List<PipePair> Pipes { get; private set; } = new();
        public ScoreManager ScoreManager { get; private set; }
        public SoundManager SoundManager { get; private set; }
        public GameState State { get; private set; } = GameState.Ready;

        public int GameWidth { get; private set; }
        public int GameHeight { get; private set; }
        public float GroundY => GameHeight - 70;

        public float BaseSpeed { get; set; } = 3.2f;
        public float CurrentSpeed => BaseSpeed + (Level - 1) * 0.35f;
        public int Level => (ScoreManager.CurrentScore / 5) + 1;

        private float _groundOffset = 0f;
        private readonly List<PointF> _clouds = new();

        public GameEngine(int width, int height)
        {
            GameWidth = width;
            GameHeight = height;

            ScoreManager = new ScoreManager();
            SoundManager = new SoundManager();

            Bird = new Bird(GameWidth * 0.22f, GameHeight * 0.45f);

            InitializePipes();
            InitializeClouds();
        }

        /// <summary>
        /// Boru çiftlerini başlangıç konumlarına yerleştirir.
        /// </summary>
        private void InitializePipes()
        {
            Pipes.Clear();
            int pipeCount = 3;
            float spacing = (GameWidth + 100) / (float)pipeCount;

            for (int i = 0; i < pipeCount; i++)
            {
                float x = GameWidth + 120 + (i * spacing);
                Pipes.Add(new PipePair(x, GroundY, minHeight: 70, maxHeight: (int)(GroundY - 220)));
            }
        }

        /// <summary>
        /// Arka planda süzülen dekoratif bulutları başlatır.
        /// </summary>
        private void InitializeClouds()
        {
            _clouds.Clear();
            _clouds.Add(new PointF(50, 70));
            _clouds.Add(new PointF(220, 120));
            _clouds.Add(new PointF(380, 50));
        }

        /// <summary>
        /// Oyuncunun zıplama isteğini işler.
        /// </summary>
        public void HandleFlap()
        {
            if (State == GameState.Ready)
            {
                State = GameState.Playing;
                Bird.Flap();
                SoundManager.PlayFlapSound();
            }
            else if (State == GameState.Playing)
            {
                Bird.Flap();
                SoundManager.PlayFlapSound();
            }
            else if (State == GameState.GameOver)
            {
                Restart();
            }
        }

        /// <summary>
        /// Oyunu duraklatır veya devam ettirir.
        /// </summary>
        public void TogglePause()
        {
            if (State == GameState.Playing)
            {
                State = GameState.Paused;
            }
            else if (State == GameState.Paused)
            {
                State = GameState.Playing;
            }
        }

        /// <summary>
        /// Oyunu yeniden başlatır.
        /// </summary>
        public void Restart()
        {
            Bird.Reset();
            ScoreManager.ResetScore();
            InitializePipes();
            State = GameState.Ready;
        }

        /// <summary>
        /// Oyun döngüsü: Her karede nesnelerin konumunu günceller ve çarpışmaları kontrol eder.
        /// </summary>
        public void Update()
        {
            // Zemin ve bulut hareketi oyun akarken devam eder
            if (State == GameState.Playing || State == GameState.Ready)
            {
                _groundOffset = (_groundOffset + (State == GameState.Playing ? CurrentSpeed : 1.2f)) % 24f;

                for (int i = 0; i < _clouds.Count; i++)
                {
                    PointF pt = _clouds[i];
                    pt.X -= 0.6f;
                    if (pt.X < -80)
                    {
                        pt.X = GameWidth + 40;
                    }
                    _clouds[i] = pt;
                }
            }

            if (State != GameState.Playing) return;

            // 1. Kuş güncelleme
            Bird.Update();

            // 2. Tavan ve Zemin çarpışma kontrolü
            if (Bird.Y + Bird.Height >= GroundY)
            {
                Bird.Y = GroundY - Bird.Height;
                TriggerGameOver();
                return;
            }
            if (Bird.Y < 0)
            {
                Bird.Y = 0;
            }

            // 3. Boruları güncelleme ve çarpışma kontrolü
            RectangleF birdBounds = Bird.GetBounds();

            foreach (var pipe in Pipes)
            {
                pipe.Update(CurrentSpeed);

                // Skor kontrolü: Kuş borunun sağına geçtiğinde 1 kere puan ver
                if (!pipe.IsScored && Bird.X > pipe.X + pipe.Width)
                {
                    pipe.IsScored = true;
                    ScoreManager.IncrementScore();
                    SoundManager.PlayScoreSound();
                }

                // Boru ile çarpışma kontrolü
                if (pipe.CollidesWith(birdBounds))
                {
                    TriggerGameOver();
                    return;
                }

                // Ekranın solundan çıkan boruyu en sağa yeniden konumlandır
                if (pipe.X + pipe.Width < -10)
                {
                    float rightmostX = Pipes.Max(p => p.X);
                    float spacing = (GameWidth + 100) / (float)Pipes.Count;
                    pipe.Reposition(rightmostX + spacing, 70, (int)(GroundY - 220));
                }
            }
        }

        private void TriggerGameOver()
        {
            State = GameState.GameOver;
            SoundManager.PlayHitSound();
        }

        /// <summary>
        /// Tüm oyun ekranını GDI+ ile çizer.
        /// </summary>
        public void Draw(Graphics g, int clientWidth, int clientHeight)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;

            // 1. Gökyüzü Arka Planı
            using (LinearGradientBrush skyBrush = new LinearGradientBrush(
                new Point(0, 0),
                new Point(0, (int)GroundY),
                Color.FromArgb(112, 197, 206),
                Color.FromArgb(220, 245, 248)))
            {
                g.FillRectangle(skyBrush, 0, 0, clientWidth, GroundY);
            }

            // 2. Arka Plan Bulutları
            using (Brush cloudBrush = new SolidBrush(Color.FromArgb(210, 255, 255, 255)))
            {
                foreach (var cloud in _clouds)
                {
                    DrawCloud(g, cloudBrush, cloud.X, cloud.Y);
                }
            }

            // 3. Borular
            foreach (var pipe in Pipes)
            {
                pipe.Draw(g);
            }

            // 4. Zemin ve Çimenler
            DrawGround(g, clientWidth, clientHeight);

            // 5. Flappy Bird
            Bird.Draw(g);

            // 6. Arayüz ve HUD (Skor, Game Over vb.)
            DrawHUD(g, clientWidth);
        }

        private void DrawCloud(Graphics g, Brush brush, float x, float y)
        {
            g.FillEllipse(brush, x, y + 10, 55, 30);
            g.FillEllipse(brush, x + 20, y, 45, 35);
            g.FillEllipse(brush, x + 35, y + 8, 40, 25);
        }

        private void DrawGround(Graphics g, int clientWidth, int clientHeight)
        {
            // Toprak tabaka
            using (Brush dirtBrush = new SolidBrush(Color.FromArgb(222, 216, 149)))
            using (Pen borderPen = new Pen(Color.FromArgb(90, 80, 50), 2f))
            {
                g.FillRectangle(dirtBrush, 0, GroundY, clientWidth, clientHeight - GroundY);
                g.DrawLine(borderPen, 0, GroundY, clientWidth, GroundY);
            }

            // Çim şerit
            using (Brush grassBrush = new SolidBrush(Color.FromArgb(115, 191, 46)))
            {
                g.FillRectangle(grassBrush, 0, GroundY, clientWidth, 14);
            }

            // Hareket hissi veren çizgili desen
            using (Pen stripePen = new Pen(Color.FromArgb(155, 140, 90), 2f))
            {
                for (float x = -_groundOffset; x < clientWidth + 24; x += 20)
                {
                    g.DrawLine(stripePen, x, GroundY + 18, x - 10, GroundY + 36);
                }
            }
        }

        private void DrawHUD(Graphics g, int clientWidth)
        {
            using (Font scoreFont = new Font("Segoe UI", 26, FontStyle.Bold))
            using (Font infoFont = new Font("Segoe UI", 12, FontStyle.Bold))
            using (Font titleFont = new Font("Segoe UI", 24, FontStyle.Bold))
            using (Font subFont = new Font("Segoe UI", 11, FontStyle.Regular))
            using (Brush textShadow = new SolidBrush(Color.FromArgb(180, 0, 0, 0)))
            using (Brush textWhite = new SolidBrush(Color.White))
            using (Brush panelBrush = new SolidBrush(Color.FromArgb(235, 255, 255, 255)))
            using (Pen panelBorder = new Pen(Color.FromArgb(60, 40, 20), 2.5f))
            {
                // Mevcut Skor (Üst orta)
                string scoreStr = ScoreManager.CurrentScore.ToString();
                SizeF scoreSize = g.MeasureString(scoreStr, scoreFont);
                float scoreX = (clientWidth - scoreSize.Width) / 2f;
                float scoreY = 25f;

                // Skor gölgesi ve kendisi
                g.DrawString(scoreStr, scoreFont, textShadow, scoreX + 2, scoreY + 2);
                g.DrawString(scoreStr, scoreFont, textWhite, scoreX, scoreY);

                // Sol üstte Seviye ve High Score bilgisi
                string levelInfo = $"Seviye: {Level} | Hız: {CurrentSpeed:0.0}x";
                string highInfo = $"En Yüksek: {ScoreManager.HighScore}";
                g.DrawString(levelInfo, infoFont, textShadow, 16, 16);
                g.DrawString(levelInfo, infoFont, Brushes.Yellow, 15, 15);
                g.DrawString(highInfo, infoFont, textShadow, 16, 38);
                g.DrawString(highInfo, infoFont, Brushes.White, 15, 37);

                // DURUM: Hazır Ekranı
                if (State == GameState.Ready)
                {
                    float cardW = 280;
                    float cardH = 150;
                    float cardX = (clientWidth - cardW) / 2f;
                    float cardY = (GameHeight - cardH) / 2.3f;

                    RectangleF cardRect = new RectangleF(cardX, cardY, cardW, cardH);
                    g.FillRectangle(panelBrush, cardRect);
                    g.DrawRectangle(panelBorder, cardRect.X, cardRect.Y, cardRect.Width, cardRect.Height);

                    string title = "FLAPPY BIRD";
                    SizeF titleSize = g.MeasureString(title, titleFont);
                    g.DrawString(title, titleFont, Brushes.DarkOrange, cardX + (cardW - titleSize.Width) / 2, cardY + 15);

                    string hint1 = "Uçmak için [BOŞLUK] veya [↑]";
                    string hint2 = "Oyunu başlatmak için tuşa basın!";
                    SizeF h1Size = g.MeasureString(hint1, subFont);
                    SizeF h2Size = g.MeasureString(hint2, subFont);

                    g.DrawString(hint1, subFont, Brushes.DimGray, cardX + (cardW - h1Size.Width) / 2, cardY + 70);
                    g.DrawString(hint2, subFont, Brushes.ForestGreen, cardX + (cardW - h2Size.Width) / 2, cardY + 98);
                }
                // DURUM: Duraklatıldı Ekranı
                else if (State == GameState.Paused)
                {
                    string paused = "OYUN DURAKLATILDI";
                    SizeF pSize = g.MeasureString(paused, titleFont);
                    float px = (clientWidth - pSize.Width) / 2f;
                    g.DrawString(paused, titleFont, textShadow, px + 2, 202);
                    g.DrawString(paused, titleFont, Brushes.Orange, px, 200);

                    string pSub = "Devam etmek için [P] tuşuna basın";
                    SizeF psSize = g.MeasureString(pSub, infoFont);
                    g.DrawString(pSub, infoFont, textWhite, (clientWidth - psSize.Width) / 2f, 250);
                }
                // DURUM: Game Over Ekranı
                else if (State == GameState.GameOver)
                {
                    float boxW = 300;
                    float boxH = 200;
                    float boxX = (clientWidth - boxW) / 2f;
                    float boxY = (GameHeight - boxH) / 2.5f;

                    RectangleF boxRect = new RectangleF(boxX, boxY, boxW, boxH);
                    g.FillRectangle(panelBrush, boxRect);
                    g.DrawRectangle(panelBorder, boxRect.X, boxRect.Y, boxRect.Width, boxRect.Height);

                    string gameOverText = "OYUN BİTTİ";
                    SizeF goSize = g.MeasureString(gameOverText, titleFont);
                    g.DrawString(gameOverText, titleFont, Brushes.Crimson, boxX + (boxW - goSize.Width) / 2, boxY + 15);

                    string endScore = $"Skorunuz: {ScoreManager.CurrentScore}";
                    string endHigh = $"En Yüksek Skor: {ScoreManager.HighScore}";
                    string restartHint = "Yeniden başlamak için [BOŞLUK] veya [R]";

                    g.DrawString(endScore, infoFont, Brushes.Black, boxX + 40, boxY + 70);
                    g.DrawString(endHigh, infoFont, Brushes.DarkGoldenrod, boxX + 40, boxY + 100);

                    SizeF rSize = g.MeasureString(restartHint, subFont);
                    g.DrawString(restartHint, subFont, Brushes.DarkBlue, boxX + (boxW - rSize.Width) / 2, boxY + 145);
                }
            }
        }
    }
}
