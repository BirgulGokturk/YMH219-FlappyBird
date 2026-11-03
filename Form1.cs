using YMH219_FlappyBird.Engine;
using YMH219_FlappyBird.Models;

namespace YMH219_FlappyBird;

public partial class Form1 : Form
{
    private readonly GameEngine _engine;

    public Form1()
    {
        InitializeComponent();

        // Ekran titremesini önlemek için çift arabellekleme
        DoubleBuffered = true;
        SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer, true);
        UpdateStyles();

        // Oyun motorunu alt çubuk yüksekliğini hesaba katarak başlat
        int playableHeight = statusStrip.Top > 0 ? statusStrip.Top : 615;
        _engine = new GameEngine(ClientSize.Width, playableHeight);

        // Oyun zamanlayıcısını başlat
        gameTimer.Start();
    }

    private void GameTimer_Tick(object? sender, EventArgs e)
    {
        _engine.Update();

        // Arayüz buton metinlerini ve durumunu güncelle
        btnPause.Text = _engine.State == GameState.Paused ? "▶ Devam Et (P)" : "⏸ Duraklat (P)";
        lblStatus.Text = _engine.State switch
        {
            GameState.Ready => "Başlamak için [BOŞLUK] basın",
            GameState.Playing => $"Oynanıyor | Hız: {_engine.CurrentSpeed:0.0}x",
            GameState.Paused => "DURAKLATILDI",
            GameState.GameOver => "Oyun Bitti! [R] ile başla",
            _ => ""
        };

        // Formu yeniden çizdir
        Invalidate();
    }

    private void Form1_Paint(object? sender, PaintEventArgs e)
    {
        int playableHeight = statusStrip.Top > 0 ? statusStrip.Top : ClientSize.Height;
        _engine.Draw(e.Graphics, ClientSize.Width, playableHeight);
    }

    private void Form1_KeyDown(object? sender, KeyEventArgs e)
    {
        switch (e.KeyCode)
        {
            case Keys.Space:
            case Keys.Up:
                _engine.HandleFlap();
                e.Handled = true;
                break;

            case Keys.R:
                _engine.Restart();
                e.Handled = true;
                break;

            case Keys.P:
                _engine.TogglePause();
                e.Handled = true;
                break;

            case Keys.M:
                ToggleSound();
                e.Handled = true;
                break;
        }
    }

    private void BtnRestart_Click(object? sender, EventArgs e)
    {
        _engine.Restart();
        Focus();
    }

    private void BtnPause_Click(object? sender, EventArgs e)
    {
        _engine.TogglePause();
        Focus();
    }

    private void BtnSound_Click(object? sender, EventArgs e)
    {
        ToggleSound();
        Focus();
    }

    private void ToggleSound()
    {
        _engine.SoundManager.IsMuted = !_engine.SoundManager.IsMuted;
        btnSound.Text = _engine.SoundManager.IsMuted ? "🔇 Ses: Kapalı" : "🔊 Ses: Açık";
    }
}
