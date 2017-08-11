using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Payments.YandexKassa.Models
{
    public class YandexRequestModel
    {
        //  Момент формирования запроса в Яндекс.Кассе.
        public DateTime requestDatetime { get; set; }
        //  Момент регистрации оплаты заказа в Яндекс.Кассе.
        public DateTime paymentDatetime { get; set; }
        //  Тип запроса
        public string action { get; set; }
        //  MD5-хэш параметров платежной формы 
        public string md5 { get; set; }
        //  Идентификатор магазина, выдается Яндекс.Кассой.
        public long shopId { get; set; }
        //  Идентификатор товара, выдается Яндекс.Кассой.
        public long shopArticleId { get; set; }
        //  Уникальный номер транзакции в Яндекс.Кассе.
        public long invoiceId { get; set; }
        //  Номер заказа в системе магазина. Передается, только если был указан в платежной форме.
        public string orderNumber { get; set; }
        //  Идентификатор плательщика (присланный в платежной форме) на стороне магазина.
        public string customerNumber { get; set; }
        //  Момент регистрации заказа в Яндекс.Кассе.
        public DateTime orderCreatedDatetime { get; set; }
        //  Сумма заказа, присылается в платежной форме в параметре sum.
        public string orderSumAmount { get; set; }
        //  Код валюты для суммы заказа.
        public string orderSumCurrencyPaycash { get; set; }
        //  Код процессингового центра Яндекс.Кассы для суммы заказа.
        public string orderSumBankPaycash { get; set; }
        //  Сумма к выплате на счет магазина (сумма заказа минус комиссия Яндекс.Кассы).
        public string shopSumAmount { get; set; }
        //  Код валюты для shopSumAmount.
        public string shopSumCurrencyPaycash { get; set; }
        //  Код процессингового центра Яндекс.Кассы для shopSumAmount.
        public string shopSumBankPaycash { get; set; }
        //  Номер кошелька в Яндекс.Деньгах, с которого производится оплата.
        public string paymentPayerCode { get; set; }
        //  Способ оплаты заказа
        public string paymentType { get; set; }
        //  Двухбуквенный код страны плательщика в соответствии с ISO 3166-1 alpha-2.
        public string cps_user_country_code { get; set; }
        //  Параметры, добавленные магазином в платежную форму.
        public string[] additionalParams {get;set;}

    }

    public class YandexResponceModel
    {
        //  Момент обработки запроса по часам в Яндекс.Кассе.
        public string performedDatetime { get; set; }
        //  Код результата обработки. Список допустимых значений приведен в таблице ниже.
        public int code { get; set; }
        //  Идентификатор магазина. Соответствует значению параметра shopId в запросе.
        public long shopId { get; set; }
        //  Идентификатор транзакции в Яндекс.Кассе. Соответствует значению параметра invoiceId в запросе.
        public long invoiceId { get; set; }
        //  Сумма заказа в валюте, определенной параметром запроса orderSumCurrencyPaycash.
        public decimal orderSumAmount { get; set; }
        //  Текстовое пояснение в случае отказа принять платеж.
        public string message { get; set; }
        //  Дополнительное текстовое пояснение ответа магазина.
        public string techMessage { get; set; }
    }
}
