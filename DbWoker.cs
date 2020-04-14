using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Globalization;
using System.Reflection;

/// <summary>
/// Test 的摘要描述
/// </summary>
public class DbWoker
{
    //System.Web.Configuration.WebConfigurationManager.ConnectionStrings["ConnFormatString"].ConnectionString;
    private string _conectionStr = "";

    public string ConectionStr
    {
        get
        {
            return _conectionStr;
        }

        set
        {
            _conectionStr = value;
        }
    }

    public DbWoker()
    {

    }
    public DbWoker(string conection)
    {
        _conectionStr = conection;
    }
    
    public DataTable GetData(OleDbCommand command)
    {
        DataTable dt1 = new DataTable();
        using (OleDbConnection connection = new OleDbConnection(_conectionStr))
        {  
            OleDbDataAdapter adapter = new OleDbDataAdapter();
            command.Connection = connection;

            adapter.SelectCommand = command;
            adapter.Fill(dt1);
        }
        return dt1;
    }
    public int InsertData(OleDbCommand command)
    {
        int result = 0;
        using (OleDbConnection connection = new OleDbConnection(_conectionStr))
        {
            connection.Close();
            connection.Open();

            OleDbDataAdapter adapter = new OleDbDataAdapter();
            command.Connection = connection;

            adapter.InsertCommand = command;
            result = command.ExecuteNonQuery();

            connection.Close();
            connection.Dispose();
        }
        return result;
    }
    public int UpdateData(OleDbCommand command)
    {
        int result = 0;
        using (OleDbConnection connection = new OleDbConnection(_conectionStr))
        {
            connection.Close();
            connection.Open();
            OleDbDataAdapter adapter = new OleDbDataAdapter();
            command.Connection = connection;

            adapter.UpdateCommand = command;
            result = adapter.UpdateCommand.ExecuteNonQuery();
            connection.Close();
            connection.Dispose();
        }
        return result;
    }
    public int DeleteData(OleDbCommand command)
    {
        int result = 0;
        using (OleDbConnection connection = new OleDbConnection(_conectionStr))
        {
            connection.Close();
            connection.Open();
            OleDbDataAdapter adapter = new OleDbDataAdapter();
            command.Connection = connection;

            adapter.DeleteCommand = command;
            result = adapter.DeleteCommand.ExecuteNonQuery();
            connection.Close();
            connection.Dispose();
        }
        return result;
    }

    /// <summary>
    /// DataTable轉成指定Class物件，簡易版本。
    /// EX: var dt1 = _dbWoker1.GetData(command).AsEnumerable();
    ///     List<StatisticsChart> svmList = GetDbData<StatisticsChart>(dt1);
    /// </summary>
    /// <typeparam name="T">轉換目標Class</typeparam>
    /// <param name="dt1">來源DB資料，dt1.AsEnumerable()</param>
    /// <returns>指定Calss物件的集合</returns>
    public List<T> DataTableToClasses<T>(EnumerableRowCollection<DataRow> dt1) where T : class, new()
    {
        List<T> rcmList = new List<T>();
        PropertyInfo[] temp;
        T rcm1 = null;
        int index = 0;
        string tempColVal = "";
        foreach (var item in dt1)
        {   
            if(index == 0)
            {
                foreach (DataColumn dc1 in item.Table.Columns)
                {
                    tempColVal = item[dc1.Ordinal].ToString();
                    if (string.IsNullOrEmpty(tempColVal)) continue;
                    dc1.ColumnName = tempColVal;
                }
            }
            else { 
                temp = typeof(T).GetProperties();
                rcm1 = MappingDataRowToClass<T>(item, temp);
                rcmList.Add(rcm1);
            }
            index++;
        }
        return rcmList;
    }

