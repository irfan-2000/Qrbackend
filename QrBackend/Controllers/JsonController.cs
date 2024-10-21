using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using QrBackend.Models;
using Razorpay.Api;
using System;
using Newtonsoft.Json;

namespace QrBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    public class JsonController : Controller
    {


        private readonly DataConversion _objConvert;

        public JsonController(DataConversion dataConversion)
        {
            _objConvert = dataConversion;
        }




        [HttpGet("GetMenuDishes")]
        public JsonResult GetMenuDishes()
        {
            var result = _objConvert.GetMenuDishes();
            return Json(result); 
        }
        

        [HttpPost("CreateOrder")]
        public JsonResult CreateOrder([FromForm] decimal amount, [FromForm] string customerName, [FromForm] int tableNumber)
        {
            var client = new RazorpayClient("rzp_test_a8TafmgeHud8zG", "sxabwORmFllVVkfjmyhufQv5");
            var options = new Dictionary<string, object>
             {
              { "amount", amount * 100 },
        { "currency", "INR" },
        { "receipt", System.Guid.NewGuid().ToString() }
                 };
            var order = client.Order.Create(options);
            SaveOrderID(order["id"].ToString(), customerName, tableNumber, Convert.ToDecimal(order["amount"]));

            return Json(new { orderId = order["id"].ToString() }); // Corrected access to the order ID

        }


                [HttpPost("VerifyPayment")]
        public JsonResult VerifyPayment([FromBody] PaymentVerificationRequest request)
        {
            try
            {
                Dictionary<string, string> attributes = new Dictionary<string, string>
        { 
            { "razorpay_payment_id", request.PaymentId },
            { "razorpay_order_id", request.OrderId },
            { "razorpay_signature", request.Signature }

                  


        };

                // Call the verification method
                Utils.verifyPaymentSignature(attributes); // This will throw if verification fails
                SaveVerfiedPayment(request.PaymentId, request.OrderId, request.Signature);
                   
                // If we reach here, it means verification was successful
                return Json(new { success = true, message = "Payment verified successfully." });
            }
            catch (Exception ex) // Handle verification failure
            {
                // Optionally log the exception (ex)
                return Json(new { success = false, message = "Payment verification failed." });
            }
        }




        [HttpGet("gettablestatus")]
        public int gettablestatus([FromQuery]  int tablenumber,string action)
        {

            var result = _objConvert.gettablestatus(tablenumber, action);
            return result;


        }

        [HttpPost("lockunlocktable")]
        public int lockunlocktable([FromBody]  int tablenumber, string action)
        {

            var result = _objConvert.lockunlocktable(tablenumber, action);
            return result;

         }

        [HttpGet("saveOrderItems")]
        public int saveOrderItems( string name,string items ,int TableNumber,string OrderId,string Quantities)
        {
            var item = JsonConvert.SerializeObject(items).Replace("\\", "");
            var result = _objConvert.saveOrderItems(name, item,TableNumber,OrderId, Quantities);
            return result;
        }


        [HttpGet("getOrderedItems")]
        public IActionResult GetOrderedItems(int tableNumber)
        {
            var result = _objConvert.GetOrderedItems(tableNumber);
            return Ok(result);
        }


        [HttpGet("getTableSessionCode")]
        public IActionResult getTableSessionCode(int tableNumber)
        {
            var result = _objConvert.getTableSessionCode(tableNumber);
            return Ok(new  { Code = result });
        }


        [NonAction]
        public  int SaveOrderID(string OrderId,string customername, int tablenumber,decimal amount)
        {   var result = _objConvert.SaveOrderID(OrderId, customername, tablenumber,amount);
            return result;
        }

        [NonAction]
        public int SaveVerfiedPayment(string PaymentId,string OrderId, string Signature)
        {
            var result = _objConvert.SaveVerfiedPayment(PaymentId, OrderId, Signature);
            return result;
        }






    }
}
