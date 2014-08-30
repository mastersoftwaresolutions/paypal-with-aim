using PayPal.PayPalAPIInterfaceService;
using PayPal.PayPalAPIInterfaceService.Model;
using Paypal_API.Models;
using PayPalAPISample;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace Paypal_API.API_Call
{
    public class Order
    {
        public string Create(TransactionModel model)
        {
            // Create request object
            SetExpressCheckoutRequestType request = new SetExpressCheckoutRequestType();
            populateRequestObject(request,model);

            // Invoke the API
            SetExpressCheckoutReq wrapper = new SetExpressCheckoutReq();
            wrapper.SetExpressCheckoutRequest = request;

        
            Dictionary<string, string> configurationMap = confiugraion.GetAcctAndConfig();

            // Create the PayPalAPIInterfaceServiceService service object to make the API call
            PayPalAPIInterfaceServiceService service = new PayPalAPIInterfaceServiceService(configurationMap);

            // # API call
            // Invoke the SetExpressCheckout method in service wrapper object
            SetExpressCheckoutResponseType setECResponse = service.SetExpressCheckout(wrapper);

            // Check for API return status
            HttpContext CurrContext = HttpContext.Current;
            CurrContext.Items.Add("paymentDetails", request.SetExpressCheckoutRequestDetails.PaymentDetails);
            var response=setKeyResponseObjects(service, setECResponse);
            return response;
        }

        private void populateRequestObject(SetExpressCheckoutRequestType request,TransactionModel model)
        {
            SetExpressCheckoutRequestDetailsType ecDetails = new SetExpressCheckoutRequestDetailsType();

                  model.ReturnURL = ConfigurationManager.AppSettings["Return_Url"];
                  model.CancelURL = ConfigurationManager.AppSettings["Cancel_Url"];
                  model.ipnNotificationUrl = "";
                  model.PaymentAction = "ORDER";
                ecDetails.ReturnURL = model.ReturnURL;
       
                ecDetails.CancelURL = model.CancelURL;
        
                ecDetails.BuyerEmail = model.BuyerEmail;
          

                ecDetails.ReqConfirmShipping = "No";
        
            PaymentDetailsType paymentDetails = new PaymentDetailsType();
            ecDetails.PaymentDetails.Add(paymentDetails);

            // (Required) Total cost of the transaction to the buyer. If shipping cost and tax charges are known, include them in this value. If not, this value should be the current sub-total of the order. If the transaction includes one or more one-time purchases, this field must be equal to the sum of the purchases. Set this field to 0 if the transaction does not include a one-time purchase such as when you set up a billing agreement for a recurring payment that is not immediately charged. When the field is set to 0, purchase-specific fields are ignored.
            double orderTotal = 0.0;

            // Sum of cost of all items in this order. For digital goods, this field is required.
            double itemTotal = 0.0;

            CurrencyCodeType currency = (CurrencyCodeType)
                Enum.Parse(typeof(CurrencyCodeType), model.CurrencyCode);

            if (model.ShippingTotalCost != string.Empty)
            {
                paymentDetails.ShippingTotal = new BasicAmountType(currency, model.ShippingTotalCost);
                orderTotal += Convert.ToDouble(model.ShippingTotalCost);
            }


            if (model.InsurenceTotal != string.Empty && !Convert.ToDouble(model.InsurenceTotal).Equals(0.0))
            {
                paymentDetails.InsuranceTotal = new BasicAmountType(currency, model.InsurenceTotal);
                paymentDetails.InsuranceOptionOffered = "true";
                orderTotal += Convert.ToDouble(model.InsurenceTotal);
            }

 
            if (model.HandlingTotal != string.Empty)
            {
                paymentDetails.HandlingTotal = new BasicAmountType(currency, model.HandlingTotal);
                orderTotal += Convert.ToDouble(model.HandlingTotal);
            }

    
            if (model.TaxTotal != string.Empty)
            {
                paymentDetails.TaxTotal = new BasicAmountType(currency, model.TaxTotal);
                orderTotal += Convert.ToDouble(model.TaxTotal);
            }

  
            if (model.ItemDescription != string.Empty)
            {
                paymentDetails.OrderDescription = model.ItemDescription;
            }

        
            paymentDetails.PaymentAction = (PaymentActionCodeType)
                Enum.Parse(typeof(PaymentActionCodeType), model.PaymentAction);

            if (model.name != string.Empty && model.Street1 != string.Empty
                && model.CityName != string.Empty && model.StateOrProvince != string.Empty
                && model.Country != string.Empty && model.PostalCode != string.Empty)
            {
                AddressType shipAddress = new AddressType();


                shipAddress.Name = model.name;


                shipAddress.Street1 = model.Street1;

                shipAddress.Street2 = model.Street2;


                shipAddress.CityName = model.CityName;

                shipAddress.StateOrProvince = model.StateOrProvince;

             if(!string.IsNullOrEmpty(model.Country))
             { 
                shipAddress.Country = (CountryCodeType)
                    Enum.Parse(typeof(CountryCodeType), model.Country);
                 }
                shipAddress.PostalCode = model.PostalCode;

                //Fix for release
                shipAddress.Phone = model.Phone;

                ecDetails.PaymentDetails[0].ShipToAddress = shipAddress;
            }

            // Each payment can include requestDetails about multiple items
            // This example shows just one payment item
            if (model.ItemName != null && model.ItemCost != null && model.Quantity != null)
            {
                PaymentDetailsItemType itemDetails = new PaymentDetailsItemType();
                itemDetails.Name = model.ItemName;
                itemDetails.Amount = new BasicAmountType(currency, model.ItemCost);
                itemDetails.Quantity = model.Quantity;

               
                itemDetails.ItemCategory = (ItemCategoryType)
                    Enum.Parse(typeof(ItemCategoryType),model.ItemCategory);
                itemTotal += Convert.ToDouble(itemDetails.Amount.value) * itemDetails.Quantity.Value;

                if (model.SalesTax != string.Empty)
                {
                    itemDetails.Tax = new BasicAmountType(currency, model.SalesTax);
                    orderTotal += Convert.ToDouble(model.SalesTax);
                }

                //(Optional) Item description.
                // Character length and limitations: 127 single-byte characters
                // This field is introduced in version 53.0.
                if (model.ItemDescription != string.Empty)
                {
                    itemDetails.Description = model.ItemDescription;
                }
                paymentDetails.PaymentDetailsItem.Add(itemDetails);
            }

            orderTotal += itemTotal;
            paymentDetails.ItemTotal = new BasicAmountType(currency, itemTotal.ToString());
            paymentDetails.OrderTotal = new BasicAmountType(currency, orderTotal.ToString());

            paymentDetails.NotifyURL = model.ipnNotificationUrl.Trim();

            // Set Billing agreement (for Reference transactions & Recurring payments)
            if (model.BillingAgreement != string.Empty)
            {

                model.BillingType = "RECURRINGPAYMENTS";
                BillingCodeType billingCodeType = (BillingCodeType)
                    Enum.Parse(typeof(BillingCodeType), model.BillingType);

                BillingAgreementDetailsType baType = new BillingAgreementDetailsType(billingCodeType);
                baType.BillingAgreementDescription = model.BillingAgreement;
                ecDetails.BillingAgreementDetails.Add(baType);
            }


            request.SetExpressCheckoutRequestDetails = ecDetails;
        }

        // A helper method used by APIResponse.aspx that returns select response parameters
        // of interest.
        private string setKeyResponseObjects(PayPalAPIInterfaceServiceService service, SetExpressCheckoutResponseType setECResponse)
        {
            Dictionary<string, string> keyResponseParameters = new Dictionary<string, string>();
            keyResponseParameters.Add("API Status", setECResponse.Ack.ToString());
            string message = "";
            HttpContext CurrContext = HttpContext.Current;
            if (setECResponse.Ack.Equals(AckCodeType.FAILURE) ||
                (setECResponse.Errors != null && setECResponse.Errors.Count > 0))
            {
                CurrContext.Items.Add("Response_error", setECResponse.Errors);
           
                message = setECResponse.Errors[0].LongMessage;
              
     
                CurrContext.Items.Add("Response_redirectURL", null);
                   return message;
            }
            else
            {
                CurrContext.Items.Add("Response_error", null);
                keyResponseParameters.Add("EC token", setECResponse.Token);
                CurrContext.Items.Add("Response_redirectURL", ConfigurationManager.AppSettings["PAYPAL_REDIRECT_URL"].ToString()
                    + "_express-checkout&token=" + setECResponse.Token);
            }
            CurrContext.Items.Add("Response_keyResponseObject", keyResponseParameters);
            CurrContext.Items.Add("Response_apiName", "SetExpressCheckout");
            CurrContext.Items.Add("Response_requestPayload", service.getLastRequest());
            CurrContext.Items.Add("Response_responsePayload", service.getLastResponse());
           
           foreach(var i in keyResponseParameters){
               if (i.Key == "API Status")
               {
                   message = i.Value;
                  
               }
           }

           if (message == "SUCCESS")
           {
           
               message = CurrContext.Items["Response_redirectURL"].ToString();
           

           }
            return message;
        }

        public TransactionModel doExpressCheckout(string token, string payerId)
        {

           
            Dictionary<string, string> configurationMap = confiugraion.GetAcctAndConfig();

            // Create the PayPalAPIInterfaceServiceService service object to make the API call
            PayPalAPIInterfaceServiceService service = new PayPalAPIInterfaceServiceService(configurationMap);

            GetExpressCheckoutDetailsReq getECWrapper = new GetExpressCheckoutDetailsReq();
       
            getECWrapper.GetExpressCheckoutDetailsRequest = new GetExpressCheckoutDetailsRequestType(token);
            // # API call 
            // Invoke the GetExpressCheckoutDetails method in service wrapper object
            GetExpressCheckoutDetailsResponseType getECResponse = service.GetExpressCheckoutDetails(getECWrapper);

            // Create request object
            DoExpressCheckoutPaymentRequestType request = new DoExpressCheckoutPaymentRequestType();
            DoExpressCheckoutPaymentRequestDetailsType requestDetails = new DoExpressCheckoutPaymentRequestDetailsType();
            request.DoExpressCheckoutPaymentRequestDetails = requestDetails;

            requestDetails.PaymentDetails = getECResponse.GetExpressCheckoutDetailsResponseDetails.PaymentDetails;
            // (Required) The timestamped token value that was returned in the SetExpressCheckout response and passed in the GetExpressCheckoutDetails request.
            requestDetails.Token = token;
            // (Required) Unique PayPal buyer account identification number as returned in the GetExpressCheckoutDetails response
            requestDetails.PayerID = payerId;
      
            requestDetails.PaymentAction = (PaymentActionCodeType)
                Enum.Parse(typeof(PaymentActionCodeType), "ORDER");

            // Invoke the API
            DoExpressCheckoutPaymentReq wrapper = new DoExpressCheckoutPaymentReq();
            wrapper.DoExpressCheckoutPaymentRequest = request;
            // # API call 
            // Invoke the DoExpressCheckoutPayment method in service wrapper object
            DoExpressCheckoutPaymentResponseType doECResponse = service.DoExpressCheckoutPayment(wrapper);

            // Check for API return status
           var responseARB= setKeyResponseObjects(service, doECResponse,token);
           responseARB.PayerId=payerId;
           
           return responseARB;

        }


        private TransactionModel setKeyResponseObjects(PayPalAPIInterfaceServiceService service, DoExpressCheckoutPaymentResponseType doECResponse, string token)
        {
            Dictionary<string, string> responseParams = new Dictionary<string, string>();
            responseParams.Add("Correlation Id", doECResponse.CorrelationID);
            responseParams.Add("API Result", doECResponse.Ack.ToString());
            HttpContext CurrContext = HttpContext.Current;
            if (doECResponse.Ack.Equals(AckCodeType.FAILURE) ||
                (doECResponse.Errors != null && doECResponse.Errors.Count > 0))
            {
                CurrContext.Items.Add("Response_error", doECResponse.Errors);
            }
            else
            {
                CurrContext.Items.Add("Response_error", null);
                responseParams.Add("EC Token", doECResponse.DoExpressCheckoutPaymentResponseDetails.Token);
                responseParams.Add("Transaction Id", doECResponse.DoExpressCheckoutPaymentResponseDetails.PaymentInfo[0].TransactionID);
                responseParams.Add("Payment status", doECResponse.DoExpressCheckoutPaymentResponseDetails.PaymentInfo[0].PaymentStatus.ToString());
                if (doECResponse.DoExpressCheckoutPaymentResponseDetails.PaymentInfo[0].PendingReason != null)
                {
                    responseParams.Add("Pending reason", doECResponse.DoExpressCheckoutPaymentResponseDetails.PaymentInfo[0].PendingReason.ToString());
                }
                if (doECResponse.DoExpressCheckoutPaymentResponseDetails.BillingAgreementID != null)
                    responseParams.Add("Billing Agreement Id", doECResponse.DoExpressCheckoutPaymentResponseDetails.BillingAgreementID);
            }

            CurrContext.Items.Add("Response_keyResponseObject_Authorized", responseParams);
            CurrContext.Items.Add("Response_apiName", "DoExpressChecoutPayment");
            CurrContext.Items.Add("Response_redirectURL", null);
            CurrContext.Items.Add("Response_requestPayload", service.getLastRequest());
            CurrContext.Items.Add("Response_responsePayload", service.getLastResponse());
            string message = "";
            foreach (var i in responseParams)
            {
                if (i.Key == "API Result")
                {
                    message = i.Value;

                }
            }
            var transactionId = doECResponse.DoExpressCheckoutPaymentResponseDetails.PaymentInfo[0].TransactionID;
            var responseARB =new TransactionModel();
            if (message == "SUCCESS")
            {

               // message = CurrContext.Items["Response_redirectURL"].ToString();
                responseARB= DoAuthorized(transactionId, "USD", "5.27",token);
                responseARB.TransactionId = doECResponse.DoExpressCheckoutPaymentResponseDetails.PaymentInfo[0].TransactionID;
                responseARB.Paymentstatus = doECResponse.DoExpressCheckoutPaymentResponseDetails.PaymentInfo[0].PaymentStatus.ToString();
            }
            return responseARB;
            
        }

        public TransactionModel DoAuthorized(string transactionId, string currencyCode, string amount, string token)
        {
            // Create request object

            DoAuthorizationRequestType request =
                new DoAuthorizationRequestType();

            // (Required) Value of the order's transaction identification number returned by PayPal.
            request.TransactionID = transactionId;
            CurrencyCodeType currency = (CurrencyCodeType)
                Enum.Parse(typeof(CurrencyCodeType), currencyCode);
            // (Required) Amount to authorize.
            request.Amount = new BasicAmountType(currency, amount);

            // Invoke the API
            DoAuthorizationReq wrapper = new DoAuthorizationReq();
            wrapper.DoAuthorizationRequest = request;

            Dictionary<string, string> configurationMap = confiugraion.GetAcctAndConfig();

            // Create the PayPalAPIInterfaceServiceService service object to make the API call
            PayPalAPIInterfaceServiceService service = new PayPalAPIInterfaceServiceService(configurationMap);

            // # API call 
            // Invoke the DoAuthorization method in service wrapper object  
            DoAuthorizationResponseType doAuthorizationResponse =
                    service.DoAuthorization(wrapper);


            // Check for API return status
           var responseARB= setKeyResponseObjectsAuthorized(service, doAuthorizationResponse,token);

           return responseARB;
        
        }

        private TransactionModel setKeyResponseObjectsAuthorized(PayPalAPIInterfaceServiceService service, DoAuthorizationResponseType response, string token)
        {

            var message = "";
            Dictionary<string, string> responseParams = new Dictionary<string, string>();
            responseParams.Add("Transaction Id", response.TransactionID);
            responseParams.Add("Payment status", response.AuthorizationInfo.PaymentStatus.ToString());
            responseParams.Add("Pending reason", response.AuthorizationInfo.PendingReason.ToString());

            HttpContext CurrContext = HttpContext.Current;
            CurrContext.Items.Add("Response_keyResponseObject", responseParams);
            CurrContext.Items.Add("Response_apiName_Authorized", "DoAuthorization");
            CurrContext.Items.Add("Response_redirectURL_Authorized", null);
            CurrContext.Items.Add("Response_requestPayload_Authorized", service.getLastRequest());
            CurrContext.Items.Add("Response_responsePayload_Authorized", service.getLastResponse());

            if (response.Ack.Equals(AckCodeType.FAILURE) ||
                (response.Errors != null && response.Errors.Count > 0))
            {
                CurrContext.Items.Add("Response_error_Authorized", response.Errors);
            }
            else
            {
                CurrContext.Items.Add("Response_error_Authorized", null);
            }

            message="Payment status" + response.AuthorizationInfo.PaymentStatus.ToString();
            ARB obj = new ARB();
           var responseARB= obj.CreateRecurring(token);

           return responseARB;
            
        }


        

    }
}