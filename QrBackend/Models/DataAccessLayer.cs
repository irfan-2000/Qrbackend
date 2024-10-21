
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace QrBackend.Models
{

    public class DataAccessLayer
    {
        private readonly SqlConnection con;
        public DataAccessLayer(IConfiguration configuration)
        {
            string connectionString = configuration.GetConnectionString("DefaultConnection");
            con = new SqlConnection(connectionString);

        }

      //  public SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);


        int i = 0;
        string cmd;
        SqlCommand sqlcmd = new SqlCommand();
        SqlDataReader sqldr;
        SqlDataAdapter sqlda;
        DataSet sqlds;
        int resint;

        string retdata;

        #region "Stored_Procedures"

        /// <summary>
        /// Run_s the sp query.
        /// </summary>
        /// <param name="sp_sqlcmd">The SP_SQLCMD.</param>
        public int Run_SPQuery(SqlCommand sp_sqlcmd)
        {
            sp_sqlcmd.CommandType = CommandType.StoredProcedure;
            sp_sqlcmd.Connection = con;
            int i = 0;
            try
            {
                con.Open();
                i = sp_sqlcmd.ExecuteNonQuery();
                con.Close();
            }
            catch (Exception ex)
            {
                con.Close();
            }
            return i;
        }

   
        public string ReadScalar_SP(SqlCommand sp_sqlcmd)
        {
            string retdata = "";
            try
            {
                sp_sqlcmd.CommandType = CommandType.StoredProcedure;
                sp_sqlcmd.Connection = con;
                con.Open();
                retdata = sp_sqlcmd.ExecuteScalar().ToString();
                con.Close();
                return retdata;
            }
            catch (Exception ex)
            {
                con.Close();
            }
            return retdata;

        }


        public double Get_Single_Data(SqlCommand sp_sqlcmd)
        {

            double num = 0;

            sp_sqlcmd.CommandType = CommandType.StoredProcedure;
            sp_sqlcmd.Connection = con;

            try
            {
                con.Open();
                num = double.Parse(sp_sqlcmd.ExecuteScalar().ToString());
                con.Close();

                return num;
            }
            catch (Exception ex)
            {
                con.Close();
                return 0;
            }

        }

        
        public DataSet Get_Data_DS(SqlCommand sp_sqlcmd)
        {

            sp_sqlcmd.CommandType = CommandType.StoredProcedure;
            sp_sqlcmd.Connection = con;

            SqlDataAdapter da = new SqlDataAdapter(sp_sqlcmd);
            DataSet ds = new DataSet();

            try
            {
                con.Open();
                da.Fill(ds);
                con.Close();
            }
            catch (Exception ex)
            {

              
                con.Close();
            }

            return ds;
        }

     

        public DataSet Get_Data_DS(SqlCommand sp_sqlcmd, SqlConnection Con)
        {

            sp_sqlcmd.CommandType = CommandType.StoredProcedure;
            sp_sqlcmd.Connection = Con;

            SqlDataAdapter da = new SqlDataAdapter(sp_sqlcmd);
            DataSet ds = new DataSet();

            try
            {
                Con.Open();
                da.Fill(ds);
                Con.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");

                Con.Close();
            }

            return ds;
        }

 
        public DataTable Bind_DataTable(SqlCommand sqlcmd)
        {
            dynamic da = new SqlDataAdapter();
            da.SelectCommand = sqlcmd;
            sqlcmd.Connection = con;

            dynamic dt = new DataTable();
            try
            {
                da.Fill(dt);
            }
            catch (Exception ex) { }
    

            return dt;
        }

        public DataTable Bind_DataTable(SqlCommand sqlcmd, SqlConnection connection)
        {
            dynamic da = new SqlDataAdapter();
            da.SelectCommand = sqlcmd;
            sqlcmd.Connection = connection;

            dynamic dt = new DataTable();
            try
            {
                da.Fill(dt);
            }
            catch (Exception ex)
            {
            }


            return dt;
        }



        public int Run_SPQuery(SqlCommand sp_sqlcmd, SqlConnection connection)
        {
            sp_sqlcmd.CommandType = CommandType.StoredProcedure;
            sp_sqlcmd.Connection = connection;
            int i = 0;
            try
            {
                connection.Open();
                i = sp_sqlcmd.ExecuteNonQuery();
                connection.Close();
            }
            catch (Exception ex)
            {
                connection.Close();
            }
            return i;
        }

        #endregion


    }
}