using System.IO;

namespace YMH219_FlappyBird.Services
{
    /// <summary>
    /// Oyuncunun mevcut ve en yüksek skorunu yöneten, dosyaya kaydeden servis.
    /// </summary>
    public class ScoreManager
    {
        public int CurrentScore { get; private set; } = 0;
        public int HighScore { get; private set; } = 0;

        private readonly string _scoreFilePath;

        public ScoreManager(string? customPath = null)
        {
            if (string.IsNullOrWhiteSpace(customPath))
            {
                // Uygulama çalışma dizininde highscore.txt
                _scoreFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "highscore.txt");
            }
            else
            {
                _scoreFilePath = customPath;
            }

            LoadHighScore();
        }

        /// <summary>
        /// Skoru 1 artırır, gerekirse en yüksek skoru günceller ve kaydeder.
        /// </summary>
        public void IncrementScore()
        {
            CurrentScore++;
            if (CurrentScore > HighScore)
            {
                HighScore = CurrentScore;
                SaveHighScore();
            }
        }

        /// <summary>
        /// Yeni oyun başladığında mevcut skoru sıfırlar.
        /// </summary>
        public void ResetScore()
        {
            CurrentScore = 0;
        }

        /// <summary>
        /// En yüksek skoru yerel dosyadan yükler.
        /// </summary>
        public void LoadHighScore()
        {
            try
            {
                if (File.Exists(_scoreFilePath))
                {
                    string text = File.ReadAllText(_scoreFilePath).Trim();
                    if (int.TryParse(text, out int score))
                    {
                        HighScore = Math.Max(0, score);
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                // Dosya erişim hatası olursa oyun çökmemeli
                System.Diagnostics.Debug.WriteLine($"High score yüklenirken hata oluştu: {ex.Message}");
            }

            HighScore = 0;
        }

        /// <summary>
        /// En yüksek skoru yerel dosyaya yazar.
        /// </summary>
        public void SaveHighScore()
        {
            try
            {
                File.WriteAllText(_scoreFilePath, HighScore.ToString());
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"High score kaydedilirken hata oluştu: {ex.Message}");
            }
        }
    }
}
