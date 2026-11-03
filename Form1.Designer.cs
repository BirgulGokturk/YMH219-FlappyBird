namespace YMH219_FlappyBird;

partial class Form1
{
    private System.ComponentModel.IContainer components = null;
    private System.Windows.Forms.Timer gameTimer;
    private System.Windows.Forms.ToolStrip statusStrip;
    private System.Windows.Forms.ToolStripButton btnRestart;
    private System.Windows.Forms.ToolStripButton btnPause;
    private System.Windows.Forms.ToolStripButton btnSound;
    private System.Windows.Forms.ToolStripLabel lblStatus;

    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container();
        gameTimer = new System.Windows.Forms.Timer(components);
        statusStrip = new System.Windows.Forms.ToolStrip();
        btnRestart = new System.Windows.Forms.ToolStripButton();
        btnPause = new System.Windows.Forms.ToolStripButton();
        btnSound = new System.Windows.Forms.ToolStripButton();
        lblStatus = new System.Windows.Forms.ToolStripLabel();
        statusStrip.SuspendLayout();
        SuspendLayout();
        // 
        // gameTimer
        // 
        gameTimer.Interval = 18;
        gameTimer.Tick += GameTimer_Tick;
        // 
        // statusStrip
        // 
        statusStrip.Dock = DockStyle.Bottom;
        statusStrip.GripStyle = ToolStripGripStyle.Hidden;
        statusStrip.BackColor = Color.FromArgb(240, 240, 240);
        statusStrip.Items.AddRange(new ToolStripItem[] {
            btnRestart,
            btnPause,
            btnSound,
            lblStatus
        });
        statusStrip.Location = new Point(0, 615);
        statusStrip.Name = "statusStrip";
        statusStrip.Size = new Size(460, 25);
        statusStrip.TabIndex = 0;
        // 
        // btnRestart
        // 
        btnRestart.DisplayStyle = ToolStripItemDisplayStyle.Text;
        btnRestart.Text = "🔄 Yeniden Başlat (R)";
        btnRestart.Click += BtnRestart_Click;
        // 
        // btnPause
        // 
        btnPause.DisplayStyle = ToolStripItemDisplayStyle.Text;
        btnPause.Text = "⏸ Duraklat (P)";
        btnPause.Click += BtnPause_Click;
        // 
        // btnSound
        // 
        btnSound.DisplayStyle = ToolStripItemDisplayStyle.Text;
        btnSound.Text = "🔊 Ses: Açık";
        btnSound.Click += BtnSound_Click;
        // 
        // lblStatus
        // 
        lblStatus.Alignment = ToolStripItemAlignment.Right;
        lblStatus.Text = "[BOŞLUK] Zıpla";
        lblStatus.ForeColor = Color.DarkSlateGray;
        // 
        // Form1
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(460, 640);
        Controls.Add(statusStrip);
        FormBorderStyle = FormBorderStyle.FixedSingle;
        KeyPreview = true;
        MaximizeBox = false;
        Name = "Form1";
        StartPosition = FormStartPosition.CenterScreen;
        Text = "YMH219 - Flappy Bird Masaüstü Oyunu";
        Paint += Form1_Paint;
        KeyDown += Form1_KeyDown;
        statusStrip.ResumeLayout(false);
        statusStrip.PerformLayout();
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion
}
