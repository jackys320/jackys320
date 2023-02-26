using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using MvcPaging;

namespace WebApplication1.Models
{
    public class MreportSQL
    {

        /*
        [DisplayName("採購編號")]
        public int m_no { get; set; }
        */

        [DisplayName("編號")]
        public string m_no { get; set; }

        [DisplayName("年份")]
        [Required(ErrorMessage = "年份一定要輸入")]
        public string yy { get; set; }

        [DisplayName("月份")]
        [Required(ErrorMessage = "月份一定要輸入")]
        public string mm { get; set; }

        [DisplayName("大分類")]
        public string bt_no { get; set; }
        [DisplayName("小分類")]
        public string st_no { get; set; }

        [DisplayName("項目")]
        [Required(ErrorMessage = "項目一定要輸入")]
        public string it_no { get; set; }

        [DisplayName("名稱")]
        [Required(ErrorMessage = "項目一定要輸入")]
        public string it_name { get; set; }

        /*
        public string name_sav { get; set; }
        public string it_no_sav { get; set; }
        */
        [DisplayName("數量")]
        [Required(ErrorMessage = "數量一定要輸入")]
        public int qty { get; set; }

        [DisplayName("備註")]
        [Required(ErrorMessage = "備註一定要輸入")]
        public string remark { get; set; }
    }

    public class MreportQuerySQL
    {
        public string yy { get; set; }  // 搜尋條件1
        public string mm { get; set; }  // 搜尋條件1
        public string m_no { get; set; }  // 搜尋條件1
        public string it_name { get; set; }  // 搜尋條件1
        public string spec { get; set; }  // 搜尋條件1
        public int price { get; set; }  // 搜尋條件1
        public string unit { get; set; }  // 搜尋條件1
        public int qty { get; set; }  // 搜尋條件1
        public string remark { get; set; }  // 搜尋條件1

    }

    public class MreportViewModelSQL
    {
        // Properties
        public string yy { get; set; }  // 搜尋條件1

        public string mm { get; set; }  // 搜尋條件2

        public IPagedList<MreportQuerySQL> MreportQuerySQL { get; set; }  // 符合條件資料

        public int Page { get; set; }  // 頁碼


        // Constructors
        public MreportViewModelSQL()
        {
            //yy = string.Empty;
            yy = DateTime.Now.Year.ToString();
            //mm = string.Empty;
            mm = DateTime.Now.Month.ToString().PadLeft(2, '0');
            //mm = "10";
            Page = 0;
        }
    }

    public class MreportHandle
    {
        private SqlConnection con;
        private void connection()
        {
            string constring = ConfigurationManager.ConnectionStrings["studentconn"].ToString();
            con = new SqlConnection(constring);
        }

