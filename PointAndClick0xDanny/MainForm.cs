using System.Diagnostics;
using System.Drawing.Drawing2D;

namespace PointAndClick0xDanny
{
    public partial class MainForm : Form
    {
        private readonly System.Windows.Forms.Timer _timer;
        private readonly Stopwatch _gameStopwatch, _fpsStopwatch;

        private readonly Random _random = new Random();

        private readonly List<Rectangle> _lifes;
        private readonly List<MyCircle> _circles = new List<MyCircle>();

        private int _timeToLevelUp = 15;
        private int _level = 1;
        private int _life = 3;
        private int _gameTime, _score, _missHits;
        private bool _restartGame, _continueGame;

        private readonly Pen _myPen = new Pen(Color.Black, 2);
        private readonly Rectangle _gameField;

        private GraphicsPath _guiEdgePath = new GraphicsPath();
        private readonly Rectangle _closeButton, _breakButton;

        public MainForm()
        {
            InitializeComponent();
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.UserPaint, true);

            Region = new Region(GetRectWithRoundCornor(0, 0, ClientSize.Width, ClientSize.Height, 52));
            _gameField = new Rectangle(160, 60, Width - 200, Height - 100);
            _closeButton = new Rectangle(Width - 30, 10, 20, 20);
            _breakButton = new Rectangle(Width - 60, 10, 20, 20);
            _lifes = new List<Rectangle>()
            {
                new Rectangle(205, 525, 15, 15),
                new Rectangle(225, 525, 15, 15),
                new Rectangle(245, 525, 15, 15),
                new Rectangle(265, 525, 15, 15),
                new Rectangle(285, 525, 15, 15),
                new Rectangle(305, 525, 15, 15),
                new Rectangle(325, 525, 15, 15),
                new Rectangle(345, 525, 15, 15),
                new Rectangle(365, 525, 15, 15),
                new Rectangle(385, 525, 15, 15),
            };

            _gameStopwatch = new Stopwatch();
            _fpsStopwatch = new Stopwatch();

            _timer = new System.Windows.Forms.Timer();
            _timer.Tick += new EventHandler(Timer_Tick);

            MouseClick += new MouseEventHandler(GameForm_MouseClick);
            KeyDown += new KeyEventHandler(GameForm_KeyDown);
        }

        private void MainForm_Load(object? sender, EventArgs e)
        {
            _guiEdgePath = GetRectWithRoundCornor(1, 1, Width - 1, Height - 1, 50);
            _timer.Interval = 12;
            _timer.Start();

            _fpsStopwatch.Start();
            _gameStopwatch.Start();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
            e.Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
            DrawUI(e);

            using (Pen edgePen = new Pen(Color.FromArgb(128, 128, 128), 2))
            {
                e.Graphics.DrawRectangle(edgePen, _gameField);
                e.Graphics.FillRectangle(Brushes.Black, _gameField);
            }

            for (int i = 0; i < _circles.Count; i++)
            {
                using (Pen pen = new Pen(_circles[i].PrimaryColor, 2))
                using (HatchBrush brush = new HatchBrush(HatchStyle.Divot, _circles[i].PrimaryColor))
                {
                    DrawGlow(e.Graphics, _circles[i].Circle, _circles[i].PrimaryColor, 5);
                    e.Graphics.DrawEllipse(pen, _circles[i].Circle);
                    e.Graphics.FillEllipse(brush, _circles[i].Circle);
                }
            }

            using (Pen pen = new Pen(Color.DarkRed, 2))
            using (HatchBrush lifeBrush = new HatchBrush(HatchStyle.Divot, Color.DarkRed))
            {
                for (int i = 0; i < _life; i++)
                    if (_lifes.Count > i)
                    {
                        DrawGlow(e.Graphics, _lifes[i], Color.DarkRed, 5);
                        e.Graphics.DrawEllipse(pen, _lifes[i]);
                        e.Graphics.FillEllipse(lifeBrush, _lifes[i]);
                    }
            }

        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            UpdateGameInfos();

            if (_gameTime > 180 || _life < 0)
            {
                _timer.Stop();
                
                string playerName = GetUserInputBox();
                LeaderBoardForm.AddEntry(new Entry(_score, _missHits, (int)(_score / (float)(_score + _missHits) * 100f), _level, _gameTime, playerName, DateTime.Today.ToLocalTime()));
                List<Entry> entrys = LeaderBoardForm.GetLeaderboardData();
                LeaderBoardForm leaderboardForm = new LeaderBoardForm(entrys);
                
                DialogResult result = leaderboardForm.ShowDialog();
                if (result == DialogResult.OK)
                    Restart();
                else
                    Close();
            }


            if (_circles.Count < _gameTime / 15f)
                _circles.Add(new MyCircle(_gameField, _random));

            for (int i = 0; i < _circles.Count; i++)
                _circles[i].DrawingCircleInstanz(_circles, ref _life, ref _missHits);

            Invalidate();
        }

