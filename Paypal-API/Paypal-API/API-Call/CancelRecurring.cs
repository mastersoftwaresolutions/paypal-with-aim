using PayPal.PayPalAPIInterfaceService;
using PayPal.PayPalAPIInterfaceService.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Paypal_API.API_Call
{
    public class CancelRecurring
    {
        public void cancelRecurring(string profileId)
        {
 // Create request object
            
            ManageRecurringPaymentsProfileStatusRequestType request =
                new ManageRecurringPaymentsProfileStatusRequestType();
            ManageRecurringPaymentsProfileStatusRequestDetailsType details =
                new ManageRecurringPaymentsProfileStatusRequestDetailsType();
            request.ManageRecurringPaymentsProfileStatusRequestDetails = details;

            details.ProfileID = profileId;
 

            details.Action = (StatusChangeActionType)
                Enum.Parse(typeof(StatusChangeActionType),"CANCEL");
        

            // Invoke the API
            ManageRecurringPaymentsProfileStatusReq wrapper = new ManageRecurringPaymentsProfileStatusReq();
            wrapper.ManageRecurringPaymentsProfileStatusRequest = request;

      
            Dictionary<string, string> configurationMap = confiugraion.GetAcctAndConfig();
            
            // Create the PayPalAPIInterfaceServiceService service object to make the API call
            PayPalAPIInterfaceServiceService service = new PayPalAPIInterfaceServiceService(configurationMap);

            // # API call 
            // Invoke the ManageRecurringPaymentsProfileStatus method in service wrapper object  
            ManageRecurringPaymentsProfileStatusResponseType manageProfileStatusResponse =
                    service.ManageRecurringPaymentsProfileStatus(wrapper);


            // Check for API return status
            setKeyResponseObjects(service, manageProfileStatusResponse);
        }

        private void setKeyResponseObjects(PayPalAPIInterfaceServiceService service, ManageRecurringPaymentsProfileStatusResponseType response)
        {
            Dictionary<string, string> responseParams = new Dictionary<string, string>();
            responseParams.Add("API Status", response.Ack.ToString());
            HttpContext CurrContext = HttpContext.Current;
            CurrContext.Items.Add("Response_redirectURL", null);
            if (response.Ack.Equals(AckCodeType.FAILURE) ||
                (response.Errors != null && response.Errors.Count > 0))
            {
                CurrContext.Items.Add("Response_error", response.Errors);
            }
            else
            {
                CurrContext.Items.Add("Response_error", null);
                responseParams.Add("Profile Id", response.ManageRecurringPaymentsProfileStatusResponseDetails.ProfileID);                
            }
            CurrContext.Items.Add("Response_keyResponseObject", responseParams);
            CurrContext.Items.Add("Response_apiName", "ManageRecurringPaymentsProfileStatus");
            CurrContext.Items.Add("Response_requestPayload", service.getLastRequest());
            CurrContext.Items.Add("Response_responsePayload", service.getLastResponse());
        }

    }
    
}