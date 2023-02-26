using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication1.Models;
using System.Data.Entity;
using System.Web.Security;

namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {
        private misEntities2 db = new misEntities2();

        /*
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            String controllerName = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName;
            String actionName = filterContext.ActionDescriptor.ActionName;
            String user_group_no = (String)Session["user_group_no"];
            if (String.IsNullOrEmpty(user_group_no) && !actionName.Substring(0,8).Equals("MenuItem"))
            {
                //重新定向至登入頁面
                //filterContext.Result = RedirectToAction("LoginFormsAuthentication", "LoginFormsAuthentication");
                filterContext.Result = RedirectToAction("Login", "Login");
                return;
            }

            //if (User.Identity == null && User.Identity.IsAuthenticated == false)
            //{//目前未登入 
            //    //return RedirectToAction("LoginFormsAuthentication", "LoginFormsAuthentication");
            //    return Redirect(FormsAuthentication.LoginUrl);
            //}

            //if (Session["user_group_no"] == null)
            //{
            //    //return RedirectToAction("Login", "Login");
            //    //return RedirectToAction("LoginFormsAuthentication", "LoginFormsAuthentication");
            //    return Redirect(FormsAuthentication.LoginUrl);
            //}

        }
       */

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult enter()
        {
            return View();
        }

        public ActionResult mis()
        {
            return View();
        }

        public ActionResult MenuItem2(int id = 0)
        {
            /*
            string key = "2012081600001";
            MisEntities2 entities = new MisEntities2();
            return View(from sys_pgmaster in entities.sys_pgmaster.Take(10)
                        select sys_pgmaster);
            */
            
            sys_pgmaster sys_pgmaster = db.sys_pgmaster.Find("2012081600001");
            //var syspgmaster = db.sys_pgmaster.Where(x => x.sys_pgmaster_no.Contains("2012081600001"));
            if (sys_pgmaster == null)
            {
                return HttpNotFound();
            }
            return View(sys_pgmaster);
            
        }

        //
        // POST: /Movies/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult MenuItem2(sys_pgmaster sys_pgmaster)
        {
            if (ModelState.IsValid)
            {
                db.Entry(sys_pgmaster).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(sys_pgmaster);
        }

        public ActionResult MenuItem()
        {
            //var syspgmaster = db.sys_pgmaster.Where(x => x.sys_pgmaster_no.Contains("2012081600001"));
            var sys_pgmaster =db.sys_pgmaster;
            //where r.Title.Contains(id)
            //select r;

            //var sys_pgdetail = db.sys_pgdetail.Where(x => x.sys_pgdetail_no.Contains(""));
            var sys_pgdetail = db.sys_pgdetail;
            //from r in db2.MovieDBDetail.Fi
            //where r.Title.Contains(id)
            //select r;

            Menu ViewModel = new Menu();
            ViewModel.sys_pgmaster = sys_pgmaster;
            ViewModel.sys_pgdetail = sys_pgdetail;
            return View(ViewModel);


        }

        public ActionResult MenuItem3()
        {
            //20171209 此段要如何用table join 方式傳到view 尚無法解決如何宣告class 

            //var syspgmaster = db.sys_pgmaster.Where(x => x.sys_pgmaster_no.Contains("2012081600001"));
            //var sys_pgmaster = db.sys_pgmaster;

            var sys_pgmaster = (from m in db.sys_pgmaster
                                join g in db.sys_pgmaster_group on m.sys_pgmaster_no equals g.sys_pgmaster_no
                                select new { sys_pgmaster_no = m.sys_pgmaster_no, name = m.name, sort = m.sort, sys_pgmaster_group = g.user_group_no }).ToList();

            var sys_pgmaster_list = new List<Menu_sys_pgmaster>();
            foreach (var t in sys_pgmaster)
            {
                sys_pgmaster_list.Add(new Menu_sys_pgmaster()
                {
                    sys_pgmaster_no = t.sys_pgmaster_no,
                    name = t.name,
                    sort = t.sort,
                    sys_pgmaster_group = t.sys_pgmaster_group
                });
            }
            //return View(teachers);


            //where r.Title.Contains(id)
            //select r;

            //var sys_pgdetail = db.sys_pgdetail.Where(x => x.sys_pgdetail_no.Contains(""));
            //var sys_pgdetail = db.sys_pgdetail;

            var sys_pgdetail = (from m in db.sys_pgdetail
                                join g in db.sys_pgdetail_group on m.sys_pgdetail_no equals g.sys_pgdetail_no
                                select new { sys_pgmaster_no = m.sys_pgmaster_no, name = m.name, sort = m.sort, sys_pgdetail_group = g.user_group_no }).ToList();

            var sys_pgdetail_list = new List<Menu_sys_pgdetail>();
            foreach (var t in sys_pgdetail)
            {
                sys_pgdetail_list.Add(new Menu_sys_pgdetail()
                {
                    sys_pgmaster_no = t.sys_pgmaster_no,
                    name = t.name,
                    sort = t.sort,
                    sys_pgdetail_group = t.sys_pgdetail_group
                });
            }

            //from r in db2.MovieDBDetail.Fi
            //where r.Title.Contains(id)
            //select r;

            //var menulist = new List<name, Sort, sys_pgdetail_group>;
            //var menulist = new List<List<menulist>>;

            /* 以下是不可行會出現 無法將類型 anonymous type ...... 
            MenuList ViewModel = new MenuList();
            ViewModel.Menu_sys_pgmaster = sys_pgmaster;
            ViewModel.Menu_sys_pgdetail = sys_pgdetail;
            */

            MenuList ViewModel = new MenuList();
            ViewModel.Menu_sys_pgmaster = sys_pgmaster_list;
            ViewModel.Menu_sys_pgdetail = sys_pgdetail_list;
            
            return View(ViewModel);
            
        }

        public ActionResult MenuItem5()
        {
            //var syspgmaster = db.sys_pgmaster.Where(x => x.sys_pgmaster_no.Contains("2012081600001"));
            var sys_pgmaster = db.sys_pgmaster;
            //where r.Title.Contains(id)
            //select r;

            //var syspgmaster = db.sys_pgmaster.Where(x => x.sys_pgmaster_no.Contains("2012081600001"));
            var sys_pgmaster_group = db.sys_pgmaster_group;
            //where r.Title.Contains(id)
            //select r;

            //var sys_pgdetail = db.sys_pgdetail.Where(x => x.sys_pgdetail_no.Contains(""));
            var sys_pgdetail = db.sys_pgdetail;
            //from r in db2.MovieDBDetail.Fi
            //where r.Title.Contains(id)
            //select r;

            //var syspgmaster = db.sys_pgmaster.Where(x => x.sys_pgmaster_no.Contains("2012081600001"));
            var sys_pgdetail_group = db.sys_pgdetail_group;
            //where r.Title.Contains(id)
            //select r;

            Menu2 ViewModel = new Menu2();
            ViewModel.sys_pgmaster = sys_pgmaster;
            ViewModel.sys_pgmaster_group = sys_pgmaster_group;
            ViewModel.sys_pgdetail = sys_pgdetail;
            ViewModel.sys_pgdetail_group = sys_pgdetail_group;
            return View(ViewModel);


        }

        public ActionResult MenuItem6()
        {
            //var syspgmaster = db.sys_pgmaster.Where(x => x.sys_pgmaster_no.Contains("2012081600001"));
            var sys_pgmaster = db.sys_pgmaster;
            //where r.Title.Contains(id)
            //select r;

            //var syspgmaster = db.sys_pgmaster.Where(x => x.sys_pgmaster_no.Contains("2012081600001"));
            var sys_pgmaster_group = db.sys_pgmaster_group;
            //where r.Title.Contains(id)
            //select r;

            //var sys_pgdetail = db.sys_pgdetail.Where(x => x.sys_pgdetail_no.Contains(""));
            var sys_pgdetail = db.sys_pgdetail;
            //from r in db2.MovieDBDetail.Fi
            //where r.Title.Contains(id)
            //select r;

            //var syspgmaster = db.sys_pgmaster.Where(x => x.sys_pgmaster_no.Contains("2012081600001"));
            var sys_pgdetail_group = db.sys_pgdetail_group;
            //where r.Title.Contains(id)
            //select r;

            Menu2 ViewModel = new Menu2();
            ViewModel.sys_pgmaster = sys_pgmaster;
            ViewModel.sys_pgmaster_group = sys_pgmaster_group;
            ViewModel.sys_pgdetail = sys_pgdetail;
            ViewModel.sys_pgdetail_group = sys_pgdetail_group;
            return View(ViewModel);


        }


    }

    public class MyController : Controller
    {
        public Menu MainLayoutViewModel { get; set; }

        public MyController()
        {
            this.MainLayoutViewModel = new Menu();//has property PageTitle

            this.ViewData["MainLayoutViewModel"] = this.MainLayoutViewModel;
        }

    }

    public abstract class ApplicationController : Controller
    {
        protected ApplicationController()
        {
            /*
            MainLayoutMenu2 = new Menu2();
            //Modify the UserStateViewModel here.
            ViewBag["MainLayoutMenu2"] = MainLayoutMenu2;
            */
        }

        /*
        public Menu2 MainLayoutMenu2 { get; set; }
        */
    }
}