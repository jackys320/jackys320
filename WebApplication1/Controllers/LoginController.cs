using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication1.Models;
using System.Data.Entity;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Security;

using System.Data;
using System.Configuration;
using System.Collections;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Data.SqlClient;
using System.Data.OleDb;

namespace WebApplication1.Controllers
{
    public class LoginController : Controller
    {
        private misEntities2 db = new misEntities2();

        // GET: Login
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(WebApplication1.Models.Login form)
        {
            if (ModelState.IsValid)
            {
                //驗證資料庫登入
                //這邊請使用自行驗證摟
                //string Sql = " Account1 == @0 and Password == @1 ";
                //var query = (from u in db.Account.Where(x => x.Account1.Equals(form.Account1) && x.Password.Equals(form.Password))
                //var query = (from u in db.Account.Where(x => x.Account1.Equals(form.Account1) && x.Password.ToString().Equals(FormsAuthentication.HashPasswordForStoringInConfigFile(form.Password, "MD5")))
                //以下應是Linq寫法
                var query = (from u in db.Account.Where(x => x.Account1.Equals(form.Account1))
                             select u).ToList();

                if (query.Count() != 0)
                {
                    string PWD = "";
                    foreach (var record in query)
                    {
                        ViewBag.LoginName = record.Account1;
                        TempData["LoginName"] = record.Account1;
                        Session["LoginName"] = record.Account1;
                        Session["user_group_no"] = record.user_group_no;

                        /*
                        ViewBag only lives for the current request.In your case you are redirecting, 
                        so everything you might have stored in the ViewBag will die along wit the current request.Use ViewBag, 
                        only if you render a view, not if you intend to redirect.
                        Use TempData instead:

                        TempData["Message"] = "Profile Updated Successfully";
                        return RedirectToAction("Index");
                        問題是TempData不是取過就沒了
                        */

                        PWD = record.Password;
                        break;
                    }
                    if (!PWD.ToString().Equals(FormsAuthentication.HashPasswordForStoringInConfigFile(form.Password, "MD5")))
                    {
                        ModelState.AddModelError(string.Empty, "密碼錯誤登入失敗");
                        //return View("Index", form);
                        return View(form);
                    }

                }
                else
                {
                    ModelState.AddModelError(string.Empty, "帳號錯誤登入失敗");
                    //return View("Index", form);
                    return View(form);
                }

                /*
                try
                {
                    query[0].LoginIP = PublicFunction.GetIpAddress();
                    query[0].LoginDate = DateTime.Now;
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message.ToString());
                    return View("Index", form);
                }


                bool isPersistent = false;//如果票證將存放於持續性 Cookie 中 (跨瀏覽器工作階段儲存)，則為 true，否則為 false。 如果票證是存放於 URL 中，則忽略這個值。
                string userData = "";//可放使用者自訂的內容
                string mAccountID = query[0].AccountID.ToString();

                //寫cookie
                //使用 Cookie 名稱、版本、到期日、核發日期、永續性和使用者特定的資料，初始化 FormsAuthenticationTicket 類別的新執行個體。 此 Cookie 路徑設定為在應用程式的組態檔中建立的預設值。
                //使用 Cookie 名稱、版本、目錄路徑、核發日期、到期日期、永續性和使用者定義的資料，初始化 FormsAuthenticationTicket 類別的新執行個體。
                FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(1,
                  mAccountID,//使用者ID
                  DateTime.Now,//核發日期
                  DateTime.Now.AddMinutes(1800),//到期日期 30分鐘 
                  isPersistent,//永續性
                  userData,//使用者定義的資料
                  FormsAuthentication.FormsCookiePath);

                string encTicket = FormsAuthentication.Encrypt(ticket);
                Response.Cookies.Add(new HttpCookie(FormsAuthentication.FormsCookieName, encTicket));

                //HttpCookie cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encTicket);
                //cookie.Expires = ticket.Expiration;
                //Response.Cookies.Add(cookie);

                //FormsAuthentication.RedirectFromLoginPage(strUsername, isPersistent);

                if (form.ReturnUrl != null)
                {
                    return Redirect(form.ReturnUrl.ToString());
                }
                else
                {
                    return RedirectToAction("Index", "Admin");
                }
                */
                //return RedirectToAction("MenuItem3","Home");
                //return RedirectToAction("MenuItem5","Home");
                //return Response.Redirect("../Home/enter.htm");
                //return RedirectToAction("enter", "Home");
                return RedirectToAction("mis", "Home");
            }
            //return RedirectToAction("Index", "Login", null);
            else
            {
                ModelState.AddModelError(string.Empty, "帳號或密碼不能空白");
                //return View("Index", form);
                return View(form);
            }
        }
    }
}