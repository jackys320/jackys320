using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace WebApplication1.Models
{
    public class Login
    {
        [DisplayName("登入帳號")]
        [Required(ErrorMessage = "請輸入登入帳號")]
        [StringLength(40, ErrorMessage = "登入帳號最多20個字")]
        public string Account1 { get; set; }

        [DisplayName("登入密碼")]
        [Required(ErrorMessage = "請輸入登入密碼")]
        [StringLength(40, ErrorMessage = "密碼最多20個字")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DisplayName("記住密碼")]
        public Boolean Remember { get; set; }
    }
}