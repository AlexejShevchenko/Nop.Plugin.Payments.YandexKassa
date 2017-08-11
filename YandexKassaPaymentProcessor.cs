using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Routing;
using Nop.Core;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Orders;
using Nop.Core.Plugins;
using Nop.Plugin.Payments.YandexKassa.Controllers;
using Nop.Services.Configuration;
using Nop.Services.Directory;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Tax;
using Nop.Services.Customers;
using System.Globalization;
using Nop.Core.Domain.Payments;
using Nop.Web.Framework;
using Nop.Services.Localization;

namespace Nop.Plugin.Payments.YandexKassa
{
    public class YandexKassaPaymentProcessor : BasePlugin, IPaymentMethod
    {

        #region Fields

        private readonly YandexKassaPaymentSettings _yandexKassaSettings;
        private readonly ISettingService _settingService;
        private readonly ICurrencyService _currencyService;
        private readonly ICustomerService _customerService;
        private readonly IWebHelper _webHelper;
        private readonly ICheckoutAttributeParser _checkoutAttributeParser;
        private readonly ITaxService _taxService;
        private readonly IOrderTotalCalculationService _orderTotalCalculationService;
        private readonly HttpContextBase _httpContext;
        private readonly ILocalizationService _localizationService;
        private readonly IWorkContext _workContext;
        public PaymentMethodType PaymentMethodType {get{return PaymentMethodType.Redirection;}}
        public RecurringPaymentType RecurringPaymentType { get { return RecurringPaymentType.NotSupported; } }

        #endregion

        #region Ctor

        public YandexKassaPaymentProcessor(YandexKassaPaymentSettings yandexKassaPaymentSettings,
            ISettingService settingService, ICurrencyService currencyService, ICustomerService customerService, IWebHelper webHelper,
            ICheckoutAttributeParser checkoutAttributeParser, ITaxService taxService, 
            IOrderTotalCalculationService orderTotalCalculationService, HttpContextBase httpContext,
            ILocalizationService localizationService,
             IWorkContext workContext)
        {
            this._yandexKassaSettings = yandexKassaPaymentSettings;
            this._settingService = settingService;
            this._currencyService = currencyService;
            this._customerService = customerService;
            this._webHelper = webHelper;
            this._checkoutAttributeParser = checkoutAttributeParser;
            this._taxService = taxService;
            this._orderTotalCalculationService = orderTotalCalculationService;
            this._httpContext = httpContext;
            this._localizationService = localizationService;
            this._workContext = workContext;
        }

        #endregion

        #region Methods

        public bool SkipPaymentInfo {
            get {
                return _yandexKassaSettings.YandexPaymentSide == YandexPaymentSide.YANDEX_SIDE;
            }
        }
        public bool SupportCapture {get{return false;}}
        public bool SupportPartiallyRefund {get{return false;}}
        public bool SupportRefund {get{return false;}}
        public bool SupportVoid {get{return false;}}

        /// <summary>
        /// Gets a payment method description that will be displayed on checkout pages in the public store
        /// </summary>
        public string PaymentMethodDescription
        {
            //return description of this payment method to be display on "payment method" checkout step. good practice is to make it localizable
            //for example, for a redirection payment method, description may be like this: "You will be redirected to PayPal site to complete the payment"
            //get { return _localizationService.GetResource("Plugins.Payments.YandexKassa.PaymentMethodDescription"); }
            get
            {
                return @"<div id='PaymentYandexKassaPaymentMethodDescription' class='hidden paymentMethod-description''></div>
<link href='/Plugins/Payments.YandexKassa/Content/styles.css' rel='stylesheet' type='text/css' />
<div class='floatBarsG-wrapper'>
    <div class='floatBarsG floatBarsG_1'></div>
    <div class='floatBarsG floatBarsG_2'></div>
    <div class='floatBarsG floatBarsG_3'></div>
    <div class='floatBarsG floatBarsG_4'></div>
    <div class='floatBarsG floatBarsG_5'></div>
    <div class='floatBarsG floatBarsG_6'></div>
    <div class='floatBarsG floatBarsG_7'></div>
    <div class='floatBarsG floatBarsG_8'></div>
</div>
<script>
	$(document).ready(function () {
        $.ajax({
            cache: false,
            type: 'GET',
            url: '/Plugins/PaymentYandexKassa/PaymentMethodDescription',
            success: function (data) {
                $('#PaymentYandexKassaPaymentMethodDescription').removeClass('hidden').html(data);
                $('#PaymentYandexKassaPaymentMethodDescription').siblings('.floatBarsG-wrapper').addClass('hidden');
            }
        });
    });
</script>";
            }
        }


