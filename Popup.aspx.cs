using System;
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
public partial class Popup : WIBase {
    DataSet ds;
    protected void Page_Load(object sender, EventArgs e) {
       // this.Trace.IsEnabled = true;
        if (!this.IsPostBack) {
            this.MyInit();
        } else {
            this.Post();
        }
    }
    void Post() {
        string eventArg = this.Request["__EVENTARGUMENT"];
        switch (this.Request["__EVENTTARGET"]) {
            case "Attachment":
                break;

            default:
                break;
        }      
    }
    void MyInit() {
        string name = this.Request.QueryString["name"];
        string label = "" ;
        switch (name) {
            case "resources":
                label = "Minimum Personnel Required";
                break;
            case "PersonalProtectiveEquipment":
                label = "Personal Protective Equipment";
                break;
            case "tool":
                label = "Equipment/Tools Update ";
                break;
            case "WI":
                label = "Associated Work Instructions ";
                break;
                
        }
        this.MyTitle.InnerText = label;
        string s = "select * from " + name + " where disabled=0 or disabled is null order by case when name='Other' then 'zzz' else name end  asc";
        string wiid = this.Request.QueryString["wiid"];
        if (name == "WI")
            s = "select id, name=Facility+'-'+name from wi where id<>" + wiid +" order by name asc " ;

        ds = this.da.GetDataSet(s);
        s = "<table width=100%  ><tr style='vertical-align:top' >";
        int colSpan = 4;
        int rowCount=ds.Tables[0].Rows.Count ;
        int rowSpan = rowCount / colSpan;
        if (rowCount % colSpan != 0)
            rowSpan++;
        for (int i = 0; i < rowSpan; i++) {
            s += "<tr style='vertical-align:top'>";
            for (int j = 0; j < colSpan; j++) {
                DataRow dr ;
                if (i + j * rowSpan < ds.Tables[0].Rows.Count) {
                    dr = ds.Tables[0].Rows[i + j * rowSpan];
                    s += "<td><input type=checkbox id=" + dr["id"] + " name='" + dr["name"] + "' />";
                    s+=" <td>"+dr["name"];
                   // if ( (name == "resources"||name=="PersonalProtectiveEquipment") && dr["name"].ToString() == "Other")
                    if ( dr["name"].ToString() == "Other")
                        s += "<input type=text />";
                    if (name == "resources") {
                        s += "<td><input id=resources type=text style='width:30px' />";
                    }
                }

            }
        }
        s += "</table>";
        this.Holder.InnerHtml = s;
    }
    void MyInit_bak() {
        string name = this.Request.QueryString["name"];
        string label = "";
        switch (name) {
            case "resources":
                label = "Minimum Personnel Required";
                break;
            case "PersonalProtectiveEquipment":
                label = "Personal Protective Equipment";
                break;
            case "tool":
                label = "Equipment/Tools Update ";
                break;
            case "WI":
                label = "Associated Work Instructions ";
                break;

        }
        this.MyTitle.InnerText = label;
        string s = "select * from " + name;
        string wiid = this.Request.QueryString["wiid"];
        if (name == "WI")
            s = "select * from wi where id<>" + wiid;

        ds = this.da.GetDataSet(s);
        int i = 0;
        s = "<table width=100%  ><tr style='vertical-align:top' >";
        foreach (DataRow dr in ds.Tables[0].Rows) {
            if (i++ % 4 == 0)
                s += "<tr style='vertical-align:top'>";
            s += "<td><input type=checkbox id=" + dr["id"] + " name='" + dr["name"] + "' />" + dr["name"];
            if (name == "resources") {
                s += "&nbsp<input type=text style='width:30px' />";
            }
        }
        s += "</table>";
        this.Holder.InnerHtml = s;
    }

}