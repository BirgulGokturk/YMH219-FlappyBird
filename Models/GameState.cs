namespace YMH219_FlappyBird.Models
{
    /// <summary>
    /// Oyunun anlık durumunu belirten numaralandırma.
    /// </summary>
    public enum GameState
    {
        Ready,       // Oyun başlamaya hazır, oyuncunun tuşa basmasını bekliyor
        Playing,     // Oyun aktif olarak oynanıyor
        Paused,      // Oyun duraklatıldı
        GameOver     // Oyuncu bir engele veya zemine çarptı
    }
}
