using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConvertExcelToDB.App_Code
{
    public class RainCompletedManhole
    {
        public int targetId { set; get; }
        public string MH_NUM { get; set; }
        public string X { get; set; }
        public string Y { get; set; }
        public string ROAD_NAME { get; set; }
        public decimal? MH_DEP { get; set; }
        public string MH_TYP { get; set; }
        public string MH_CLASS { get; set; }
        public int? MH_LENG { get; set; }
        public int? MH_WID { get; set; }
        public decimal? ROAD_WID { get; set; }
        public decimal? FALL_MAX { get; set; }
        public decimal? MH_TLE { get; set; }
        public DateTime? CONS_DATE { get; set; }
        public string CITY_ID { get; set; }
        public string CONS_ID { get; set; }
        public string CONS_TIT { get; set; }
        public string CONS_DEPT { get; set; }
        public string CONS_NAME { get; set; }
        public DateTime? KEYIN_DATE { get; set; }
        public string MH_PIC { get; set; }
        public string MH_EXVIEW { get; set; }
        public string MH_MARK { get; set; }
        public string NOTE { get; set; }
    }

    public class RainCompletedPipeline
    {
        private decimal? _piSlop;

        public int? targetId { set; get; }
        public string SSEW_CAT { set; get; }
        public string PI_NUM { get; set; }
        public string US_MH { get; set; }
        public string DS_MH { get; set; }
        public string PI_TYP { get; set; }
        public string PI_CLASS { get; set; }
        public decimal? PI_WIDT { get; set; }
        public decimal? PI_WIDB { get; set; }
        public decimal? PI_HEI { get; set; }
        public decimal? PI_LENG { get; set; }
        public string PI_MAT { get; set; }
        public decimal? PI_SLOP
        {
            get
            {
                if (US_BLE == null || DS_BLE == null || PI_LENG == null) return null;
                try
                {
                    decimal val = (US_BLE.Value - DS_BLE.Value) / PI_LENG.Value;
                    return Decimal.Round(val, 7);
                }
                catch(System.DivideByZeroException ex)
                {
                    return 0;
                }
            }
            set { _piSlop = value; }
        }
        public decimal? DES_FLOW { get; set; }
        public decimal? DES_VELO { get; set; }
        public decimal? US_BLE { get; set; }
        public decimal? DS_BLE { get; set; }
        public string CATCH_NUM { get; set; }
        public string CITY_ID { get; set; }
        public string CONS_ID { get; set; }
        public string CONS_TIT { get; set; }
        public string CONS_DEPT { get; set; }
        public string CONS_NAME { get; set; }
        public DateTime? CONS_DATE { get; set; }
        public DateTime? KEYIN_DATE { get; set; }
        public string NOTE { get; set; }
    }

    public class ConnectingPipe
    {
        public int targetId { set; get; }
        public string CPI_NUM { get; set; }
        public string IN_MHNUM { get; set; }
        public string IN_PINUM { get; set; }
        public decimal? CPI_WID { get; set; }
        public decimal? CPI_HEI { get; set; }
        public decimal? CPI_LENG { get; set; }
        public string PI_MAT { get; set; }
        public decimal? US_BLE { get; set; }
        public decimal? IN_TDIS { get; set; }
        public string CONS_ID { get; set; }
        public string CITY_ID { get; set; }
        public string NOTE { get; set; }
        public string NOTE2 { get; set; }
        public string NOTE3 { get; set; }
    }

    public class SetWells
    {
        public int targetId { set; get; }
        public string CP_NUM { get; set; }
        public string X { get; set; }
        public string Y { get; set; }
        public decimal? CP_BLE { get; set; }
        public decimal? CP_DEP { get; set; }
        public int? CP_LENG { get; set; }
        public int? CP_WID { get; set; }
        public string IN_CPNUM { get; set; }
        public string CONS_ID { get; set; }
        public string CITY_ID { get; set; }
        public string NOTE { get; set; }
    }

    public class RainwaterDitch
    {
        private decimal? _piSlop;

        public int targetId { set; get; }
        public string SPI_TYP { get; set; }
        public string SPI_NUM { get; set; }
        public string STR_X { get; set; }
        public string STR_Y { get; set; }
        public decimal? STR_LE { get; set; }
        public string END_X { get; set; }
        public string END_Y { get; set; }
        public decimal? END_LE { get; set; }
        public int? STR_DEP { get; set; }
        public int? END_DEP { get; set; }
        public int? STR_WID { get; set; }
        public int? END_WID { get; set; }
        public decimal? LENG { get; set; }
        public decimal? SLOP
        {
            get
            {
                if (STR_LE == null || END_LE == null || LENG == null) return null;
                if (LENG.Value == 0) return null;
                decimal val = (STR_LE.Value - END_LE.Value) / LENG.Value;
                return Decimal.Round(val, 7);
            }
            set { _piSlop = value; }
        }
        public string CATCH_NUM { get; set; }
        public DateTime? KEYIN_DATE { get; set; }
        public DateTime? CONS_DATE { get; set; }
        public string NOTE { get; set; }
    }

    public class PipeSilt
    {
        public int targetId { set; get; }
        public string P_NO { set; get; }
        public string CHART_NO { set; get; }
        public string TOWN_NAME { set; get; }
        public string TOWN_ID { set; get; }
        public string MSHED { set; get; }
        public string DIS_STMH { set; get; }
        public string DIS_NUM { set; get; }
        public string SEDI_DH { set; get; }
        public string FD_DEPTH { set; get; }
        public string INV_DATE { set; get; }
        public string INV_GRP { set; get; }
        public string DEV_OK { set; get; }
        public string DEV_DATE { set; get; }
        public string DEV_GRP { set; get; }
        public string PICTURE { set; get; }
        public string VIDEO { set; get; }
        public string CLASS { set; get; }
        public string MEMO { set; get; }
    }

    public class PipeCross
    {
        public int targetId { set; get; }
        public string P_NO { set; get; }
        public string CHART_NO { set; get; }
        public string TOWN_NAME { set; get; }
        public string TOWN_ID { set; get; }
        public string MSHED { set; get; }
        public string DIS_STMH { set; get; }
        public string DIS_NUM { set; get; }
        public string DIS_TOP { set; get; }
        public string CROSS_TP { set; get; }
        public string STATEM { set; get; }
        public string INV_DATE { set; get; }
        public string INV_GRP { set; get; }
        public string DEV_OK { set; get; }
        public string DEV_DATE { set; get; }
        public string DEV_GRP { set; get; }
        public string PICTURE { set; get; }
        public string VIDEO { set; get; }
        public string CLASS { set; get; }
        public string MEMO { set; get; }
    }

    public class PipeOther
    {
        public int targetId { set; get; }
        public string P_NO { set; get; }
        public string CHART_NO { set; get; }
        public string TOWN_NAME { set; get; }
        public string TOWN_ID { set; get; }
        public string MSHED { set; get; }
        public string DIS_STMH { set; get; }
        public string DIS_NUM { set; get; }
        public string STATEM { set; get; }
        public string INV_DATE { set; get; }
        public string INV_GRP { set; get; }
        public string DEV_OK { set; get; }
        public string DEV_DATE { set; get; }
        public string DEV_GRP { set; get; }
        public string PICTURE { set; get; }
        public string VIDEO { set; get; }
        public string CLASS { set; get; }
        public string MEMO { set; get; }
    }

    public class PipeCableAttach
    {
        public int targetId { set; get; }
        public string P_NO { set; get; }
        public string CHART_NO { set; get; }
        public string TOWN_NAME { set; get; }
        public string TOWN_ID { set; get; }
        public string MSHED { set; get; }
        public string DIS_STMH { set; get; }
        public string DIS_NUM { set; get; }
        public string STATEM { set; get; }
        public string INV_DATE { set; get; }
        public string INV_GRP { set; get; }
        public string DEV_OK { set; get; }
        public string DEV_DATE { set; get; }
        public string DEV_GRP { set; get; }
        public string PICTURE { set; get; }
        public string MEMO { set; get; }
    }

    public class PipeUnableWalk
    {
        public int targetId { set; get; }
        public string P_NO { set; get; }
        public string CHART_NO { set; get; }
        public string TOWN_NAME { set; get; }
        public string TOWN_ID { set; get; }
        public string MSHED { set; get; }
        public string DIS_STMH { set; get; }
        public string DIS_NUM { set; get; }
        public string STATEM { set; get; }
        public string INV_DATE { set; get; }
        public string INV_GRP { set; get; }
        public string DEV_OK { set; get; }
        public string DEV_DATE { set; get; }
        public string DEV_GRP { set; get; }
        public string PICTURE { set; get; }
        public string MEMO { set; get; }
    }
}
