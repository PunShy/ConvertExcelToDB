using ConvertExcelToDB.App_Code;
using Syncfusion.XlsIO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace ConvertExcelToDB
{
    class Program
    {
        private static string curDirectory = @"D:\GoogleDrive\CPC_Document\(S20883)雨水下水道普查及空間資料庫(CpamiSewer01)\PG\普查一期資料\新竹市\新竹市漁港特定區\";
        private static string curFileName = @"GIS建置規範OK.xlsx";

        private static ReadExcel _re1;

        static void Main(string[] args)
        {
            if (!Directory.Exists(curDirectory)) return;
            //ProcessDirectory(curFile, (a) => ProcessFile(a));
            
            _re1 = new ReadExcel();
            #region 撈指定資料夾底下所有檔案方式
            //ProcessFilesInDirectory dfid1 = new ProcessFilesInDirectory();
            //dfid1.ProcessDirectory(curDirectory, (filePath)=> ReadExcel(filePath));
            #endregion

            _re1.OpenExcel(curDirectory + curFileName);
            Console.WriteLine("OK");
            Console.Read();
        }

        //public static void ReadExcel(string fileName)
        //{
        //    _re1.OpenExcel(fileName);
        //}

    }

    public partial class ReadExcel
    {
        private string _conectionStr = @"Provider=SQLOLEDB;Server=localhost;uid=sa;pwd=as;database=CPAMI";//System.Web.Configuration.WebConfigurationManager.ConnectionStrings["ConnFormatString"].ConnectionString;
        private DbWoker _dw1;
        private ExcelEngine _excelEngine;
        private IApplication _application;
        private string targetId = "";

        public ReadExcel()
        {
            _excelEngine = new ExcelEngine();
            _application = _excelEngine.Excel;
            _dw1 = new DbWoker();
            _dw1.ConectionStr = _conectionStr;
        }

        private bool CheckVersion(string fileName)
        {
            if (fileName.IndexOf(".xls") > -1)
            {
                _application.DefaultVersion = ExcelVersion.Excel97to2003;
                return true;
            }
            else if (fileName.IndexOf(".xlsx") > -1)
            {
                _application.DefaultVersion = ExcelVersion.Excel2013;
                return true;
            }
            else return false;                 
        }

        public void OpenExcel(string fileName)
        {
            //try
            //{
            if (!CheckVersion(fileName)) return;

            IWorkbook workBook = _application.Workbooks.Open(fileName);
            IWorksheets sheets = workBook.Worksheets;

            int targetId = 285;//InsertGisMenu("T05", "10018", "新竹市", "新竹市漁港特定區"); //195;
            GetWorksheetRcp(sheets[0], targetId);//雨水竣工管線（102）- 804020102
            GetWorksheetRcm(sheets[1], targetId);//雨水竣工人孔（202）- 804020202
            GetWorksheetCp(sheets[2], targetId);//連接管（103）- 804020103
            GetWorksheetSw(sheets[3], targetId);//集水井（204）- 804020204
            GetWorksheetRd(sheets[4], targetId);//雨水側溝(601) - 804020601

            //}catch(Exception ex)
            //{
            //}
        }

        private int InsertGisMenu(string CityId,string CITY_ID,string townId,string text)
        {
            string str = @"INSERT INTO [dbo].[GIsMenu]
                          ([CityId]
                          ,[CITY_ID]
                          ,[townId]
                          ,[Status]
                          ,[Text]
                          ,[isShow])
                         VALUES
                          (?,?,?,?,?,?)";            
            OleDbCommand command = new OleDbCommand(str);
            command.Parameters.Add("CityId", OleDbType.VarChar).Value = CityId;
            command.Parameters.Add("CITY_ID", OleDbType.Integer).Value = CITY_ID;
            command.Parameters.Add("townId", OleDbType.VarChar).Value = townId;
            command.Parameters.Add("Status", OleDbType.VarChar).Value = "1";//1普查 3工程 4檢討規劃
            command.Parameters.Add("Text", OleDbType.VarChar).Value = text;//"雨水下水道系統檢討規劃";
            command.Parameters.Add("isShow", OleDbType.Boolean).Value = true;
            _dw1.InsertData(command);

            command.CommandText = @"SELECT id
                                    FROM GIsMenu
                                    WHERE CityId = ? and CITY_ID = ? and townId = ? and Status = ? and Text = ? and isShow = ?";
            DataTable dt1 = _dw1.GetData(command);
            return Int32.Parse(dt1.Rows[0][0].ToString());
        }

        private void GetWorksheetRcp(IWorksheet worksheet, int targetId)
        {
            var data = worksheet.ExportDataTable(worksheet.UsedRange, ExcelExportDataTableOptions.ComputedFormulaValues)
                .AsEnumerable();
            if (data == null) return;
            var dt1 = _dw1.DataTableToClasses<RainCompletedPipeline>(data);
            string commond = "";
            string nowDate = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
            foreach (var row1 in dt1)
            {
                row1.targetId = targetId;
                commond += _dw1.ClassToSqlInserCommand(row1, nowDate) + "\n";
            }
            OleDbCommand command = new OleDbCommand(commond);
            _dw1.InsertData(command);
        }
        private void GetWorksheetRcm(IWorksheet worksheet, int targetId)
        {
            var data = worksheet.ExportDataTable(worksheet.UsedRange, ExcelExportDataTableOptions.ComputedFormulaValues)
                .AsEnumerable();
            if (data == null) return;
            var dt1 = _dw1.DataTableToClasses<RainCompletedManhole>(data);
            string commond = "";
            string nowDate = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
            foreach (var row1 in dt1)
            {
                row1.targetId = targetId;
                commond += _dw1.ClassToSqlInserCommand(row1, nowDate) + "\n";
            }
            OleDbCommand command = new OleDbCommand(commond);
            _dw1.InsertData(command);
        }
        private void GetWorksheetCp(IWorksheet worksheet, int targetId)
        {
            var data = worksheet.ExportDataTable(worksheet.UsedRange, ExcelExportDataTableOptions.ComputedFormulaValues)
                .AsEnumerable();
            if (data == null) return;
            var dt1 = _dw1.DataTableToClasses<ConnectingPipe>(data);
            string commond = "";
            string nowDate = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
            foreach (var row1 in dt1)
            {
                row1.targetId = targetId;
                commond += _dw1.ClassToSqlInserCommand(row1, nowDate) + "\n";
            }
            OleDbCommand command = new OleDbCommand(commond);
            _dw1.InsertData(command);
        }
        private void GetWorksheetSw(IWorksheet worksheet, int targetId)
        {
            var data = worksheet.ExportDataTable(worksheet.UsedRange, ExcelExportDataTableOptions.ComputedFormulaValues)
                .AsEnumerable();
            if (data == null) return;
            var dt1 = _dw1.DataTableToClasses<SetWells>(data);
            string commond = "";
            string nowDate = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
            foreach (var row1 in dt1)
            {
                row1.targetId = targetId;
                commond += _dw1.ClassToSqlInserCommand(row1, nowDate) + "\n";
            }
            OleDbCommand command = new OleDbCommand(commond);
            _dw1.InsertData(command);
        }
        private void GetWorksheetRd(IWorksheet worksheet, int targetId)
        {
            var data = worksheet.ExportDataTable(worksheet.UsedRange, ExcelExportDataTableOptions.ComputedFormulaValues)
                .AsEnumerable();
            if (data == null) return;
            var dt1 = _dw1.DataTableToClasses<RainwaterDitch>(data);
            string commond = "";
            string nowDate = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
            foreach (var row1 in dt1)
            {
                row1.targetId = targetId;
                commond += _dw1.ClassToSqlInserCommand(row1, nowDate) + "\n";
            }
            OleDbCommand command = new OleDbCommand(commond);
            _dw1.InsertData(command);
        }


    }

    public class ProcessFilesInDirectory
    {
        public void ProcessDirectory(string targetDirectory, Action<string> ProcessFile)
        {
            // Process the list of files found in the directory.
            ProcessFilePath(targetDirectory, ProcessFile);

            // Recurse into subdirectories of this directory.
            string[] subdirectoryEntries = Directory.GetDirectories(targetDirectory);
            foreach (string subdirectory in subdirectoryEntries)
            {
                ProcessDirectory(subdirectory, ProcessFile);
            }
        }

        public void ProcessFilePath(string targetFile, Action<string> ProcessFile)
        {
            string[] fileEntries = Directory.GetFiles(targetFile);
            foreach (string fileName in fileEntries)
            {
                ProcessFile(fileName);
            }
        }
    }

    
}
