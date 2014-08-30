using PayPal.PayPalAPIInterfaceService;
using PayPal.PayPalAPIInterfaceService.Model;
using Paypal_API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Paypal_API.API_Call
{
    public class ARB
    {
        #region Create Recurring=============================
        public TransactionModel CreateRecurring(string token)
        {
            // Create request object
            CreateRecurringPaymentsProfileRequestType request = new CreateRecurringPaymentsProfileRequestType();
            populateRequest(request,token);

            // Invoke the API
            CreateRecurringPaymentsProfileReq wrapper = new CreateRecurringPaymentsProfileReq();
            wrapper.CreateRecurringPaymentsProfileRequest = request;

           
            Dictionary<string, string> configurationMap = confiugraion.GetAcctAndConfig();

            // Create the PayPalAPIInterfaceServiceService service object to make the API call
            PayPalAPIInterfaceServiceService service = new PayPalAPIInterfaceServiceService(configurationMap);

            // # API call 
            // Invoke the CreateRecurringPaymentsProfile method in service wrapper object  
            CreateRecurringPaymentsProfileResponseType createRPProfileResponse = service.CreateRecurringPaymentsProfile(wrapper);

            // Check for API return status            
            var response=setKeyResponseObjectsRecurring(service, createRPProfileResponse);

            return response;
        }

        private void populateRequest(CreateRecurringPaymentsProfileRequestType request,string token)
        {
            CurrencyCodeType currency = (CurrencyCodeType)
                Enum.Parse(typeof(CurrencyCodeType), "USD");

            CreateRecurringPaymentsProfileRequestDetailsType profileDetails = new CreateRecurringPaymentsProfileRequestDetailsType();
            request.CreateRecurringPaymentsProfileRequestDetails = profileDetails;
         
                profileDetails.Token = token;

            // Populate Recurring Payments Profile Details
            RecurringPaymentsProfileDetailsType rpProfileDetails = 
                new RecurringPaymentsProfileDetailsType(DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss"));
            profileDetails.RecurringPaymentsProfileDetails = rpProfileDetails;

           

            ScheduleDetailsType scheduleDetails = new ScheduleDetailsType();
          
                scheduleDetails.AutoBillOutstandingAmount = (AutoBillType)
                    Enum.Parse(typeof(AutoBillType), "ADDTONEXTBILLING");
          
      
      
       
               
                int frequency = 1;
              
               //Recurring payment Amount is hard Code for now  but need to get from Model
                BasicAmountType paymentAmount = new BasicAmountType(currency, "10");
                BillingPeriodType period = (BillingPeriodType)
                    Enum.Parse(typeof(BillingPeriodType),"MONTH");
                
                int numCycles = 6;

                BillingPeriodDetailsType paymentPeriod = new BillingPeriodDetailsType(period, frequency, paymentAmount);
                paymentPeriod.TotalBillingCycles = numCycles;
                scheduleDetails.PaymentPeriod = paymentPeriod;
            //Pass value of Description as Billing Agreement here
                scheduleDetails.Description = "Test";
         

            profileDetails.ScheduleDetails = scheduleDetails;

        }

        private TransactionModel setKeyResponseObjectsRecurring(PayPalAPIInterfaceServiceService service, CreateRecurringPaymentsProfileResponseType response)
        {
            TransactionModel model = new TransactionModel();
            Dictionary<string, string> responseParams = new Dictionary<string, string>();
            responseParams.Add("API Status", response.Ack.ToString());
            HttpContext CurrContext = HttpContext.Current;
            CurrContext.Items.Add("Response_redirectURL_rec", null);
            //Save here Profile Id from Resonse bcz need during cancel Subscription

            var profileId = response.CreateRecurringPaymentsProfileResponseDetails.ProfileID;
            //

            if (response.Ack.Equals(AckCodeType.FAILURE) ||
                (response.Errors != null && response.Errors.Count > 0))
            {
                CurrContext.Items.Add("Response_error_rec", response.Errors);
            }
            else
            {
                CurrContext.Items.Add("Response_error_rec", null);
                responseParams.Add("Transaction Id", response.CreateRecurringPaymentsProfileResponseDetails.TransactionID);
                responseParams.Add("Profile Id", response.CreateRecurringPaymentsProfileResponseDetails.ProfileID);
                responseParams.Add("Profile Status", response.CreateRecurringPaymentsProfileResponseDetails.ProfileStatus.ToString());
            }
            CurrContext.Items.Add("Response_keyResponseObjec_rect", responseParams);
            CurrContext.Items.Add("Response_apiName_rec", "CreateRecurringPaymentsProfile");
            CurrContext.Items.Add("Response_requestPayload_rec", service.getLastRequest());
            CurrContext.Items.Add("Response_responsePayload_rec", service.getLastResponse());

            model.TransactionId = response.CreateRecurringPaymentsProfileResponseDetails.TransactionID;
            model.ProfileId = response.CreateRecurringPaymentsProfileResponseDetails.ProfileID;
            model.status = response.CreateRecurringPaymentsProfileResponseDetails.ProfileStatus.ToString();

            return model;
        }

    }

        #endregion
   
}