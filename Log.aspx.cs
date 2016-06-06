using System;
using System.Xml;
using System.Xml.Linq;
using System.IO;
using System.Collections;

using System.Collections.Generic;
using System.Configuration;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.ApplicationBlocks.Data;
using System.Data;
using System.Linq;
using System.Text;
using System.Data.SqlClient;

using System.Text.RegularExpressions;
public partial class Log : WIBase {
    string id;
    protected void Page_Load(object sender, EventArgs e) {
        this.id = this.Request.QueryString["id"];
        if (!this.IsPostBack) {
            this.MyInit();
        } 
    }
    void Post() {
        string eventArg = this.Request["__EVENTARGUMENT"];
        switch (this.Request["__EVENTTARGET"]) {
            case "Submit":
                break;
            default:
                break;
        }
    }
    string GetString(int i, string s) {
       char[] chars=s.ToCharArray();
       string r1 = "", r2 = "";
        int len=chars.Length;
        int j=0, jj = len-1;
        while (i>0 && j < len && chars[j] >= 'a' && chars[j] <= 'z')
            r1 += chars[j++];

        while (i< len-1 && jj >-1 && chars[jj] >= 'a' && chars[jj] <= 'z') 
            r2 += chars[jj--];
        if (r1 != "" && r2 != "")
            return r1 + "_" + r2;
        else
            return "";
        return ( i==0? "":s.Substring(0, j)) + "_" + (i==len-1? "": s.Substring(jj, s.Length - jj));
    }

    void MyInit() {
        ds = this.da.GetDataSet("select * from tbl_log order by d desc  select * from tbl_error order by dt desc ");
        this.GridView1.DataSource = ds.Tables[0];
        this.GridView1.DataBind();

        this.GridView2.DataSource = ds.Tables[1];
        this.GridView2.DataBind();
    }
}