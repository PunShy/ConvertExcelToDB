using ConvertExcelToDB.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;
using System.Data.Entity.Spatial;

namespace ConvertExcelToDB
{
    class GeoJsonToDb
    {
        private static CPAMIEntities _cpi = new CPAMIEntities();

        static void Main(string[] args)
        {
            string path = @"D:\GoogleDrive\CPC_CODE\ConvertExcelToDB\ConvertExcelToDB\Content\鄉鎮市框線.geojson";
            Rootobject movie1 = JsonConvert.DeserializeObject<Rootobject>(File.ReadAllText(path));

            //// deserialize JSON directly from a file
            //using (StreamReader file = File.OpenText(path))
            //{
            //    JsonSerializer serializer = new JsonSerializer();
            //    Rootobject movie2 = (Rootobject)serializer.Deserialize(file, typeof(Rootobject));
            //}
            List<Feature> ff1 = new List<Feature>();
            TaiwanCode tc1;
            foreach (var feature in movie1.features)
            {
                //if (feature.properties.名稱 != "南投縣國姓鄉") continue;
                tc1 = _cpi.TaiwanCode.Where(item => item.Name == feature.properties.名稱 ).FirstOrDefault();
                if (tc1 == null) continue;

                string multiPolygon = "";//@"MULTIPOLYGON(((1 1, 1 -1, -1 -1, -1 1, 1 1)),((1 1, 3 1, 3 3, 1 3, 1 1)))";
                string pointString = "", lineString = "";
                foreach (var lines in feature.geometry.coordinates)
                {
                    lineString = "";
                    foreach (var line in lines)
                    {
                        pointString = "";
                        foreach (var point in line)
                        {
                            pointString = string.Format("{0}{1} {2}, ", pointString, point[0], point[1]);
                        }
                        lineString = string.Format("{0}(({1})),", lineString, pointString.Substring(0, pointString.Length - 2));
                    }
                    lineString = lineString.Substring(0, lineString.Length - 1);
                }
                multiPolygon = string.Format("MULTIPOLYGON({0})", lineString);

                //MULTIPOLYGON(((120.8221 24.1035, 120.8592 24.0894, 120.8681 24.1035, 120.8221 24.1035)), ((120.8746 24.1000, 120.8712 24.0887, 120.9134 24.0975, 120.8746 24.1000)))
                tc1.Polygon = DbGeometry.MultiPolygonFromText(multiPolygon, 4326);
            }
            //_cpi.SaveChanges();
        }
    }


    public class Rootobject
    {
        public string type { get; set; }
        public string name { get; set; }
        public Crs crs { get; set; }
        public Feature[] features { get; set; }
    }

    public class Crs
    {
        public string type { get; set; }
        public Properties properties { get; set; }
    }

    public class Properties
    {
        public string name { get; set; }
    }

    public class Feature
    {
        public string type { get; set; }
        public Properties1 properties { get; set; }
        public Geometry geometry { get; set; }
    }

    public class Properties1
    {
        public string 名稱 { get; set; }
        public int 行政區域代碼 { get; set; }
        public int 比例尺分母 { get; set; }
    }

    public class Geometry
    {
        public string type { get; set; }
        public string[][][][] coordinates { get; set; }
    }

}
