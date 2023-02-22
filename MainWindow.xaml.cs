using System;
using System.Windows;
using System.Windows.Threading;

namespace Match_3_Test
{
    /// <summary>
    /// Main game window behaviour.
    /// </summary>
    public partial class MainWindow : Window
    {
        private GameField _mainGameField;
        private int _rows = 8;
        private int _columns = 8;
        private int _types = 5;
        private int _GameDuration = 60;
        private float _logicFPS = 10;
        private float _graphicsFPS = 60;
        // Separate timers for game logic and visual representation
        public static DispatcherTimer gameLogicTimer;
        public static DispatcherTimer animationTimer;
        public static CountDownTimer countdownTimer;

        public MainWindow()
        {
            ResourceManager.LoadGraphics();
            InitializeComponent();

            gameLogicTimer = new DispatcherTimer();
            gameLogicTimer.Interval = TimeSpan.FromMilliseconds(1000/_logicFPS);

            animationTimer = new DispatcherTimer();
            animationTimer.Interval = TimeSpan.FromMilliseconds(1000/_graphicsFPS);

            StartButton.Click += StartGame;
            ReturnToMainMenuButton.Click += ReturnToMenu;
            HideMenus();
            SetMainMenu(true);

        }

        private void StartGame(object sender, EventArgs e)
        {
            gameLogicTimer.Start();
            animationTimer.Start();
            countdownTimer = new CountDownTimer(_GameDuration * 1000);
            countdownTimer.TimeIsUp += StopGame;
            countdownTimer.TimerChanged += UpdateTimer;

            _mainGameField = new GameField(_rows, _columns, _types, GameCanvas);
            UpdateScore(0);
            _mainGameField.ScoreChanged += UpdateScore;
            HideMenus();
        }

        private void StopGame()
        {
            gameLogicTimer.Stop();
            animationTimer.Stop();

            _mainGameField.ClearField();
            _mainGameField = null;
            HideMenus();
            SetGameOverMenu(true);
        }

        private void ReturnToMenu(object sender, EventArgs e)
        {
            HideMenus();
            SetMainMenu(true);
        }

        private void UpdateTimer(TimeSpan time)
        {
            TimerText.Content = ("Time left: " + time.Seconds);
        }

        private void UpdateScore(int score)
        {
            ScoreText.Content = ("Score: " + score);
        }

        private void HideMenus()
        {
            SetGameOverMenu(false);
            SetMainMenu(false);
        }

        private void SetGameOverMenu(bool state)
        {
            if (state)
                GameOverMenu.Visibility = Visibility.Visible;
            else
                GameOverMenu.Visibility = Visibility.Hidden;
        }

        private void SetMainMenu(bool state)
        {
            if (state)
                MainMenu.Visibility = Visibility.Visible;
            else
                MainMenu.Visibility = Visibility.Hidden;
        }
    }
}
