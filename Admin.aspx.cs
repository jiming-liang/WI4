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
public partial class Admin : WIBase {
    string id;
    protected void Page_Load(object sender, EventArgs e) {
        this.Function = "AdminFunction";
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
            case "Save":
                this.Save(eventArg);
                break;
            default:
                break;
        }
    }
    void Save(string sql) {
        this.da.GetDataSet(sql);
        this.Response.Redirect("WiAdmin.aspx");
    }
    void MyInit() {
        string[,] arr = new string[,] { {"Rig", "Rig"}, { "RigDesign", "RigDesign" }, { "RigType", "RigType" }, { "Minimum Personnel", "Resources" }, { "Personal Protective Equipment", "PersonalProtectiveEquipment" }, { "Equipment/Tools", "Tool" }, { "Job Title", "JobDescription" }, { "Job Category", "WICategory" }, { "Equipment Make", "EquipmentMake" }, { "Equipment Type", "EquipmentType" } };
       // string[] arr = new string[] { "", "RigDesign", "RigType", "JobDescription", "WICategory", "EquipmentMake", "EquipmentType" };
        for (int i = 0; i<= arr.GetUpperBound(0); i++)
                this.Entity.Items.Add(new ListItem(arr[i, 0], arr[i, 1]));

        ds = da.GetDataSet("select Facility,  Name , Status, id from wi");
        this.GridView1.DataSource = ds;
        this.GridView1.DataBind();
        this.GridView1.HeaderRow.BackColor = this.deepblue;
        this.FormatGrid();
    }
    void FormatGrid() {
        foreach (TableRow tr in this.GridView1.Rows) {
            string name =  Guid.NewGuid().ToString();
           // name = name.Substring(0, 5);
            string s0="<input id=fd type=radio name="+name+" value='{0}' {1} />{0}";
            string s = "";
            TableCell cell=tr.Cells[2];
            string status = cell.Text;
            for (int i = 0; i < this.StatusList.Length; i++) {
                s += string.Format(s0, this.StatusList[i], this.StatusList[i] == status ? "checked='checked'" : "");
            }
            string id = tr.Cells[3].Text;
            s += @"&nbsp<input type=button onclick=""on_Save('"+name+"', " + id + @")"" value=Update />";
            cell.Text = s;
          //  tr.Cells.RemoveAt(2);
        }
     //   this.GridView1.HeaderRow.Cells.RemoveAt(2);
    }
    

}