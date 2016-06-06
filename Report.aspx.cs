using System;
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
public partial class Report : WIBase {
    string id;
    protected void Page_Load(object sender, EventArgs e) {
        this.id = this.Request.QueryString["id"];
        if (!this.IsPostBack) {
            this.MyInit();
        } else {
            this.Post();
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

    void MyInit() {
        ds = this.da.GetDataSet("usp_report");
        this.GridView1.DataSource = ds.Tables[0];
        this.GridView1.DataBind();
        this.GridView2.DataSource = ds.Tables[1];
        this.GridView2.DataBind();

        this.GridView1.HeaderRow.BackColor = this.deepblue;
        this.GridView2.HeaderRow.BackColor = this.deepblue;
    }
}