using ConvertExcelToDB.App_Code;
using ConvertExcelToDB.Model;
using Syncfusion.XlsIO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Spatial;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Xml.Linq;

namespace ConvertExcelToDB
{
    class KmlToDB
    {
        private static CPAMIEntities _db;
        //private static string curDirectory = @"D:\GoogleDrive\CPC_Document\(S20883)雨水下水道普查及空間資料庫(CpamiSewer01)\2017-10-06至台灣世曦開會\Cpamisewer\MAPBASE\UrbanPlanOuterline\";
        private static string curDirectory = @"D:\Downloads\";

        static void Main(string[] args)
        {
            _db = new CPAMIEntities();

            XNamespace ns = @"http://www.opengis.net/kml/2.2";
            string[] files = Directory.GetFiles(curDirectory);

            if (!Directory.Exists(curDirectory)) return;
            string insertString = "";
            string ParentId = "10016";
            foreach (var item in files)
            {
                //if (!item.Contains("澎湖縣")) continue;
                if (!item.Contains("a123.kml")) continue;
                var doc = XDocument.Load(item);

                var taiwnaCode = _db.TaiwanCode.Where(a => string.IsNullOrEmpty(a.ParentId))
                    .Select(a => new
                    {
                        a.Code,
                        Name = a.Name.Substring(0, a.Name.Length - 1),
                    }).ToList();
                //kml內文規則不同ver.1
                //var query = doc.Root
                //               .Element(ns + "Document")
                //               .Elements(ns + "Placemark")
                //               .Select(x => new
                //               {
                //                   Name = x.Element(ns + "name").Value,
                //                   Description = x.Element(ns + "description").Value,
                //                   MultiGeometry = x.Element(ns + "MultiGeometry")
                //                   .Elements(ns + "Polygon").Select(a => new
                //                   {
                //                       coordinates = a.Element(ns + "outerBoundaryIs").Element(ns + "LinearRing").Element(ns + "coordinates").Value.Split(' ')
                //                   }).ToList()
                //               }).ToList();
                //kml內文規則不同ver.2
                var query = doc.Root.Element(ns + "Document").Element(ns + "Folder").Elements(ns + "Placemark")
                        .Select(x => new K1Model {
                            Name = x.Element(ns+ "ExtendedData").Element(ns+ "SchemaData").Elements(ns + "SimpleData").Attributes().Where(a=>a.Name == "name" && a.Value == "名稱").Select(a=>a.Parent.Value).FirstOrDefault(),
                            Code = x.Element(ns + "ExtendedData").Element(ns + "SchemaData").Elements(ns + "SimpleData").Attributes().Where(a => a.Name == "name" && a.Value == "行政區域代碼").Select(a => a.Parent.Value).FirstOrDefault(),
                            MultiGeometry = x.Element(ns + "MultiGeometry")
                                               .Elements(ns + "Polygon").Select(a => new K2Model
                                               {
                                                   coordinates = a.Element(ns + "outerBoundaryIs").Element(ns + "LinearRing").Element(ns + "coordinates").Value.Split(' ')
                                               }).ToList()
                        }).ToList();
                for (int i = 0, length = query.Count; i < length; i++)
                {
                    var val = query[i];
                    if (val.Code[0] == '6') { val.Code = val.Code.Substring(0, 2); }
                    if (val.Code[0] == '9') { val.Code = "0" + val.Code; }
                }


                string muiltPolygon = "";
                foreach (var val in query)
                {
                    muiltPolygon = "";
                    foreach (var polygon in val.MultiGeometry)
                    {
                        //將KML coordinates格式轉成SQL空間用格式
                        var xyList = polygon.coordinates.Where(a => !string.IsNullOrEmpty(a))
                            .Select(a =>
                            {

                                var aa = a.Split(',');//.Replace(',', ' ');
                                var X = Convert.ToDecimal(aa[0]).ToString();
                                var Y = Convert.ToDecimal(aa[1]).ToString();
                                return X + " " + Y;
                            }).ToArray();
                        muiltPolygon += "((" + string.Join(",", xyList) + ")),";
                    }
                    //TO-DO 已將組出來
                    muiltPolygon = @"MULTIPOLYGON(" + muiltPolygon.Substring(0, muiltPolygon.Length - 1) + ")";
                    //TaiwanCode tc1 = new TaiwanCode();
                    //tc1.Name = val.Name;
                    //tc1.Code = "temp123";
                    //tc1.ParentId = taiwnaCode.Where(a => tc1.Name.Contains(a.Name) == true).Select(a => a.Code).FirstOrDefault();
                    //tc1.Polygon = DbGeography.MultiPolygonFromText(muiltPolygon, 4326);

                    //ver.1
                    insertString += @"INSERT INTO [dbo].[TaiwanCode]
                                       ([Code]
                                       ,[Name]
                                       ,[ParentId]
                                       ,[WGS84_X]
                                       ,[WGS84_Y]
                                       ,[Polygon])
                                    VALUES
                                       ('fail_Code'
                                       ,'" + val.Name + @"'
                                       ,'" + ParentId + @"'
                                       ,NULL
                                       ,NULL
                                       ,geometry::STGeomFromText('" + muiltPolygon + @"', 4326).MakeValid()
                                 )
                                    ";




                    //_db.TaiwanCode.SqlQuery(@"INSERT INTO [dbo].[TaiwanCode]
                    //           ([Code]
                    //           ,[Name]
                    //           ,[ParentId]
                    //           ,[WGS84_X]
                    //           ,[WGS84_Y]
                    //           ,[Polygon])
                    //     VALUES
                    //           ('S0102_G'
                    //           ,'岡山都市計畫'
                    //           ,64
                    //           ,NULL
                    //           ,NULL
                    //           ,geography::STGeomFromText('MULTIPOLYGON(((120.20622253417969 22.478933900916914, 120.4541015625 22.478933900916914, 120.4541015625 22.7641520036934, 120.20622253417969 22.7641520036934, 120.20622253417969 22.478933900916914)), ((120.26870727539062 23.318296703316737, 120.65322875976562 23.318296703316737, 120.65322875976562 23.613070293780517, 120.26870727539062 23.613070293780517,120.26870727539062 23.318296703316737)))', 4326)
                    //     )");
                }
            }
            _db.Database.Log = Console.WriteLine;
            //_db.SaveChanges();
            Console.WriteLine("OK");
            Console.Read();
        }

    }

    public class K1Model
    {
        public string Name { set; get; }
        public string Code { set; get; }
        public List<K2Model> MultiGeometry { set; get; }
    }
    public class K2Model
    {
        public string[] coordinates { set; get; }
    }
}
