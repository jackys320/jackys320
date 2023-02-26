using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace WebApplication1.Models
{
    public class Menu_sys_pgmaster
    {
        public string sys_pgmaster_no { get; set; }
        public string name { get; set; }
        public string sort { get; set; }
        public string sys_pgmaster_group { get; set; }
    }

    public class Menu_sys_pgdetail
    {
        public string sys_pgmaster_no { get; set; }
        public string name { get; set; }
        public string sort { get; set; }
        public string sys_pgdetail_group { get; set; }
    }

    public class Menu 
    {
        public IEnumerable<sys_pgmaster> sys_pgmaster { get; set; }
        public IEnumerable<sys_pgdetail> sys_pgdetail { get; set; }
    }

    public class Menu2
    {
        public IEnumerable<sys_pgmaster> sys_pgmaster { get; set; }
        public IEnumerable<sys_pgmaster_group> sys_pgmaster_group { get; set; }
        public IEnumerable<sys_pgdetail> sys_pgdetail { get; set; }
        public IEnumerable<sys_pgdetail_group> sys_pgdetail_group { get; set; }
    }

    public class MenuList
    {
        public List<Menu_sys_pgmaster> Menu_sys_pgmaster { get; set; }
        public List<Menu_sys_pgdetail> Menu_sys_pgdetail { get; set; }
    }
}