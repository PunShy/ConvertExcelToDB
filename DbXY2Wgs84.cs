using ConvertExcelToDB.Model;
using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConvertExcelToDB
{
    class DbXY2Wgs84
    {
        private static CPAMIEntities _cpi = new CPAMIEntities();

        static void Main(string[] args)
        {
            GetWorksheetCp();
            Console.WriteLine("OK");
            Console.Read();
        }
        /// <summary>
        /// 轉換程式
        /// 資料庫的 TWD97的X,Y => WGS84的 X,Y 並寫回對應欄位
        /// </summary>
        private static void GetWorksheetCp()
        {
            var query = _cpi.RainCompletedManhole.Where(a => a.Wgs84X == null && a.Wgs84Y == null).ToList();

            //var query = _cpi.RainCompletedPipeline.Where(a => (a.US_84X == null && a.US_84Y == null) || (a.DS_84X == null && a.DS_84Y == null));
            //.Where(a => a.targetId == 27);

            //var query = _cpi.RainwaterDitch.Where(a => (!string.IsNullOrEmpty(a.STR_X) && !string.IsNullOrEmpty(a.STR_Y))
            //&& (!string.IsNullOrEmpty(a.END_X) && !string.IsNullOrEmpty(a.END_Y))).ToList();

            //var query = _cpi.SetWells.Where(a => a.Wgs84X == null && a.Wgs84Y == null).ToList();

            foreach (var item in query)
            {
                //RainCompletedManhole
                double x = Convert.ToDouble(item.X);
                double y = Convert.ToDouble(item.Y);
                double[] coordinate = new double[] { x, y };
                var cor = xy_2_lnglat(coordinate);
                item.Wgs84X = cor[0].ToString();
                item.Wgs84Y = cor[1].ToString();

                ////RainCompletedPipeline
                //if (string.IsNullOrEmpty(item.US_X)) continue;
                //double x = Convert.ToDouble(item.US_X);
                //double y = Convert.ToDouble(item.US_Y);
                //double[] coordinate = new double[] { x, y };
                //var cor = xy_2_lnglat(coordinate);
                //item.US_84X = cor[0].ToString();
                //item.US_84Y = cor[1].ToString();
                //x = Convert.ToDouble(item.DS_X);
                //y = Convert.ToDouble(item.DS_Y);
                //coordinate[0] = x; coordinate[1] = y;
                //cor = xy_2_lnglat(coordinate);
                //item.DS_84X = cor[0].ToString();
                //item.DS_84Y = cor[1].ToString();

                ////RainwaterDitch
                //if (string.IsNullOrEmpty(item.STR_X)) continue;
                //double x = Convert.ToDouble(item.STR_X);
                //double y = Convert.ToDouble(item.STR_Y);
                //double[] coordinate = new double[] { x, y };
                //var cor = xy_2_lnglat(coordinate);
                //item.STR_84X = cor[0].ToString();
                //item.STR_84Y = cor[1].ToString();
                //x = Convert.ToDouble(item.END_X);
                //y = Convert.ToDouble(item.END_Y);
                //coordinate[0] = x; coordinate[1] = y;
                //cor = xy_2_lnglat(coordinate);
                //item.END_84X = cor[0].ToString();
                //item.END_84Y = cor[1].ToString();

                ////SetWells
                //if (string.IsNullOrEmpty(item.X)) continue;
                //double x = Convert.ToDouble(item.X);
                //double y = Convert.ToDouble(item.Y);
                //double[] coordinate = new double[] { x, y };
                //var cor = xy_2_lnglat(coordinate);
                //item.Wgs84X = cor[0].ToString();
                //item.Wgs84Y = cor[1].ToString();
            }
            _cpi.Database.Log = Console.WriteLine;
            _cpi.SaveChanges();
        }
        
        /// <summary>
        /// TWD97座標 轉 經緯度
        /// </summary>
        /// <param name="coordinate">TWD97座標</param>
        /// <returns>經緯度座標</returns>
        private static List<double> xy_2_lnglat(double[] coordinate)
        {
            double tr_x = coordinate[0]; double tr_y = coordinate[1];
            double LNG_X, LAT_Y;
            double lon_origin = 121;
            //double lon_origin = 120; //金門、馬祖
            //double lon_origin = 119; //金門、馬祖
            double k = 0.9999;
            double pi = 4 * Math.Atan(1.0);
            double offset = 250000;
            double axis_a = 6378137.000;
            double axis_b = 6356752.314;

            double x = tr_x - offset;
            double y = tr_y;

            double M = y / k;
            double eccentricity = (Math.Pow(axis_a, 2) - Math.Pow(axis_b, 2)) / Math.Pow(axis_a, 2);
            double eccentricity_ = eccentricity / (1 - eccentricity);
            double e1 = (1 - Math.Sqrt(1 - eccentricity)) / (1 + Math.Sqrt(1 - eccentricity));
            double mu = M / (axis_a * (1 - eccentricity / 4 - 3 * Math.Pow(eccentricity, 2) / 64 - 5 * Math.Pow(eccentricity, 3) / 256));
            double phi = mu + (3 * e1 / 2 - 27 * Math.Pow(e1, 3) / 32) * Math.Sin(2 * mu) + (21 * Math.Pow(e1, 2) / 16 - 55 * Math.Pow(e1, 4) / 32) * Math.Sin(4 * mu) + (151 * Math.Pow(e1, 3) / 96) * Math.Sin(6 * mu);
            double N1 = axis_a / Math.Sqrt(1 - eccentricity * Math.Sin(phi) * Math.Sin(phi));
            double T1 = Math.Pow(Math.Tan(phi), 2);
            double C1 = eccentricity_ * Math.Pow(Math.Cos(phi), 2);
            double R1 = axis_a * (1 - eccentricity) / Math.Pow(1 - eccentricity * Math.Sin(phi) * Math.Sin(phi), 1.5);
            double D = x / (N1 * k);

            LAT_Y = phi - (N1 * Math.Tan(phi) / R1) * (Math.Pow(D, 2) / 2 - (5 + 3 * T1 + 10 * C1 - 4 * Math.Pow(C1, 2) - 9 * eccentricity_) * Math.Pow(D, 4) / 24 + (61 + 90 * T1 + 298 * C1 + 45 * Math.Pow(T1, 2) - 252 * eccentricity_ - 3 * C1 * C1) * Math.Pow(D, 6) / 720);
            LAT_Y = LAT_Y * 180 / pi;

            LNG_X = (D - (1 + 2 * T1 + C1) * Math.Pow(D, 3) / 6 + (5 - 2 * C1 + 28 * T1 - 3 * Math.Pow(C1, 2) + 8 * eccentricity_ + 24 * Math.Pow(T1, 2)) * Math.Pow(D, 5) / 120) / Math.Cos(phi);
            LNG_X = lon_origin + LNG_X * 180 / pi;
            return new List<double> { LNG_X, LAT_Y };
        }
    }
}
