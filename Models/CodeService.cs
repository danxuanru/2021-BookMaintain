using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Web.Mvc;

namespace BookMaintain.Models
{
    public class CodeService
    {
        //取得DB連線字串
        private string GetDBConnectionString()
        {
            return System.Configuration.ConfigurationManager.ConnectionStrings["DBConn"].ConnectionString;
        }

        //取得書籍類別資料
        public List<SelectListItem> GetBookClass()
        {
            DataTable dt = new DataTable();
            string sql = @"Select BOOK_CLASS_ID AS Id, BOOK_CLASS_NAME AS Name FROM BOOK_CLASS";
            using (SqlConnection conn = new SqlConnection(this.GetDBConnectionString()))
            {
                conn.Open();
                SqlDataAdapter sqlAdapter = new SqlDataAdapter(sql, conn);

                sqlAdapter.Fill(dt);
                conn.Close();
            }
            return this.MapCodeData(dt);
        }

        // 取得書籍借閱狀態資料
        public List<SelectListItem> GetBookStatus()
        {
            DataTable dt = new DataTable();
            string sql = @"SELECT CODE_ID AS Id, CODE_NAME AS Name FROM BOOK_CODE WHERE CODE_TYPE='BOOK_STATUS'";
            using (SqlConnection conn = new SqlConnection(this.GetDBConnectionString()))
            {
                conn.Open();
                SqlDataAdapter sqlAdapter = new SqlDataAdapter(sql, conn);
                sqlAdapter.Fill(dt);
                conn.Close();
            }
            return this.MapCodeData(dt);
        }

        // 取得借閱者資料
        public List<SelectListItem> GetBookKeeper()
        {
            DataTable dt = new DataTable();
            string sql = @"SELECT USER_ID AS Id, USER_ENAME AS Name FROM MEMBER_M";
            using (SqlConnection conn = new SqlConnection(this.GetDBConnectionString()))
            {
                conn.Open();
                SqlDataAdapter sqlAdapter = new SqlDataAdapter(sql, conn);
                sqlAdapter.Fill(dt);
                conn.Close();
            }
            return this.MapCodeData(dt);
        }

        //Map Name and ID
        private List<SelectListItem> MapCodeData(DataTable dt)
        {
            List<SelectListItem> result = new List<SelectListItem>();
            foreach (DataRow row in dt.Rows)
            {
                result.Add(new SelectListItem()
                {
                    Text = row["Name"].ToString(),
                    Value = row["Id"].ToString()
                });
            }
            return result;
        }
    }
}