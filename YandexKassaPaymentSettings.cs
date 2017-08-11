using Nop.Core.Configuration;

namespace Nop.Plugin.Payments.YandexKassa
{
    public class YandexKassaPaymentSettings : ISettings
    {
        // Использовать плагин в тестовом режиме
        public bool IsDemo { get; set; }

        public int ShopId { get; set; }
        public int Scid { get; set; }
        public string ShopPassword { get; set; }
        public YandexPaymentSide YandexPaymentSide { get; set; }

        public bool PC { get; set; }
        public bool AC { get; set; }
        public bool MC { get; set; }
        public bool GP { get; set; }
        public bool EP { get; set; }
        public bool WM { get; set; }
        public bool SB { get; set; }
        public bool MP { get; set; }
        public bool AB { get; set; }
        public bool MA { get; set; }
        public bool PB { get; set; }
        public bool QW { get; set; }
        public bool KV { get; set; }
    }

    public enum YandexPaymentSide
    {
        SHOP_SIDE,
        YANDEX_SIDE
    }

}
