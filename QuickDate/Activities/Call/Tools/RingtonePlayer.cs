using Android.App;
using Android.Content;
using Android.Media;
using Android.OS;
using QuickDate.Helpers.Utils;
using System;
using Uri = Android.Net.Uri;

namespace QuickDate.Activities.Call.Tools
{
    public class RingtonePlayer
    {
        private readonly Ringtone DefaultRingtone;
        private Vibrator Vibrator;
        private readonly Context Context;

        private readonly long[] Vibrate = new long[]
        {
            1000, 1000, 2000, 1000, 2000, 1000, 2000, 1000, 2000, 1000, 2000, 1000, 2000, 1000, 2000, 1000,
            2000, 1000, 2000, 1000, 2000, 1000, 2000, 1000, 2000
        };

        public RingtonePlayer(Context context)
        {
            try
            {
                Context = context;
                Uri defaultRingtoneUri = RingtoneManager.GetActualDefaultRingtoneUri(context.ApplicationContext, RingtoneType.Ringtone);
                DefaultRingtone = RingtoneManager.GetRingtone(context, defaultRingtoneUri);
                DefaultRingtone.Looping = true;
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public void PlayIncomingRingtone()
        {
            try
            {
                if (DefaultRingtone != null && !DefaultRingtone.IsPlaying)
                    DefaultRingtone.Play();

                if (Build.VERSION.SdkInt >= BuildVersionCodes.S)
                {
                    VibratorManager vibratorManager = (VibratorManager)Application.Context.GetSystemService(Context.VibratorManagerService);
                    Vibrator = vibratorManager?.DefaultVibrator;
                }
                else
                {
                    Vibrator = (Vibrator)Context.GetSystemService("vibrator");
                }

                // Vibrate for 500 milliseconds
                Vibrator?.Vibrate(VibrationEffect.CreateWaveform(Vibrate, 3));
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public void StopRingtone()
        {
            try
            {
                if (DefaultRingtone != null)
                    DefaultRingtone.Stop();

                StopProgressTone();

                Vibrator?.Cancel();
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public void PlayProgressTone()
        {
            try
            {
                Methods.AudioRecorderAndPlayer.PlayAudioFromAsset("outgoin_call.mp3", "Looping");
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public void StopProgressTone()
        {
            try
            {
                Methods.AudioRecorderAndPlayer.StopAudioFromAsset();
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }
    }
}
