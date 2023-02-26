using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcPaging;
using WebApplication1.Models;
using System.Data.SqlClient;
using System.Data;
using System.Text;
using WebApplication1.Helpers;

namespace WebApplication1.Controllers
{
    public class PurchaseController : Controller
    {
        private purchaseEntities db = new purchaseEntities();

        private const int PageSize = 10;

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            String controllerName = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName;
            String actionName = filterContext.ActionDescriptor.ActionName;
            String user_group_no = (String)Session["user_group_no"];
            if (String.IsNullOrEmpty(user_group_no))
            {
                //重新定向至登入頁面
                //filterContext.Result = RedirectToAction("LoginFormsAuthentication", "LoginFormsAuthentication");
                filterContext.Result = RedirectToAction("Login", "Login");
                return;
            }

            /*
            if (User.Identity == null && User.Identity.IsAuthenticated == false)
            {//目前未登入 
                //return RedirectToAction("LoginFormsAuthentication", "LoginFormsAuthentication");
                return Redirect(FormsAuthentication.LoginUrl);
            }

            if (Session["user_group_no"] == null)
            {
                //return RedirectToAction("Login", "Login");
                //return RedirectToAction("LoginFormsAuthentication", "LoginFormsAuthentication");
                return Redirect(FormsAuthentication.LoginUrl);
            }
            */
        }

