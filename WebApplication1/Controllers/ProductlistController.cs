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

namespace WebApplication1.Controllers
{
    public class ProductlistController : Controller
    {
        private misEntities2 misEntities2 = new misEntities2();

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

        // GET: Productlist
        public ActionResult MvcPaging()
        {
            // 進入搜尋頁面 不主動撈取資料
            ProductViewModel viewModel = new ProductViewModel();

            if (Session["product_no"] != null)
            {
                viewModel.product_no = (String)Session["product_no"];
            }
            if (Session["name"] != null)
            {
                viewModel.name = (String)Session["name"];
            }


            // 搜尋條件為 = 產品名稱 AND 生產工廠
            IQueryable<product> products = misEntities2.product;

            // 如果有輸入產品編號名稱作為搜尋條件時
            if (!string.IsNullOrWhiteSpace(viewModel.name))
            { products = products.Where(p => p.product_no.Contains(viewModel.product_no)); }

            // 如果有輸入產品名稱作為搜尋條件時
            if (!string.IsNullOrWhiteSpace(viewModel.name))
            { products = products.Where(p => p.name.Contains(viewModel.name)); }



            var page = Request["page"];
            if (page != null)
            {
                viewModel.Page = int.Parse(Request["page"]);
            }

            // 回傳搜尋結果

            viewModel.Products = products.Include(p => p.product_type)
                                        .OrderBy(p => p.product_no)
                                        .ToPagedList(viewModel.Page > 0 ? viewModel.Page - 1 : 0, PageSize);

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult MvcPaging(ProductViewModel viewModel)
        {

            Session["product_no"] = viewModel.product_no;
            Session["name"] = viewModel.name;

            // 搜尋條件為 = 產品名稱 AND 生產工廠
            IQueryable<product> products = misEntities2.product;

            // 如果有輸入產品編號名稱作為搜尋條件時
            if (!string.IsNullOrWhiteSpace(viewModel.product_no))
            { products = products.Where(p => p.product_no.Contains(viewModel.product_no)); }

            // 如果有輸入產品名稱作為搜尋條件時
            if (!string.IsNullOrWhiteSpace(viewModel.name))
            { products = products.Where(p => p.name.Contains(viewModel.name)); }

            // 回傳搜尋結果

            viewModel.Products = products.Include(p => p.product_type)
                                        .OrderBy(p => p.product_no)
                                        .ToPagedList(viewModel.Page > 0 ? viewModel.Page - 1 : 0, PageSize);

            return View(viewModel);
        }

        public ActionResult ProductQuerySQL()
        {
            // 進入搜尋頁面 不主動撈取資料
            ProductQuerySQL viewModel = new ProductQuerySQL();
            //var data = db.product.SqlQuery("select top 10 product.* from product left join product_type on product.product_type_no=product_type.no").ToList<product>();
            var data = misEntities2.Database.SqlQuery<ProductQuerySQL>("select top 10 product.product_no,product.name,price,remark," +
                "product_type.name product_type_name from product left join product_type on product.product_type_no=product_type.no").ToList();

            
            var ProductQuerySQL = new List<ProductQuerySQL>();
            foreach (var t in data)
            {
                ProductQuerySQL.Add(new ProductQuerySQL()
                {

                    product_no = t.product_no,
                    name = t.name,
                    price = t.price,
                    remark = t.remark,
                    product_type_name = t.product_type_name
                });
            }

            return View(ProductQuerySQL);

        }

        [HttpPost]
        public ActionResult ProductQuerySQL(ProductViewModel viewModel)
        {
            // 搜尋條件為 = 產品名稱 AND 生產工廠
            IQueryable<product> products = misEntities2.product;

            // 如果有輸入產品編號名稱作為搜尋條件時
            if (!string.IsNullOrWhiteSpace(viewModel.product_no))
            { products = products.Where(p => p.product_no.Contains(viewModel.product_no)); }

            // 如果有輸入產品名稱作為搜尋條件時
            if (!string.IsNullOrWhiteSpace(viewModel.name))
            { products = products.Where(p => p.name.Contains(viewModel.name)); }

            // 回傳搜尋結果

            viewModel.Products = products.Include(p => p.product_type)
                                        .OrderBy(p => p.product_no)
                                        .ToPagedList(viewModel.Page > 0 ? viewModel.Page - 1 : 0, PageSize);

            return View(viewModel);
        }

        public ActionResult ProductQuerySQLMvcPaging()
        {
            // 進入搜尋頁面 不主動撈取資料
            ProductViewModelSQL viewModel = new ProductViewModelSQL();

            if (Session["product_no"] != null)
            {
                viewModel.product_no = (String)Session["product_no"];
            }
            if (Session["name"] != null)
            {
                viewModel.name = (String)Session["name"];
            }

            //var data = db.product.SqlQuery("select top 10 product.* from product left join product_type on product.product_type_no=product_type.no").ToList<product>();
            var data = misEntities2.Database.SqlQuery<ProductQuerySQL>("select product.product_no,product.name,price,remark," +
                "product_type.name product_type_name from product left join product_type on product.product_type_no=product_type.no" +
                " where product_no like {0} and product.name like {1}", (string.IsNullOrWhiteSpace(viewModel.product_no)) ? "%%" : "%" + viewModel.product_no + "%", (string.IsNullOrWhiteSpace(viewModel.name)) ? "%%" : "%" + viewModel.name + "%").ToList();

            var p0 = new SqlParameter("p0", (string.IsNullOrWhiteSpace(viewModel.product_no)) ? "%%" : "%" + viewModel.product_no + "%");
            var p1 = new SqlParameter("p1", (string.IsNullOrWhiteSpace(viewModel.name)) ? "%%" : "%" + viewModel.name + "%");

            var parameter = new SqlParameter[] { p0, p1 };

            data = misEntities2.Database.SqlQuery<ProductQuerySQL>("select product.product_no,product.name,price,remark," +
                "product_type.name product_type_name from product left join product_type on product.product_type_no=product_type.no" +
                " where product_no like @p0 and product.name like @p1", parameter).ToList();
            

            var ProductQuerySQL = new List<ProductQuerySQL>();
            foreach (var t in data)
            {
                ProductQuerySQL.Add(new ProductQuerySQL()
                {

                    product_no = t.product_no,
                    name = t.name,
                    price = t.price,
                    remark = t.remark,
                    product_type_name = t.product_type_name
                });
            }

            var page = Request["page"];
            if (page != null)
            {
                viewModel.Page = int.Parse(Request["page"]);
            }

            viewModel.Products = ProductQuerySQL.ToPagedList(viewModel.Page > 0 ? viewModel.Page - 1 : 0, PageSize);

            return View(viewModel);

            /*
            using (var ctx = new misEntities2())
            {
                var studentList = ctx.product.SqlQuery("Select * from Student");

            }
            */

            //return View(data);
        }

        [HttpPost]
        public ActionResult ProductQuerySQLMvcPaging(ProductViewModelSQL viewModel)
        {
            //var data = db.product.SqlQuery("select top 10 product.* from product left join product_type on product.product_type_no=product_type.no").ToList<product>();

            /*
            var data = misEntities2.Database.SqlQuery<ProductQuery>("select product.product_no,product.name,price,remark," +
                "product_type.name product_type_name from product left join product_type on product.product_type_no=product_type.no").ToList();
            */
            Session["product_no"] = viewModel.product_no;
            Session["name"] = viewModel.name;

            var data = misEntities2.Database.SqlQuery<ProductQuerySQL>("select product.product_no,product.name,price,remark," +
                "product_type.name product_type_name from product left join product_type on product.product_type_no=product_type.no" +
                " where product_no like {0} and product.name like {1}", (string.IsNullOrWhiteSpace(viewModel.product_no)) ? "%%" : "%"+ viewModel.product_no + "%",(string.IsNullOrWhiteSpace(viewModel.name)) ? "%%" : "%" + viewModel.name + "%").ToList();

            var p0 = new SqlParameter("p0", (string.IsNullOrWhiteSpace(viewModel.product_no)) ? "%%" : "%" + viewModel.product_no + "%");
            var p1 = new SqlParameter("p1", (string.IsNullOrWhiteSpace(viewModel.name)) ? "%%" : "%" + viewModel.name + "%");

            var parameter = new SqlParameter[] { p0, p1 };

            data = misEntities2.Database.SqlQuery<ProductQuerySQL>("select product.product_no,product.name,price,remark," +
                "product_type.name product_type_name from product left join product_type on product.product_type_no=product_type.no" +
                " where product_no like @p0 and product.name like @p1", parameter).ToList();

            var ProductQuerySQL = new List<ProductQuerySQL>();
            foreach (var t in data)
            {
                ProductQuerySQL.Add(new ProductQuerySQL()
                {

                    product_no = t.product_no,
                    name = t.name,
                    price = t.price,
                    remark = t.remark,
                    product_type_name = t.product_type_name
                });
            }

            viewModel.Products = ProductQuerySQL.ToPagedList(viewModel.Page > 0 ? viewModel.Page - 1 : 0, PageSize);

            return View(viewModel);
        }

        public ActionResult ProductSQLMvcPaging()
        {
            // 進入搜尋頁面 不主動撈取資料
            ProductViewModelSQL viewModel = new ProductViewModelSQL();

            if (Session["product_no"] != null)
            {
                viewModel.product_no = (String)Session["product_no"];
            }
            if (Session["name"] != null)
            {
                viewModel.name = (String)Session["name"];
            }

            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["misConnectionString"].ConnectionString;
            //string connectionString = "Data Source=Whatever; Initial Catalog=Northwind; Integrated security=True;";
            // your "normal" sql here
            string commandText = "select product.product_no,product.name,isnull(price,0) price,isnull(remark,'') remark," +
                "isnull(product_type.name,'') product_type_name from product left join product_type on product.product_type_no=product_type.no" +
                " where product_no like @p0 and product.name like @p1";

            List<string> result = new List<string>();
            var ProductQuerySQL = new List<ProductQuerySQL>();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(commandText, conn);
                cmd.Parameters.Add("@p0", SqlDbType.NVarChar);
                cmd.Parameters["@p0"].Value = (string.IsNullOrWhiteSpace(viewModel.product_no)) ? "%%" : "%" + viewModel.product_no + "%";
                cmd.Parameters.Add("@p1", SqlDbType.NVarChar);
                cmd.Parameters["@p1"].Value = (string.IsNullOrWhiteSpace(viewModel.name)) ? "%%" : "%" + viewModel.name + "%";
                conn.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        ProductQuerySQL.Add(new ProductQuerySQL()
                        {
                            /*
                            product_no = reader.GetString(0),
                            name = reader.GetString(1),
                            price = reader.GetInt32(2),
                            remark = reader.GetString(3),
                            product_type_name = reader.GetString(4)
                            */

                            product_no = reader["product_no"].ToString(),
                            name = reader["name"].ToString(),
                            price = Int32.Parse(reader["price"].ToString()),
                            remark = reader["remark"].ToString(),
                            product_type_name = reader["product_type_name"].ToString(),
                        });
                    }
                }
            }

