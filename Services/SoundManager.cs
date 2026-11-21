namespace YMH219_FlappyBird.Services
{
    /// <summary>
    /// Oyun içi zıplama, skor ve çarpışma ses efektlerini arka planda çalan servis.
    /// </summary>
    public class SoundManager
    {
        // Ses efektlerinin frekans (Hz) ve süre (ms) değerleri tek yerde tutulur
        private const int FlapFrequency = 700, FlapDuration = 35;
        private const int ScoreFrequency = 1000, ScoreDuration = 45;
        private const int HitFrequency = 350, HitDuration = 150;

        public bool IsMuted { get; set; } = false;

        /// <summary>
        /// Zıplama ses efekti (kısa tiz ton).
        /// </summary>
        public void PlayFlapSound() => PlayTone(FlapFrequency, FlapDuration);

        /// <summary>
        /// Boru geçildiğinde puan alma ses efekti.
        /// </summary>
        public void PlayScoreSound() => PlayTone(ScoreFrequency, ScoreDuration);

        /// <summary>
        /// Çarpışma / Game Over ses efekti.
        /// </summary>
        public void PlayHitSound() => PlayTone(HitFrequency, HitDuration);

        /// <summary>
        /// Verilen frekans ve sürede kısa bir ton çalar. Console.Beep çalarken bekletir (bloklar);
        /// oyun döngüsü takılmasın diye arka plan görevinde (Task) çalıştırılır.
        /// </summary>
        private void PlayTone(int frequency, int durationMs)
        {
            if (IsMuted) return;

            Task.Run(() =>
            {
                try
                {
                    if (OperatingSystem.IsWindows())
                    {
                        Console.Beep(frequency, durationMs);
                    }
                }
                catch
                {
                    // Ses aygıtı yoksa veya bip desteklenmiyorsa oyun sessiz devam eder
                }
            });
        }
    }
}
