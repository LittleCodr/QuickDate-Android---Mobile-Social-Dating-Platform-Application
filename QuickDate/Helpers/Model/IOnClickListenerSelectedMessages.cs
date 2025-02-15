using Android.Views;
using QuickDateClient.Classes.Chat;

namespace QuickDate.Helpers.Model
{
    public interface IOnClickListenerSelectedMessages
    {
        void ItemClick(View view, GetChatConversationsObject.Messages obj, int pos);
        void ItemLongClick(View view, GetChatConversationsObject.Messages obj, int pos);

    }
}