            var page = Request["page"];
            if (page != null)
            {
                viewModel.Page = int.Parse(Request["page"]);
            }

            viewModel.Products = ProductQuerySQL.ToPagedList(viewModel.Page > 0 ? viewModel.Page - 1 : 0, PageSize);

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult ProductSQLMvcPaging(ProductViewModelSQL viewModel)
        {
            Session["product_no"] = viewModel.product_no;
            Session["name"] = viewModel.name;

            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["misConnectionString"].ConnectionString;
            //string connectionString = "Data Source=Whatever; Initial Catalog=Northwind; Integrated security=True;";
            // your "normal" sql here
            string commandText = "select product.product_no,product.name,isnull(price,0) price,isnull(remark,'') remark," +
                "isnull(product_type.name,'') product_type_name from product left join product_type on product.product_type_no=product_type.no" +
                " where product_no like @p0 and product.name like @p1";

            List<string> result = new List<string>();
            var ProductQuerySQL = new List<ProductQuerySQL>();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(commandText, conn);
                cmd.Parameters.Add("@p0", SqlDbType.NVarChar);
                cmd.Parameters["@p0"].Value = (string.IsNullOrWhiteSpace(viewModel.product_no)) ? "%%" : "%" + viewModel.product_no + "%";
                cmd.Parameters.Add("@p1", SqlDbType.NVarChar);
                cmd.Parameters["@p1"].Value = (string.IsNullOrWhiteSpace(viewModel.name)) ? "%%" : "%" + viewModel.name + "%";
                conn.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        ProductQuerySQL.Add(new ProductQuerySQL()
                        {
                            /*
                            product_no = reader.GetString(0),
                            name = reader.GetString(1),
                            price = reader.GetInt32(2),
                            remark = reader.GetString(3),
                            product_type_name = reader.GetString(4)
                            */

                            product_no = reader["product_no"].ToString(),
                            name = reader["name"].ToString(),
                            price = Int32.Parse(reader["price"].ToString()),
                            remark = reader["remark"].ToString(),
                            product_type_name = reader["product_type_name"].ToString(),
                        });
                    }
                }
            }

            viewModel.Products = ProductQuerySQL.ToPagedList(viewModel.Page > 0 ? viewModel.Page - 1 : 0, PageSize);

            return View(viewModel);
        }

    }

    /*
    public ActionResult Index()
    {
        string connectionString = "Data Source=Whatever; Initial Catalog=Northwind; Integrated security=True;";
        // your "normal" sql here
        string commandText = "select" +
                                " _c.CompanyName" +
                            " from Customers _c" +
                            " inner join Orders _o on _o.CustomerID = _c.CustomerID" +
                            " Where _o.ShipCity = @city";
        List<string> result = new List<string>();
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            SqlCommand cmd = new SqlCommand(commandText, connection);
            cmd.Parameters.Add("@city", SqlDbType.NVarChar);
            cmd.Parameters["@city"].Value = "Madrid";

            connection.Open();
            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    result.Add(reader.GetString(0));
                }
            }
        }

        return View(result);
    }
    */
}