using Android.Views;

namespace QuickDate.Library.Anjo.CardStackView
{
    public interface ICardStackListener
    {
        void OnCardDragging(SwipeDirection direction, float ratio);
        void OnCardSwiped(SwipeDirection direction);
        void OnCardRewound();
        void OnCardCanceled();
        void OnCardAppeared(View view, int position);
        void OnCardDisappeared(View view, int position);
    }

}