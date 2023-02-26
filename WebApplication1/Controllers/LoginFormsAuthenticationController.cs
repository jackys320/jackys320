using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using WebApplication1.Models;
using System.Net;

namespace WebApplication1.Controllers
{
    [Authorize]
    public class LoginFormsAuthenticationController : Controller
    {
        private misEntities2 db = new misEntities2();
        // GET: LoginFormsAuthentication
        //public ActionResult Index()
        //{
        //    return View();
        //}

        /// <summary>
        /// 呈現後台使用者登入頁
        /// </summary>
        /// <param name="ReturnUrl">使用者原本Request的Url</param>
        /// <returns></returns>
        [AllowAnonymous]
        public ActionResult LoginFormsAuthentication()
        {
            //表單驗證 已從登入頁面登入過,狀態並儲存到硬碟
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {//目前已登入 
                string Account1 = User.Identity.Name;

                var query = (from u in db.Account.Where(x => x.Account1.Equals(Account1))
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

                    if (User.Identity != null && User.Identity.IsAuthenticated)
                    {//目前已登入 

                        //刪除Cookie
                        //Response.Cookies["mvcUserData"].Expires = DateTime.Now.AddDays(-1);

                        string ReturnPath = Request.QueryString["ReturnUrl"];
                        if (ReturnPath != null)
                        {
                            return Redirect(ReturnPath);
                        }
                        else
                        {
                            return Redirect(FormsAuthentication.DefaultUrl);
                        }
                    }

                }

            }
            return View();
        }


        /// <summary>
        /// 後台使用者進行登入
        /// </summary>
        /// <param name="vm"></param>
        /// <param name="u">使用者原本Request的Url</param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost]
        public ActionResult LoginFormsAuthentication(WebApplication1.Models.Login form)
        {

            /*
            //沒通過Model驗證(必填欄位沒填，DB無此帳密)
            if (!ModelState.IsValid)
            {
                return View(form);
            }



            //都成功...
            //進行表單登入 ※之後User.Identity.Name的值就是vm.Account帳號的值
            //導向預設Url(Web.config裡的defaultUrl定義)或使用者原先Request的Url
            FormsAuthentication.RedirectFromLoginPage(form.Account1, false);





            //剛剛已導向，此行不會執行到
            return Redirect(FormsAuthentication.GetRedirectUrl(form.Account1, false));
            */

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
                //return RedirectToAction("mis", "Home");

                //都成功...
                //進行表單登入 ※之後User.Identity.Name的值就是vm.Account帳號的值
                //導向預設Url(Web.config裡的defaultUrl定義)或使用者原先Request的Url

                //這個方法的第二個參數，若設定為 true ，則 Cookie 資訊會儲存在 Client 端的磁碟中，
                //有效期限預設為 30 分鐘； 若設定為 false ，則 Cookie 資訊只會存放在 Client 端瀏灠器的記憶體中，
                //所以則當使用者關閉瀏灠器後， Cookie 就會失效。
                //30分鐘設定在web.config timeout = "30"
                //< authentication mode = "Forms" >
                //     < forms defaultUrl = "~/Home/mis/" loginUrl = "~/LoginFormsAuthentication/LoginFormsAuthentication/" timeout = "30" />
                //</ authentication >
                if (form.Remember)
                {
                    //儲存到硬碟Cookie .ASPXAUTH User.Identity.Name 就會有 admin 但只有預設30分鐘
                    FormsAuthentication.RedirectFromLoginPage(form.Account1, true);
                }
                else
                {
                    FormsAuthentication.RedirectFromLoginPage(form.Account1, false);
                }

                //剛剛已導向，此行不會執行到
                return Redirect(FormsAuthentication.GetRedirectUrl(form.Account1, false));
            }
            //return RedirectToAction("Index", "Login", null);
            else
            {
                ModelState.AddModelError(string.Empty, "帳號或密碼不能空白");
                //return View("Index", form);
                return View(form);
            }

        }

        /// <summary>
        /// 呈現後台使用者登入頁
        /// </summary>
        /// <param name="ReturnUrl">使用者原本Request的Url</param>
        /// <returns></returns>
        [AllowAnonymous]
        public ActionResult LoginFormsAuthCookie()
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {//目前已登入 

            }

            //HttpCookie cookie = new GetCookie(FormsAuthentication.FormsCookieName);
            Login Login = new Login();
            HttpCookie cookie = Request.Cookies[FormsAuthentication.FormsCookieName];
            //HttpCookie cookie = Request.Cookies["mvcUserData"];
            
            FormsAuthenticationTicket authTicket = null;

