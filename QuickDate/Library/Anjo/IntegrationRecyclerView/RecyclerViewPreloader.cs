using Android.App;
using AndroidX.Fragment.App;
using AndroidX.RecyclerView.Widget;
using Bumptech.Glide;
using Fragment = AndroidX.Fragment.App.Fragment;

namespace QuickDate.Library.Anjo.IntegrationRecyclerView
{
    public sealed class RecyclerViewPreloader<T> : RecyclerView.OnScrollListener
    {

        private readonly RecyclerToListViewScrollListener recyclerScrollListener;

        /// <summary>
        /// Helper constructor that accepts an <seealso cref="Activity"/>. </summary>
        public RecyclerViewPreloader(Activity activity, ListPreloader.IPreloadModelProvider preloadModelProvider, ListPreloader.IPreloadSizeProvider preloadDimensionProvider, int maxPreload) : this(Glide.With(activity.BaseContext), preloadModelProvider, preloadDimensionProvider, maxPreload)
        {
        }

        /// <summary>
        /// Helper constructor that accepts an <seealso cref="FragmentActivity"/>. </summary>
        public RecyclerViewPreloader(FragmentActivity fragmentActivity, ListPreloader.IPreloadModelProvider preloadModelProvider, ListPreloader.IPreloadSizeProvider preloadDimensionProvider, int maxPreload) : this(Glide.With(fragmentActivity), preloadModelProvider, preloadDimensionProvider, maxPreload)
        {
        }

        /// <summary>
        /// Helper constructor that accepts an <seealso cref="Android.App.Fragment"/>. </summary>
        public RecyclerViewPreloader(Fragment fragment, ListPreloader.IPreloadModelProvider preloadModelProvider, ListPreloader.IPreloadSizeProvider preloadDimensionProvider, int maxPreload) : this(Glide.With(fragment), preloadModelProvider, preloadDimensionProvider, maxPreload)
        {
        }

        /// <summary>
        /// Constructor that accepts interfaces for providing the dimensions of images to preload, the list
        /// of models to preload for a given position, and the request to use to load images.
        /// </summary>
        /// <param name="preloadModelProvider"> Provides models to load and requests capable of loading them. </param>
        /// <param name="preloadDimensionProvider"> Provides the dimensions of images to load. </param>
        /// <param name="maxPreload"> Maximum number of items to preload. </param>
        public RecyclerViewPreloader(RequestManager requestManager, ListPreloader.IPreloadModelProvider preloadModelProvider, ListPreloader.IPreloadSizeProvider preloadDimensionProvider, int maxPreload)
        {

            var listPreloader = new ListPreloader(requestManager, preloadModelProvider, preloadDimensionProvider, maxPreload);
            recyclerScrollListener = new RecyclerToListViewScrollListener(listPreloader);
        }

        public override void OnScrolled(RecyclerView recyclerView, int dx, int dy)
        {
            recyclerScrollListener.OnScrolled(recyclerView, dx, dy);
        }
    }
}