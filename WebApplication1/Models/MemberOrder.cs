//------------------------------------------------------------------------------
// <auto-generated>
//     這個程式碼是由範本產生。
//
//     對這個檔案進行手動變更可能導致您的應用程式產生未預期的行為。
//     如果重新產生程式碼，將會覆寫對這個檔案的手動變更。
// </auto-generated>
//------------------------------------------------------------------------------

namespace WebApplication1.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class MemberOrder
    {
        public string MemberOrderNo { get; set; }
        public string MemberNo { get; set; }
        public string DesignerNo { get; set; }
        public string ScheduleNo { get; set; }
        public string Service { get; set; }
        public Nullable<bool> Confirm { get; set; }
        public string ScheduleDate { get; set; }
        public string ScheduleTime { get; set; }
        public Nullable<System.DateTime> OrderTime { get; set; }
        public string PushNotice { get; set; }
        public Nullable<bool> PushNotConfirm { get; set; }
    }
}