        // GET: Purchase
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult MreportMvcPaging()
        {
            // 進入搜尋頁面 不主動撈取資料
            MreportViewModelSQL viewModel = new MreportViewModelSQL();

            if (Session["yy"] != null)
            {
                viewModel.yy = (String)Session["yy"];
            }
            if (Session["mm"] != null)
            {
                viewModel.mm = (String)Session["mm"];
            }
            if (Session["Page"] != null && Session["Status"] != null)
            {
                viewModel.Page = (int)Session["Page"];
            }
            if (Request["page"] != null)
            {
                /* MvcPaging 是用 get 傳 page 到 cshtml(內部運作猜這時候會把Request["page"]設定到viewModel.Page)
                 底下原始程式寫法印證

                var page = Request["page"];
                if (page != null)
                {
                    viewModel.Page = int.Parse(Request["page"]);
                }

                viewModel.MreportQuerySQL = MreportQuerySQL.ToPagedList(viewModel.Page > 0 ? viewModel.Page - 1 : 0, PageSize);

                 所以把Request["page"]存到Session["Page"], 這樣修改存檔後回到MreportSQLMvcPaging才能用Session["Page"]回到原來的頁次
                < div class="pager">
                    <span class="disabled">«</span><span class="current">1</span><a href = "/Purchase/MreportSQLMvcPaging?page=2" title="">2</a><a href = "/Purchase/MreportSQLMvcPaging?page=2" title="Next page">»</a>
                    Displaying 1 - 10 of 11 item(s)
                </div>

                
                */
                Session["Page"] = int.Parse(Request["page"]);
            }
            else
            {
                //連續新增第二次回因為 Request["page"] == null 跑到第一頁
                Session["Page"] = 0;
            }

            //上下架的下拉選單的資料清單
            ViewData["mmlist"] = new List<SelectListItem>()
            {
              new SelectListItem(){ Text="請選擇",Value="-1",Selected=true},
              new SelectListItem(){ Text="1月",Value="01" },
              new SelectListItem(){ Text="2月",Value="02" },
              new SelectListItem(){ Text="3月",Value="03" },
              new SelectListItem(){ Text="4月",Value="04" },
              new SelectListItem(){ Text="5月",Value="05" },
              new SelectListItem(){ Text="6月",Value="06" },
              new SelectListItem(){ Text="7月",Value="07" },
              new SelectListItem(){ Text="8月",Value="08" },
              new SelectListItem(){ Text="9月",Value="09" },
              new SelectListItem(){ Text="10月",Value="10" },
              new SelectListItem(){ Text="11月",Value="11" },
              new SelectListItem(){ Text="12月",Value="12" },
            };

            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["purchaseConnectionString"].ConnectionString;
            //string connectionString = "Data Source=Whatever; Initial Catalog=Northwind; Integrated security=True;";
            // your "normal" sql here
            string yl_id = "208";
            //string commandText = "select yy '年份',mm '月份',m_no '採購編號',item.it_name '名稱',spec '規格',price '價格',unit '單位',qty '數量',mreport.remark '備註'  "
            //      + " from mreport inner join item on item.it_no=mreport.it_no where yl_id=@yl_id and yy=@yy and mm=@mm order by m_no";
            string commandText = "select yy,mm,m_no,item.it_name,spec,price,unit,qty,mreport.remark "
                  + " from mreport inner join item on item.it_no=mreport.it_no where yl_id=@yl_id and yy=@yy and mm=@mm order by m_no";

            List<string> result = new List<string>();
            var MreportQuerySQL = new List<MreportQuerySQL>();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(commandText, conn);
                cmd.Parameters.Add("@yl_id", SqlDbType.NVarChar, 100).Value = yl_id;
                cmd.Parameters.Add("@yy", SqlDbType.NVarChar, 100).Value = viewModel.yy;
                cmd.Parameters.Add("@mm", SqlDbType.NVarChar, 100).Value = viewModel.mm; // ViewData["mm"];
                conn.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        MreportQuerySQL.Add(new MreportQuerySQL()
                        {
                            /*
                            product_no = reader.GetString(0),
                            name = reader.GetString(1),
                            price = reader.GetInt32(2),
                            remark = reader.GetString(3),
                            product_type_name = reader.GetString(4)
                            */

                            yy = reader["yy"].ToString(),
                            mm = reader["mm"].ToString(),
                            m_no = reader["m_no"].ToString(),
                            it_name = reader["it_name"].ToString(),
                            spec = reader["spec"].ToString(),
                            price = Int32.Parse(reader["price"].ToString()),
                            qty = Int32.Parse(reader["qty"].ToString()),
                            remark = reader["remark"].ToString(),
                        });
                    }
                }
            }

            var page = Request["page"];
            if (page != null)
            {
                viewModel.Page = int.Parse(Request["page"]);
            }

            viewModel.MreportQuerySQL = MreportQuerySQL.ToPagedList(viewModel.Page > 0 ? viewModel.Page - 1 : 0, PageSize);

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult MreportMvcPaging(MreportViewModelSQL viewModel)
        {
            //上下架的下拉選單的資料清單
            var mm = Request["mm"];
            Session["yy"] = viewModel.yy;
            Session["mm"] = mm;

            /*
            if (mm != null)
            {
                ViewData["mm"] = mm;
            }
            ViewData["yy"] = viewModel.yy;
            */

            ViewData["mmlist"] = new List<SelectListItem>()
            {
              new SelectListItem(){ Text="請選擇",Value="-1",Selected=true},
              new SelectListItem(){ Text="1月",Value="01" },
              new SelectListItem(){ Text="2月",Value="02" },
              new SelectListItem(){ Text="3月",Value="03" },
              new SelectListItem(){ Text="4月",Value="04" },
              new SelectListItem(){ Text="5月",Value="05" },
              new SelectListItem(){ Text="6月",Value="06" },
              new SelectListItem(){ Text="7月",Value="07" },
              new SelectListItem(){ Text="8月",Value="08" },
              new SelectListItem(){ Text="9月",Value="09" },
              new SelectListItem(){ Text="10月",Value="10" },
              new SelectListItem(){ Text="11月",Value="11" },
              new SelectListItem(){ Text="12月",Value="12" },
            };

            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["purchaseConnectionString"].ConnectionString;
            //string connectionString = "Data Source=Whatever; Initial Catalog=Northwind; Integrated security=True;";
            // your "normal" sql here
            string yl_id = "208";
            //string commandText = "select yy '年份',mm '月份',m_no '採購編號',item.it_name '名稱',spec '規格',price '價格',unit '單位',qty '數量',mreport.remark '備註'  "
            //      + " from mreport inner join item on item.it_no=mreport.it_no where yl_id=@yl_id and yy=@yy and mm=@mm order by m_no";
            string commandText = "select yy,mm,m_no,item.it_name,spec,price,unit,qty,mreport.remark "
                  + " from mreport inner join item on item.it_no=mreport.it_no where yl_id=@yl_id and yy=@yy and mm=@mm order by m_no";

            List<string> result = new List<string>();
            var MreportQuerySQL = new List<MreportQuerySQL>();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(commandText, conn);
                cmd.Parameters.Add("@yl_id", SqlDbType.NVarChar, 100).Value = yl_id;
                cmd.Parameters.Add("@yy", SqlDbType.NVarChar, 100).Value = viewModel.yy;
                cmd.Parameters.Add("@mm", SqlDbType.NVarChar, 100).Value = mm;
                conn.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        MreportQuerySQL.Add(new MreportQuerySQL()
                        {
                            /*
                            product_no = reader.GetString(0),
                            name = reader.GetString(1),
                            price = reader.GetInt32(2),
                            remark = reader.GetString(3),
                            product_type_name = reader.GetString(4)
                            */

                            yy = reader["yy"].ToString(),
                            mm = reader["mm"].ToString(),
                            m_no = reader["m_no"].ToString(),
                            it_name = reader["it_name"].ToString(),
                            spec = reader["spec"].ToString(),
                            price = Int32.Parse(reader["price"].ToString()),
                            qty = Int32.Parse(reader["qty"].ToString()),
                            remark = reader["remark"].ToString(),
                        });
                    }
                }
            }

            viewModel.MreportQuerySQL = MreportQuerySQL.ToPagedList(viewModel.Page > 0 ? viewModel.Page - 1 : 0, PageSize);

            return View(viewModel);
        }

        public ActionResult MreportSQLMvcPaging()
        {
            // 進入搜尋頁面 不主動撈取資料
            MreportViewModelSQL viewModel = new MreportViewModelSQL();

            if (Session["yy"] != null)
            {
                viewModel.yy = (String)Session["yy"];
            }
            if (Session["mm"] != null)
            {
                viewModel.mm = (String)Session["mm"];
            }
            if (Session["Page"] != null && Session["Status"] != null)
            {
                viewModel.Page = (int)Session["Page"];
            }
            if (Request["page"] != null)
            {
                /* MvcPaging 是用 get 傳 page 到 cshtml(內部運作猜這時候會把Request["page"]設定到viewModel.Page)
                 底下原始程式寫法印證

                var page = Request["page"];
                if (page != null)
                {
                    viewModel.Page = int.Parse(Request["page"]);
                }

                viewModel.MreportQuerySQL = MreportQuerySQL.ToPagedList(viewModel.Page > 0 ? viewModel.Page - 1 : 0, PageSize);

                 所以把Request["page"]存到Session["Page"], 這樣修改存檔後回到MreportSQLMvcPaging才能用Session["Page"]回到原來的頁次
                < div class="pager">
                    <span class="disabled">«</span><span class="current">1</span><a href = "/Purchase/MreportSQLMvcPaging?page=2" title="">2</a><a href = "/Purchase/MreportSQLMvcPaging?page=2" title="Next page">»</a>
                    Displaying 1 - 10 of 11 item(s)
                </div>

                
                */
                Session["Page"] = int.Parse(Request["page"]);
            }
            else
            {
                //連續新增第二次回因為 Request["page"] == null 跑到第一頁
                Session["Page"] = 0;
            }

            //上下架的下拉選單的資料清單
            ViewData["mmlist"] = new List<SelectListItem>()
            {
              new SelectListItem(){ Text="請選擇",Value="-1",Selected=true},
              new SelectListItem(){ Text="1月",Value="01" },
              new SelectListItem(){ Text="2月",Value="02" },
              new SelectListItem(){ Text="3月",Value="03" },
              new SelectListItem(){ Text="4月",Value="04" },
              new SelectListItem(){ Text="5月",Value="05" },
              new SelectListItem(){ Text="6月",Value="06" },
              new SelectListItem(){ Text="7月",Value="07" },
              new SelectListItem(){ Text="8月",Value="08" },
              new SelectListItem(){ Text="9月",Value="09" },
              new SelectListItem(){ Text="10月",Value="10" },
              new SelectListItem(){ Text="11月",Value="11" },
              new SelectListItem(){ Text="12月",Value="12" },
            };

            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["purchaseConnectionString"].ConnectionString;
            //string connectionString = "Data Source=Whatever; Initial Catalog=Northwind; Integrated security=True;";
            // your "normal" sql here
            string yl_id = "208";
            //string commandText = "select yy '年份',mm '月份',m_no '採購編號',item.it_name '名稱',spec '規格',price '價格',unit '單位',qty '數量',mreport.remark '備註'  "
            //      + " from mreport inner join item on item.it_no=mreport.it_no where yl_id=@yl_id and yy=@yy and mm=@mm order by m_no";
            string commandText = "select yy,mm,m_no,item.it_name,spec,price,unit,qty,mreport.remark "
                  + " from mreport inner join item on item.it_no=mreport.it_no where yl_id=@yl_id and yy=@yy and mm=@mm order by m_no";

            List<string> result = new List<string>();
            var MreportQuerySQL = new List<MreportQuerySQL>();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(commandText, conn);
                cmd.Parameters.Add("@yl_id", SqlDbType.NVarChar, 100).Value = yl_id;
                cmd.Parameters.Add("@yy", SqlDbType.NVarChar, 100).Value = viewModel.yy;
                cmd.Parameters.Add("@mm", SqlDbType.NVarChar, 100).Value = viewModel.mm; // ViewData["mm"];
                conn.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        MreportQuerySQL.Add(new MreportQuerySQL()
                        {
                            /*
                            product_no = reader.GetString(0),
                            name = reader.GetString(1),
                            price = reader.GetInt32(2),
                            remark = reader.GetString(3),
                            product_type_name = reader.GetString(4)
                            */

                            yy = reader["yy"].ToString(),
                            mm = reader["mm"].ToString(),
                            m_no = reader["m_no"].ToString(),
                            it_name = reader["it_name"].ToString(),
                            spec = reader["spec"].ToString(),
                            price = Int32.Parse(reader["price"].ToString()),
                            qty = Int32.Parse(reader["qty"].ToString()),
                            remark = reader["remark"].ToString(),
                        });
                    }
                }
            }

            var page = Request["page"];
            if (page != null)
            {
                viewModel.Page = int.Parse(Request["page"]);
            }

            viewModel.MreportQuerySQL = MreportQuerySQL.ToPagedList(viewModel.Page > 0 ? viewModel.Page - 1 : 0, PageSize);

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult MreportSQLMvcPaging(MreportViewModelSQL viewModel)
        {
            //上下架的下拉選單的資料清單
            var mm = Request["mm"];
            Session["yy"] = viewModel.yy;
            Session["mm"] = mm;

            /*
            if (mm != null)
            {
                ViewData["mm"] = mm;
            }
            ViewData["yy"] = viewModel.yy;
            */

            ViewData["mmlist"] = new List<SelectListItem>()
            {
              new SelectListItem(){ Text="請選擇",Value="-1",Selected=true},
              new SelectListItem(){ Text="1月",Value="01" },
              new SelectListItem(){ Text="2月",Value="02" },
              new SelectListItem(){ Text="3月",Value="03" },
              new SelectListItem(){ Text="4月",Value="04" },
              new SelectListItem(){ Text="5月",Value="05" },
              new SelectListItem(){ Text="6月",Value="06" },
              new SelectListItem(){ Text="7月",Value="07" },
              new SelectListItem(){ Text="8月",Value="08" },
              new SelectListItem(){ Text="9月",Value="09" },
              new SelectListItem(){ Text="10月",Value="10" },
              new SelectListItem(){ Text="11月",Value="11" },
              new SelectListItem(){ Text="12月",Value="12" },
            };

            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["purchaseConnectionString"].ConnectionString;
            //string connectionString = "Data Source=Whatever; Initial Catalog=Northwind; Integrated security=True;";
            // your "normal" sql here
            string yl_id = "208";
            //string commandText = "select yy '年份',mm '月份',m_no '採購編號',item.it_name '名稱',spec '規格',price '價格',unit '單位',qty '數量',mreport.remark '備註'  "
            //      + " from mreport inner join item on item.it_no=mreport.it_no where yl_id=@yl_id and yy=@yy and mm=@mm order by m_no";
            string commandText = "select yy,mm,m_no,item.it_name,spec,price,unit,qty,mreport.remark "
                  + " from mreport inner join item on item.it_no=mreport.it_no where yl_id=@yl_id and yy=@yy and mm=@mm order by m_no";

            List<string> result = new List<string>();
            var MreportQuerySQL = new List<MreportQuerySQL>();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(commandText, conn);
                cmd.Parameters.Add("@yl_id", SqlDbType.NVarChar, 100).Value = yl_id;
                cmd.Parameters.Add("@yy", SqlDbType.NVarChar, 100).Value = viewModel.yy;
                cmd.Parameters.Add("@mm", SqlDbType.NVarChar, 100).Value = mm;
                conn.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        MreportQuerySQL.Add(new MreportQuerySQL()
                        {
                            /*
                            product_no = reader.GetString(0),
                            name = reader.GetString(1),
                            price = reader.GetInt32(2),
                            remark = reader.GetString(3),
                            product_type_name = reader.GetString(4)
                            */

                            yy = reader["yy"].ToString(),
                            mm = reader["mm"].ToString(),
                            m_no = reader["m_no"].ToString(),
                            it_name = reader["it_name"].ToString(),
                            spec = reader["spec"].ToString(),
                            price = Int32.Parse(reader["price"].ToString()),
                            qty = Int32.Parse(reader["qty"].ToString()),
                            remark = reader["remark"].ToString(),
                        });
                    }
                }
            }

            viewModel.MreportQuerySQL = MreportQuerySQL.ToPagedList(viewModel.Page > 0 ? viewModel.Page - 1 : 0, PageSize);

            return View(viewModel);
        }

        public ActionResult MreportCreate(String yy,String mm, MreportViewModelSQL MreportViewModelSQL)
        {
            MreportSQL MreportView = new MreportSQL();
            MreportView.yy = yy;
            MreportView.mm = mm;
            MreportView.yy = MreportViewModelSQL.yy;
            MreportView.mm = MreportViewModelSQL.mm;

            item_set(MreportViewModelSQL);
            return View(MreportView);
        }

        // POST: Mreport/Create
        [HttpPost]
        public ActionResult MreportCreate(MreportSQL smodel)
        {
            try
            {
                //if (ModelState.IsValid)
                //{
                //很奇怪it_no DropDownList 原來選擇的項目都不會清掉加上這次畫面帶回來的會有兩個值, 加在後面 2017081500776,2017081500776
                //很奇怪mm DropDownList 原來選擇的項目都不會清掉加上這次畫面帶回來的會有兩個值, 卻是加在前面 如 09,01
                MreportHandle sdb = new MreportHandle();
                var bt_no = (Request["bt"] == null ? "" : Request["bt"]);
                var st_no = (Request["st"] == null ? "" : Request["st"]);
                var it_no = (Request["it"] == null ? "" : Request["it"]);
                /*
                smodel.bt_no = bt_no.Substring(bt_no.IndexOf(",") + 1, 13);
                //smodel.st_no = st_no.Substring(st_no.IndexOf(",") + 1, 13);
                smodel.st_no = "";
                smodel.it_no = it_no.Substring(it_no.IndexOf(",") + 1, 13);
                */
                smodel.bt_no = bt_no;
                smodel.st_no = st_no;
                smodel.it_no = it_no;

                var month = (Request["month"] == null ? "" : Request["month"]);
                smodel.mm = month;

                String mno = "";
                String yl_id = "208";

                using (SqlConnection con = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["purchaseConnectionString"].ConnectionString))
                {
                    SqlDataReader r1;
                    string sql = @"Select Convert(varchar(8),Getdate(),112) + rtrim(ltrim(replicate('0',5-len(isnull(max(Convert(int,substring(m_no,9,5))),0)+1))+convert(varchar,isnull(max(Convert(int,substring(m_no,9,5))),0)+1))) from mreport where substring(m_no,1,8) = Convert(varchar(8),Getdate(),112)";
                    SqlCommand cmd = new SqlCommand(sql, con);
                    con.Open();

                    r1 = cmd.ExecuteReader();
                    if (r1.Read())
                    {
                        mno = r1[0].ToString();
                    }

                }

                var mreport = new mreport
                {
                    m_no = mno,
                    yl_id = yl_id,
                    yy = smodel.yy,
                    mm = smodel.mm,
                    bt_no = smodel.bt_no,
                    st_no = smodel.st_no,
                    it_no = smodel.it_no,
                    qty = smodel.qty,
                    remark = (smodel.remark == null ? "" : smodel.remark),
                    mt_no = "",
                };

                db.mreport.Add(mreport);
                db.SaveChanges();
                ViewBag.Message = "新增完成";
                Session["Status"] = "新增完成";
                ModelState.Clear();

                return RedirectToAction("MreportMvcPaging");
            }
            catch
            { 
                return View();
            }
        }

        // 3. ************* EDIT Mreport DETAILS ******************
        // GET: Mreport/Edit/5
        public ActionResult MreportEdit(String id)
        {
            MreportHandle sdb = new MreportHandle();
            item_set(sdb.GetMreport().Find(smodel => smodel.m_no == id));
            return View(sdb.GetMreport().Find(smodel => smodel.m_no == id));
        }

        // POST: Mreport/Edit/5
        [HttpPost]
        public ActionResult MreportEdit(String id, MreportSQL smodel)
        {
            try
            {
                MreportHandle sdb = new MreportHandle();
                var bt_no = (Request["bt"] == null ? "" : Request["bt"]);
                var st_no = (Request["st"] == null ? "" : Request["st"]);
                var it_no = (Request["it"] == null ? "" : Request["it"]);
                /*
                smodel.bt_no = bt_no.Substring(bt_no.IndexOf(",") + 1, 13);
                //smodel.st_no = st_no.Substring(st_no.IndexOf(",") + 1, 13);
                smodel.st_no = "";
                smodel.it_no = it_no.Substring(it_no.IndexOf(",") + 1, 13);
                */
                smodel.bt_no = bt_no;
                smodel.st_no = st_no;
                smodel.it_no = it_no;

                mreport mreport = db.mreport.Find(id);

                mreport.yy = smodel.yy;
                mreport.mm = smodel.mm;
                mreport.bt_no = smodel.bt_no;
                mreport.st_no = smodel.st_no;
                mreport.it_no = smodel.it_no;
                mreport.qty = smodel.qty;
                mreport.remark = (smodel.remark == null ? "" : smodel.remark);
                db.SaveChanges();

                ViewBag.Message = "修改完成";
                Session["Status"] = "修改完成";
                //item_set(smodel);
                //return View();

                return RedirectToAction("MreportMvcPaging");
            }
            catch
            {
                return View();
            }
        }

        // 4. ************* DELETE Mreport DETAILS ******************
        // GET: Mreport/Delete/5
        public ActionResult MreportDelete(String id)
        {
            try
            {
                mreport mreport = db.mreport.Find(id);

                db.mreport.Remove(mreport);
                db.SaveChanges();

                ViewBag.Message = "刪除完成";
                Session["Status"] = "刪除完成";

                return RedirectToAction("MreportMvcPaging");
            }
            catch
            {
                return View();
            }
        }

        // 1. *************RETRIEVE ALL Mreport DETAILS ******************
        // GET: Mreport

        // 2. *************ADD NEW Mreport ******************
        // GET: Mreport/Create
        public ActionResult MreportSQLCreate(String yy, String mm, MreportViewModelSQL MreportViewModelSQL)
        {
            MreportSQL MreportView = new MreportSQL();
            MreportView.yy = yy;
            MreportView.mm = mm;
            MreportView.yy = MreportViewModelSQL.yy;
            MreportView.mm = MreportViewModelSQL.mm;

            item_set(MreportViewModelSQL);

            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["purchaseConnectionString"].ConnectionString;
            string sql = "select bt_no,bt_name from btype order by bt_name";

            List<CheckBoxListInfo> infos = new List<CheckBoxListInfo>();
            using (SqlConnection conn2 = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(sql, conn2);
                conn2.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        infos.Add(new CheckBoxListInfo(reader["bt_no"].ToString(), reader["bt_name"].ToString(), false));
                    }
                }
            }

            ViewData["btcheckbox"] = infos;

            List<SelectListItem> btls = new List<SelectListItem>();
            using (SqlConnection conn2 = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(sql, conn2);
                conn2.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        btls.Add(new SelectListItem() { Text = reader["bt_name"].ToString(), Value = reader["bt_no"].ToString() });
                    }
                }
            }

            ViewBag.btls = btls;

            return View(MreportView);
        }

        // POST: Mreport/Create
        [HttpPost]
        public ActionResult MreportSQLCreate(MreportSQL smodel)
        {
            try
            {
                //if (ModelState.IsValid)
                //{
                MreportHandle sdb = new MreportHandle();
                //很奇怪it_no DropDownList 原來選擇的項目都不會清掉加上這次畫面帶回來的會有兩個值, 加在後面 2017081500776,2017081500776
                //很奇怪mm DropDownList 原來選擇的項目都不會清掉加上這次畫面帶回來的會有兩個值, 卻是加在前面 如 09,01

                var bt_no = (Request["bt"] == null ? "" : Request["bt"]);
                var st_no = (Request["st"] == null ? "" : Request["st"]);
                var it_no = (Request["it"] == null ? "" : Request["it"]);
                /*
                smodel.bt_no = bt_no.Substring(bt_no.IndexOf(",") + 1, 13);
                //smodel.st_no = st_no.Substring(st_no.IndexOf(",") + 1, 13);
                smodel.st_no = "";
                smodel.it_no = it_no.Substring(it_no.IndexOf(",") + 1, 13);
                */
                smodel.bt_no = bt_no;
                smodel.st_no = st_no;
                smodel.it_no = it_no;

                var btchk = (Request["btchk"] == null ? "" : Request["btchk"]);
                smodel.remark = btchk;

                var month = (Request["month"] == null ? "" : Request["month"]);
                smodel.mm = month;

                if (sdb.MreportCreate(smodel))
                {
                    ViewBag.Message = "新增完成";
                    Session["Status"] = "新增完成";
                    ModelState.Clear();
                }
                //}
                //return View();
                return RedirectToAction("MreportSQLMvcPaging");
            }
            catch
            {
                return View();
            }
        }

        // 3. ************* EDIT Mreport DETAILS ******************
        // GET: Mreport/Edit/5
        public ActionResult MreportSQLEdit(String id)
        {
            MreportHandle sdb = new MreportHandle();
            item_set(sdb.GetMreport().Find(smodel => smodel.m_no == id));

            String[] chks = sdb.GetMreport().Find(smodel => smodel.m_no == id).remark.Split(',');
            int chked = 0;

            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["purchaseConnectionString"].ConnectionString;
            string sql = "select bt_no,bt_name from btype order by bt_name";

            List<CheckBoxListInfo> infos = new List<CheckBoxListInfo>();
            using (SqlConnection conn2 = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(sql, conn2);
                conn2.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        foreach (string chk in chks)
                        {
                            if (reader["bt_no"].ToString().Equals(chk))
                            {
                                chked = 1;
                            }
                        }
                        if (chked == 1)
                        {
                            infos.Add(new CheckBoxListInfo(reader["bt_no"].ToString(), reader["bt_name"].ToString(), true));
                        }
                        else
                        {
                            infos.Add(new CheckBoxListInfo(reader["bt_no"].ToString(), reader["bt_name"].ToString(), false));
                        }
                        chked = 0;
                    }
                }
            }

            ViewData["btcheckbox"] = infos;

            chked = 0;
            List<SelectListItem> btls = new List<SelectListItem>();
            using (SqlConnection conn2 = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(sql, conn2);
                conn2.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        foreach (string chk in chks)
                        {
                            if (reader["bt_no"].ToString().Equals(chk))
                            {
                                chked = 1;
                            }
                        }
                        if (chked == 1)
                        {
                            btls.Add(new SelectListItem() { Text = reader["bt_name"].ToString(), Value = reader["bt_no"].ToString(), Selected=true });
                        }
                        else
                        {
                            btls.Add(new SelectListItem() { Text = reader["bt_name"].ToString(), Value = reader["bt_no"].ToString() });
                        }
                        chked = 0;
                    }
                }
            }

            ViewBag.btls = btls;

            return View(sdb.GetMreport().Find(smodel => smodel.m_no == id));
        }

        // POST: Mreport/Edit/5
        [HttpPost]
        public ActionResult MreportSQLEdit(String id, MreportSQL smodel)
        {
            try
            {
                MreportHandle sdb = new MreportHandle();
                var bt_no = (Request["bt"] == null ? "" : Request["bt"]);
                var st_no = (Request["st"] == null ? "" : Request["st"]);
                var it_no = (Request["it"] == null ? "" : Request["it"]);
                /*
                smodel.bt_no = bt_no.Substring(bt_no.IndexOf(",") + 1, 13);
                //smodel.st_no = st_no.Substring(st_no.IndexOf(",") + 1, 13);
                smodel.st_no = "";
                smodel.it_no = it_no.Substring(it_no.IndexOf(",") + 1, 13);
                */
                smodel.bt_no = bt_no;
                smodel.st_no = st_no;
                smodel.it_no = it_no;

                var btchk = (Request["btchk"] == null ? "" : Request["btchk"]);
                smodel.remark = btchk;

                //smodel.it_no = Request["it_no"].Substring(;
                if (sdb.MreportEdit(smodel))
                {
                    ViewBag.Message = "修改完成";
                    Session["Status"] = "修改完成";
                    //item_set(smodel);
                    //return View();
                }
                return RedirectToAction("MreportSQLMvcPaging");
            }
            catch
            {
                return View();
            }
        }

        // 4. ************* DELETE Mreport DETAILS ******************
        // GET: Mreport/Delete/5
        public ActionResult MreportSQLDelete(String id)
        {
            try
            {
                MreportHandle sdb = new MreportHandle();
                if (sdb.MreportDelete(id))
                {
                    ViewBag.Message = "刪除完成";
                    Session["Status"] = "刪除完成";
                }


                return RedirectToAction("MreportSQLMvcPaging");
            }
            catch
            {
                return View();
            }
        }

        private void item_set(MreportViewModelSQL MreportViewModelSQL)
        {
            ViewData["mmlist"] = new List<SelectListItem>()
            {
              new SelectListItem(){ Text="請選擇",Value="-1",Selected=MreportViewModelSQL.mm.Equals("-1")},
              new SelectListItem(){ Text="1月",Value="01",Selected=MreportViewModelSQL.mm.Equals("01") },
              new SelectListItem(){ Text="2月",Value="02",Selected=MreportViewModelSQL.mm.Equals("02") },
              new SelectListItem(){ Text="3月",Value="03",Selected=MreportViewModelSQL.mm.Equals("03") },
              new SelectListItem(){ Text="4月",Value="04",Selected=MreportViewModelSQL.mm.Equals("04") },
              new SelectListItem(){ Text="5月",Value="05",Selected=MreportViewModelSQL.mm.Equals("05") },
              new SelectListItem(){ Text="6月",Value="06",Selected=MreportViewModelSQL.mm.Equals("06") },
              new SelectListItem(){ Text="7月",Value="07",Selected=MreportViewModelSQL.mm.Equals("07") },
              new SelectListItem(){ Text="8月",Value="08",Selected=MreportViewModelSQL.mm.Equals("08") },
              new SelectListItem(){ Text="9月",Value="09",Selected=MreportViewModelSQL.mm.Equals("09") },
              new SelectListItem(){ Text="10月",Value="10",Selected=MreportViewModelSQL.mm.Equals("10") },
              new SelectListItem(){ Text="11月",Value="11",Selected=MreportViewModelSQL.mm.Equals("11") },
              new SelectListItem(){ Text="12月",Value="12",Selected=MreportViewModelSQL.mm.Equals("12") },
            };


            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["purchaseConnectionString"].ConnectionString;
            string sql = "select bt_no,bt_name from btype order by bt_name";

            List<SelectListItem> btls = new List<SelectListItem>();
            using (SqlConnection conn2 = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(sql, conn2);
                conn2.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        btls.Add(new SelectListItem() { Text = reader["bt_name"].ToString() , Value = reader["bt_no"].ToString() });
                    }
                }
            }

            ViewData["btlist"] = btls;

            //sql = "select st_no,st_name from stype where bt_no like '" + bt_no.SelectedValue + "%" + "' order by st_name";
            sql = "select st_no,st_name from stype where bt_no like '" + btls[0].Value.ToString() + "%" + "' order by st_name";

            List<SelectListItem> stls = new List<SelectListItem>();
            //ViewData["btlist"] = new List<SelectListItem>()
            using (SqlConnection conn2 = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(sql, conn2);
                conn2.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        btls.Add(new SelectListItem() { Text = reader["st_name"].ToString(), Value = reader["st_no"].ToString() });
                    }
                }
            }

            ViewData["stlist"] = stls;

            //sql = "select st_no,st_name from stype where bt_no like '" + bt_no.SelectedValue + "%" + "' order by st_name";
            //Sql = "select it_no,it_name+'(規格:'+isnull(spec,'')+' 價格:'+convert(varchar,isnull(price,0))+' 單位:'+isnull(unit,'')+')' it_name " +
            //      " from item where bt_no like '" + bt_no.SelectedValue + "%" + "' and st_no like '" + st_no.SelectedValue + "%" + "' order by it_name";
            sql = "select it_no,it_name+'(規格:'+isnull(spec,'')+' 價格:'+convert(varchar,isnull(price,0))+' 單位:'+isnull(unit,'')+')' it_name " +
                  " from item where bt_no like '" + btls[0].Value.ToString() + "%" + "' order by it_name";

            List<SelectListItem> itls = new List<SelectListItem>();
            //ViewData["btlist"] = new List<SelectListItem>()
            //btls.Add(new SelectListItem() { Text = "請選擇", Value = "-1", Selected = true });
            using (SqlConnection conn2 = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(sql, conn2);
                conn2.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        itls.Add(new SelectListItem() { Text = reader["it_name"].ToString(), Value = reader["it_no"].ToString() });
                    }
                }
            }

            ViewData["itlist"] = itls;

        }

        private void item_set(MreportSQL Mreport)
        {
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["purchaseConnectionString"].ConnectionString;
            string sql = "select bt_no,bt_name from btype order by bt_name";

            List<SelectListItem> btls = new List<SelectListItem>();
            using (SqlConnection conn2 = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(sql, conn2);
                conn2.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (Mreport.bt_no.Equals(reader["bt_no"].ToString()))
                        {
                            btls.Add(new SelectListItem() { Text = reader["bt_name"].ToString(), Value = reader["bt_no"].ToString(), Selected = true });
                        }
                        else
                        {
                            btls.Add(new SelectListItem() { Text = reader["bt_name"].ToString(), Value = reader["bt_no"].ToString() });
                        }
                    }
                }
            }

            ViewData["btlist"] = btls;

            //sql = "select st_no,st_name from stype where bt_no like '" + bt_no.SelectedValue + "%" + "' order by st_name";
            sql = "select st_no,st_name from stype where bt_no like '" + Mreport.bt_no + "%" + "' order by st_name";

            List<SelectListItem> stls = new List<SelectListItem>();
            //ViewData["btlist"] = new List<SelectListItem>()
            using (SqlConnection conn2 = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(sql, conn2);
                conn2.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        btls.Add(new SelectListItem() { Text = reader["st_name"].ToString(), Value = reader["st_no"].ToString() });
                    }
                }
            }

            ViewData["stlist"] = stls;

            //sql = "select st_no,st_name from stype where bt_no like '" + bt_no.SelectedValue + "%" + "' order by st_name";
            //Sql = "select it_no,it_name+'(規格:'+isnull(spec,'')+' 價格:'+convert(varchar,isnull(price,0))+' 單位:'+isnull(unit,'')+')' it_name " +
            //      " from item where bt_no like '" + bt_no.SelectedValue + "%" + "' and st_no like '" + st_no.SelectedValue + "%" + "' order by it_name";
            sql = "select it_no,it_name+'(規格:'+isnull(spec,'')+' 價格:'+convert(varchar,isnull(price,0))+' 單位:'+isnull(unit,'')+')' it_name " +
                  " from item where bt_no like '" + Mreport.bt_no + "%" + "' and st_no like '" + Mreport.st_no + "%" + "' order by it_name";

            List<SelectListItem> itls = new List<SelectListItem>();
            //ViewData["btlist"] = new List<SelectListItem>()
            //btls.Add(new SelectListItem() { Text = "請選擇", Value = "-1", Selected = true });
            using (SqlConnection conn2 = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(sql, conn2);
                conn2.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (Mreport.it_no.Equals(reader["it_no"].ToString()))
                        {
                            itls.Add(new SelectListItem() { Text = reader["it_name"].ToString(), Value = reader["it_no"].ToString(), Selected = true });
                        }
                        else
                        {
                            itls.Add(new SelectListItem() { Text = reader["it_name"].ToString(), Value = reader["it_no"].ToString() });
                        }
                    }
                }
            }

            ViewData["itlist"] = itls;

        }

        [HttpPost]
        public ActionResult itlist(string bt_no)
        {
            StringBuilder sb = new StringBuilder();
            if (!string.IsNullOrWhiteSpace(bt_no))
            {
                string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["purchaseConnectionString"].ConnectionString;

                string st_no = "";
                string sql = "select it_no,it_name+'(規格:'+isnull(spec,'')+' 價格:'+convert(varchar,isnull(price,0))+' 單位:'+isnull(unit,'')+')' it_name " +
                      " from item where bt_no like '" + bt_no + "%" + "' and st_no like '" + st_no + "%" + "' order by it_name";

                using (SqlConnection conn2 = new SqlConnection(connectionString))
                {
                    SqlCommand cmd = new SqlCommand(sql, conn2);
                    conn2.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            sb.AppendFormat("<option value=\"{0}\">{1}</option>",
                                reader["it_no"].ToString(),
                                reader["it_name"].ToString()
                            );
                        }
                    }
                }
            }

            return Content(sb.ToString());
        }


        public ActionResult MDMReportSQLMvcPaging()
        {
            // 進入搜尋頁面 不主動撈取資料
            MreportViewModelSQL viewModel = new MreportViewModelSQL();

            if (Session["yy"] != null)
            {
                viewModel.yy = (String)Session["yy"];
            }
            if (Session["mm"] != null)
            {
                viewModel.mm = (String)Session["mm"];
            }
            if (Session["Page"] != null && Session["Status"] != null)
            {
                viewModel.Page = (int)Session["Page"];
            }
            if (Request["page"] != null)
            {
                /* MvcPaging 是用 get 傳 page 到 cshtml(內部運作猜這時候會把Request["page"]設定到viewModel.Page)
                 底下原始程式寫法印證

                var page = Request["page"];
                if (page != null)
                {
                    viewModel.Page = int.Parse(Request["page"]);
                }

                viewModel.MreportQuerySQL = MreportQuerySQL.ToPagedList(viewModel.Page > 0 ? viewModel.Page - 1 : 0, PageSize);

                 所以把Request["page"]存到Session["Page"], 這樣修改存檔後回到MreportSQLMvcPaging才能用Session["Page"]回到原來的頁次
                < div class="pager">
                    <span class="disabled">«</span><span class="current">1</span><a href = "/Purchase/MreportSQLMvcPaging?page=2" title="">2</a><a href = "/Purchase/MreportSQLMvcPaging?page=2" title="Next page">»</a>
                    Displaying 1 - 10 of 11 item(s)
                </div>

                
                */
                Session["Page"] = int.Parse(Request["page"]);
            }
            else
            {
                //連續新增第二次回因為 Request["page"] == null 跑到第一頁
                Session["Page"] = 0;
            }

            //上下架的下拉選單的資料清單
            ViewData["mmlist"] = new List<SelectListItem>()
            {
              new SelectListItem(){ Text="請選擇",Value="-1",Selected=true},
              new SelectListItem(){ Text="1月",Value="01" },
              new SelectListItem(){ Text="2月",Value="02" },
              new SelectListItem(){ Text="3月",Value="03" },
              new SelectListItem(){ Text="4月",Value="04" },
              new SelectListItem(){ Text="5月",Value="05" },
              new SelectListItem(){ Text="6月",Value="06" },
              new SelectListItem(){ Text="7月",Value="07" },
              new SelectListItem(){ Text="8月",Value="08" },
              new SelectListItem(){ Text="9月",Value="09" },
              new SelectListItem(){ Text="10月",Value="10" },
              new SelectListItem(){ Text="11月",Value="11" },
              new SelectListItem(){ Text="12月",Value="12" },
            };

            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["purchaseConnectionString"].ConnectionString;
            //string connectionString = "Data Source=Whatever; Initial Catalog=Northwind; Integrated security=True;";
            // your "normal" sql here
            string yl_id = "208";
            //string commandText = "select yy '年份',mm '月份',m_no '採購編號',item.it_name '名稱',spec '規格',price '價格',unit '單位',qty '數量',mreport.remark '備註'  "
            //      + " from mreport inner join item on item.it_no=mreport.it_no where yl_id=@yl_id and yy=@yy and mm=@mm order by m_no";
            string commandText = "select yy,mm,m_no,item.it_name,spec,price,unit,qty,mreport.remark "
                  + " from mreport inner join item on item.it_no=mreport.it_no where yl_id=@yl_id and yy=@yy and mm=@mm order by m_no";

            List<string> result = new List<string>();
            var MreportQuerySQL = new List<MreportQuerySQL>();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(commandText, conn);
                cmd.Parameters.Add("@yl_id", SqlDbType.NVarChar, 100).Value = yl_id;
                cmd.Parameters.Add("@yy", SqlDbType.NVarChar, 100).Value = viewModel.yy;
                cmd.Parameters.Add("@mm", SqlDbType.NVarChar, 100).Value = viewModel.mm; // ViewData["mm"];
                conn.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        MreportQuerySQL.Add(new MreportQuerySQL()
                        {
                            /*
                            product_no = reader.GetString(0),
                            name = reader.GetString(1),
                            price = reader.GetInt32(2),
                            remark = reader.GetString(3),
                            product_type_name = reader.GetString(4)
                            */

                            yy = reader["yy"].ToString(),
                            mm = reader["mm"].ToString(),
                            m_no = reader["m_no"].ToString(),
                            it_name = reader["it_name"].ToString(),
                            spec = reader["spec"].ToString(),
                            price = Int32.Parse(reader["price"].ToString()),
                            qty = Int32.Parse(reader["qty"].ToString()),
                            remark = reader["remark"].ToString(),
                        });
                    }
                }
            }

            var page = Request["page"];
            if (page != null)
            {
                viewModel.Page = int.Parse(Request["page"]);
            }

            viewModel.MreportQuerySQL = MreportQuerySQL.ToPagedList(viewModel.Page > 0 ? viewModel.Page - 1 : 0, PageSize);

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult MDMreportSQLMvcPaging(MreportViewModelSQL viewModel)
        {
            //上下架的下拉選單的資料清單
            var mm = Request["mm"];
            Session["yy"] = viewModel.yy;
            Session["mm"] = mm;

            /*
            if (mm != null)
            {
                ViewData["mm"] = mm;
            }
            ViewData["yy"] = viewModel.yy;
            */

            ViewData["mmlist"] = new List<SelectListItem>()
            {
              new SelectListItem(){ Text="請選擇",Value="-1",Selected=true},
              new SelectListItem(){ Text="1月",Value="01" },
              new SelectListItem(){ Text="2月",Value="02" },
              new SelectListItem(){ Text="3月",Value="03" },
              new SelectListItem(){ Text="4月",Value="04" },
              new SelectListItem(){ Text="5月",Value="05" },
              new SelectListItem(){ Text="6月",Value="06" },
              new SelectListItem(){ Text="7月",Value="07" },
              new SelectListItem(){ Text="8月",Value="08" },
              new SelectListItem(){ Text="9月",Value="09" },
              new SelectListItem(){ Text="10月",Value="10" },
              new SelectListItem(){ Text="11月",Value="11" },
              new SelectListItem(){ Text="12月",Value="12" },
            };

            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["purchaseConnectionString"].ConnectionString;
            //string connectionString = "Data Source=Whatever; Initial Catalog=Northwind; Integrated security=True;";
            // your "normal" sql here
            string yl_id = "208";
            //string commandText = "select yy '年份',mm '月份',m_no '採購編號',item.it_name '名稱',spec '規格',price '價格',unit '單位',qty '數量',mreport.remark '備註'  "
            //      + " from mreport inner join item on item.it_no=mreport.it_no where yl_id=@yl_id and yy=@yy and mm=@mm order by m_no";
            string commandText = "select yy,mm,m_no,item.it_name,spec,price,unit,qty,mreport.remark "
                  + " from mreport inner join item on item.it_no=mreport.it_no where yl_id=@yl_id and yy=@yy and mm=@mm order by m_no";

            List<string> result = new List<string>();
            var MreportQuerySQL = new List<MreportQuerySQL>();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(commandText, conn);
                cmd.Parameters.Add("@yl_id", SqlDbType.NVarChar, 100).Value = yl_id;
                cmd.Parameters.Add("@yy", SqlDbType.NVarChar, 100).Value = viewModel.yy;
                cmd.Parameters.Add("@mm", SqlDbType.NVarChar, 100).Value = mm;
                conn.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        MreportQuerySQL.Add(new MreportQuerySQL()
                        {
                            /*
                            product_no = reader.GetString(0),
                            name = reader.GetString(1),
                            price = reader.GetInt32(2),
                            remark = reader.GetString(3),
                            product_type_name = reader.GetString(4)
                            */

                            yy = reader["yy"].ToString(),
                            mm = reader["mm"].ToString(),
                            m_no = reader["m_no"].ToString(),
                            it_name = reader["it_name"].ToString(),
                            spec = reader["spec"].ToString(),
                            price = Int32.Parse(reader["price"].ToString()),
                            qty = Int32.Parse(reader["qty"].ToString()),
                            remark = reader["remark"].ToString(),
                        });
                    }
                }
            }

            viewModel.MreportQuerySQL = MreportQuerySQL.ToPagedList(viewModel.Page > 0 ? viewModel.Page - 1 : 0, PageSize);

            return View(viewModel);
        }

    }
}