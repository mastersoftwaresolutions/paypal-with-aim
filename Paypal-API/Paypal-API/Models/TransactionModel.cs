using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Paypal_API.Models
{
    public class TransactionModel
    {
        public string name { get; set; }
        public string Street1 { get; set; }
        public string Street2 { get; set; }
        public string CityName { get; set; }
        public string StateOrProvince { get; set; }
        public string Country { get; set; }
        public string PostalCode { get; set; }
        public string Phone { get; set; }
        public string ItemName { get; set; }
        public string ItemQuentity { get; set; }
        public string ItemDescription { get; set; }
        public string PaymentAction { get; set; }
        public string BillingAgreement { get; set; }
        public string BillingType { get; set; }
        public string ItemAmount { get; set; }
        public decimal OrderTotal { get; set; }
        public string InsurenceTotal { get; set; }
        public string HandlingTotal { get; set; }
        public string TaxTotal { get; set; }
        public string ShippingTotalCost { get; set; }
        public string OrderDescription { get; set; }
      //  public decimal ShippingTotal { get; set; }
        public Int32? Quantity { get; set; }
        public string ReturnURL { get; set; }
        public string CancelURL { get; set; }
        public string BuyerEmail { get; set; }
        public string ReqConfirmShipping { get; set; }
        public string ItemCategory { get; set; }
        public string SalesTax { get; set; }
        public string ipnNotificationUrl { get; set; }
        public string CurrencyCode { get; set; }
        public string ItemCost { get; set; }
        public string PayerName { get; set; }
        public string TransactionType { get; set; }
        public string NetAmount { get; set; }
        public string GrossAmount { get; set; }
        public string TransactionId { get; set; }
        public string TransactionStatus { get; set; }
        public string Time { get; set; }
        public string ProfileId { get; set; }
        public string status { get; set; }
        public string Paymentstatus { get; set; }
        public string PayerId { get; set; }
      
            }
}