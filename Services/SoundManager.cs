using System.Media;

namespace YMH219_FlappyBird.Services
{
    /// <summary>
    /// Oyun içi zıplama, skor ve çarpışma ses efektlerini arka planda çalan servis.
    /// </summary>
    public class SoundManager
    {
        public bool IsMuted { get; set; } = false;

        /// <summary>
        /// Zıplama ses efekti (kısa tiz ton).
        /// </summary>
        public void PlayFlapSound()
        {
            if (IsMuted) return;

            Task.Run(() =>
            {
                try
                {
                    if (OperatingSystem.IsWindows())
                    {
                        Console.Beep(700, 35);
                    }
                }
                catch { }
            });
        }

        /// <summary>
        /// Boru geçildiğinde puan alma ses efekti.
        /// </summary>
        public void PlayScoreSound()
        {
            if (IsMuted) return;

            Task.Run(() =>
            {
                try
                {
                    if (OperatingSystem.IsWindows())
                    {
                        Console.Beep(1000, 45);
                    }
                }
                catch { }
            });
        }

        /// <summary>
        /// Çarpışma / Game Over ses efekti.
        /// </summary>
        public void PlayHitSound()
        {
            if (IsMuted) return;

            Task.Run(() =>
            {
                try
                {
                    if (OperatingSystem.IsWindows())
                    {
                        Console.Beep(350, 150);
                    }
                }
                catch { }
            });
        }
    }
}
