using QuickDate.Helpers.Model;
using QuickDate.SQLite;
using QuickDateClient.Classes.Blogs;
using QuickDateClient.Classes.Common;
using QuickDateClient.Classes.Global;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;

namespace QuickDate.Helpers.Utils
{
    public static class ListUtils
    {
        //############# DON'T MODIFY HERE #############
        //List Items Declaration 
        //*********************************************************
        public static ObservableCollection<DataTables.LoginTb> DataUserLoginList = new ObservableCollection<DataTables.LoginTb>();
        public static GetOptionsObject.DataOptions SettingsSiteList;
        public static ObservableCollection<Classes.Languages> LanguagesSiteList = new ObservableCollection<Classes.Languages>();
        public static ObservableCollection<UserInfoObject> MyUserInfo = new ObservableCollection<UserInfoObject>();
        public static ObservableCollection<DataFile> GiftsList = new ObservableCollection<DataFile>();
        public static ObservableCollection<DataFile> StickersList = new ObservableCollection<DataFile>();
        public static ObservableCollection<UserInfoObject> AllMatchesList = new ObservableCollection<UserInfoObject>();
        public static readonly ObservableCollection<UserInfoObject> LikedList = new ObservableCollection<UserInfoObject>();
        public static ObservableCollection<UserInfoObject> DisLikedList = new ObservableCollection<UserInfoObject>();
        public static ObservableCollection<UserInfoObject> OldMatchesList = new ObservableCollection<UserInfoObject>();
        public static ObservableCollection<GetNotificationsObject.Datum> MatchList = new ObservableCollection<GetNotificationsObject.Datum>();
        public static ObservableCollection<GetNotificationsObject.Datum> VisitsList = new ObservableCollection<GetNotificationsObject.Datum>();
        public static ObservableCollection<GetNotificationsObject.Datum> LikesList = new ObservableCollection<GetNotificationsObject.Datum>();
        public static ObservableCollection<GetNotificationsObject.Datum> RequestsList = new ObservableCollection<GetNotificationsObject.Datum>();
        public static ObservableCollection<GetNotificationsObject.Datum> NotificationList = new ObservableCollection<GetNotificationsObject.Datum>();
        //public static ObservableCollection<GetConversationListObject.DataConversation> ChatList = new ObservableCollection<GetConversationListObject.DataConversation>();
        public static ObservableCollection<ArticleDataObject> ListCachedDataArticle = new ObservableCollection<ArticleDataObject>();
        public static ObservableCollection<UserInfoObject> FriendRequestsList = new ObservableCollection<UserInfoObject>();

        public static void ClearAllList()
        {
            try
            {
                DataUserLoginList.Clear();
                SettingsSiteList = null;
                LanguagesSiteList.Clear();
                MyUserInfo.Clear();
                GiftsList.Clear();
                StickersList.Clear();
                LikedList.Clear();
                DisLikedList.Clear();
                OldMatchesList.Clear();
                MatchList.Clear();
                VisitsList.Clear();
                LikesList.Clear();
                // ChatList.Clear();
                AllMatchesList.Clear();
                FriendRequestsList.Clear();
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public static void AddRange<T>(ObservableCollection<T> collection, IEnumerable<T> items)
        {
            try
            {
                items.ToList().ForEach(collection.Add);
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public static List<List<T>> SplitList<T>(List<T> locations, int nSize = 30)
        {
            var list = new List<List<T>>();

            for (int i = 0; i < locations.Count; i += nSize)
            {
                list.Add(locations.GetRange(i, Math.Min(nSize, locations.Count - i)));
            }

            return list;
        }

        public static IEnumerable<T> TakeLast<T>(IEnumerable<T> source, int n)
        {
            var enumerable = source as T[] ?? source.ToArray();

            return enumerable.Skip(Math.Max(0, enumerable.Count() - n));
        }

        public static void Copy<T>(T from, T to)
        {
            Type t = typeof(T);
            PropertyInfo[] props = t.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo p in props)
            {
                try
                {
                    if (!p.CanRead || !p.CanWrite) continue;

                    object val = p.GetGetMethod().Invoke(from, null);
                    object defaultVal = p.PropertyType.IsValueType ? Activator.CreateInstance(p.PropertyType) : null;
                    if (null != defaultVal && !val.Equals(defaultVal))
                    {
                        p.GetSetMethod().Invoke(to, new[] { val });
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }

        public static int Remove<T>(this ObservableCollection<T> coll, Func<T, bool> condition)
        {
            var itemsToRemove = coll.Where(condition).ToList();

            foreach (var itemToRemove in itemsToRemove)
            {
                coll.Remove(itemToRemove);
            }

            return itemsToRemove.Count;
        }

        /// <summary>
        /// Extends ObservableCollection adding a RemoveAll method to remove elements based on a boolean condition function
        /// </summary>
        /// <typeparam name="T">The type contained by the collection</typeparam>
        /// <param name="observableCollection">The ObservableCollection</param>
        /// <param name="condition">A function that evaluates to true for elements that should be removed</param>
        /// <returns>The number of elements removed</returns>
        public static int RemoveAll<T>(this ObservableCollection<T> observableCollection, Func<T, bool> condition)
        {
            // Find all elements satisfying the condition, i.e. that will be removed
            var toRemove = observableCollection
                .Where(condition)
                .ToList();

            // Remove the elements from the original collection, using the Count method to iterate through the list, 
            // incrementing the count whenever there's a successful removal
            return toRemove.Count(observableCollection.Remove);
        }

        /// <summary>
        /// Extends ObservableCollection adding a RemoveAll method to remove elements based on a boolean condition function
        /// </summary>
        /// <typeparam name="T">The type contained by the collection</typeparam>
        /// <param name="observableCollection">The ObservableCollection</param>
        /// <param name="toRemove">Find all elements satisfying the condition, i.e. that will be removed</param>
        /// <returns>The number of elements removed</returns>
        public static int RemoveAll<T>(this ObservableCollection<T> observableCollection, List<T> toRemove)
        {
            // Remove the elements from the original collection, using the Count method to iterate through the list, 
            // incrementing the count whenever there's a successful removal
            return toRemove.Count(observableCollection.Remove);
        }


    }
}