using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;

namespace Nop.Plugin.Payments.YandexKassa.Models
{
    public class ConfigurationModel : BaseNopModel
    {
        public int ActiveStoreScopeConfiguration { get; set; }

        [NopResourceDisplayName("Plugins.Payments.YandexKassa.Fields.IsDemo")]
        public bool IsDemo { get; set; }
        public bool IsDemo_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.YandexKassa.Fields.ShopId")]
        public string ShopId { get; set; }
        public bool ShopId_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.YandexKassa.Fields.Scid")]
        public string Scid { get; set; }
        public bool Scid_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.YandexKassa.Fields.ShopPassword")]
        public string ShopPassword { get; set; }
        public bool ShopPassword_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.YandexKassa.Fields.YandexPayentSide")]
        public YandexPaymentSide YandexPaymentSide { get; set; }
        public bool YandexPaymentSide_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.YandexKassa.Fields.PC")]
        public bool PC { get; set; }
        public bool PC_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.YandexKassa.Fields.AC")]
        public bool AC { get; set; }
        public bool AC_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.YandexKassa.Fields.MC")]
        public bool MC { get; set; }
        public bool MC_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.YandexKassa.Fields.GP")]
        public bool GP { get; set; }
        public bool GP_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.YandexKassa.Fields.EP")]
        public bool EP { get; set; }
        public bool EP_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.YandexKassa.Fields.WM")]
        public bool WM { get; set; }
        public bool WM_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.YandexKassa.Fields.SB")]
        public bool SB { get; set; }
        public bool SB_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.YandexKassa.Fields.MP")]
        public bool MP { get; set; }
        public bool MP_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.YandexKassa.Fields.AB")]
        public bool AB { get; set; }
        public bool AB_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.YandexKassa.Fields.MA")]
        public bool MA { get; set; }
        public bool MA_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.YandexKassa.Fields.PB")]
        public bool PB { get; set; }
        public bool PB_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.YandexKassa.Fields.QW")]
        public bool QW { get; set; }
        public bool QW_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.YandexKassa.Fields.KV")]
        public bool KV { get; set; }
        public bool KV_OverrideForStore { get; set; }

    }
}