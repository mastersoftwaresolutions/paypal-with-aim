using PayPal.PayPalAPIInterfaceService;
using PayPal.PayPalAPIInterfaceService.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Paypal_API.API_Call
{
    public class ManageTransaction
    {
        public void VoidTransaction(string transactionId)
        {
            // Create request object
            DoVoidRequestType request =
                new DoVoidRequestType();   
        
            request.AuthorizationID =transactionId;
         

            // Invoke the API
            DoVoidReq wrapper = new DoVoidReq();
            wrapper.DoVoidRequest = request;

            Dictionary<string, string> configurationMap = confiugraion.GetAcctAndConfig();
            
            // Create the PayPalAPIInterfaceServiceService service object to make the API call
            PayPalAPIInterfaceServiceService service = new PayPalAPIInterfaceServiceService(configurationMap);

            // # API call 
            // Invoke the DoVoid method in service wrapper object  
            DoVoidResponseType doVoidResponse =
                    service.DoVoid(wrapper);


            // Check for API return status
            setKeyResponseObjects(service, doVoidResponse);
        }

        private void setKeyResponseObjects(PayPalAPIInterfaceServiceService service, DoVoidResponseType doVoidResponse)
        {
            Dictionary<string, string> responseParams = new Dictionary<string, string>();
            HttpContext CurrContext = HttpContext.Current;
            CurrContext.Items.Add("Response_keyResponseObject", responseParams);

            CurrContext.Items.Add("Response_apiName", "DoVoid");
            CurrContext.Items.Add("Response_redirectURL", null);
            CurrContext.Items.Add("Response_requestPayload", service.getLastRequest());
            CurrContext.Items.Add("Response_responsePayload", service.getLastResponse());

            if (doVoidResponse.Ack.Equals(AckCodeType.FAILURE) ||
                (doVoidResponse.Errors != null && doVoidResponse.Errors.Count > 0))
            {
                CurrContext.Items.Add("Response_error", doVoidResponse.Errors);
            }
            else
            {
                CurrContext.Items.Add("Response_error", null);
                responseParams.Add("Authorization Id", doVoidResponse.AuthorizationID);

                //Selenium Test Case
                responseParams.Add("Acknowledgement", doVoidResponse.Ack.ToString());

            }

        }


        public string Refund(string transactionId,string amount,string currencyCode)
        {
            // Create request object
            RefundTransactionRequestType request = new RefundTransactionRequestType();
        
            request.TransactionID = transactionId;
          
                request.RefundType = (RefundType)
                    Enum.Parse(typeof(RefundType), "FULL");
                // (Optional) Refund amount. The amount is required if RefundType is Partial.
                // Note: If RefundType is Full, do not set the amount.
                if (request.RefundType.Equals(RefundType.PARTIAL) && amount != string.Empty)
                {
                    CurrencyCodeType currency = (CurrencyCodeType)
                        Enum.Parse(typeof(CurrencyCodeType), currencyCode);
                    request.Amount = new BasicAmountType(currency, amount);
                }
            
       
          

            // Invoke the API
            RefundTransactionReq wrapper = new RefundTransactionReq();
            wrapper.RefundTransactionRequest = request;

       
            Dictionary<string, string> configurationMap = confiugraion.GetAcctAndConfig();

            // Create the PayPalAPIInterfaceServiceService service object to make the API call
            PayPalAPIInterfaceServiceService service = new PayPalAPIInterfaceServiceService(configurationMap);

            // # API call 
            // Invoke the RefundTransaction method in service wrapper object  
            RefundTransactionResponseType refundTransactionResponse = service.RefundTransaction(wrapper);

            // Check for API return status
           string response= processResponse(service, refundTransactionResponse);
           return response;
        }

        private string processResponse(PayPalAPIInterfaceServiceService service, RefundTransactionResponseType response)
        {
            HttpContext CurrContext = HttpContext.Current;
            CurrContext.Items.Add("Response_apiName", "RefundTransaction");
            CurrContext.Items.Add("Response_redirectURL", null);
            CurrContext.Items.Add("Response_requestPayload", service.getLastRequest());
            CurrContext.Items.Add("Response_responsePayload", service.getLastResponse());

            Dictionary<string, string> keyParameters = new Dictionary<string, string>();
            keyParameters.Add("Correlation Id", response.CorrelationID);
            keyParameters.Add("API Result", response.Ack.ToString());

            if (response.Ack.Equals(AckCodeType.FAILURE) ||
                (response.Errors != null && response.Errors.Count > 0))
            {
                CurrContext.Items.Add("Response_error", response.Errors);
            }
            else
            {
                CurrContext.Items.Add("Response_error", null);
                keyParameters.Add("Refund transaction Id", response.RefundTransactionID);
                keyParameters.Add("Total refunded amount",
                    response.TotalRefundedAmount.value + response.TotalRefundedAmount.currencyID);
            }
            CurrContext.Items.Add("Response_keyResponseObject", keyParameters);

            string message = response.Ack.ToString();
            return message;
        }
    }
    }