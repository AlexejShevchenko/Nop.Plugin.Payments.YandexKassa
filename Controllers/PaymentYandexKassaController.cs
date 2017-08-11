using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Nop.Services.Payments;
using Nop.Core;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Stores;
using Nop.Web.Framework.Controllers;
using Nop.Plugin.Payments.YandexKassa.Models;
using Nop.Core.Domain.Payments;
using Nop.Services.Orders;
using Nop.Core.Domain.Orders;
using System.Text;
using Nop.Plugin.Payments.YandexKassa.Utils;
using System.Xml.Serialization;
using System.IO;
using System.Xml;
using Nop.Services.Directory;
using System.Xml.Linq;
using Nop.Services.Logging;
using System.Globalization;

namespace Nop.Plugin.Payments.YandexKassa.Controllers
{
    public class PaymentYandexKassaController : BasePaymentController
    {
        private readonly IWorkContext _workContext;
        private readonly IStoreService _storeService;
        private readonly ISettingService _settingService;
        private readonly ILocalizationService _localizationService;
        private readonly PaymentSettings _paymentSettings;
        private readonly IPaymentService _paymentService;
        private readonly IOrderService _orderService;
        private readonly IOrderProcessingService _orderProcessingService;
        private readonly IWebHelper _webHelper;
        private readonly YandexKassaPaymentSettings _yandexKassaSettings;
        private readonly ICurrencyService _currencyService;
        private readonly ILogger _logger;

        public PaymentYandexKassaController(IWorkContext workContext,
            PaymentSettings paymentSettings, IPaymentService paymentService,
            IStoreService storeService,
            ISettingService settingService,
            ILocalizationService localizationService,
            IOrderService orderService,
            IOrderProcessingService orderProcessingService,
            IWebHelper webHelper,
            YandexKassaPaymentSettings yandexKassaSettings,
            ICurrencyService currencyService,
            ILogger logger)
        {
            this._workContext = workContext;
            this._paymentSettings = paymentSettings;
            this._paymentService = paymentService;
            this._storeService = storeService;
            this._settingService = settingService;
            this._localizationService = localizationService;
            this._orderService = orderService;
            this._orderProcessingService = orderProcessingService;
            this._webHelper = webHelper;
            this._yandexKassaSettings = yandexKassaSettings;
            this._currencyService = currencyService;
            this._logger = logger;
        }

        [AdminAuthorize]
        [ChildActionOnly]
        public ActionResult Configure()
        {
            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var yandexKassaPaymentSettings = _settingService.LoadSetting<YandexKassaPaymentSettings>(storeScope);

            var model = new ConfigurationModel();
            model.IsDemo = Convert.ToBoolean(yandexKassaPaymentSettings.IsDemo);
            model.ShopId = Convert.ToString(yandexKassaPaymentSettings.ShopId);
            model.Scid = Convert.ToString(yandexKassaPaymentSettings.Scid);
            model.ShopPassword = yandexKassaPaymentSettings.ShopPassword;

            model.YandexPaymentSide = yandexKassaPaymentSettings.YandexPaymentSide;

            model.AB = yandexKassaPaymentSettings.AB;
            model.AC = yandexKassaPaymentSettings.AC;
            model.EP = yandexKassaPaymentSettings.EP;
            model.GP = yandexKassaPaymentSettings.GP;
            model.KV = yandexKassaPaymentSettings.KV;
            model.MA = yandexKassaPaymentSettings.MA;
            model.MC = yandexKassaPaymentSettings.MC;
            model.MP = yandexKassaPaymentSettings.MP;
            model.PB = yandexKassaPaymentSettings.PB;
            model.PC = yandexKassaPaymentSettings.PC;
            model.QW = yandexKassaPaymentSettings.QW;
            model.SB = yandexKassaPaymentSettings.SB;
            model.WM = yandexKassaPaymentSettings.WM;

            model.ActiveStoreScopeConfiguration = storeScope;
            if (storeScope > 0)
            {
                model.IsDemo_OverrideForStore = _settingService.SettingExists(yandexKassaPaymentSettings, x => x.IsDemo, storeScope);
                model.ShopId_OverrideForStore = _settingService.SettingExists(yandexKassaPaymentSettings, x => x.ShopId, storeScope);
                model.Scid_OverrideForStore = _settingService.SettingExists(yandexKassaPaymentSettings, x => x.Scid, storeScope);
                model.ShopPassword_OverrideForStore = _settingService.SettingExists(yandexKassaPaymentSettings, x => x.ShopPassword, storeScope);

                model.YandexPaymentSide_OverrideForStore = _settingService.SettingExists(yandexKassaPaymentSettings, x => x.YandexPaymentSide, storeScope);

                model.AB_OverrideForStore = _settingService.SettingExists(yandexKassaPaymentSettings, x => x.AB, storeScope);
                model.AC_OverrideForStore = _settingService.SettingExists(yandexKassaPaymentSettings, x => x.AC, storeScope);
                model.EP_OverrideForStore = _settingService.SettingExists(yandexKassaPaymentSettings, x => x.EP, storeScope);
                model.GP_OverrideForStore = _settingService.SettingExists(yandexKassaPaymentSettings, x => x.GP, storeScope);
                model.KV_OverrideForStore = _settingService.SettingExists(yandexKassaPaymentSettings, x => x.KV, storeScope);
                model.MA_OverrideForStore = _settingService.SettingExists(yandexKassaPaymentSettings, x => x.MA, storeScope);
                model.MC_OverrideForStore = _settingService.SettingExists(yandexKassaPaymentSettings, x => x.MC, storeScope);
                model.MP_OverrideForStore = _settingService.SettingExists(yandexKassaPaymentSettings, x => x.MP, storeScope);
                model.PB_OverrideForStore = _settingService.SettingExists(yandexKassaPaymentSettings, x => x.PB, storeScope);
                model.PC_OverrideForStore = _settingService.SettingExists(yandexKassaPaymentSettings, x => x.PC, storeScope);
                model.QW_OverrideForStore = _settingService.SettingExists(yandexKassaPaymentSettings, x => x.QW, storeScope);
                model.SB_OverrideForStore = _settingService.SettingExists(yandexKassaPaymentSettings, x => x.SB, storeScope);
                model.WM_OverrideForStore = _settingService.SettingExists(yandexKassaPaymentSettings, x => x.WM, storeScope);
            }

            ViewBag.Host = _webHelper.GetStoreLocation(true);

            return View("~/Plugins/Payments.YandexKassa/Views/PaymentYandexKassa/Configure.cshtml", model);
        }