            //目前測試chrome可以 IE11 跑一次就不行
            if (cookie != null)
            {
                authTicket = FormsAuthentication.Decrypt(cookie.Value);
                Login.Account1 = authTicket.Name;
                Login.Password = authTicket.UserData;

                var query = (from u in db.Account.Where(x => x.Account1.Equals(Login.Account1))
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

                    if (User.Identity != null && User.Identity.IsAuthenticated)
                    {//目前已登入 

                    }

                    if (!PWD.ToString().Equals(FormsAuthentication.HashPasswordForStoringInConfigFile(Login.Password, "MD5")))
                    {
                        ModelState.AddModelError(string.Empty, "密碼錯誤登入失敗");
                        //return View("Index", form);
                        return View(Login);
                    }
                    else
                    {
                        //刪除Cookie
                        Response.Cookies["mvcUserData"].Expires = DateTime.Now.AddDays(-1);

                        string ReturnPath = Request.QueryString["ReturnUrl"];
                        if (ReturnPath != null)
                        {
                            return Redirect(ReturnPath);
                        }
                        else
                        {
                            return Redirect(FormsAuthentication.DefaultUrl);
                        }
                    }

                }
                else
                {
                    ModelState.AddModelError(string.Empty, "帳號錯誤登入失敗");
                    //return View("Index", form);
                    return View(Login);
                }

            }
            return View(Login);
        }

        /// <summary>
        /// 後台使用者進行登入
        /// </summary>
        /// <param name="vm"></param>
        /// <param name="u">使用者原本Request的Url</param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost]
        public ActionResult LoginFormsAuthCookie(WebApplication1.Models.Login form)
        {

            /*
            //沒通過Model驗證(必填欄位沒填，DB無此帳密)
            if (!ModelState.IsValid)
            {
                return View(form);
            }



            //都成功...
            //進行表單登入 ※之後User.Identity.Name的值就是vm.Account帳號的值
            //導向預設Url(Web.config裡的defaultUrl定義)或使用者原先Request的Url
            FormsAuthentication.RedirectFromLoginPage(form.Account1, false);





            //剛剛已導向，此行不會執行到
            return Redirect(FormsAuthentication.GetRedirectUrl(form.Account1, false));
            */

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

                */

                /*
                string name = account;
                DateTime issueDate = System.DateTime.Now;
                DateTime expiration = System.DateTime.Now.AddMinutes(2);  //1分
                bool isPersistent = rememberme.Checked;
                string userData = password;
                string cookiePath = FormsAuthentication.FormsCookiePath;
                */

                //FormsAuthentication.RedirectFromLoginPage(form.Account1, false);
                //FormsAuthentication.RedirectFromLoginPage(form.Account1, true);


                if (form.Remember)
                {
                    FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(
                        version: 1,                                 //票證的版本號碼
                        name : form.Account1,                       //使用者名稱
                        issueDate: DateTime.Now,                    //使用者名稱
                        expiration: DateTime.Now.AddDays(1),        //到期時間 DateTime.Now.AddMinutes(1800) 30 分鐘
                        isPersistent: form.Remember ,               //是否將票證資訊存放到硬碟的 Cookie 中
                        userData: form.Password ,                   //使用者定義的資料
                        cookiePath: FormsAuthentication.FormsCookiePath  //存放於 Cookie 中時的路徑
                    );

                    string encTicket = FormsAuthentication.Encrypt(ticket);
                    //Response.Cookies.Add(new HttpCookie(FormsAuthentication.FormsCookieName, encTicket));

                    HttpCookie cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encTicket);
                    //HttpCookie cookie = new HttpCookie("mvcUserData", encTicket);
                    cookie.Expires = ticket.Expiration;
                    Response.Cookies.Add(cookie);

                    //FormsAuthentication.SetAuthCookie(name, isPersistent);

                    // 因為採用 SetAuthCookie 回應 authentication ticket , 所以必須自行做重新導向.
                    // 若使用 FormsAuthentication.RedirectFromLoginPage 則不用.
                    string ReturnPath = Request.QueryString["ReturnUrl"];
                    if (ReturnPath != null)
                    {
                        return Redirect(ReturnPath);
                    }
                    else
                    {
                        return Redirect(FormsAuthentication.DefaultUrl);
                    }
                }
                else
                {
                    FormsAuthentication.RedirectFromLoginPage(form.Account1, false);
                }

                //FormsAuthentication.RedirectFromLoginPage(strUsername, isPersistent);

                /*
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
                //return RedirectToAction("mis", "Home");

                //都成功...
                //進行表單登入 ※之後User.Identity.Name的值就是vm.Account帳號的值
                //導向預設Url(Web.config裡的defaultUrl定義)或使用者原先Request的Url
                //FormsAuthentication.RedirectFromLoginPage(form.Account1, false);



                //剛剛已導向，此行不會執行到
                return Redirect(FormsAuthentication.GetRedirectUrl(form.Account1, false));
            }
            //return RedirectToAction("Index", "Login", null);
            else
            {
                ModelState.AddModelError(string.Empty, "帳號或密碼不能空白");
                //return View("Index", form);
                return View(form);
            }

        }
        /// <summary>
        /// 後台使用者登出動作
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        public ActionResult Logout()
        {
            //清除Session中的資料
            Session.Abandon();
            //登出表單驗證
            FormsAuthentication.SignOut();
            //導至登入頁
            return RedirectToAction("LoginFormsAuthentication", "LoginFormsAuthentication");
        }


        /// <summary>
        /// 後台首頁 
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            //取得目前登入者的帳號
            string Account = User.Identity.Name;
            //可以依帳號到DB抓登入者的資料...





            return View();
        }
        /// <summary>
        /// 測試用的
        /// </summary>
        /// <returns></returns>
        public ActionResult Index2()
        {
            //如果要判斷此登入者有沒有登入的寫法
            if (User.Identity != null &&  User.Identity.IsAuthenticated)
            {//目前已登入 
                
            }
            return View();
        }
    }
}