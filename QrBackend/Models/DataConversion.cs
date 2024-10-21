using Microsoft.IdentityModel.Protocols;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Configuration;
using Microsoft.Extensions.Configuration;
using Razorpay.Api;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text; // Add this line

namespace QrBackend.Models
{
    public class DataConversion
    {
        SqlCommand sqlcmd = new SqlCommand();

        private readonly DataAccessLayer cs;
        private readonly IConfiguration _configuration;
        private readonly SqlConnection con;
        public DataConversion(DataAccessLayer _cs, IConfiguration configuration)
        {
            this.cs = _cs;
            _configuration = configuration;
            string connectionString = configuration.GetConnectionString("DefaultConnection");
            con = new SqlConnection(connectionString);
        }


        public Listofmenudish GetMenuDishes()
        {
            var listofmenusdishes = new Listofmenudish();
            var listofmenus = new List<MenuItems>();
            var listofdishes = new List<Dishes>();


            try
            {

                using (SqlCommand cmd = new SqlCommand("Sp_Get_Menu_Dishes"))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Clear();

                    var connectionString = _configuration.GetConnectionString("DefaultConnection");
                    var dt = cs.Get_Data_DS(cmd, new SqlConnection(connectionString));
                    if (dt != null && dt.Tables.Count > 0)
                    {

                        foreach (DataRow item in dt.Tables[0].Rows)
                        {
                            MenuItems menus = new MenuItems
                            {
                                Menuname = !string.IsNullOrEmpty(Convert.ToString(item["Menuname"])) ? Convert.ToString(item["Menuname"]) : "",
                                MenuDescription = !string.IsNullOrEmpty(Convert.ToString(item["Description"])) ? Convert.ToString(item["Description"]) : ""
                            };
                            listofmenus.Add(menus);
                        }



                        foreach (DataRow item in dt.Tables[1].Rows)
                        {
                            Dishes dish = new Dishes
                            {
                                DishId = !string.IsNullOrEmpty(Convert.ToString(item["DishId"])) ? Convert.ToInt32(item["DishId"]) : 0,
                                DishName = !string.IsNullOrEmpty(Convert.ToString(item["DishName"])) ? Convert.ToString(item["DishName"]) : "",
                                Price = !string.IsNullOrEmpty(Convert.ToString(item["Price"])) ? Convert.ToDecimal(item["Price"]) : 0,
                                Imageurl = !string.IsNullOrEmpty(Convert.ToString(item["Imageurl"])) ? Convert.ToString(item["Imageurl"]) : "",
                                Food = !string.IsNullOrEmpty(Convert.ToString(item["Food"])) ? Convert.ToString(item["Food"]) : "",
                                Menuname = !string.IsNullOrEmpty(Convert.ToString(item["Menuname"])) ? Convert.ToString(item["Menuname"]) : ""
                            };
                            listofdishes.Add(dish);
                        }

                        listofmenusdishes.menus = listofmenus;
                        listofmenusdishes.Dishes = listofdishes;


                        return listofmenusdishes;
                    }

                }
            }
            catch (Exception ex) {
                Console.WriteLine($"An error occurred: {ex.Message}");

            }


            return listofmenusdishes;

        }




        public int gettablestatus(int tablenumber, string action)
        {
            int status = 0;
            try
            {
                using (SqlCommand cmd = new SqlCommand("ManageTableLock"))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@tableNumber", tablenumber);
                    cmd.Parameters.AddWithValue("@action", action);
                    var dt = cs.Get_Data_DS(cmd);

                    // Check if the DataSet has the expected data
                    if (dt != null && dt.Tables.Count > 0 && dt.Tables[0].Rows.Count > 0)
                    {
                        // Check if the column exists and is of the correct type
                        if (dt.Tables[0].Columns.Contains("IsLocked"))
                        {
                            bool isLocked = dt.Tables[0].Rows[0].Field<bool>("IsLocked");
                            status = isLocked ? 1 : 0;                        }
                        else
                        {
                            throw new Exception("Column 'IsLocked' does not exist.");
                        }
                    }
                    else
                    {
                        throw new Exception("No data returned from the stored procedure.");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                // Optionally log the exception or handle it as needed
            }
            return status;
        }



        public int lockunlocktable(int tablenumber,string action)
        {
            int status = 0;
            try
            {
                using (SqlCommand cmd = new SqlCommand("ManageTableLock"))
                {
                    cmd.Parameters.AddWithValue("@tableNumber", tablenumber);
                    cmd.Parameters.AddWithValue("@action", action);
                    var dt = cs.Run_SPQuery(cmd);
                    if(dt!=null)
                    {
                        status = 1;
                    }
                    return status;
                }
            }
            catch (Exception ex) { }

            return status;
        }


        public int SaveOrderID(string OrderId,string customername, int tablenumber,decimal amount)
        { string PaymentStatus = "Pending";
            int status = 0;
            try
            {
                using (SqlCommand cmd = new SqlCommand("UspInsertOrder"))
                {
                    sqlcmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@OrderId", OrderId);
                    cmd.Parameters.AddWithValue("@customername", customername);
                    cmd.Parameters.AddWithValue("@tablenumber", tablenumber);
                    cmd.Parameters.AddWithValue("@amount", amount);
                    cmd.Parameters.AddWithValue("@paymentstatus", "pending");
                    var dt = cs.Run_SPQuery(cmd);
                    status = dt;

                }
            }
            catch (Exception ex) { }
            return status;
        }



        public int SaveVerfiedPayment(string paymentId, string orderId, string signature)
        {
            int result = 0;
            string paymentStatus = "Verified";

            if (string.IsNullOrEmpty(paymentId) || string.IsNullOrEmpty(orderId) || string.IsNullOrEmpty(signature))
            {
                throw new ArgumentException("PaymentId, OrderId, and Signature cannot be null or empty.");
            }

            try
            {
                using (SqlCommand cmd = new SqlCommand("uspInsertVerfiedPayment"))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Explicitly specify parameter types if needed
                    cmd.Parameters.Add(new SqlParameter("@PaymentId", SqlDbType.NVarChar) { Value = paymentId });
                    cmd.Parameters.Add(new SqlParameter("@OrderId", SqlDbType.NVarChar) { Value = orderId });
                    cmd.Parameters.Add(new SqlParameter("@Signature", SqlDbType.NVarChar) { Value = signature });
                    cmd.Parameters.Add(new SqlParameter("@PaymentStatus", SqlDbType.NVarChar) { Value = paymentStatus });

                    result = cs.Run_SPQuery(cmd); // Assuming cs is a properly defined database helper
                    return result > 0 ? result : 0;
                }
            }
            catch (Exception ex)
            {
                // Log the exception here (consider using a logging framework)
                Console.WriteLine($"Error saving verified payment: {ex.Message}");
                // Optionally rethrow or handle the exception as needed
            }

            return result;
        }

