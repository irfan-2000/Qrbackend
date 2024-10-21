using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QrBackend.Models
{
    public class RestaurantDataModel
    {





    }


    public class OrderDetails
    {
        public string OrderID { get; set; }
        public List<OrderData> orderdata { get; set; }
    }
    public class OrderData
    {
        public string OrderId { get; set; }
        public string DishName { get; set; }
        public decimal Price { get; set; }

        public string Food { get; set; }

        public string Imageurl { get; set; }

        public int Quantity { get; set; }
    }



    public class MenuItems
    {
        public string Menuname { get; set; }

        public string MenuDescription { get; set; }

    }


    public class Listofmenudish
    {
            public List<Dishes> Dishes { get; set; }
        public List<MenuItems> menus { get; set; }
    } 

    public class Dishes
    {
        public int  DishId { get; set; }

        public string DishName { get; set; }


        public decimal Price { get; set; }

        public string Imageurl { get; set; }

        public string Food { get; set; }


        public string Menuname { get; set; }


    }



    public class PaymentVerificationRequest
    {
        public string PaymentId { get; set; }
        public string OrderId { get; set; }
        public string Signature { get; set; }
    }




}
