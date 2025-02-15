using Android.Media;
using Android.OS;

namespace QuickDate.Activities.Call.Tools
{
    public class AudioHelper
    {
        //determine if user connected a headset (normal or Bluetooth)
        public static bool IsHeadsetOn(AudioManager audioManager)
        {
            if (Build.VERSION.SdkInt < BuildVersionCodes.M)
            {
                return audioManager.WiredHeadsetOn || audioManager.BluetoothA2dpOn;
            }
            else
            {
                AudioDeviceInfo[] devices = audioManager.GetDevices(GetDevicesTargets.Outputs);
                for (int i = 0; i < devices.Length; i++)
                {
                    AudioDeviceInfo device = devices[i];
                    if (device.Type == AudioDeviceType.WiredHeadphones || device.Type == AudioDeviceType.BluetoothA2dp || device.Type == AudioDeviceType.BluetoothSco)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public static bool IsBluetoothHeadsetOn(AudioManager audioManager)
        {
            if (Build.VERSION.SdkInt < BuildVersionCodes.M)
            {
                return audioManager.BluetoothA2dpOn;
            }
            else
            {
                AudioDeviceInfo[] devices = audioManager.GetDevices(GetDevicesTargets.Outputs);
                for (int i = 0; i < devices.Length; i++)
                {
                    AudioDeviceInfo device = devices[i];
                    if (device.Type is AudioDeviceType.BluetoothA2dp or AudioDeviceType.BluetoothSco)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

    }
}