        public int saveOrderItems(string name, dynamic items, int TableNumber, string OrderId, dynamic Quantities)
        {
            int result = 0;

            // Convert items and quantities to comma-separated strings
            string itemIds = items != null ? Convert.ToString(items).Replace("\"", "") : ""; // Remove extra quotes
            string quantities = Quantities != null ? Convert.ToString(Quantities) : ""; // This will also be passed as VARCHAR(MAX)

            string customername = !string.IsNullOrEmpty(name) ? name : "";

            try
            {
                using (SqlCommand cmd = new SqlCommand("PlaceOrder"))
                {
                    cmd.Parameters.AddWithValue("@p_CustomerName", (object)customername ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@p_MenuItemIDs", (object)itemIds ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@p_Quantities", (object)quantities ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@p_TableNumber", (object)TableNumber ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@p_OrderID", (object)OrderId ?? DBNull.Value); // Removed extra space

                    result = cs.Run_SPQuery(cmd);
                    return result;
                }
            }
            catch (Exception ex)
            {
                // Handle exception (e.g., log it)
            }

            return result;
        }



        public List<OrderDetails> GetOrderedItems(int tableNumber)
        {
            List<OrderData> orderDataList = new List<OrderData>();
            List<OrderDetails> newMainList = new List<OrderDetails>();
            try
            {
                using (SqlCommand cmd = new SqlCommand("Get_Order_Details", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@TableNumber", tableNumber);

                    var dt = cs.Bind_DataTable(cmd);
                    if (dt.Rows.Count > 0)
                    {
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            OrderData data = new OrderData();
                            data.OrderId = !string.IsNullOrEmpty(Convert.ToString(dt.Rows[i]["OrderID"])) ? Convert.ToString(dt.Rows[i]["OrderID"]) : "";
                            data.DishName = !string.IsNullOrEmpty(Convert.ToString(dt.Rows[i]["DishName"])) ? Convert.ToString(dt.Rows[i]["DishName"]) : "";
                            data.Price = !string.IsNullOrEmpty(Convert.ToString(dt.Rows[i]["Price"])) ? Convert.ToDecimal(dt.Rows[i]["Price"]) : 0;
                            data.Food = !string.IsNullOrEmpty(Convert.ToString(dt.Rows[i]["Food"])) ? Convert.ToString(dt.Rows[i]["Food"]) : "";
                            data.Imageurl = !string.IsNullOrEmpty(Convert.ToString(dt.Rows[i]["Imageurl"])) ? Convert.ToString(dt.Rows[i]["Imageurl"]) : "";
                            data.Quantity = !string.IsNullOrEmpty(Convert.ToString(dt.Rows[i]["Quantity"])) ? Convert.ToInt32(dt.Rows[i]["Quantity"]) : 0;

                            orderDataList.Add(data);
                        }

                        var uniqueIds = orderDataList.Select(p => p.OrderId).Distinct().ToList();
                        foreach (var item in uniqueIds)
                        {
                            OrderDetails newDetails = new OrderDetails();
                            newDetails.orderdata = new List<OrderData>();
                            newDetails.OrderID = item;
                            newDetails.orderdata = orderDataList.Where(p => p.OrderId == item).ToList();
                            newMainList.Add(newDetails);
                        }
                    }
                }

            }
            catch (Exception ex) { }
            

            return newMainList; // Change to return newMainList if needed
        }





        public string getTableSessionCode(int tableNumber)
        {

            var  Code= "";


            try
            {
                using (SqlCommand cmd = new SqlCommand("CheckTable"))
                {
                    cmd.CommandType = CommandType.StoredProcedure; // Specify that it's a stored procedure

                    cmd.Parameters.AddWithValue("@Tablenumber", tableNumber);

                    var dt = cs.Bind_DataTable(cmd);
                    if(dt != null && dt.Rows.Count > 0)
                    {
                        Code = !string.IsNullOrEmpty(Convert.ToString(dt.Rows[0]["SessionCode"])) ? Convert.ToString(dt.Rows[0]["SessionCode"]) : "";
                    }
                    else
                    {
                        Code = "No session Code";

                    }


                }
            }

            catch (Exception ex) 
            {
               
                

            }
            return Code;
        }



    }
}
