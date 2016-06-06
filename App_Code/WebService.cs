using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Collections;
using System.Data;
using Microsoft.ApplicationBlocks.Data;
using mUtilities.Data;
using System.Web.Script.Serialization;
using System.Web.Script.Services;
using System.Configuration;

[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
// To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
[System.Web.Script.Services.ScriptService]
public class WebService : System.Web.Services.WebService {
    DataAccessor da = new DataAccessor(ConfigurationManager.AppSettings["ConnectionStringWI"]);
    DataSet ds;
    SortedList sl = new SortedList();
    private static readonly log4net.ILog _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
    public WebService() {
        //Uncomment the following line if using designed components 
        // InitializeComponent(); 
    }
    [WebMethod]
    public string RunSP(object items) {
        List<object> list = new JavaScriptSerializer().ConvertToType<List<object>>(items);
        string sp = "";
        SortedList sl = new SortedList();
        foreach (object o in list) {
            Dictionary<string, object> dict = (Dictionary<string, object>)o;
            foreach (string k in dict.Keys) {
                if (k == "sp")
                    sp = dict[k].ToString();
                else
                    sl.Add("@" + k, dict[k].ToString());
            }
        }
        ds = da.GetDataSet(sp, sl);
        if (ds.Tables.Count == 0)
            return "";
        foreach (DataRow dr in ds.Tables[0].Rows) {
            foreach (DataColumn dc in ds.Tables[0].Columns) {
                if (dr[dc] == System.DBNull.Value)
                    switch (dc.DataType.Name) {
                        case "Int32":
                            dr[dc] = 0;
                            break;
                        case "String":
                            dr[dc] = "";
                            break;
                        case "DateTime":
                            break;
                        default:
                            break;
                    }
            }
        }

        return ds.GetXml();
    }
    public bool IsNumeric(DataColumn col) {
        if (col == null)
            return false;
        // Make this const
        var numericTypes = new[] { typeof(Byte), typeof(Decimal), typeof(Double),
        typeof(Int16), typeof(Int32), typeof(Int64), typeof(SByte),
        typeof(Single), typeof(UInt16), typeof(UInt32), typeof(UInt64)};
        return numericTypes.Contains(col.DataType);
    }
    [WebMethod]
    public string RunSP2(string sp, string where) {
        where = HttpUtility.HtmlDecode(where);
        SortedList sl = new SortedList();
        sl.Add("@where", where);
        ds = da.GetDataSet(sp, sl);
        return ds.GetXml();
    }
    [WebMethod]
    public string ValidateWI(string xml, int JobDescriptionId, string userId, int wiid = 0) {
        xml = HttpUtility.UrlDecode(xml);
        xml = HttpUtility.HtmlDecode(xml);
        xml = "<wi><MappingXml >" + xml + "</MappingXml ></wi>";
        xml = xml.Replace("&", "&amp;");//
        sl.Add("@xml", xml);
        sl.Add("@JobDescriptionId", JobDescriptionId);
        sl.Add("@wiid", wiid);
        sl.Add("@userId", userId);
        ds = da.GetDataSet("usp_ValidateWI", sl);
        return ds.GetXml();
    }
    [WebMethod]
    public string ValidatePermission(string xml, int JobDescriptionId, string userId, int wiid = 0) {
        xml = HttpUtility.UrlDecode(xml);
        xml = HttpUtility.HtmlDecode(xml);
        xml = "<wi><MappingXml >" + xml + "</MappingXml ></wi>";
        xml = xml.Replace("&", "&amp;");//
        sl.Add("@xml", xml);
        sl.Add("@JobDescriptionId", JobDescriptionId);
        sl.Add("@wiid", wiid);
        sl.Add("@userId", userId);
        ds = da.GetDataSet("usp_ValidatePermission", sl);
        return ds.GetXml();
    }
    [WebMethod]
    public string GetHrHelp(string id) {
        da = new DataAccessor(ConfigurationManager.AppSettings["ConnectionStringKPIhr"]);
        ds = da.GetDataSet("select name  from tbl_help where id=" + id);
        return ds.GetXml();
    }
    [WebMethod]
    public string RunSql(string sql) {
        sql = HttpUtility.HtmlDecode(sql);
        ds = da.GetDataSet(sql);
        return ds.GetXml();
    }
    [WebMethod]
    public string GetDependence(string name) {
        sl.Add("@name", name);
        ds = da.GetDataSet("usp_getDependence", sl);
        return ds.GetXml();
    }
    [WebMethod]
    // [System.Web.Script.Services.ScriptMethod(ResponseFormat = System.Web.Script.Services.ResponseFormat.Xml)]
    public string GetQuery(string query) {
        sl.Add("@query", query);
        ds = da.GetDataSet("usp_GetQuery", sl);
        return ds.GetXml();
    }
    [WebMethod]
    public string GetWILookup(string name, string id) {
        sl.Add("@name", name);
        sl.Add("@id", id);
        ds = da.GetDataSet("usp_GetWILookup", sl);
        return ds.GetXml();
    }
    [WebMethod]
    public string GetRigs(string bu) {
        sl.Add("@start", bu);
        sl.Add("@end", bu);
        sl.Add("@division", bu);
        ds = da.GetDataSet("GetRigsIADC", sl);
        return ds.GetXml();
    }
    [WebMethod]
    public string GetSCHelp(string id, string page) {
        sl.Add("@id", id);
        sl.Add("@page", page);
        da = new DataAccessor(ConfigurationManager.AppSettings["ConnectionStringSc"]);
        ds = da.GetDataSet("usp_getHelp", sl);
        return ds.GetXml();
    }
    [WebMethod]
    // [System.Web.Script.Services.ScriptMethod(ResponseFormat = System.Web.Script.Services.ResponseFormat.Xml)]
    public string GetSubsystem() {
        da = new DataAccessor(ConfigurationManager.AppSettings["ConnectionStringRimDrillDev"]);
        ds = da.GetDataSet("select * from subsystem");
        return ds.GetXml();
    }
    [WebMethod]
    public string GetDowntimeCode(string start, string end, string rigname, string code) {
        sl.Add("@start", start);
        sl.Add("@end", end);
        sl.Add("@rigname", rigname);
        sl.Add("@code", code);
        ds = da.GetDataSet("GetDowntimeCode", sl);
        return ds.GetXml();
    }
    [WebMethod]
    public string GetDowntimeDetail(string start, string end, string rigname) {
        sl.Add("@start", start);
        sl.Add("@end", end);
        sl.Add("@rigname", rigname);
        ds = da.GetDataSet("GetDowntimeDetail", sl);
        return ds.GetXml();
    }
    [WebMethod]
    public string GetData(string select, string where) {
        sl.Add("@select", select);
        sl.Add("@where", where);
        ds = da.GetDataSet("sp_getkpitree", sl);
        return ds.GetXml();
    }
    [WebMethod]
    public void ResetCache() {
        lock (HttpContext.Current.Cache) {
            List<string> keys = new List<string>();
            IDictionaryEnumerator enumerator = HttpContext.Current.Cache.GetEnumerator();

            while (enumerator.MoveNext())
                keys.Add(enumerator.Key.ToString());

            for (int i = 0; i < keys.Count; i++)
                HttpContext.Current.Cache.Remove(keys[i]);
        }
        //this.Application["cache"] = null;
    }
}
