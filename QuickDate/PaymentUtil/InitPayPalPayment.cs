using Android.Gms.Wallet;
using AndroidX.Fragment.App;
using Com.Braintreepayments.Api;
using QuickDate.Activities.Tabbes;
using QuickDate.Helpers.Controller;
using QuickDate.Helpers.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Object = Java.Lang.Object;

namespace QuickDate.PaymentUtil
{
    public class InitPayPalPayment : Object, IDropInListener, IFetchMostRecentPaymentMethodCallback
    {
        private readonly FragmentActivity ActivityContext;
        private readonly HomeActivity GlobalContext;
        public string Price, PayType, Credits, Id;
        private DropInClient DropInClient;

        /// <summary>
        /// https://github.com/braintree/braintree_android/blob/master/v4_MIGRATION_GUIDE.md
        /// </summary>
        /// <param name="activity"></param>
        public InitPayPalPayment(FragmentActivity activity)
        {
            try
            {
                ActivityContext = activity;
                GlobalContext = HomeActivity.GetInstance();
                ConfigureDropInClient();
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        //Paypal
        public void BtnPaypalOnClick(string price, string payType, string credits, string id)
        {
            try
            {
                Price = price; PayType = payType; Credits = credits; Id = id;

                var dropInRequest = InitPayPal();
                if (dropInRequest == null)
                    return;

                DropInClient?.LaunchDropIn(dropInRequest);
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        private void ConfigureDropInClient()
        {
            try
            {
                // DropInClient can also be instantiated with a tokenization key
                switch (ListUtils.SettingsSiteList?.PaypalMode)
                {
                    case "sandbox":
                        DropInClient = new DropInClient(ActivityContext, AppSettings.SandboxTokenizationKey);
                        break;
                    case "live":
                        DropInClient = new DropInClient(ActivityContext, AppSettings.ProductionTokenizationKey);
                        break;
                    default:
                        DropInClient = new DropInClient(ActivityContext, AppSettings.SandboxTokenizationKey);
                        break;
                }
                // Make sure to register listener in onCreate
                DropInClient.SetListener(this);
                // DropInClient.FetchMostRecentPaymentMethod(ActivityContext, this);
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        private DropInRequest InitPayPal()
        {
            try
            {
                var option = ListUtils.MyUserInfo.FirstOrDefault();

                var currency = ListUtils.SettingsSiteList?.Currency ?? "USD";

                //GooglePay Request 
                var googlePayment = new GooglePayRequest();
                googlePayment.TransactionInfo = TransactionInfo.NewBuilder().SetTotalPrice(Price).SetCurrencyCode(currency).SetTotalPriceStatus(WalletConstants.TotalPriceStatusFinal).Build();
                googlePayment.PayPalEnabled = true;
                googlePayment.EmailRequired = true;
                googlePayment.BillingAddressRequired = true;

                ThreeDSecureAdditionalInformation additionalInformation = new ThreeDSecureAdditionalInformation();
                additionalInformation.AccountId = "account-id";

                //3D Secure
                ThreeDSecurePostalAddress billingAddress = new ThreeDSecurePostalAddress();
                billingAddress.GivenName = "Jill";
                billingAddress.Surname = "Doe";
                billingAddress.PhoneNumber = "5551234567";
                billingAddress.StreetAddress = "555 Smith St";
                billingAddress.ExtendedAddress = "#2";
                billingAddress.Locality = "Chicago";
                billingAddress.Region = "IL";
                billingAddress.PostalCode = "12345";
                billingAddress.CountryCodeAlpha2 = "US";

                ThreeDSecureRequest threeDSecureRequest = new ThreeDSecureRequest();
                threeDSecureRequest.Amount = Price;
                threeDSecureRequest.VersionRequested = ThreeDSecureRequest.Version2;

                if (option != null)
                {
                    threeDSecureRequest.Email = option.Email;
                    threeDSecureRequest.MobilePhoneNumber = option.PhoneNumber;
                }

                threeDSecureRequest.BillingAddress = billingAddress;
                threeDSecureRequest.AdditionalInformation = additionalInformation;
                threeDSecureRequest.ShippingMethod = IThreeDSecureShippingMethod.Ground;

                //PayPal Request
                PayPalCheckoutRequest paypalRequest = new PayPalCheckoutRequest(Price);
                paypalRequest.CurrencyCode = currency;
                paypalRequest.MerchantAccountId = AppSettings.MerchantAccountId;
                paypalRequest.DisplayName = AppSettings.ApplicationName;
                paypalRequest.BillingAgreementDescription = "Pay the card";
                //paypalRequest.LandingPageType = ("billing");
                paypalRequest.Intent = IPayPalPaymentIntent.Authorize;

                DropInRequest dropInRequest = new DropInRequest();

                //dropInRequest.RequestThreeDSecureVerification = (true);
                //dropInRequest.CollectDeviceData = (true);
                dropInRequest.GooglePayRequest = googlePayment;
                dropInRequest.PayPalRequest = paypalRequest;
                dropInRequest.MaskCardNumber = true;
                dropInRequest.MaskSecurityCode = true;
                dropInRequest.AllowVaultCardOverride = false;
                dropInRequest.VaultCardDefaultValue = true;
                dropInRequest.VaultManagerEnabled = false;
                dropInRequest.ThreeDSecureRequest = threeDSecureRequest;

                return dropInRequest;
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
                return null;
            }
        }

        /// <summary>
        /// handle error
        /// </summary>
        /// <param name="error"></param>
        public void OnDropInFailure(Java.Lang.Exception error)
        {
            try
            {

            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        /// <summary>
        /// use the result to update your UI and send the payment method nonce to your server
        /// </summary>
        /// <param name="result"></param>
        public void OnDropInSuccess(DropInResult result)
        {
            try
            {
                Console.WriteLine("Device Data :" + result.DeviceData);
                if (result.PaymentMethodType != null || result.PaymentMethodType == DropInPaymentMethod.GooglePay || result.PaymentMethodType == DropInPaymentMethod.Paypal)
                {
                    // google pay doesn't have a payment method nonce to display; fallback to OG ui

                    if (PayType == "membership")
                    {
                        PollyController.RunRetryPolicyFunction(new List<Func<Task>> { () => GlobalContext?.SetPro("PayPal") });
                    }
                    else
                    {
                        PollyController.RunRetryPolicyFunction(new List<Func<Task>> { () => GlobalContext?.SetCredit("PayPal") });
                    }
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public void OnResult(DropInResult result, Java.Lang.Exception error)
        {
            try
            {
                Console.WriteLine("Device Data :" + result.DeviceData);
                if (result.PaymentMethodType != null || result.PaymentMethodType == DropInPaymentMethod.GooglePay || result.PaymentMethodType == DropInPaymentMethod.Paypal)
                {
                    // google pay doesn't have a payment method nonce to display; fallback to OG ui

                    if (PayType == "membership")
                    {
                        PollyController.RunRetryPolicyFunction(new List<Func<Task>> { () => GlobalContext?.SetPro("PayPal") });
                    }
                    else
                    {
                        PollyController.RunRetryPolicyFunction(new List<Func<Task>> { () => GlobalContext?.SetCredit("PayPal") });
                    }
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public static void DisplayResult(DropInResult PaymentMethodType)
        {
            try
            {
                var mNonce = PaymentMethodType.PaymentMethodNonce;
                var mPaymentMethodType = PaymentMethodType.PaymentMethodType;

                var mPaymentMethodIcon = mPaymentMethodType.Drawable;


                string details = "";
                //if (mNonce is CardNonce cardNonce)
                //{
                //    details = "Card Last Two: " + cardNonce.LastTwo + "\n";
                //    details += "3DS isLiabilityShifted: " + cardNonce.ThreeDSecureInfo.IsLiabilityShifted + "\n";
                //    details += "3DS isLiabilityShiftPossible: " + cardNonce.ThreeDSecureInfo.IsLiabilityShiftPossible;
                //}
                //else
                if (mNonce is PayPalAccountNonce paypalAccountNonce)
                {
                    details = "First name: " + paypalAccountNonce.FirstName + "\n";
                    details += "Last name: " + paypalAccountNonce.LastName + "\n";
                    details += "Email: " + paypalAccountNonce.Email + "\n";
                    details += "Phone: " + paypalAccountNonce.Phone + "\n";
                    details += "Payer id: " + paypalAccountNonce.PayerId + "\n";
                    details += "Client metadata id: " + paypalAccountNonce.ClientMetadataId + "\n";
                    details += "Billing address: " + paypalAccountNonce.BillingAddress + "\n";
                    details += "Shipping address: " + paypalAccountNonce.ShippingAddress;
                }
                else if (mNonce is VenmoAccountNonce venmoAccountNonce)
                {
                    details = "Username: " + venmoAccountNonce.Username;
                }
                else if (mNonce is GooglePayCardNonce googlePaymentCardNonce)
                {
                    details = "Underlying Card Last Two: " + googlePaymentCardNonce.LastTwo + "\n";
                    details += "Email: " + googlePaymentCardNonce.Email + "\n";
                    details += "Billing address: " + googlePaymentCardNonce.BillingAddress + "\n";
                    details += "Shipping address: " + googlePaymentCardNonce.ShippingAddress;
                }

                Console.WriteLine(details);
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

    }
}