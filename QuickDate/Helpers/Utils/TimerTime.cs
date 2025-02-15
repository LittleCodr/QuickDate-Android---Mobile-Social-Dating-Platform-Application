using System;
using Exception = System.Exception;
using Timer = System.Timers.Timer;

namespace QuickDate.Helpers.Utils
{
    public class TimerTime
    {
        private int Minutes = 5;
        private int Seconds;
        private string Count = "";
        public static Timer TimerCount { set; get; }

        public TimerTime()
        {
            try
            {
                TimerCount = new Timer
                {
                    Interval = 1000,
                    Enabled = false,
                };
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public void SetMinutes(int minutes)
        {
            try
            {
                Minutes = minutes;
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        public void SetSeconds(int seconds)
        {
            try
            {
                Seconds = seconds;
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        public Timer GetTimer()
        {
            try
            {
                return TimerCount;
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
                return null;
            }
        }

        public void SetStartTimer()
        {
            try
            {
                TimerCount.Enabled = true;
                TimerCount.Start();
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        public (string, string) TimerCountOnElapsed()
        {
            try
            {
                return SetCountDown(Minutes, Seconds);
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
                return ("", "");
            }
        }

        public void SetStopTimer()
        {
            try
            {
                TimerCount.Enabled = false;
                TimerCount.Stop();
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        public (string, string) SetCountDown(int iMinutes, int iSeconds)
        {
            try
            {
                iSeconds -= 1;

                if (iSeconds < 0)
                {
                    iSeconds = 0;
                }

                if (iSeconds == 0 && iMinutes > 0)
                {
                    iSeconds = 60;
                    iMinutes -= 1;
                }

                if (iMinutes < 0)
                {
                    iMinutes = 0;
                }

                Seconds = iSeconds;
                Minutes = iMinutes;

                if (iMinutes == 0 && iSeconds == 0)
                {
                    SetStopTimer();
                    return ("", "");
                }

                TimeSpan tsTemp = new TimeSpan(0, iMinutes, iSeconds);
                Count = tsTemp.Minutes + ":" + tsTemp.Seconds;
                return (tsTemp.Minutes.ToString(), tsTemp.Seconds.ToString());
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
                return ("", "");
            }
        }

        public string CheckCountTime(int time)
        {
            try
            {
                var timeBoost = ListUtils.SettingsSiteList?.BoostExpireTime ?? "4";
                var timeBoostMilliseconds = Methods.Time.ConvertMinutesToMilliseconds(Convert.ToDouble(timeBoost));
                var timeBoostSeconds = Methods.Time.ConvertMinutesToSeconds(Convert.ToDouble(timeBoost));

                double minutes = 0;
                double seconds = 0;
                string result = "";

                if (time == (int)timeBoostMilliseconds)
                {
                    minutes = Methods.Time.ConvertMillisecondsToMinutes(Convert.ToDouble(time));
                    seconds = Methods.Time.ConvertMillisecondsToSeconds(Convert.ToDouble(time));
                }
                else
                {
                    DateTime date = Methods.Time.UnixTimeStampToDateTime(time);
                    var timeSpan = DateTime.Now.Subtract(date);

                    if (timeSpan <= TimeSpan.FromSeconds(60))
                    {
                        seconds = 60 - timeSpan.Seconds;
                    }

                    if (timeSpan <= TimeSpan.FromMinutes(60))
                    {
                        minutes = timeSpan.Minutes > 1 ? timeSpan.Minutes : 0;
                        if (timeSpan.Minutes == 0)
                        {
                            minutes = Convert.ToInt32(timeBoost) - 1;
                        }
                        else
                        {
                            minutes = Convert.ToInt32(timeBoost) - timeSpan.Minutes;
                        }
                    }
                }

                if (minutes < 0)
                    minutes = 0;

                if (seconds < 0)
                    seconds = 0;

                result = seconds < 10 ? minutes + ":0" + seconds : minutes + ":" + seconds;
                return result;
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
                return "";
            }
        }
    }
}