        // ********** VIEW STUDENT DETAILS ********************
        public List<MreportSQL> GetMreport()
        {
            List<MreportSQL> Mreportlist = new List<MreportSQL>();
            //SqlCommand Cmd = null;

            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["purchaseConnectionString"].ConnectionString;
            //string connectionString = "Data Source=Whatever; Initial Catalog=Northwind; Integrated security=True;";
            // your "normal" sql here
            string yl_id = "208";
            //string commandText = "select yy '年份',mm '月份',m_no '採購編號',item.it_name '名稱',spec '規格',price '價格',unit '單位',qty '數量',mreport.remark '備註'  "
            //      + " from mreport inner join item on item.it_no=mreport.it_no where yl_id=@yl_id and yy=@yy and mm=@mm order by m_no";
            string commandText = "select yy,mm,m_no,mreport.bt_no,mreport.st_no,mreport.it_no,item.it_name,qty,mreport.remark "
                  + " from mreport inner join item on item.it_no=mreport.it_no";

            List<string> result = new List<string>();
            var MreportQuerySQL = new List<MreportQuerySQL>();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(commandText, conn);
                conn.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Mreportlist.Add(new MreportSQL()
                        {
                            m_no = reader["m_no"].ToString(),
                            yy = reader["yy"].ToString(),
                            mm = reader["mm"].ToString(),
                            bt_no = reader["bt_no"].ToString(),
                            st_no = reader["st_no"].ToString(),
                            it_no = reader["it_no"].ToString(),
                            it_name = reader["it_name"].ToString(),
                            qty = Int32.Parse(reader["qty"].ToString()),
                            remark = reader["remark"].ToString(),
                        });
                    }
                }
            }

            return Mreportlist;
        }

        // **************** ADD NEW Mreport *********************
        public bool MreportCreate(MreportSQL smodel)
        {
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["purchaseConnectionString"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string yl_id = "208";
                string sql = @" insert mreport(m_no,yl_id,yy,mm,bt_no,st_no,it_no,qty,remark,create_date,modi_date)
                                 values(( Select Convert(varchar(8),Getdate(),112) + rtrim(ltrim(replicate('0',5-len(isnull(max(Convert(int,substring(m_no,9,5))),0)+1))+convert(varchar,isnull(max(Convert(int,substring(m_no,9,5))),0)+1))) from mreport where substring(m_no,1,8) = Convert(varchar(8),Getdate(),112)  ),
                                        @yl_id,@yy,@mm,@bt_no,@st_no,@it_no,@qty,@remark,getdate(),getdate()) ";
                SqlCommand cmd = new SqlCommand(sql, conn);
                conn.Open();
                cmd.Parameters.Add("@yl_id", SqlDbType.NVarChar, 50).Value = yl_id;
                cmd.Parameters.Add("@yy", SqlDbType.NVarChar, 50).Value = smodel.yy;
                cmd.Parameters.Add("@mm", SqlDbType.NVarChar, 50).Value = smodel.mm;
                cmd.Parameters.Add("@bt_no", SqlDbType.NVarChar, 50).Value = smodel.bt_no;
                cmd.Parameters.Add("@st_no", SqlDbType.NVarChar, 50).Value = smodel.st_no;
                cmd.Parameters.Add("@it_no", SqlDbType.NVarChar, 50).Value = smodel.it_no;
                cmd.Parameters.Add("@qty", SqlDbType.Int).Value = smodel.qty;
                cmd.Parameters.Add("@remark", SqlDbType.NVarChar, 100).Value = (smodel.remark == null ? "" : smodel.remark);

                try
                {
                    int i = cmd.ExecuteNonQuery();
                    if (i > 0)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                catch
                {
                    return false;
                }
            }

            /*
            connection();
            SqlCommand cmd = new SqlCommand("insert into AddNewStudent", con);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@Name", smodel.Name);
            cmd.Parameters.AddWithValue("@City", smodel.City);
            cmd.Parameters.AddWithValue("@Address", smodel.Address);

            con.Open();
            int i = cmd.ExecuteNonQuery();
            con.Close();

            if (i >= 1)
                return true;
            else
                return false;
            */
        }

        public bool MreportEdit(MreportSQL smodel)
        {
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["purchaseConnectionString"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string sql = @" update mreport set yy=@yy,mm=@mm,bt_no=@bt_no,st_no=@st_no,it_no=@it_no,qty=@qty,remark=@remark,modi_date=getdate() where m_no=@m_no ";
                SqlCommand cmd = new SqlCommand(sql, conn);
                conn.Open();
                cmd.Parameters.Add("@m_no", SqlDbType.NVarChar, 50).Value = smodel.m_no;
                cmd.Parameters.Add("@yy", SqlDbType.NVarChar, 50).Value = smodel.yy;
                cmd.Parameters.Add("@mm", SqlDbType.NVarChar, 50).Value = smodel.mm;
                cmd.Parameters.Add("@bt_no", SqlDbType.NVarChar, 50).Value = smodel.bt_no;
                cmd.Parameters.Add("@st_no", SqlDbType.NVarChar, 50).Value = smodel.st_no;
                cmd.Parameters.Add("@it_no", SqlDbType.NVarChar, 50).Value = smodel.it_no;
                cmd.Parameters.Add("@qty", SqlDbType.Int).Value = smodel.qty;
                cmd.Parameters.Add("@remark", SqlDbType.NVarChar, 100).Value = (smodel.remark == null ? "" : smodel.remark);

                try
                {
                    int i = cmd.ExecuteNonQuery();
                    if (i > 0)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                catch
                {
                    return false;
                }

            }
            
        }
        
        // ********************** DELETE STUDENT DETAILS *******************
        public bool MreportDelete(String id)
        {
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["purchaseConnectionString"].ConnectionString;

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string sql = @"delete from mreport where m_no=@m_no";
                SqlCommand cmd = new SqlCommand(sql, con);
                con.Open();

                cmd.Parameters.Add("@m_no", SqlDbType.NVarChar, 50).Value = id;

                int i = cmd.ExecuteNonQuery();
                if (i > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }
        }
    }
}