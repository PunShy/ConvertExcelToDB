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
    
    public partial class SetWells
    {
        public int id { get; set; }
        public Nullable<int> targetId { get; set; }
        public string CP_NUM { get; set; }
        public string X { get; set; }
        public string Y { get; set; }
        public Nullable<decimal> CP_BLE { get; set; }
        public Nullable<decimal> CP_DEP { get; set; }
        public Nullable<decimal> CP_LENG { get; set; }
        public Nullable<decimal> CP_WID { get; set; }
        public string IN_CPNUM { get; set; }
        public string CONS_ID { get; set; }
        public string CITY_ID { get; set; }
        public string NOTE { get; set; }
        public Nullable<System.DateTime> ImportDate { get; set; }
        public string Wgs84X { get; set; }
        public string Wgs84Y { get; set; }
        public System.Data.Entity.Spatial.DbGeometry coordinate { get; set; }
    }
}
