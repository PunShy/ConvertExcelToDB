//------------------------------------------------------------------------------
// <auto-generated>
//     這個程式碼是由範本產生。
//
//     對這個檔案進行手動變更可能導致您的應用程式產生未預期的行為。
//     如果重新產生程式碼，將會覆寫對這個檔案的手動變更。
// </auto-generated>
//------------------------------------------------------------------------------

namespace ConvertExcelToDB.Model
{
    using System;
    using System.Collections.Generic;
    
    public partial class RainwaterDitch
    {
        public int id { get; set; }
        public Nullable<int> targetId { get; set; }
        public string SPI_TYP { get; set; }
        public string SPI_NUM { get; set; }
        public string STR_X { get; set; }
        public string STR_Y { get; set; }
        public Nullable<decimal> STR_LE { get; set; }
        public string END_X { get; set; }
        public string END_Y { get; set; }
        public Nullable<decimal> END_LE { get; set; }
        public Nullable<decimal> STR_DEP { get; set; }
        public Nullable<decimal> END_DEP { get; set; }
        public Nullable<decimal> STR_WID { get; set; }
        public Nullable<decimal> END_WID { get; set; }
        public Nullable<decimal> LENG { get; set; }
        public Nullable<decimal> SLOP { get; set; }
        public string CATCH_NUM { get; set; }
        public Nullable<System.DateTime> KEYIN_DATE { get; set; }
        public string CONS_DATE { get; set; }
        public string NOTE { get; set; }
        public Nullable<System.DateTime> ImportDate { get; set; }
        public string STR_84X { get; set; }
        public string STR_84Y { get; set; }
        public string END_84X { get; set; }
        public string END_84Y { get; set; }
        public System.Data.Entity.Spatial.DbGeometry coordinate { get; set; }
    }
}
