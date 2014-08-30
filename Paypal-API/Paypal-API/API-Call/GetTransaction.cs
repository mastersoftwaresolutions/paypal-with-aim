using PayPal.PayPalAPIInterfaceService;
using PayPal.PayPalAPIInterfaceService.Model;
using Paypal_API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Paypal_API.API_Call
{
    public class GetTransaction
    {
        public List<TransactionModel> Get(string status)
        {
            TransactionModel model = new TransactionModel();

            // Create request object
            TransactionSearchRequestType request = new TransactionSearchRequestType();

            var startDate = DateTime.Now.AddDays(-1).ToString("yyyy-MM-ddTHH:mm:ss");
            var endDate = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss");

            request.StartDate = startDate;

            request.EndDate = endDate;
       
           
                //request.Receiver = "mss.parveensachdeva@gmail.com";



                request.Status = (PaymentTransactionStatusCodeType)
                        Enum.Parse(typeof(PaymentTransactionStatusCodeType), status);
         


            // Invoke the API
            TransactionSearchReq wrapper = new TransactionSearchReq();
            wrapper.TransactionSearchRequest = request;

            Dictionary<string, string> configurationMap = confiugraion.GetAcctAndConfig();

            // Create the PayPalAPIInterfaceServiceService service object to make the API call
            PayPalAPIInterfaceServiceService service = new PayPalAPIInterfaceServiceService(configurationMap);

            // # API call 
            // Invoke the TransactionSearch method in service wrapper object  
            TransactionSearchResponseType transactionDetails = service.TransactionSearch(wrapper);

            // Check for API return status
            var responseList=processResponse(service, transactionDetails);
            return responseList;

        }

        private List<TransactionModel> processResponse(PayPalAPIInterfaceServiceService service, TransactionSearchResponseType response)
        {
            HttpContext CurrContext = HttpContext.Current;
            CurrContext.Items.Add("Response_apiName", "TransactionSearch");
            CurrContext.Items.Add("Response_redirectURL", null);
            CurrContext.Items.Add("Response_requestPayload", service.getLastRequest());
            CurrContext.Items.Add("Response_responsePayload", service.getLastResponse());

            Dictionary<string, string> keyParameters = new Dictionary<string, string>();
            keyParameters.Add("Correlation Id", response.CorrelationID);
            keyParameters.Add("API Result", response.Ack.ToString());

            if (response.Errors != null && response.Errors.Count > 0)
            {
                CurrContext.Items.Add("Response_error", response.Errors);
            }
            else
            {
                CurrContext.Items.Add("Response_error", null);
            }
            List<TransactionModel> list = new List<TransactionModel>();

            if (!response.Ack.Equals(AckCodeType.FAILURE))
            {
                keyParameters.Add("Total matching transactions", response.PaymentTransactions.Count.ToString());

                for (int i = 0; i < response.PaymentTransactions.Count; i++)
                {
                    var model=new TransactionModel();
                    PaymentTransactionSearchResultType result = response.PaymentTransactions[i];
                    string label = "Result " + (i + 1);
                    model.PayerName = result.Payer;
                    model.TransactionId= result.TransactionID;
                    model.TransactionStatus=result.Status;
                    model.Time=result.Timestamp;
                    model.TransactionType= result.Type;
                   
                    if (result.NetAmount != null)
                    {
                        
                     model.NetAmount=result.NetAmount.value + result.NetAmount.currencyID.ToString();
                     model.ItemCost = result.NetAmount.value;
                    }
                    if (result.GrossAmount != null)
                    {

                        model.GrossAmount = result.GrossAmount.value + result.GrossAmount.currencyID.ToString();
                        model.CurrencyCode = result.GrossAmount.currencyID.ToString();
                    }
                    list.Add(model);
                }
            }
            return list;
        }

     

    }
}