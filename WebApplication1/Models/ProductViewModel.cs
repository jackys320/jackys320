using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MvcPaging;

namespace WebApplication1.Models
{
    public class ProductViewModel
    {
        // Properties
        public string product_no { get; set; }  // 搜尋條件1

        public string name { get; set; }  // 搜尋條件2

        public IPagedList<product> Products { get; set; }  // 符合條件資料

        public int Page { get; set; }  // 頁碼


        // Constructors
        public ProductViewModel()
        {
            product_no = string.Empty;
            name = string.Empty;
            Page = 0;
        }
    }

    public class ProductQuerySQL
    {
        public string product_no { get; set; }  // 搜尋條件1
        public string name { get; set; }  // 搜尋條件1
        public int price { get; set; }  // 搜尋條件1
        public string remark { get; set; }  // 搜尋條件1
        public string product_type_name { get; set; }  // 搜尋條件1
    }

    public class ProductViewModelSQL
    {
        // Properties
        public string product_no { get; set; }  // 搜尋條件1

        public string name { get; set; }  // 搜尋條件2

        public IPagedList<ProductQuerySQL> Products { get; set; }  // 符合條件資料

        public int Page { get; set; }  // 頁碼


        // Constructors
        public ProductViewModelSQL()
        {
            product_no = string.Empty;
            name = string.Empty;
            Page = 0;
        }
    }

}