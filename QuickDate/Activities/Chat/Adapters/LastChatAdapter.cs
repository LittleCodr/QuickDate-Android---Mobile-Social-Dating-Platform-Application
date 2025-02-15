using AmulyaKhare.TextDrawableLib;
using Android.App;
using Android.Graphics;
using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.Widget;
using AndroidX.RecyclerView.Widget;
using Bumptech.Glide;
using Bumptech.Glide.Load.Engine;
using Bumptech.Glide.Request;
using Java.Util;
using QuickDate.Helpers.CacheLoaders;
using QuickDate.Helpers.Fonts;
using QuickDate.Helpers.Model;
using QuickDate.Helpers.Utils;
using QuickDateClient.Classes.Chat;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using IList = System.Collections.IList;
using Object = Java.Lang.Object;

namespace QuickDate.Activities.Chat.Adapters
{
    public class LastChatAdapter : RecyclerView.Adapter, ListPreloader.IPreloadModelProvider
    {
        private readonly Activity ActivityContext;

        public ObservableCollection<GetConversationListObject.DataConversation> UserList = new ObservableCollection<GetConversationListObject.DataConversation>();
        public event EventHandler<LastChatAdapterClickEventArgs> OnItemClick;
        public event EventHandler<LastChatAdapterClickEventArgs> OnItemLongClick;

        private IOnClickListenerSelected ClickListener;
        private readonly SparseBooleanArray SelectedItems;
        private int CurrentSelectedIdx = -1;