        public CancelRecurringPaymentResult CancelRecurringPayment (CancelRecurringPaymentRequest cancelPaymentRequest){return new CancelRecurringPaymentResult ();}
        public bool CanRePostProcessPayment (Order order) {
            return true;
        }
        public CapturePaymentResult Capture (CapturePaymentRequest capturePaymentRequest) {
            return new CapturePaymentResult();}
        public decimal GetAdditionalHandlingFee (IList<ShoppingCartItem> cart) {
            var result = 0; return result;
        }
        public bool HidePaymentMethod(IList<ShoppingCartItem> cart) {
            return false;
        }
        public ProcessPaymentResult ProcessRecurringPayment(ProcessPaymentRequest processPaymentRequest) {
            return new ProcessPaymentResult();
        }
        public RefundPaymentResult Refund(RefundPaymentRequest refundPaymentRequest) { return new RefundPaymentResult(); }
        public VoidPaymentResult Void(VoidPaymentRequest voidPaymentRequest) {
            return new VoidPaymentResult();
        }
        public Type GetControllerType () {
            return typeof(PaymentYandexKassaController);
        }

        // Routes
        public void GetPaymentInfoRoute (out string actionName, out string controllerName, out RouteValueDictionary routeValues) 
        {
            actionName = "PaymentInfo";
            controllerName = "PaymentYandexKassa";
            routeValues = new RouteValueDictionary { { "Namespaces", "Nop.Plugin.Payments.YandexKassa.Controllers" }, { "area", null } };
        }
        public void GetConfigurationRoute(out string actionName, out string controllerName, out RouteValueDictionary routeValues)
        {
            actionName = "Configure";
            controllerName = "PaymentYandexKassa";
            routeValues = new RouteValueDictionary { { "Namespaces", "Nop.Plugin.Payments.YandexKassa.Controllers" }, { "area", null } };
        }

        public void PostProcessPayment(PostProcessPaymentRequest postProcessPaymentRequest)
        {
            var customer = _customerService.GetCustomerById(postProcessPaymentRequest.Order.CustomerId);

            string paymentType = "";
            HttpCookie yandexPaymentType = _httpContext.Request.Cookies.Get("yandex_payment_type");
            if (_yandexKassaSettings.YandexPaymentSide == YandexPaymentSide.SHOP_SIDE && yandexPaymentType != null)
            {
                paymentType = yandexPaymentType.Value;
                yandexPaymentType.Expires = DateTime.Now.AddDays(-1);
                _httpContext.Response.Cookies.Add(yandexPaymentType);
            }

            var post = new RemotePost();

            post.FormName = "YandexKassaPaymentForm";
            post.Url = GetYandexKassaUrl();
            post.Method = "POST";

            post.Add("shopId", _yandexKassaSettings.ShopId.ToString());
            post.Add("scid", _yandexKassaSettings.Scid.ToString());
            post.Add("sum", String.Format(CultureInfo.InvariantCulture, "{0:0.00}", Math.Round(_currencyService.ConvertFromPrimaryStoreCurrency(postProcessPaymentRequest.Order.OrderTotal, _currencyService.GetCurrencyByCode("RUB")/* _workContext.WorkingCurrency*/),2)));
            post.Add("customerNumber", postProcessPaymentRequest.Order.BillingAddress.Email);
            post.Add("orderNumber", postProcessPaymentRequest.Order.Id.ToString());

            post.Add("shopSuccessURL", String.Format("{0}Checkout/Completed/{1}", _webHelper.GetStoreLocation(false), postProcessPaymentRequest.Order.Id));
            post.Add("shopFailURL", String.Format("{0}Checkout/Completed/{1}", _webHelper.GetStoreLocation(false), postProcessPaymentRequest.Order.Id));
            post.Add("shopDefaultUrl", String.Format("{0}", _webHelper.GetStoreLocation(false)));
            post.Add("cps_email", customer.Email);
            post.Add("cps_phone", customer.BillingAddress.PhoneNumber);
            post.Add("paymentType", paymentType);
            /*
            post.Add("custName", postProcessPaymentRequest.Order.BillingAddress.LastName + postProcessPaymentRequest.Order.BillingAddress.FirstName);
            post.Add("custAddr", customer.BillingAddress.Address1);
            post.Add("custEMail", postProcessPaymentRequest.Order.BillingAddress.Email);
            post.Add("orderDetails", "without");
            */
            post.Post();

        }

        public ProcessPaymentResult ProcessPayment(ProcessPaymentRequest processPaymentRequest)
        {
            var result = new ProcessPaymentResult();
            //result.AllowStoringCreditCardNumber = true;
            //result.SubscriptionTransactionId = processPaymentRequest.CustomValues["yandexKassaMode"].ToString();
            result.NewPaymentStatus = PaymentStatus.Pending;
            return result;
        }

        #endregion

        #region Install & Uninstall

