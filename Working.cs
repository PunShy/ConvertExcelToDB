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
    class Working
    {
        private static string curDirectory = @"D:\GoogleDrive\CPC_Document\(S20883)雨水下水道普查及空間資料庫(CpamiSewer01)\PG\普查一期資料\新竹市\新竹市區\";
        private static string curFileName = @"二期-縱走調查屬性資料表(含TV)OK-1080628.xls";

        private static ReadExcel _re1;

        //管線新增XY軸
        static void Main(string[] args)
        {
            if (!Directory.Exists(curDirectory)) return;
            //ProcessDirectory(curFile, (a) => ProcessFile(a));

            _re1 = new ReadExcel();

            _re1.OpenExcel2(curDirectory + curFileName);

            foreach (var item in _re1.logPiNo)
            {
                Console.WriteLine(item);
            }
            Console.WriteLine("OK");
            Console.Read();
        }
    }

    public partial class ReadExcel
    {
        public List<string> logPiNo = new List<string>();
        public void OpenExcel2(string fileName)
        {
            //try
            //{
            if (!CheckVersion(fileName)) return;

            IWorkbook workBook = _application.Workbooks.Open(fileName);
            IWorksheets sheets = workBook.Worksheets;

            int targetId = 286;
            //GetWorksheetPipeSilt(sheets[0], targetId);//雨水下水道淤積紀錄
            //GetWorksheetPipeCross(sheets[1], targetId);//雨水下水道管線橫越紀錄
            //GetWorksheetPipeOther(sheets[2], targetId);//雨水下水道管線破損及其他紀錄
            //GetWorksheetPipeCableAttach(sheets[3], targetId);//雨水下水道纜線附掛紀錄
            GetWorksheetPipeUnableWalk(sheets[4], targetId);//雨水下水道無法縱走紀錄

            ReWorkingReport(targetId);

            //}catch(Exception ex)
            //{
            //}
        }

        /// <summary>
        /// 缺失-擴充項目
        /// 針對有疑慮項目再施工再出針對性的報告
        /// 可能會缺管缺孔之類的
        /// </summary>
        private void ReWorkingReport(int targetId)
        {
            
        }

        private void GetWorksheetPipeSilt(IWorksheet worksheet, int targetId)
        {
            var data = worksheet.ExportDataTable(worksheet.UsedRange, ExcelExportDataTableOptions.ComputedFormulaValues)
                .AsEnumerable();
            if (data == null) return;
            var dt1 = _dw1.DataTableToClasses<PipeSilt>(data);
            string commond = "";
            string nowDate = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
            foreach (var row1 in dt1)
            {
                row1.targetId = targetId;
                string existStr = IsExistOfPiNum(targetId, row1.P_NO, 0);
                if (existStr == "")
                {
                    row1.MEMO += (string.IsNullOrEmpty(row1.MEMO)) ? "找不到管線" : ",找不到管線";
                    logPiNo.Add(row1.P_NO);
                }
                else
                {
                    row1.P_NO = existStr;
                }
                commond += _dw1.ClassToSqlInserCommand(row1, nowDate) + "\n";
            }
            OleDbCommand command = new OleDbCommand(commond);
            _dw1.InsertData(command);
        }

        private void GetWorksheetPipeCross(IWorksheet worksheet, int targetId)
        {
            var data = worksheet.ExportDataTable(worksheet.UsedRange, ExcelExportDataTableOptions.ComputedFormulaValues)
                .AsEnumerable();
            if (data == null) return;
            var dt1 = _dw1.DataTableToClasses<PipeCross>(data);
            string commond = "";
            string nowDate = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
            foreach (var row1 in dt1)
            {
                row1.targetId = targetId;
                string existStr = IsExistOfPiNum(targetId, row1.P_NO, 0);
                if (existStr == "")
                {
                    row1.MEMO += (string.IsNullOrEmpty(row1.MEMO)) ? "找不到管線" : ",找不到管線";
                    logPiNo.Add(row1.P_NO);
                }
                else
                {
                    row1.P_NO = existStr;
                }
                commond += _dw1.ClassToSqlInserCommand(row1, nowDate) + "\n";
            }
            OleDbCommand command = new OleDbCommand(commond);
            _dw1.InsertData(command);
        }

        private void GetWorksheetPipeOther(IWorksheet worksheet, int targetId)
        {
            var data = worksheet.ExportDataTable(worksheet.UsedRange, ExcelExportDataTableOptions.ComputedFormulaValues)
                .AsEnumerable();
            if (data == null) return;
            var dt1 = _dw1.DataTableToClasses<PipeOther>(data);
            string commond = "";
            string nowDate = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
            foreach (var row1 in dt1)
            {
                row1.targetId = targetId;
                string existStr = IsExistOfPiNum(targetId, row1.P_NO, 0);
                if (existStr == "")
                {
                    row1.MEMO += (string.IsNullOrEmpty(row1.MEMO)) ? "找不到管線" : ",找不到管線";
                    logPiNo.Add(row1.P_NO);
                }
                else
                {
                    row1.P_NO = existStr;
                }
                commond += _dw1.ClassToSqlInserCommand(row1, nowDate) + "\n";
            }
            OleDbCommand command = new OleDbCommand(commond);
            _dw1.InsertData(command);
        }

        private void GetWorksheetPipeCableAttach(IWorksheet worksheet, int targetId)
        {
            var data = worksheet.ExportDataTable(worksheet.UsedRange, ExcelExportDataTableOptions.ComputedFormulaValues)
                .AsEnumerable();
            if (data == null) return;
            var dt1 = _dw1.DataTableToClasses<PipeCableAttach>(data);
            string commond = "";
            string nowDate = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
            foreach (var row1 in dt1)
            {
                row1.targetId = targetId;
                string existStr = IsExistOfPiNum(targetId, row1.P_NO, 0);
                if (existStr == "")
                {
                    row1.MEMO += (string.IsNullOrEmpty(row1.MEMO)) ? "找不到管線" : ",找不到管線";
                    logPiNo.Add(row1.P_NO);
                }
                else
                {
                    row1.P_NO = existStr;
                }
                commond += _dw1.ClassToSqlInserCommand(row1, nowDate) + "\n";
            }
            OleDbCommand command = new OleDbCommand(commond);
            _dw1.InsertData(command);
        }

        private void GetWorksheetPipeUnableWalk(IWorksheet worksheet, int targetId)
        {
            var data = worksheet.ExportDataTable(worksheet.UsedRange, ExcelExportDataTableOptions.ComputedFormulaValues)
                .AsEnumerable();
            if (data == null) return;
            var dt1 = _dw1.DataTableToClasses<PipeUnableWalk>(data);
            string commond = "";
            string nowDate = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
            foreach (var row1 in dt1)
            {
                row1.targetId = targetId;
                string existStr = IsExistOfPiNum(targetId, row1.P_NO, 0);
                if (existStr == "")
                {
                    row1.MEMO += (string.IsNullOrEmpty(row1.MEMO)) ? "找不到管線" : ",找不到管線";
                    logPiNo.Add(row1.P_NO);
                }
                else
                {
                    row1.P_NO = existStr;
                }
                commond += _dw1.ClassToSqlInserCommand(row1, nowDate) + "\n";
            }
            OleDbCommand command = new OleDbCommand(commond);
            _dw1.InsertData(command);
        }

        /// <summary>
        /// 管線是否存在
        /// </summary>
        /// <returns></returns>
        private string IsExistOfPiNum(int targetId, string piNum, int intoCount)
        {
            intoCount++;
            string str = @"select * from RainCompletedPipeline where targetId = ? and PI_NUM = ?";
            OleDbCommand command = new OleDbCommand(str);
            command.Parameters.Add("targetId", OleDbType.VarChar).Value = targetId;
            command.Parameters.Add("PI_NUM", OleDbType.VarChar).Value = piNum;
            if (piNum == null) return "";
            DataRow dr1 = _dw1.GetData(command).AsEnumerable().FirstOrDefault();
            if (dr1 == null && intoCount < 2)
            {
                char mid = '~';
                string[] ss = piNum.Split(mid);
                if(ss.Length == 1)
                {
                    ss = piNum.Split('-');
                }
                string piNum2 = ss[1] + mid + ss[0];
                string val = IsExistOfPiNum(targetId, piNum2 , intoCount);
                return val;
            }
            else if(dr1 != null)
            {
                return piNum;
            }
            return "";
        }

    }
}
