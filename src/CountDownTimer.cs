using System;
using System.Windows.Threading;

public class CountDownTimer
{
    public delegate void CountDownTimerHandler(TimeSpan timeLeft);
    public event CountDownTimerHandler TimerChanged;
    public Action TimeIsUp;

    private DispatcherTimer _timer;
    private DateTime _endTime;

    public CountDownTimer(int miliseconds)
    {
        _endTime = DateTime.Now + TimeSpan.FromMilliseconds(miliseconds);
        _timer = new DispatcherTimer();
        _timer.Interval = TimeSpan.FromMilliseconds(100);
        _timer.Tick += UpdateTimer;
        _timer.Start();
    }

    private void UpdateTimer(object sender, EventArgs e)
    {
        TimeSpan timeLeft = _endTime - DateTime.Now;
        if (TimerChanged != null)
            TimerChanged.Invoke(timeLeft);
        if (timeLeft <= TimeSpan.Zero)
        {
            _timer.Stop();
            _timer = null;
            if (TimeIsUp != null)
                TimeIsUp.Invoke();
        }
    }
}