        public LastChatAdapter(Activity context)
        {
            try
            {
                HasStableIds = true;
                ActivityContext = context;
                SelectedItems = new SparseBooleanArray();
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public void SetOnClickListener(IOnClickListenerSelected onClickListener)
        {
            ClickListener = onClickListener;
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder viewHolder, int position, IList<Object> payloads)
        {
            try
            {
                if (payloads.Count > 0)
                {
                    var item = UserList[position];
                    if (viewHolder is LastChatAdapterViewHolder holder)
                    {
                        if (payloads[0].ToString() == "WithoutBlobText")
                            Initialize(holder, item);
                    }
                    else
                    {
                        base.OnBindViewHolder(viewHolder, position, payloads);
                    }
                }
                else
                {
                    base.OnBindViewHolder(viewHolder, position, payloads);
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
                base.OnBindViewHolder(viewHolder, position, payloads);
            }
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder viewHolder, int position)
        {
            try
            {
                if (viewHolder is LastChatAdapterViewHolder holder)
                {
                    var item = UserList[position];
                    if (item != null)
                    {
                        Initialize(holder, item);

                        holder.LytParent.Activated = SelectedItems.Get(position, false);

                        if (!holder.LytParent.HasOnClickListeners)
                        {
                            holder.LytParent.Click += delegate
                            {
                                try
                                {
                                    var data = GetItem(holder.BindingAdapterPosition);
                                    ClickListener?.ItemClick(holder.MainView, data, holder.BindingAdapterPosition);
                                }
                                catch (Exception e)
                                {
                                    Methods.DisplayReportResultTrack(e);
                                }
                            };

                            holder.LytParent.LongClick += delegate
                            {
                                try
                                {
                                    var data = GetItem(holder.BindingAdapterPosition);
                                    ClickListener?.ItemLongClick(holder.MainView, data, holder.BindingAdapterPosition);
                                }
                                catch (Exception e)
                                {
                                    Methods.DisplayReportResultTrack(e);
                                }
                            };
                        }

                        ToggleCheckedIcon(holder, position);
                    }
                }
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        private void Initialize(LastChatAdapterViewHolder holder, GetConversationListObject.DataConversation item)
        {
            try
            {
                GlideImageLoader.LoadImage(ActivityContext, item.User.Avater, holder.ImageAvatar, ImageStyle.CircleCrop, ImagePlaceholders.Drawable);

                holder.TxtUsername.Text = QuickDateTools.GetNameFinal(item.User);

                //If message contains Media files 
                switch (item.MessageType)
                {
                    case "text":
                        {
                            holder.LastMessagesIcon.Visibility = ViewStates.Gone;
                            holder.TxtLastMessages.Text = item.Text.Contains("http")
                                ? Methods.FunString.SubStringCutOf(item.Text, 30)
                                : Methods.FunString.DecodeString(Methods.FunString.SubStringCutOf(item.Text, 30))
                                ?? ActivityContext.GetText(Resource.String.Lbl_SendMessage);
                            break;
                        }
                    case "media":
                        {
                            holder.LastMessagesIcon.Visibility = ViewStates.Visible;
                            FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, holder.LastMessagesIcon, IonIconsFonts.Images);
                            holder.TxtLastMessages.Text = Application.Context.GetText(Resource.String.Lbl_SendImageFile);
                            break;
                        }
                    case "sticker" when item.Sticker.Contains(".gif"):
                        {
                            holder.LastMessagesIcon.Visibility = ViewStates.Visible;
                            FontUtils.SetTextViewIcon(FontsIconFrameWork.FontAwesomeLight, holder.LastMessagesIcon, FontAwesomeIcon.Gift);
                            holder.TxtLastMessages.Text = Application.Context.GetText(Resource.String.Lbl_SendGifFile);
                            break;
                        }
                    case "sticker":
                        {
                            holder.LastMessagesIcon.Visibility = ViewStates.Visible;
                            FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, holder.LastMessagesIcon, IonIconsFonts.Happy);
                            holder.TxtLastMessages.Text = Application.Context.GetText(Resource.String.Lbl_SendStickerFile);
                            break;
                        }
                }

                //last seen time  
                holder.TxtTimestamp.Text = Methods.Time.ReplaceTime(item.Time);

                //Check read message
                if (item.FromId != null && item.ToId != null && item.ToId.Value != UserDetails.UserId && item.FromId.Value == UserDetails.UserId)
                {
                    if (item.Seen > 0)
                    {
                        holder.IconCheckCountMessages.Visibility = ViewStates.Visible;
                        holder.TxtUsername.SetTypeface(Typeface.Default, TypefaceStyle.Normal);
                        holder.TxtLastMessages.SetTypeface(Typeface.Default, TypefaceStyle.Normal);
                        Glide.With(ActivityContext?.BaseContext).Load(Resource.Drawable.icon_tick_vector).Into(holder.IconCheckCountMessages);
                    }
                    else
                    {
                        holder.IconCheckCountMessages.Visibility = ViewStates.Gone;
                        holder.TxtUsername.SetTypeface(Typeface.Default, TypefaceStyle.Normal);
                        holder.TxtLastMessages.SetTypeface(Typeface.Default, TypefaceStyle.Normal);
                    }
                }
                else if (item.FromId != null && item.ToId != null && item.ToId.Value == UserDetails.UserId && item.FromId.Value != UserDetails.UserId)
                {
                    if (item.Seen == 0)
                    {
                        holder.TxtUsername.SetTypeface(Typeface.Default, TypefaceStyle.Bold);
                        holder.TxtLastMessages.SetTypeface(Typeface.Default, TypefaceStyle.Bold);

                        holder.IconCheckCountMessages.Visibility = ViewStates.Visible;
                        if (item.NewMessages != 0)
                        {
                            var drawable = TextDrawable.InvokeBuilder().BeginConfig().FontSize(25).EndConfig().BuildRound(item.NewMessages.ToString(), Color.ParseColor(AppSettings.MainColor));
                            holder.IconCheckCountMessages.SetImageDrawable(drawable);
                        }
                        else
                        {
                            var drawable = TextDrawable.InvokeBuilder().BeginConfig().FontSize(25).EndConfig().BuildRound("N", Color.ParseColor(AppSettings.MainColor));
                            holder.IconCheckCountMessages.SetImageDrawable(drawable);
                        }
                    }
                    else
                    {
                        holder.IconCheckCountMessages.Visibility = ViewStates.Gone;
                        holder.TxtUsername.SetTypeface(Typeface.Default, TypefaceStyle.Normal);
                        holder.TxtLastMessages.SetTypeface(Typeface.Default, TypefaceStyle.Normal);
                    }
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            try
            {
                //Setup your layout here >> Style_LastChatView
                var itemView = LayoutInflater.From(parent.Context)?.Inflate(Resource.Layout.Style_LastChatView, parent, false);
                var vh = new LastChatAdapterViewHolder(itemView, Click, LongClick);
                return vh;
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
                return null;
            }
        }

        public override int ItemCount => UserList?.Count ?? 0;


        #region Toolbar & Selected

        private void ToggleCheckedIcon(LastChatAdapterViewHolder holder, int position)
        {
            try
            {
                if (SelectedItems.Get(position, false))
                {
                    holder.LytImage.Visibility = ViewStates.Gone;
                    holder.LytChecked.Visibility = ViewStates.Visible;
                    if (CurrentSelectedIdx == position) ResetCurrentItems();
                }
                else
                {
                    holder.LytChecked.Visibility = ViewStates.Gone;
                    holder.LytImage.Visibility = ViewStates.Visible;
                    if (CurrentSelectedIdx == position) ResetCurrentItems();
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }

        }

        private void ResetCurrentItems()
        {
            try
            {
                CurrentSelectedIdx = -1;
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }


        public int GetSelectedItemCount()
        {
            return SelectedItems.Size();
        }

        public List<int> GetSelectedItems()
        {
            List<int> items = new List<int>(SelectedItems.Size());
            for (int i = 0; i < SelectedItems.Size(); i++)
            {
                items.Add(SelectedItems.KeyAt(i));
            }
            return items;
        }

        public void RemoveData()
        {
            try
            {
                ResetCurrentItems();
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public void ClearSelections()
        {
            try
            {
                SelectedItems.Clear();
                NotifyDataSetChanged();
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public void ToggleSelection(int pos)
        {
            try
            {
                CurrentSelectedIdx = pos;
                if (SelectedItems.Get(pos, false))
                {
                    SelectedItems.Delete(pos);
                }
                else
                {
                    SelectedItems.Put(pos, true);
                }
                NotifyItemChanged(pos);
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }


        #endregion

        public GetConversationListObject.DataConversation GetItem(int position)
        {
            return UserList[position];
        }

        public override long GetItemId(int position)
        {
            try
            {
                return position;
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
                return 0;
            }
        }

        public override int GetItemViewType(int position)
        {
            try
            {
                return position;
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
                return 0;
            }
        }

        private void Click(LastChatAdapterClickEventArgs args)
        {
            OnItemClick?.Invoke(this, args);
        }

        private void LongClick(LastChatAdapterClickEventArgs args)
        {
            OnItemLongClick?.Invoke(this, args);
        }

        public IList GetPreloadItems(int p0)
        {
            try
            {
                var d = new List<string>();
                var item = UserList[p0];

                if (item == null)
                    return Collections.SingletonList(p0);

                if (item.User?.Avater != "")
                {
                    d.Add(item.User.Avater);
                    return d;
                }

                return d;
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
                return Collections.SingletonList(p0);
            }
        }

        public RequestBuilder GetPreloadRequestBuilder(Object p0)
        {
            return Glide.With(ActivityContext?.BaseContext).Load(p0.ToString())
                .Apply(new RequestOptions().CircleCrop().SetDiskCacheStrategy(DiskCacheStrategy.All));
        }
    }

    public class LastChatAdapterViewHolder : RecyclerView.ViewHolder
    {
        #region Variables Basic

        public View MainView { get; private set; }

        public RelativeLayout LytParent { get; private set; }
        public TextView TxtUsername { get; private set; }
        public TextView LastMessagesIcon { get; private set; }
        public TextView TxtLastMessages { get; private set; }
        public TextView TxtTimestamp { get; private set; }
        public ImageView ImageAvatar { get; private set; }
        public ImageView IconCheckCountMessages { get; private set; }

        public RelativeLayout LytChecked { get; private set; }
        public RelativeLayout LytImage { get; private set; }

        #endregion

        public LastChatAdapterViewHolder(View itemView, Action<LastChatAdapterClickEventArgs> clickListener, Action<LastChatAdapterClickEventArgs> longClickListener) : base(itemView)
        {
            try
            {
                MainView = itemView;

                //Get values
                LytParent = (RelativeLayout)MainView.FindViewById(Resource.Id.main);
                TxtUsername = (TextView)MainView.FindViewById(Resource.Id.Txt_Username);
                LastMessagesIcon = (AppCompatTextView)MainView.FindViewById(Resource.Id.LastMessages_icon);
                TxtLastMessages = (TextView)MainView.FindViewById(Resource.Id.Txt_LastMessages);
                TxtTimestamp = (TextView)MainView.FindViewById(Resource.Id.Txt_timestamp);
                ImageAvatar = (ImageView)MainView.FindViewById(Resource.Id.ImageAvatar);

                IconCheckCountMessages = (ImageView)MainView.FindViewById(Resource.Id.IconCheckRead);

                LytChecked = (RelativeLayout)MainView.FindViewById(Resource.Id.lyt_checked);
                LytImage = (RelativeLayout)MainView.FindViewById(Resource.Id.lyt_image);


                //Create an Event
                //itemView.Click += (sender, e) => clickListener(new LastChatAdapterClickEventArgs { View = itemView, Position = BindingAdapterPosition });
                //itemView.LongClick += (sender, e) => longClickListener(new LastChatAdapterClickEventArgs { View = itemView, Position = BindingAdapterPosition });

            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }
    }

    public class LastChatAdapterClickEventArgs : EventArgs
    {
        public View View { get; set; }
        public int Position { get; set; }
    }
}