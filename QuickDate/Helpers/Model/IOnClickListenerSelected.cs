using Android.Views;
using QuickDateClient.Classes.Chat;

namespace QuickDate.Helpers.Model
{
    public interface IOnClickListenerSelected
    {
        void ItemClick(View view, GetConversationListObject.DataConversation obj, int pos);

        void ItemLongClick(View view, GetConversationListObject.DataConversation obj, int pos);
    }
}