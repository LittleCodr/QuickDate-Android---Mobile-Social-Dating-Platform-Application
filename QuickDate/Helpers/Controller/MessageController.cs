using Android.App;
using Android.Widget;
using QuickDate.Activities.Chat;
using QuickDate.Helpers.Model;
using QuickDate.Helpers.Utils;
using QuickDate.SQLite;
using QuickDateClient;
using QuickDateClient.Classes.Chat;
using QuickDateClient.Classes.Global;
using QuickDateClient.Requests;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace QuickDate.Helpers.Controller
{
    public static class MessageController
    {
        //############# DON'T MODIFY HERE #############
        //========================= Functions =========================
        private static MessagesBoxActivity Activity;

        public static async Task SendMessageTask(MessagesBoxActivity activity, int userId, string text, string stickerId, string path, string hashId, UserInfoObject userData)
        {
            try
            {
                Activity = activity;
                var (apiStatus, respond) = await RequestsAsync.Chat.SendMessageAsync(userId.ToString(), text, stickerId, path, hashId);
                if (apiStatus == 200)
                {
                    if (respond is SendMessageObject result)
                    {
                        if (result.Data != null)
                        {
                            activity?.RunOnUiThread(() =>
                            {
                                try
                                {
                                    UpdateLastIdMessage(result, userData, hashId);
                                }
                                catch (Exception e)
                                {
                                    Methods.DisplayReportResultTrack(e);
                                }
                            });
                        }
                    }
                }
                else
                {
                    if (respond is ErrorObject error)
                    {
                        var errorText = "Error Send Message";
                        if (!string.IsNullOrEmpty(error.ErrorData.ErrorText))
                        {
                            errorText = error.ErrorData.ErrorText;
                        }
                        else if (!string.IsNullOrEmpty(error.Message))
                        {
                            errorText = error.Message;
                        }
                        activity?.RunOnUiThread(() =>
                        {
                            try
                            {
                                Toast.MakeText(activity, errorText, ToastLength.Short)?.Show();

                                var data = Activity.MAdapter?.MessageList?.FirstOrDefault(a => a.Id == Convert.ToInt32(hashId));
                                if (data != null)
                                {
                                    var index = Activity.MAdapter.MessageList.IndexOf(data);
                                    Activity.MAdapter.RemoveData(index, data);
                                } 
                            }
                            catch (Exception e)
                            {
                                Methods.DisplayReportResultTrack(e);
                            }
                        });
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        /// <summary>
        /// ///////////////////////////////////
        /// </summary>
        /// <param name="messages"></param>
        /// <param name="userData"></param>
        /// <param name="hashId"></param>
        private static void UpdateLastIdMessage(SendMessageObject messages, UserInfoObject userData, string hashId)
        {
            try
            {
                var checker = Activity?.MAdapter?.MessageList?.FirstOrDefault(a => a.Id == Convert.ToInt32(hashId));
                if (checker != null)
                {
                    checker.Id = messages.Data.Id;
                    checker.FromName = UserDetails.FullName;
                    checker.FromAvater = UserDetails.Avatar;
                    checker.ToName = userData?.FullName ?? "";
                    checker.ToAvater = userData?.Avater ?? "";
                    checker.From = messages.Data.From;
                    checker.To = messages.Data.To;
                    checker.Text = Methods.FunString.DecodeString(messages.Data.Text);
                    checker.Media = checker.Media.Contains("upload/chat/") && !messages.Data.Media.Contains(InitializeQuickDate.WebsiteUrl) ? InitializeQuickDate.WebsiteUrl + "/" + messages.Data.Media : messages.Data.Media;
                    checker.FromDelete = messages.Data.FromDelete;
                    checker.ToDelete = messages.Data.ToDelete;
                    checker.Sticker = messages.Data.Sticker;
                    //checker.CreatedAt = messages.Data.CreatedAt;
                    checker.Seen = messages.Data.Seen;
                    checker.Type = "Sent";
                    checker.MessageType = messages.Data.MessageType;

                    string text = "";
                    switch (checker.MessageType)
                    {
                        case "text":
                            text = string.IsNullOrEmpty(text) ? Application.Context.GetText(Resource.String.Lbl_SendMessage) : Methods.FunString.DecodeString(messages.Data.Text);
                            break;
                        case "media":
                            text = Application.Context.GetText(Resource.String.Lbl_SendImageFile);
                            break;
                        case "sticker" when checker.Sticker.Contains(".gif"):
                            text = Application.Context.GetText(Resource.String.Lbl_SendGifFile);
                            break;
                        case "sticker":
                            text = Application.Context.GetText(Resource.String.Lbl_SendStickerFile);
                            break;
                        default:
                            text = Methods.FunString.DecodeString(messages.Data.Text);
                            break;
                    }

                    var instance = LastChatFragment.GetInstance();
                    var dataUser = instance?.MAdapter?.UserList?.FirstOrDefault(a => a.User.Id == messages.Data.To);
                    if (dataUser != null)
                    {
                        //var index = instance.MAdapter?.UserList?.IndexOf(instance.MAdapter.UserList?.FirstOrDefault(x => x.User.Id == messages.Data.To));
                        //if (index > -1)
                        //{
                        //    instance.MAdapter?.UserList?.Move(Convert.ToInt32(index), 0);
                        //    instance.MAdapter?.NotifyItemMoved(Convert.ToInt32(index), 0);

                        //    var data = instance.MAdapter?.UserList?.FirstOrDefault(a => a.User.Id == dataUser.User.Id);
                        //    if (data != null)
                        //    {
                        //        data.Id = dataUser.Id;
                        //        data.Owner = dataUser.Owner;
                        //        data.User = dataUser.User;
                        //        data.Seen = messages.Data.Seen;
                        //        data.Text = dataUser.Text;
                        //        data.Media = dataUser.Media;
                        //        data.Sticker = dataUser.Sticker;
                        //        data.Time = dataUser.Time;
                        //        data.CreatedAt = dataUser.CreatedAt;
                        //        data.NewMessages = dataUser.NewMessages;
                        //        data.MessageType = dataUser.MessageType;
                        //        data.FromId = dataUser.FromId;
                        //        data.ToId = dataUser.ToId;
                        //        data.MessageType = messages.Data.MessageType;

                        //        instance.MAdapter.NotifyDataSetChanged();
                        //    }
                        //}
                    }
                    else
                    {
                        if (userData != null)
                        {
                            instance?.MAdapter?.UserList?.Insert(0, new GetConversationListObject.DataConversation
                            {
                                Id = userData.Id,
                                Owner = 0,
                                User = userData,
                                Seen = messages.Data.Seen,
                                Text = text,
                                Media = messages.Data.Media,
                                Sticker = messages.Data.Sticker,
                                Time = messages.Data.CreatedAt,
                                CreatedAt = messages.Data.CreatedAt,
                                NewMessages = 0,
                                FromId = messages.Data.From,
                                ToId = messages.Data.To,
                                MessageType = messages.Data.MessageType
                            });

                            instance?.MAdapter?.NotifyItemInserted(0);
                        }
                    }

                    SqLiteDatabase dbDatabase = new SqLiteDatabase();
                    //GetChatConversationsObject.Messages message = new GetChatConversationsObject.Messages
                    //{
                    //    Id = messages.Data.Id,
                    //    FromName = UserDetails.FullName,
                    //    FromAvater = UserDetails.Avatar,
                    //    ToName = userData?.Fullname ?? "",
                    //    ToAvater = userData?.Avater ?? "",
                    //    From = messages.Data.From,
                    //    To = messages.Data.To,
                    //    Text = Methods.FunString.DecodeString(checker.Text),
                    //    Media = checker.Media,
                    //    FromDelete = checker.FromDelete,
                    //    ToDelete = checker.ToDelete,
                    //    Sticker = checker.Sticker,
                    //    //CreatedAt = checker.CreatedAt,
                    //    Seen = 1,
                    //    Type = "Sent",
                    //    MessageType = messages.Data.MessageType,
                    //};
                    //Update All data users to database
                    dbDatabase.InsertOrUpdateToOneMessages(checker);


                    Activity.UpdateOneMessage(checker);
                    Activity?.ChatBoxRecyclerView.ScrollToPosition(Activity.MAdapter.MessageList.IndexOf(Activity.MAdapter.MessageList.Last()));

                    if (AppSettings.RunSoundControl)
                        Methods.AudioRecorderAndPlayer.PlayAudioFromAsset("Popup_SendMesseges.mp3");
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

    }
}