    private T MappingDataRowToClass<T>(DataRow row, IList<PropertyInfo> properties) where T : new()
    {
        T item = new T();
        string propertyName;
        bool ispass;
        foreach (var property in properties)
        { 
            propertyName = property.Name;

            if (row.Table.Columns.Contains(propertyName))
            {
                var colVal = row[propertyName];

                //針對欄位的型態去轉換
                if (property.PropertyType == typeof(DateTime?))
                {
                    colVal.ToString();
                    DateTime dt = new DateTime();
                    if (DateTime.TryParse(colVal.ToString(), out dt) || CheckDateType(colVal.ToString(), out dt))
                    {
                        property.SetValue(item, dt, null);
                    }
                    else
                    {
                        property.SetValue(item, null, null);
                    }
                }
                else if (property.PropertyType == typeof(decimal?))
                {
                    decimal val = new decimal();
                    ispass = decimal.TryParse(colVal.ToString(), out val);
                    decimal? val1 = null;
                    if (ispass) val1 = Decimal.Round(val,10);
                    property.SetValue(item, val1, null);
                }
                else if (property.PropertyType == typeof(double?))
                {
                    double val = new double();
                    double.TryParse(colVal.ToString(), out val);
                    property.SetValue(item, val, null);
                }
                else if (property.PropertyType == typeof(int?))
                {
                    int val;
                    ispass = int.TryParse(colVal.ToString(), out val);
                    int? val1 = null;
                    if (ispass) val1 = val;
                    property.SetValue(item, val1, null);
                }
                else
                {
                    if (colVal != DBNull.Value)
                    {
                        if (colVal.ToString() == "NULL") colVal = null;
                        property.SetValue(item, colVal, null);
                    }
                }
            }
        }
        return item;
    }

    /// <summary>
    /// 將物件轉成Insert指令
    /// </summary>
    /// <returns></returns>
    public string ClassToSqlInserCommand<T>(T data, string nowDate)
    {
        var typeCols = data.GetType().GetProperties();
        string insertCom = "INSERT [dbo].[{0}] ({1}) VALUES ({2})", insertCol = "", insertVal = "";
        foreach (var item in typeCols)
        {
            insertCol = string.Format(@"{0}[{1}],", insertCol, item.Name);
            insertVal = string.Format(@"{0}{1}", insertVal, GetInsertOfVal(item, data));// item.GetValue(data)
        }
        insertCol += "ImportDate";
        insertVal += "'" + nowDate + "'";
        //insertCol = insertCol.Substring(0,insertCol.Length-1);
        //insertVal = insertVal.Substring(0,insertVal.Length-1);
        insertCom = string.Format(insertCom, typeCols[0].ReflectedType.Name, insertCol, insertVal);
        return insertCom;
    }

    private string GetInsertOfVal<T>(PropertyInfo pInfo,T data)
    {
        string val = "";
        Type propertyType = pInfo.PropertyType;
        var temp = pInfo.GetValue(data);
        if (propertyType == typeof(DateTime?))
        {
            if (temp == null)
            {
                val = string.Format(@"NULL,");
            }
            else
            {
                val = string.Format(@"'{0}',", ((DateTime)temp).ToString("yyyy-MM-dd"));
            }
        }
        else if (propertyType == typeof(decimal?))
        {
            if (temp == null)
            {
                val = string.Format(@"NULL,");
            }
            else
            {
                val = string.Format(@"{0},", pInfo.GetValue(data));
            }
        }
        else if (propertyType == typeof(double))
        {
            val = string.Format(@"{0},", temp);
        }
        else if (propertyType == typeof(int?))
        {
            if(temp == null)
            {
                val = string.Format(@"NULL,");
            }
            else
            {
                val = string.Format(@"{0},", temp);
            }
        }
        else
        {
            object vals = (temp != null)? temp.ToString().Replace("'", "''"): temp;
            val = string.Format(@"'{0}',", vals);
        }
        return val;
    }

