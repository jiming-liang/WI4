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
public partial class WiByRig : WIBase {
    string id;
    public bool IsLocal = false;
    protected void Page_Load(object sender, EventArgs e) {
        this.id = this.Request.QueryString["id"];
        if (this.Request.QueryString["IsLocal"] != null) {
            this.IsLocal = true;
           // this.MyTitle.InnerText = "Work Instruction Search";
        }
        if (!this.IsPostBack) {
            this.GetData();
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

    void GetData() {
        SortedList sl = new SortedList();
        sl.Add("@rig", this.Rig.Text.Trim());
        sl.Add("@IsLocal", this.IsLocal);
       // sl.Add("@userId", this.UserId);
        ds = this.da.GetDataSet("usp_getWiByRig", sl);

        this.GridView1.DataSource = null;
        this.GridView1.DataSource = ds;
        this.GridView1.DataBind();
        this.GridView1.HeaderRow.BackColor = this.deepblue;
        this.FormatGrid();                                                                                                                                                                        
    }
    void FormatGrid() {
        if (ds.Tables[0].Rows.Count == 0)
            return;
        string curRig = "";
        int i = 1;
        GridViewRow curRow=null;
        foreach (GridViewRow row in this.GridView1.Rows) {
            string id = row.Cells[3].Text;
            string rig = row.Cells[0].Text;
            
            string s;
            s = "<input type=button id="+id+" rig='"+rig+"'  value=Preview />";
            s += "<input type=button id=" + id + " rig='" + rig + "'  value=Edit />";
            row.Cells[3].Text = s;

            if (rig != curRig) {
                if (curRow != null)
                    curRow.Cells[0].RowSpan = i;
                i = 1;
                curRow = row;
                curRig = rig;
            } else {
                i++;
                row.Cells.RemoveAt(0);
            }
        }
        curRow.Cells[0].RowSpan = i;
    }
    protected void Button1_Click(object sender, EventArgs e) {
        this.GetData();
    }
}