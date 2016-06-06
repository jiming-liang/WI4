using System;
using System.Text.RegularExpressions;
using System.Web.UI.HtmlControls;
//using System.Drawing;
using System.Xml;
using System.IO;
using System.Collections;
using System.Xml.Linq;
using System.Collections.Generic;
using System.Configuration;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.ApplicationBlocks.Data;
using System.Linq;
using System.Text;
using System.DirectoryServices.AccountManagement;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using Tools;

using iTextSharp.tool.xml.html;
public partial class CopyWI : WIBase {
    string id;
    string LocalId;
    string RigId;
    bool IsLocal = false;
    string[] Headers = { "Global Step #", "Local Step #", "Warning / Caution / Note / Job Step ", "Photo", "Barriers (Initial)" };

    string BaseUrl;
    string FacilityList;
    float TotalWidth = 800f;
   
    string Action = "";
    public string Dict;
    public string MappingXml {
        get {
            if (this.Session["MappingXml"] == null)
                return "";
            else
                return this.Session["MappingXml"].ToString();
        }
        set {
            this.Session["MappingXml"] = value;
            this.MappingXmlHidden.Value = value;
        }
    }
    public string Rig;
    bool IsNoteBlank = true;
    protected string LessThanEncode = "!23!@#1";
    protected string AmpEncode = "q2sfd@";
    public string ExportPath ;

    protected void Page_Load(object sender, EventArgs e) {
        this.BaseUrl = ConfigurationManager.AppSettings["BaseUrl"];
        this.ExportPath = ConfigurationManager.AppSettings["ExportPath"];
        this.CopyPDF();
    }
    void CopyPDF() {
        SortedList sl = new SortedList();
        sl.Add("@rig", "");
        sl.Add("@userId", this.UserId);
        DataSet ds0 = this.da.GetDataSet("usp_getApprovedWI", sl);
        string userId, pwd;
        userId = ConfigurationManager.AppSettings["userId"];
        pwd = ConfigurationManager.AppSettings["pwd"];
       //  using (new Impersonator(userId, "ensco", pwd)) {
        foreach (DataRow dr in ds0.Tables[0].Rows) {
            this.id = dr["id"].ToString();
            string cat = dr[1].ToString().Split('/')[0];
            this.Rig = dr["rig"].ToString();
            string name = "WI-" + dr["rig"] + "-" + dr["name"] + ".pdf";
            string path = this.ExportPath + this.Rig + @"\" + cat + @"\" + name;
            if (File.Exists(path)) {
                FileInfo fi = new FileInfo(path);

                if (DateTime.Parse(dr["RevisionDate"].ToString()) < fi.LastWriteTime)
                    continue;
            }
            sl = new SortedList();
            sl.Add("@id", this.id);
            sl.Add("@wiid", 0);
            sl.Add("@rigId", dr["rigId"].ToString());
            sl.Add("@action", this.Action);

            ds = this.da.GetDataSet("usp_getWIlocal", sl);
            this.Export(cat);
        }
       // this.DeletePDF(ds0);
     //   }
    }
    void DeleteRecursive(DataSet ds  , DirectoryInfo di) {
        foreach (FileInfo fi in di.GetFiles()) {
            bool found = false;
            foreach (DataRow dr in ds.Tables[0].Rows) {
                string name = "WI-" + dr["rig"] + "-" + dr["name"] + ".pdf";
                if (fi.Name.ToLower() == name.ToLower()) {
                    found = true;
                }
            }
            if (!found)
                fi.Delete();
        }
        foreach(DirectoryInfo di2 in di.GetDirectories()){
            this.DeleteRecursive(ds, di2);
        }        
    }
    void DeletePDF(DataSet ds) {
       // string [] files= Directory.GetFiles(this.ExportPath, "*", SearchOption.AllDirectories);
        DirectoryInfo di = new DirectoryInfo(this.ExportPath);
        this.DeleteRecursive(ds, di);
    }
    void Export(string cat) {
        PreviewPDF preview = new PreviewPDF();
        preview.ds = ds;
        preview.Rig = this.Rig;
        preview.ExportPath = this.ExportPath;
        preview.Headers = this.Headers;
        preview.BaseUrl = this.BaseUrl;
        preview.LessThanEncode = this.LessThanEncode;
        preview.AmpEncode = this.AmpEncode;
        DataRow dr = this.ds.Tables[0].Rows[0];
        preview.Prefix = "WI-";
        string wiNo = "WI-" + dr["rig"] + "-" + dr["name"] ; // this.ds.Tables[0].Rows[0]["WINo"].ToString();
        if (this.Rig != null) {
            wiNo = wiNo.Replace(wiNo.Split('-')[1], this.Rig);
        }
        preview.WiNo = wiNo;
        preview.Copy(cat, this.UserName);
    }

}
