using Android.Content;
using QuickDate.Activities.Live.Page;

namespace QuickDate.Activities.Live.Utils
{
    public class PrefManager
    {
        public static ISharedPreferences GetPreferences(Context context)
        {
            return context.GetSharedPreferences(LiveConstants.PrefName, FileCreationMode.Private);
        }
    }
}