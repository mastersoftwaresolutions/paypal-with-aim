using PayPal.PayPalAPIInterfaceService;
using PayPal.PayPalAPIInterfaceService.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Paypal_API.API_Call
{
    public class Capture
    {
        public void CapturePayment(string transactionId,string currencyCode,string amount)
        {

            // Create request object

            DoCaptureRequestType request =
                new DoCaptureRequestType();

            // (Required) Authorization identification number of the payment you want to capture. This is the transaction ID returned from DoExpressCheckoutPayment, DoDirectPayment, or CheckOut. For point-of-sale transactions, this is the transaction ID returned by the CheckOut call when the payment action is Authorization.
            request.AuthorizationID = transactionId;

            // (Required) Amount to capture.
            // Note: You must set the currencyID attribute to one of the three-character currency codes for any of the supported PayPal currencies.
            CurrencyCodeType currency = (CurrencyCodeType)
                Enum.Parse(typeof(CurrencyCodeType), currencyCode);
            request.Amount = new BasicAmountType(currency, amount);

        
        

            // Invoke the API
            DoCaptureReq wrapper = new DoCaptureReq();
            wrapper.DoCaptureRequest = request;

          
            Dictionary<string, string> configurationMap = confiugraion.GetAcctAndConfig();

            // Create the PayPalAPIInterfaceServiceService service object to make the API call
            PayPalAPIInterfaceServiceService service = new PayPalAPIInterfaceServiceService(configurationMap);

            // # API call 
            // Invoke the DoCapture method in service wrapper object  
            DoCaptureResponseType doCaptureResponse =
                    service.DoCapture(wrapper);


            // Check for API return status
            setKeyResponseObjects(service, doCaptureResponse);
        }

        // A helper method used by APIResponse.aspx that returns select response parameters 
        // of interest. You must process API response objects as applicable to your application
        private void setKeyResponseObjects(PayPalAPIInterfaceServiceService service, DoCaptureResponseType doCaptureResponse)
        {
            Dictionary<string, string> responseParams = new Dictionary<string, string>();
            HttpContext CurrContext = HttpContext.Current;
            CurrContext.Items.Add("Response_keyResponseObject", responseParams);

            CurrContext.Items.Add("Response_apiName", "DoCapture");
            CurrContext.Items.Add("Response_redirectURL", null);
            CurrContext.Items.Add("Response_requestPayload", service.getLastRequest());
            CurrContext.Items.Add("Response_responsePayload", service.getLastResponse());

            if (doCaptureResponse.Ack.Equals(AckCodeType.FAILURE) ||
                (doCaptureResponse.Errors != null && doCaptureResponse.Errors.Count > 0))
            {
                CurrContext.Items.Add("Response_error", doCaptureResponse.Errors);
            }
            else
            {
                CurrContext.Items.Add("Response_error", null);
                responseParams.Add("Transaction Id", doCaptureResponse.DoCaptureResponseDetails.PaymentInfo.TransactionID);
                responseParams.Add("Payment status", doCaptureResponse.DoCaptureResponseDetails.PaymentInfo.PaymentStatus.ToString());
                responseParams.Add("Pending reason", doCaptureResponse.DoCaptureResponseDetails.PaymentInfo.PendingReason.ToString());
            }

        }
    }
}