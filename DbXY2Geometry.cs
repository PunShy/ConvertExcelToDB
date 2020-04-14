using ConvertExcelToDB.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity.Spatial;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConvertExcelToDB
{
    /// <summary>
    /// 目的 : 將既有的wgs84_XY文字 轉成 DbGeometry空間資料 
    /// </summary>
    class DbXY2Geometry
    {

        private static CPAMIEntities _cpi = new CPAMIEntities();

        static void Main(string[] args)
        {
            _cpi.Database.Log = Console.WriteLine;
            RainwaterDitch();
            _cpi.SaveChanges();
            Console.WriteLine("OK");
            Console.Read();
        }

        private static void RainCompletedManhole()
        {
            var datas = _cpi.RainCompletedManhole//.Where(a => a.targetId == 162)
                        .Where(a => a.Wgs84X != null && a.Wgs84Y != null);
            string geometryStr = "";
            foreach (var item in datas)
            {
                geometryStr = string.Format("POINT({0} {1})", item.Wgs84X, item.Wgs84Y);
                item.coordinate = DbGeometry.FromText(geometryStr, 4326);
            }
        }

        /// <summary>
        /// 有幾筆資料本身有問題
        /// </summary>
        private static void RainCompletedPipeline()
        {
            //先過濾掉資料本身有問題，需要檢查的部分，先不轉換
            var datas = _cpi.RainCompletedPipeline//.Where(a => a.targetId == 27)
                            .Where(a => a.US_84X != a.DS_84X || a.US_84Y != a.DS_84Y)
                            .Where(a => a.US_84X != "118.754566070609" && a.US_84Y != "0")
                            .Where(a => a.DS_84X != "118.754566070609" && a.DS_84Y != "0")
                            .Where(a => a.US_84X != null && a.US_84Y != null && a.DS_84X != null && a.DS_84Y != null);
            string geometryStr = "";
            foreach (var item in datas)
            {
                geometryStr = string.Format("LINESTRING({0} {1}, {2} {3})", item.US_84X, item.US_84Y, item.DS_84X, item.DS_84Y);
                item.coordinate = DbGeometry.LineFromText(geometryStr, 4326);
            }
        }

        private static void SetWells()
        {
            var datas = _cpi.SetWells//.Where(a => a.targetId == 162)
                            .Where(a => a.Wgs84X != null && a.Wgs84Y != null);
            string geometryStr = "";
            foreach (var item in datas)
            {
                geometryStr = string.Format("POINT({0} {1})", item.Wgs84X, item.Wgs84Y);
                item.coordinate = DbGeometry.FromText(geometryStr, 4326);
            }
        }
        private static void RainwaterDitch()
        {
            var datas = _cpi.RainwaterDitch//.Where(a => a.targetId == 164)
                            .Where(a => a.STR_84X != a.END_84X || a.STR_84Y != a.END_84Y)
                            .Where(a => a.STR_84X != null && a.STR_84Y != null && a.END_84X != null && a.END_84Y != null);
            string geometryStr = "";
            foreach (var item in datas)
            {
                geometryStr = string.Format("LINESTRING({0} {1}, {2} {3})", item.STR_84X, item.STR_84Y, item.END_84X, item.END_84Y);
                item.coordinate = DbGeometry.LineFromText(geometryStr, 4326);
            }
        }
    }
}
