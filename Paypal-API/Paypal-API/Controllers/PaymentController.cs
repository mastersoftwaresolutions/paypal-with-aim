using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Paypal_API.API_Call;
using Paypal_API.Models;

namespace Paypal_API.Controllers
{
    public class PaymentController : Controller
    {
        //
        // GET: /Payment/
        Order orderObj = new Order();
        [HttpGet]
        public ActionResult CreateOrder()
        {
            if (Request.QueryString["token"] != null && Request.QueryString["PayerID"]!=null)
            {
                orderObj.doExpressCheckout(Request.QueryString["token"], Request.QueryString["PayerID"]);
            }
            return View();
        }
        [HttpPost]
        public ActionResult CreateOrder(TransactionModel model)
        {
           
           var response= orderObj.Create(model);
           if (response.Contains("http"))
           {
               Response.Redirect(response);

           }
           
               return Json(response);
           
        }

        public ActionResult GetTransactionDetails()
        {
            GetTransaction obj = new GetTransaction();
            ViewBag.PendingTransaction= obj.Get("PENDING");
            return View();
        }

        public ActionResult GetTransactionDetailsCompleted()
        {
            GetTransaction obj = new GetTransaction();
            ViewBag.CompletedTransaction = obj.Get("SUCCESS");
            return View();
        }
      
        public ActionResult DoCapture(string transactionId,string currencyCode,string amount)
        {
            var obj = new Capture();
            obj.CapturePayment(transactionId, currencyCode, amount);
            var objRecurring = new ARB();
            return RedirectToAction("GetTransactionDetails", "Payment");

        }

        public ActionResult DenyTransaction(string transactionId)
        {
            var obj = new ManageTransaction();
            obj.VoidTransaction(transactionId);
            return RedirectToAction("GetTransactionDetails", "Payment");

        }

        public ActionResult Refund(string transactionId,string amount,string currencyCode)
        {
            var obj = new ManageTransaction();
            var response=obj.Refund(transactionId,amount,currencyCode);
            return Json(response,JsonRequestBehavior.AllowGet);

        }
        public ActionResult CancelARB ()
        {
            //Get Profile Id  from DB
            var profileId = "I-3X7UJM0C3V4X";
            var obj = new CancelRecurring();
            obj.cancelRecurring(profileId);
            var objRecurring = new ARB();
            return RedirectToAction("GetTransactionDetailsCompleted", "Payment");

        }

    }
}