        public override void Install()
        {
            var settings = new YandexKassaPaymentSettings()
            {
                IsDemo = true,
                ShopId = 0,
                Scid = 0,
                ShopPassword = "",
                YandexPaymentSide = YandexPaymentSide.YANDEX_SIDE
            };
            _settingService.SaveSetting(settings);

            //locales
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.YandexKassa.YaParameters", "Параметры для личного кабинета Яндекс.Кассы");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.YandexKassa.YaPaymentSettings", "Настройки приема платежей");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.YandexKassa.yapaymentscenario", "Сценарий");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.YandexKassa.ShopSide", "Выбор способа оплаты на стороне магазина");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.YandexKassa.YandexSide", "Выбор способа оплаты на стороне Яндекс.Кассы");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.YandexKassa.PaymentType", "Способы оплаты");
            


            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.YandexKassa.Fields.IsDemo", "Использовать для тестирования");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.YandexKassa.Fields.ShopId", "Shop ID");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.YandexKassa.Fields.ShopId.Hint", "Введите свой shop ID.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.YandexKassa.Fields.Scid", "Scid витрины магазина");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.YandexKassa.Fields.Scid.Hint", "Для реальных платежей");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.YandexKassa.Fields.ShopPassword", "Пароль магазина");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.YandexKassa.Fields.ShopPassword.Hint", "Введите пароль магазина");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.YandexKassa.Fields.AboutYandexKassa", " Яндекс.Касса - это инструмент для приема платежей. Все популярные способы оплаты");

            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.YandexKassa.Fields.YandexPayentSide", "Способ выбора платежа");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.YandexKassa.Fields.YandexPayentSide.Hint", "Выберете способ выбора платежа");

            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.YandexKassa.PaymentMethodDescription", "Оплата через Yandex Kassa");


            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.YandexKassa.Fields.PC", "Яндекс.Деньги");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.YandexKassa.Fields.AC", "Банковские карты");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.YandexKassa.Fields.MC", "Баланс телефона");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.YandexKassa.Fields.GP", "Наличные");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.YandexKassa.Fields.EP", "ЕРИП (Беларусь)");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.YandexKassa.Fields.WM", "WebMoney");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.YandexKassa.Fields.SB", "Сбербанк Онлайн");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.YandexKassa.Fields.MP", "Мобильный терминал (mPOS)");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.YandexKassa.Fields.AB", "Альфа-Клик");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.YandexKassa.Fields.MA", "MasterPass");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.YandexKassa.Fields.PB", "Интернет-банк Промсвязьбанка");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.YandexKassa.Fields.QW", "QIWI Wallet");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.YandexKassa.Fields.KV", "КупиВкредит");
            


            base.Install();
        }

        public override void Uninstall()
        {
            //settings
            _settingService.DeleteSetting<YandexKassaPaymentSettings>();
            //locales
            this.DeletePluginLocaleResource("Plugins.Payments.YandexKassa.Fields.IsDemo");
            this.DeletePluginLocaleResource("Plugins.Payments.YandexKassa.Fields.ShopId");
            this.DeletePluginLocaleResource("Plugins.Payments.YandexKassa.Fields.ShopId.Hint");
            this.DeletePluginLocaleResource("Plugins.Payments.YandexKassa.Fields.Scid");
            this.DeletePluginLocaleResource("Plugins.Payments.YandexKassa.Fields.Scid.Hint");
            this.DeletePluginLocaleResource("Plugins.Payments.YandexKassa.Fields.ShopPassword");
            this.DeletePluginLocaleResource("Plugins.Payments.YandexKassa.Fields.ShopPassword.Hint");
            this.DeletePluginLocaleResource("Plugins.Payments.YandexKassa.Fields.AboutYandexKassa");

            this.DeletePluginLocaleResource("Plugins.Payments.YandexKassa.Fields.YandexPayentSide");
            this.DeletePluginLocaleResource("Plugins.Payments.YandexKassa.Fields.YandexPayentSide.Hint");

            this.DeletePluginLocaleResource("Plugins.Payments.YandexKassa.Fields.PC");
            this.DeletePluginLocaleResource("Plugins.Payments.YandexKassa.Fields.AC");
            this.DeletePluginLocaleResource("Plugins.Payments.YandexKassa.Fields.MC");
            this.DeletePluginLocaleResource("Plugins.Payments.YandexKassa.Fields.GP");
            this.DeletePluginLocaleResource("Plugins.Payments.YandexKassa.Fields.WM");
            this.DeletePluginLocaleResource("Plugins.Payments.YandexKassa.Fields.SB");
            this.DeletePluginLocaleResource("Plugins.Payments.YandexKassa.Fields.MP");
            this.DeletePluginLocaleResource("Plugins.Payments.YandexKassa.Fields.AB");
            this.DeletePluginLocaleResource("Plugins.Payments.YandexKassa.Fields.MA");
            this.DeletePluginLocaleResource("Plugins.Payments.YandexKassa.Fields.PB");
            this.DeletePluginLocaleResource("Plugins.Payments.YandexKassa.Fields.QW");
            this.DeletePluginLocaleResource("Plugins.Payments.YandexKassa.Fields.KV");

            base.Uninstall();
        }

        #endregion

        #region Utilities

        public string GetYandexKassaUrl()
        {
            return _yandexKassaSettings.IsDemo ?
                "https://demomoney.yandex.ru/eshop.xml" :
                "https://money.yandex.ru/eshop.xml";
        }

        #endregion
    }
}