        private void GameForm_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Escape)
                BreakMenu();
        }

        private async void GameForm_MouseClick(object? sender, MouseEventArgs e)
        {
            for (int i = 0; i < _circles.Count; i++)
                if (_circles[i].Circle.Contains(e.Location))
                {
                    label3.Text = $"Hit Confirm {_circles[i].LivingTime.ElapsedMilliseconds}ms";
                    label3.Location = new Point(e.Location.X - 60, e.Location.Y - 50);
                    _circles.Remove(_circles[i]);
                    _score++;
                    HitConfirm();
                    return;
                }
            if (_breakButton.Contains(e.Location))
            {
                _continueGame = true;
                BreakMenu();
            }
            if (_closeButton.Contains(e.Location))
            {
                _restartGame= true;
                Close();
            }
            if (_restartGame||_continueGame)
            {
                _restartGame = false;
                _continueGame = false;
                return;
            }

            _missHits++;
            _life--;
        }

        private async void UpdateGameInfos()
        {
            _gameTime = (int)_gameStopwatch.ElapsedTicks / 10_000_000;

            if (_gameTime > _timeToLevelUp)
            {
                _timeToLevelUp += 15;
                _level++;
                _life = 3 + _level - 1;

                label4.Visible = true;
                await Task.Delay(300);
                label4.Visible = false;
            }

            label5.Text = $"Time: {_gameTime}";
            label6.Text = $"Level: {_level}";
            label7.Text = $"Score: {_score}";
            label8.Text = $"FPS: {(int)(1 / (_fpsStopwatch.ElapsedTicks / 10_000_000f))}";

            _fpsStopwatch.Restart();
        }

        private void Restart()
        {
            _gameTime = 0;
            _timeToLevelUp = 15;
            _level = 1;
            _life = 3;
            _score = 0;
            _missHits = 0;
            _circles.Clear();
            _gameStopwatch.Restart();
            _timer.Start();
        }

        private string GetUserInputBox()
        {
            using (Form inputBox = new Form())
            using (Label label = new Label())
            using (TextBox textBox = new TextBox())
            using (RoundButton button = new RoundButton())
            using (GraphicsPath path = GetRectWithRoundCornor(0, 0, 240, 100, 50))
            {
                inputBox.Region = new Region(path);
                inputBox.BackColor = Color.FromArgb(41, 41, 41);
                inputBox.Controls.AddRange([label, textBox, button]);
                inputBox.FormBorderStyle = FormBorderStyle.None;
                inputBox.StartPosition = FormStartPosition.CenterScreen;

                button.Text = "OK";
                button.Size = new Size(100, 30);
                button.Location = new Point(20, 100);
                button.ArcLength = 20;
                button.ForeColor = Color.Black;
                button.EdgeColor = Color.DarkRed;
                button.SetBounds(80, 72, 80, 25);

                textBox.SetBounds(10, 40, 220, 20);

                label.SetBounds(75, 10, 372, 20);
                label.Text = "Add your Name";
                label.Font = Font;
                button.DialogResult = DialogResult.OK;
                inputBox.ShowDialog();

                return textBox.Text;
            }
        }

        private void BreakMenu()
        {
            _timer.Stop();
            _gameStopwatch.Stop();

            using (Form breakMenu = new Form())
            using (GraphicsPath path = GetRectWithRoundCornor(0, 0, 240, 100, 50))
            using (RoundButton button = new RoundButton())
            using (RoundButton button2 = new RoundButton())
            using (Label label = new Label())
            {
                breakMenu.Region = new Region(path);
                breakMenu.FormBorderStyle = FormBorderStyle.None;
                breakMenu.StartPosition = FormStartPosition.CenterScreen;
                breakMenu.BackColor = Color.FromArgb(41, 41, 41);

                label.Text = "Break Menu";
                label.Font = new Font("Comic San MS", 16, FontStyle.Bold);
                label.TextAlign = ContentAlignment.MiddleCenter;
                label.SetBounds(0, 0, 240, 50);

                button.Text = "Continue";
                button.Font = new Font("Comic San MS", 9, FontStyle.Regular);
                button.ForeColor = Color.Black;
                button.EdgeColor = Color.DarkRed;
                button.ArcLength = 20;
                button.Location = new Point(30, 80);
                button.SetBounds(25, 70, 80, 25);
                button.DialogResult = DialogResult.Cancel;


                button2.Text = "Restart";
                button.Font = new Font("Comic San MS", 9, FontStyle.Regular);
                button2.ForeColor = Color.Black;
                button2.EdgeColor = Color.DarkRed;
                button2.ArcLength = 20;
                button2.Location = new Point(130, 100);
                button2.SetBounds(135, 70, 80, 25);
                button2.DialogResult = DialogResult.OK;

                breakMenu.Controls.AddRange([label, button, button2]);
                DialogResult result = breakMenu.ShowDialog();
                switch (result)
                {
                    case DialogResult.OK:
                        Restart();
                        break;
                    case DialogResult.Cancel:
                        _timer.Start();
                        _gameStopwatch.Start();
                        break;
                }

            }
        }

        private async Task HitConfirm()
        {
            label3.Visible = true;
            await Task.Delay(300);
            label3.Visible = false;
        }

        private void DrawUI(PaintEventArgs e)
        {
            e.Graphics.DrawPath(_myPen, _guiEdgePath);
            e.Graphics.DrawLine(_myPen, 0, 40, Width, 40);
            e.Graphics.DrawLine(_myPen, 140, 40, 140, Height);
            e.Graphics.DrawRectangle(_myPen, _gameField);
            e.Graphics.FillRectangle(Brushes.Black, _gameField);
            e.Graphics.DrawEllipse(_myPen, _closeButton);
            e.Graphics.DrawLine(_myPen, _closeButton.Location.X + 4, _closeButton.Location.Y + 4, _closeButton.Location.X + 16, _closeButton.Location.Y + 16);
            e.Graphics.DrawLine(_myPen, _closeButton.Location.X + 16, _closeButton.Location.Y + 4, _closeButton.Location.X + 4, _closeButton.Location.Y + 16);
            e.Graphics.DrawEllipse(_myPen, _breakButton);
            e.Graphics.DrawLine(_myPen, _breakButton.Location.X + 16, _breakButton.Location.Y + 4, _breakButton.Location.X + 4, _breakButton.Location.Y + 16);
        }

        private void DrawGlow(Graphics g, RectangleF rect, Color color, int glowSize)
        {
            using (GraphicsPath path = new GraphicsPath())
            {
                path.AddEllipse(rect.Location.X - 2, rect.Location.Y - 2, rect.Width + 4, rect.Height + 4);

                for (int i = 1; i <= glowSize; i++)
                {
                    int alpha = (int)(32 * (1 - (float)i / glowSize));
                    using (Pen pen = new Pen(Color.FromArgb(alpha, color), i * 2))
                    {
                        pen.LineJoin = LineJoin.Round;
                        g.DrawPath(pen, path);
                    }
                }
            }
        }

        public static GraphicsPath GetRectWithRoundCornor(int x, int y, int width, int height, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            path.StartFigure();
            path.AddArc(x, y, radius, radius, 180, 90);
            path.AddArc(width - radius, y, radius, radius, 270, 90);
            path.AddArc(width - radius, height - radius, radius, radius, 0, 90);
            path.AddArc(x, height - radius, radius, radius, 90, 90);
            path.CloseFigure();
            return path;
        }

    }
}
