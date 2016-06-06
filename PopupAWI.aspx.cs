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
public partial class PopupAWI : WIBase {
    DataSet ds;
    protected void Page_Load(object sender, EventArgs e) {
       // this.Trace.IsEnabled = true;
        if (!this.IsPostBack) {
            this.MyInit();
        } else {

        }
    }
    void MyInit() {
        ds = this.da.GetDataSet(" select * from WICategory select * from EquipmentType");
        this.WICategory.DataSource = ds.Tables[0];
        this.WICategory.DataTextField = "name";
        this.WICategory.DataValueField = "id";
        this.WICategory.DataBind();
        this.WICategory.Items.Insert(0, "");

        this.EquipmentType.DataSource = ds.Tables[1];
        this.EquipmentType.DataTextField = "name";
        this.EquipmentType.DataValueField = "id";
        this.EquipmentType.DataBind();
        this.EquipmentType.Items.Insert(0, "");

    }
    protected void WICategory_SelectedIndexChanged(object sender, EventArgs e) {
        this.UpdateDropDownList("WICategory", "JobDescription");
    }
    protected void EquipmentType_SelectedIndexChanged(object sender, EventArgs e) {
        this.UpdateDropDownList("EquipmentType", "EquipmentMake");
    }
    void UpdateDropDownList(string id1, string id2) {
        DropDownList list1 = (DropDownList)this.form1.FindControl(id1);
        DropDownList list2 = (DropDownList)this.form1.FindControl(id2);
        list2.Items.Clear();
        if (list1.SelectedValue == "")
            return;
        ds = da.GetDataSet("select * from " + id2 + " where " + id1 + "Id=" + list1.SelectedValue);
        list2.DataSource = ds;
        list2.DataTextField = "name";
        list2.DataValueField = "id";
        list2.DataBind();
        list2.Items.Insert(0, "");
    }
    void SearchAWI(string where) {

    }
    void FormatGridView() {

    }
    protected void Submit_Click(object sender, EventArgs e) {

    }
    protected void Search_Click(object sender, EventArgs e) {
        string where = "  where 1=1";
        foreach (Control c in Form.Controls) {
            if (c is DropDownList) {
                DropDownList list = (DropDownList)c;
                if (list.SelectedValue != "")
                    where += " and " + list.ID + "Id in (" + list.SelectedValue + ")";
            }
        }
        if (this.SearchText.Text.Trim()!="")
            where += " and j.name like '%" + this.SearchText.Text.Trim() + "%'";
        this.SearchAWI(where);
    }
}