    public bool CheckDateType(string date, out DateTime dt)
    {
        if(DateTime.TryParseExact(date, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
        {
            return true;
        }
        if(DateTime.TryParseExact(date, "yyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out dt))
        {
            dt = dt.TwToUSDate();
            return true;
        }
        return false;
    }
}

public static class Extend
{
    public static DateTime TwToUSDate(this DateTime dtx)
    {
        return dtx.AddYears(1911);
    }
}

#region DbWoker 調用範例
public class DbWorker_Sample
{
    public static string delete()
    {
        DbWoker dw1 = new DbWoker();
        dw1.ConectionStr = @"Provider=SQLOLEDB;Server=localhost;uid=sa;pwd=as;database=CPAMI";//System.Web.Configuration.WebConfigurationManager.ConnectionStrings["ConnFormatString"].ConnectionString;
        string str = @"DELETE FROM [dbo].[ContentData]
                           WHERE kind=? and city=? and step=?";
        OleDbCommand command = new OleDbCommand(str);
        command.Parameters.Add("kind", OleDbType.VarChar).Value = "2";
        command.Parameters.Add("city", OleDbType.VarChar).Value = "T11";
        command.Parameters.Add("step", OleDbType.VarChar).Value = "1";

        int count = dw1.InsertData(command);
        return "";
    }
    public static string update()
    {
        DbWoker dw1 = new DbWoker();
        dw1.ConectionStr = @"Provider=SQLOLEDB;Server=localhost;uid=sa;pwd=as;database=CPAMI";//System.Web.Configuration.WebConfigurationManager.ConnectionStrings["ConnFormatString"].ConnectionString;
        string str = @"UPDATE [dbo].[ContentData]
                           SET [textInfo]=?
                           WHERE kind=? and city=? and step=?";
        OleDbCommand command = new OleDbCommand(str);
        command.Parameters.Add("textInfo", OleDbType.VarWChar).Value = "ER只是測試看看內文物???";
        command.Parameters.Add("kind", OleDbType.VarChar).Value = "2";
        command.Parameters.Add("city", OleDbType.VarChar).Value = "T11";
        command.Parameters.Add("step", OleDbType.VarChar).Value = "1";

        int count = dw1.InsertData(command);
        return "";
    }
    public static string Insert()
    {
        DbWoker dw1 = new DbWoker();
        dw1.ConectionStr = @"Provider=SQLOLEDB;Server=localhost;uid=sa;pwd=as;database=CPAMI";//System.Web.Configuration.WebConfigurationManager.ConnectionStrings["ConnFormatString"].ConnectionString;
        string str = @"INSERT INTO [dbo].[ContentData]
           ([kind]
           ,[city]
           ,[step]
           ,[planName]
           ,[textInfo]
           ,[date])
     VALUES
           (?,?,?,?,?,?)";
        OleDbCommand command = new OleDbCommand(str);
        command.Parameters.Add("kind", OleDbType.VarChar).Value = "2";
        command.Parameters.Add("city", OleDbType.VarChar).Value = "T11";
        command.Parameters.Add("step", OleDbType.VarChar).Value = "1";
        command.Parameters.Add("planName", OleDbType.VarWChar).Value = "只是測試看看";
        command.Parameters.Add("textInfo", OleDbType.VarWChar).Value = "只是測試看看內文物";
        command.Parameters.Add("date", OleDbType.Date).Value = DateTime.Now.ToString("yyyy/MM/dd HH:mm:sss");

        int count = dw1.InsertData(command);
        return "";
    }

    public static string Get()
    {
        DbWoker dw1 = new DbWoker();
        dw1.ConectionStr = @"Provider=SQLOLEDB;Server=localhost;uid=sa;pwd=as;database=CPAMI";//System.Web.Configuration.WebConfigurationManager.ConnectionStrings["ConnFormatString"].ConnectionString;
        string str = @"SELECT distinct city,planName,textInfo
                       FROM ContentData
                       where kind = ? and city = ?";
        OleDbCommand command = new OleDbCommand(str);
        command.Parameters.Add("kind", OleDbType.VarChar).Value = "2";
        command.Parameters.Add("city", OleDbType.VarChar).Value = "T10";

        DataTable dt1 = dw1.GetData(command);
        return "";
    }
}
#endregion