        [HttpPost]
        [AdminAuthorize]
        [ChildActionOnly]
        public ActionResult Configure(ConfigurationModel model)
        {
            if (!ModelState.IsValid)
                return Configure();

            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var yandexKassaPaymentSettings = _settingService.LoadSetting<YandexKassaPaymentSettings>(storeScope);

            yandexKassaPaymentSettings.IsDemo = model.IsDemo;
            yandexKassaPaymentSettings.ShopId = Convert.ToInt32(model.ShopId);
            yandexKassaPaymentSettings.Scid = Convert.ToInt32(model.Scid);
            yandexKassaPaymentSettings.ShopPassword = model.ShopPassword;

            yandexKassaPaymentSettings.YandexPaymentSide = model.YandexPaymentSide;

            yandexKassaPaymentSettings.AB = model.AB;
            yandexKassaPaymentSettings.AC = model.AC;
            yandexKassaPaymentSettings.EP = model.EP;
            yandexKassaPaymentSettings.GP = model.GP;
            yandexKassaPaymentSettings.KV = model.KV;
            yandexKassaPaymentSettings.MA = model.MA;
            yandexKassaPaymentSettings.MC = model.MC;
            yandexKassaPaymentSettings.MP = model.MP;
            yandexKassaPaymentSettings.PB = model.PB;
            yandexKassaPaymentSettings.PC = model.PC;
            yandexKassaPaymentSettings.QW = model.QW;
            yandexKassaPaymentSettings.SB = model.SB;
            yandexKassaPaymentSettings.WM = model.WM;

            if (model.IsDemo_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(yandexKassaPaymentSettings, x => x.IsDemo, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(yandexKassaPaymentSettings, x => x.IsDemo, storeScope);

            if (model.ShopId_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(yandexKassaPaymentSettings, x => x.ShopId, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(yandexKassaPaymentSettings, x => x.ShopId, storeScope);

            if (model.Scid_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(yandexKassaPaymentSettings, x => x.Scid, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(yandexKassaPaymentSettings, x => x.Scid, storeScope);

            if (model.ShopPassword_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(yandexKassaPaymentSettings, x => x.ShopPassword, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(yandexKassaPaymentSettings, x => x.ShopPassword, storeScope);

            
            if (model.YandexPaymentSide_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(yandexKassaPaymentSettings, x => x.YandexPaymentSide, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(yandexKassaPaymentSettings, x => x.YandexPaymentSide, storeScope);


            if (model.AB_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(yandexKassaPaymentSettings, x => x.AB, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(yandexKassaPaymentSettings, x => x.AB, storeScope);

            if (model.AC_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(yandexKassaPaymentSettings, x => x.AC, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(yandexKassaPaymentSettings, x => x.AC, storeScope);

            if (model.EP_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(yandexKassaPaymentSettings, x => x.EP, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(yandexKassaPaymentSettings, x => x.EP, storeScope);

            if (model.GP_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(yandexKassaPaymentSettings, x => x.GP, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(yandexKassaPaymentSettings, x => x.GP, storeScope);

            if (model.KV_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(yandexKassaPaymentSettings, x => x.KV, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(yandexKassaPaymentSettings, x => x.KV, storeScope);

            if (model.MA_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(yandexKassaPaymentSettings, x => x.MA, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(yandexKassaPaymentSettings, x => x.MA, storeScope);

            if (model.MC_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(yandexKassaPaymentSettings, x => x.MC, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(yandexKassaPaymentSettings, x => x.MC, storeScope);

            if (model.MP_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(yandexKassaPaymentSettings, x => x.MP, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(yandexKassaPaymentSettings, x => x.MP, storeScope);

            if (model.PB_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(yandexKassaPaymentSettings, x => x.PB, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(yandexKassaPaymentSettings, x => x.PB, storeScope);

            if (model.PC_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(yandexKassaPaymentSettings, x => x.PC, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(yandexKassaPaymentSettings, x => x.PC, storeScope);

            if (model.QW_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(yandexKassaPaymentSettings, x => x.QW, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(yandexKassaPaymentSettings, x => x.QW, storeScope);

            if (model.SB_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(yandexKassaPaymentSettings, x => x.SB, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(yandexKassaPaymentSettings, x => x.SB, storeScope);

            if (model.WM_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(yandexKassaPaymentSettings, x => x.WM, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(yandexKassaPaymentSettings, x => x.WM, storeScope);

            // now clear settings cache
            _settingService.ClearCache();

            SuccessNotification(_localizationService.GetResource("Admin.Plugins.Saved"));

            return Configure();
        }

        [ChildActionOnly]
        public ActionResult PaymentInfo()
        {
            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var yandexKassaPaymentSettings = _settingService.LoadSetting<YandexKassaPaymentSettings>(storeScope);

            return View("~/Plugins/Payments.YandexKassa/Views/PaymentyandexKassa/PaymentInfo.cshtml", yandexKassaPaymentSettings);
        }

        public override ProcessPaymentRequest GetPaymentInfo(FormCollection form)
        {
            var paymentInfo = new ProcessPaymentRequest();
            return paymentInfo;
        }

        [ValidateInput(false)]
        [AllowAnonymous]
        public ActionResult Return(FormCollection form)
        {
            var processor = _paymentService.LoadPaymentMethodBySystemName("Payments.YandexKassa") as YandexKassaPaymentProcessor;
            if (processor == null ||
                !processor.IsPaymentMethodActive(_paymentSettings) || !processor.PluginDescriptor.Installed)
                throw new NopException("Модуль Яндекс.Кассы не может быть загружен");

            string[] orderStringArr = Request.QueryString["orderNumber"].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            int orderId;
            if (orderStringArr.Length > 1)
                orderId = Convert.ToInt32(orderStringArr[0]);
            else
                orderId = _webHelper.QueryString<int>("orderNumber");

            var order = _orderService.GetOrderById(orderId);
            if (order == null || order.Deleted || _workContext.CurrentCustomer.Id != order.CustomerId)
                return new HttpUnauthorizedResult();

            return RedirectToRoute("CheckoutCompleted", new { orderId = order.Id });
        }

        [ValidateInput(false)]
        [AllowAnonymous]
        public ActionResult PaymentMethodDescription()
        {
            return View("~/Plugins/Payments.YandexKassa/Views/PaymentyandexKassa/PaymentMethodDescription.cshtml");
        }

        [NonAction]
        public override IList<string> ValidatePaymentForm(FormCollection form)
        {
            var warnings = new List<string>();
            return warnings;
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateInput(false)]
        [RequireHttps]
        public ActionResult CallBack(YandexRequestModel model)
        {

            YandexResponceModel responce = _CheckRequest(model, model.action);

            string responseType = "";
            switch (this.Request.Form["action"])
            {
                case "checkOrder":
                    responseType = "checkOrderResponse";
                    break;
                case "paymentAviso":
                    responseType = "paymentAvisoResponse";
                    if (responce.code == 0)
                    {
                        try
                        {
                            Order order = _orderService.GetOrderById(Convert.ToInt32(model.orderNumber));

                            if (order != null && _orderProcessingService.CanMarkOrderAsPaid(order))
                            {

                                order.AuthorizationTransactionId = model.invoiceId.ToString();
                                _orderService.UpdateOrder(order);

                                _orderProcessingService.MarkOrderAsPaid(order);

                                responce.code = 0;
                            }
                            else
                            {
                                responce.code = 200;
                            }
                        }
                        catch
                        {
                            responce.code = 200;
                        }
                    }
                    break;
            }

            return new XmlActionResult(MakeXml(responseType, responce));
        }
        
        private YandexResponceModel _CheckRequest(YandexRequestModel model, string action)
        {
            YandexResponceModel responce = new YandexResponceModel()
            {
                code = 200,
                performedDatetime = DateTime.Now.ToString("O"),
                shopId = model.shopId,
                invoiceId = model.invoiceId
            };
            
            try
            {
                responce.orderSumAmount = decimal.Parse(model.orderSumAmount, CultureInfo.InvariantCulture);

                Order order = _orderService.GetOrderById(Convert.ToInt32(model.orderNumber));
                if (order == null)
                {
                    responce.code = 100;
                    responce.message = "Ордер не найден";
                }
                else if(order.PaymentStatus == PaymentStatus.Pending)
                {
                    var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
                    var yandexKassaPaymentSettings = _settingService.LoadSetting<YandexKassaPaymentSettings>(storeScope);

                    if (!_VerifyMd5(action, model, order, yandexKassaPaymentSettings))
                    {
                        responce.code = 1;
                    }
                    else
                    {
                        if (yandexKassaPaymentSettings.ShopId != model.shopId)
                            throw new Exception();
                        if (Math.Abs(_currencyService.ConvertFromPrimaryStoreCurrency(order.OrderTotal, _workContext.WorkingCurrency) - responce.orderSumAmount) > 0.01M)
                            throw new Exception();

                        responce.code = 0;
                    }
                }
            }
            catch
            {
                responce.code = 200;
            }

            return responce;
        }

        private bool _VerifyMd5(string action, YandexRequestModel model, Order order, YandexKassaPaymentSettings settings)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("{0};", action.Trim());
            sb.AppendFormat("{0};", Math.Round(_currencyService.ConvertFromPrimaryStoreCurrency(order.OrderTotal, _workContext.WorkingCurrency), 2).ToString(CultureInfo.InvariantCulture).Trim());
            sb.AppendFormat("{0};", model.orderSumCurrencyPaycash.Trim());
            sb.AppendFormat("{0};", model.orderSumBankPaycash.Trim());
            sb.AppendFormat("{0};", settings.ShopId.ToString().Trim());
            sb.AppendFormat("{0};", model.invoiceId.ToString().Trim());
            sb.AppendFormat("{0};", order.BillingAddress.Email.Trim());
            sb.AppendFormat("{0}", settings.ShopPassword.Trim());



            return sb.ToString().VerifyMd5Hash(model.md5.Trim());
        }

        private class Utf8StringWriter : StringWriter
        {
            public override Encoding Encoding { get { return Encoding.UTF8; } }
        }

        private XDocument MakeXml(string responseType, YandexResponceModel model)
        {
            XDocument xml = new XDocument();
            XElement rootX = new XElement(responseType);
            XAttribute performedDatetimeX = new XAttribute("performedDatetime", DateTime.Now.ToString("O"));
            XAttribute codeX = new XAttribute("code", model.code);
            XAttribute invoiceIdX = new XAttribute("invoiceId", model.invoiceId);
            XAttribute shopIdX = new XAttribute("shopId", model.shopId);
            XAttribute orderSumAmountX = new XAttribute("orderSumAmount", model.orderSumAmount);
            if (!string.IsNullOrEmpty(model.message))
            {
                XAttribute messageX = new XAttribute("message", model.message);
                rootX.Add(messageX);
            }
            if (!string.IsNullOrEmpty(model.techMessage))
            {
                XAttribute techMessageX = new XAttribute("techMessage", model.techMessage);
                rootX.Add(techMessageX);
            }
            rootX.Add(performedDatetimeX);
            rootX.Add(codeX);
            rootX.Add(invoiceIdX);
            rootX.Add(shopIdX);
            rootX.Add(orderSumAmountX);
            
            xml.Add(rootX);

            return xml;
        }
    }
}
