using Android.Content;
using Android.Hardware;
using Android.OS;
using QuickDate.Helpers.Utils;
using System;
using System.Linq;
using Object = Java.Lang.Object;

namespace QuickDate.Activities.Call.Tools
{
    public class ProximitySensor : Object, ISensorEventListener
    {
        public interface IDelegate
        {
            void OnProximitySensorNear();

            void OnProximitySensorFar();
        }

        private readonly SensorManager MSensorManager;
        private readonly Sensor MSensor;
        private readonly PowerManager.WakeLock MScreenLock;
        private readonly IDelegate MDelegate;

        public ProximitySensor(Context context, IDelegate @delegate)
        {
            try
            {
                Context appContext = context.ApplicationContext;
                MSensorManager = (SensorManager)appContext.GetSystemService(Context.SensorService);
                MDelegate = @delegate;

                MSensor = MSensorManager.GetDefaultSensor(SensorType.Proximity);

                //there is no proximity sensor in device
                if (MSensor == null) return;

                //request turn screen lock (turn to black)
                PowerManager powerManager = (PowerManager)appContext.GetSystemService(Context.PowerService);
                MScreenLock = powerManager?.NewWakeLock(WakeLockFlags.ProximityScreenOff, "My Tag");
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public void Acquire()
        {
            try
            {
                if (MScreenLock != null && !MScreenLock.IsHeld)
                {
                    MScreenLock.Acquire();
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }

        }

        //revert screen to normal
        public void Release()
        {
            try
            {
                if (MScreenLock != null && MScreenLock.IsHeld)
                    MScreenLock.Release();
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        //the listener will not work unless this is called
        public void ListenForSensor()
        {
            try
            {
                if (MSensorManager != null && MSensor != null)
                {
                    MSensorManager.RegisterListener(this, MSensor, SensorDelay.Normal);
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public void StopListenForSensor()
        {
            try
            {
                if (MSensorManager != null && MSensor != null)
                {
                    MSensorManager.UnregisterListener(this);
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public void OnAccuracyChanged(Sensor sensor, SensorStatus accuracy)
        {
            try
            {

            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public void OnSensorChanged(SensorEvent e)
        {
            try
            {
                if (e.Sensor.Type != SensorType.Proximity) return;

                //NEAR
                if (e.Values != null)
                {
                    var first = e.Values.First();
                    if (first < 5f && first != MSensor.MaximumRange)
                    {
                        MDelegate.OnProximitySensorNear();
                    }
                    else
                    {
                        //FAR
                        MDelegate.OnProximitySensorFar();
                    }
                }
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }
    }
}
