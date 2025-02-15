using Android.BillingClient.Api;
using System.Collections.Generic;

namespace QuickDate.PaymentGoogle
{
    public static class InAppBillingGoogle
    {
        public const string BagOfCredits = "bag.of.credits";
        public const string BoxofCredits = "box.of.credits";
        public const string ChestofCredits = "chest.of.credits";
        public const string MembershipWeekly = "membershipweekly";
        public const string MembershipMonthly = "membershipmonthly";
        public const string MembershipYearly = "membershipyearly";
        public const string MembershipLifetime = "membership.lifetime";

        public static readonly List<QueryProductDetailsParams.Product> ListProductSku = new List<QueryProductDetailsParams.Product> // ID Product
        {
            //All products should be of the same product type.
            QueryProductDetailsParams.Product.NewBuilder().SetProductId(BagOfCredits).SetProductType(BillingClient.IProductType.Subs).Build(),
            QueryProductDetailsParams.Product.NewBuilder().SetProductId(BoxofCredits).SetProductType(BillingClient.IProductType.Subs).Build(),
            QueryProductDetailsParams.Product.NewBuilder().SetProductId(ChestofCredits).SetProductType(BillingClient.IProductType.Subs).Build(),
            QueryProductDetailsParams.Product.NewBuilder().SetProductId(MembershipWeekly).SetProductType(BillingClient.IProductType.Subs).Build(),
            QueryProductDetailsParams.Product.NewBuilder().SetProductId(MembershipMonthly).SetProductType(BillingClient.IProductType.Subs).Build(),
            QueryProductDetailsParams.Product.NewBuilder().SetProductId(MembershipYearly).SetProductType(BillingClient.IProductType.Subs).Build(),
            QueryProductDetailsParams.Product.NewBuilder().SetProductId(MembershipLifetime).SetProductType(BillingClient.IProductType.Subs).Build(),
        };
    }
}