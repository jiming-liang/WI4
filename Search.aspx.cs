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
public partial class Search : WIBase {
    string id;
    bool IsLocal = false;
    protected void Page_Load(object sender, EventArgs e) {
       // this.Trace.IsEnabled = true;
        this.id = this.Request.QueryString["id"];
        if (this.Request.QueryString["IsLocal"] != null) {
            this.IsLocal = true;
            this.MyTitle.InnerText = "Work Instruction Search";
        }
        if (!this.IsPostBack) {
            this.MyInit();
        } else {
            this.Post();
        }
    }
    void Post() {
        string s = this.Request["__EVENTARGUMENT"];
        switch (this.Request["__EVENTTARGET"]) {
            case "Export":
                this.GridViewToExcel(this.Response, s);
                break;
            case "Search":
                this.Submit_Click(null, null);
                break;
            case "Paging":
                this.SearchWI(int.Parse(s));
                break;
            default:
                break;
        }
    }
    void SearchWI(  int index ) {
        SortedList sl = new SortedList();
        sl.Add("@where",  this.GetWhere());
        // sl.Add("@IsLocal", this.IsLocal);
        sl.Add("@userId", this.UserId);
        sl.Add("@Status", this.Status.SelectedValue.Replace(" Approval", ""));
        sl.Add("@index", index);
        if (this.IsLocal)
            ds = da.GetDataSet("usp_searchWIlocal", sl);
        else 
            ds = da.GetDataSet("usp_searchWI", sl);
        this.GridView1.DataSource = ds.Tables[0 ];
        this.GridView1.DataBind();
        this.GridView1.HeaderRow.BackColor = this.deepblue;
        this.FormatGridView();
        int total = int.Parse(ds.Tables[1].Rows[0][0].ToString());
        if (total == 1)
            return;
        for (int i = 1; i <= total; i++) {
            LinkButton l = new LinkButton();
            l.Text = i.ToString();
            l.Attributes.Add("onclick", "on_Paging(this)");
            l.Attributes.Add("href", "#");
            if (i == index)
                this.Paging.Controls.Add(new LiteralControl(index.ToString()));
            else
                this.Paging.Controls.Add(l);
            this.Paging.Controls.Add(new LiteralControl(" "));
        }
    }
    void FormatGridView() {
        foreach (GridViewRow row in this.GridView1.Rows) {
            TableCell cell = this.GetCell(row, "id");
            string id = cell.Text;
            string para = "id=" + id;
            string wiType = "WIT";
            cell = this.GetCell(row, "Work Instruction Template No.");
            if (this.IsLocal) {
                wiType = "WI";
                string rigId = this.GetCellText(row, "rigId");
                if (this.GetCellText(row, "Work Instruction No.").Contains("WIS-")) {
                    wiType = "WIS";
                }
                cell = this.GetCell(row, "Work Instruction No.");
            }
            para += "&WiType=" + wiType;
            cell.Text = "<a href=Create.aspx?" + para + " >" + cell.Text + "</a>";

            cell = this.GetCell(row, "Updated on");

            cell.Text = "<a href=# onclick=on_HistoryLog(" + id + ",'" + wiType + "') >" + cell.Text + "</a>";

            cell = this.GetCell(row, "flag");
            if (cell == null)
                return;
            string flag = cell.Text;
            string s = "";
            if (flag != "Inactive")
                flag = "Active";
            s = "<input type=button value=" + flag + " id=" + id + " onclick='on_Flag(this, " + id + ")' />";
            cell.Text = s;
        }
    }
    void FormatGridView011416() {
        foreach (GridViewRow row in this.GridView1.Rows) {
            TableCell cell = this.GetCell(row, "id");
            string id = cell.Text;
            string para = "id=" + id;
            bool isWis = false;
            string wiType = "WIT";
            cell = this.GetCell(row, "Work Instruction Template No.");
            if (this.IsLocal) {
                wiType = "WI";
                para = "localId=" + id;
                string rigId = this.GetCellText(row, "rigId");
                if (this.GetCellText(row, "Work Instruction No.").Contains("WIS-")) {
                    wiType = "WIS";
                    para = "WisId=" + id;
                }

                //else if (this.GetCellText(row, "Status") == "New") {
                //    para += "&rigId=" + rigId;
                //} else {
                //    para = "localId=" + id;
                //}
                cell = this.GetCell(row, "Work Instruction No.");
            }
            para += "&WiType=" + wiType;
            cell.Text = "<a href=Create.aspx?" + para + " >" + cell.Text + "</a>";

            cell = this.GetCell(row, "Updated on");
            //if (this.IsLocal) {
            //    string rigId = this.GetCellText(row, "rigId");
            //    if (isWis)
            //        para = "wisId=" + id;
            //    if (this.GetCellText(row, "Status") == "New") {
            //        para += "&rigId=" + rigId;
            //        isNew = true;
            //    } else {
            //        para = "localId=" + id;
            //    }
            //}
            cell.Text = "<a href=# onclick=on_HistoryLog("+id+",'"+wiType+"') >" + cell.Text + "</a>";

            cell = this.GetCell(row, "flag");
            if (cell == null)
                return;
            string flag = cell.Text;
            string s = "";
            if (flag != "Inactive")
                flag = "Active";
            s = "<input type=button value=" + flag + " id=" + id + " onclick='on_Flag(this, " + id +")' />";
            cell.Text = s;            
        }
    }
    TableCell  GetCell(GridViewRow row, string name) {
        int index=0;
        foreach (TableCell cell in this.GridView1.HeaderRow.Cells)  {
            if (cell.Text.Trim().ToLower() == name.ToLower())
                return row.Cells[index]; 
            index++;    
        }
        return null;
        
    }
    string  GetCellText(GridViewRow row, string name) {
        return this.GetCell(row, name).Text;
    }
    void MyInit() {
        string sql = "select * from rigType order by name ";
        if (this.IsLocal)
            sql = "select * from rig order by name ";
        sql += "select * from WICategory order by name select * from EquipmentType order by name select * from userRole where userId= '" + this.UserId + "' and role in ('local', 'user', 'oim', 'rig manager','operations manager')";
        sql += " select * from Resources order by Name ";
        sql += " select '' 'name', '' 'id' union select 'Rig-Specific Work Instruction','WIS' union select 'Work Instruction with Global Content','WI'";
        ds = this.da.GetDataSet(sql);
        if (!this.IsLocal) {
            this.RigType.DataSource = ds.Tables[0];
            this.RigType.DataTextField = "name";
            this.RigType.DataValueField = "id";
            this.RigType.DataBind();
            this.RigType.Items.Insert(0, "ALL_RIGS");
            this.RigType.Items.Insert(0, "");
        } else {
            this.RigListBox.DataSource = ds.Tables[0];
            this.RigListBox.DataTextField = "name";
            this.RigListBox.DataValueField = "id";
            this.RigListBox.DataBind();
            foreach (DataRow dr in ds.Tables[3].Rows) {
                string rigId = dr["rigId"].ToString();
                this.RigListBox.Items.FindByValue(rigId).Selected = true;
            }
        }

        this.WICategory.DataSource = ds.Tables[1];
        this.WICategory.DataTextField = "name";
        this.WICategory.DataValueField = "id";
        this.WICategory.DataBind();
        this.WICategory.Items.Insert(0, "");

        this.EquipmentType.DataSource = ds.Tables[2];
        this.EquipmentType.DataTextField = "name";
        this.EquipmentType.DataValueField = "id";
        this.EquipmentType.DataBind();
        this.EquipmentType.Items.Insert(0, "");

        this.Position.DataSource = ds.Tables[4];
        this.Position.DataTextField = "name";
        this.Position.DataValueField = "id";
        this.Position.DataBind();
        this.Position.Items.Insert(0, "");

        this.Criticality.DataSource = new string[] { "", "Low", "Medium", "High" };
        this.Criticality.DataBind();
        string[] arr = new string[] { "", "In Progress", "Under Review", "In Testing", "Approved" };
        if (this.IsLocal)
            arr = new string[] { "", "New", "Draft", "Pending Approval", "Approved" };
        this.Status.DataSource = arr;
        this.Status.DataBind();

        this.WorkInstructionType.DataSource = ds.Tables[5];
        this.WorkInstructionType.DataTextField = "Name";
        this.WorkInstructionType.DataValueField = "id";
        this.WorkInstructionType.DataBind();
    }
    protected void RigType_SelectedIndexChanged(object sender, EventArgs e) {
        this.Rig.Items.Clear();
        this.UpdateDropDownList("RigType", "RigDesign");
    }
    void UpdateDropDownList(string id1, string id2) {
        DropDownList list1 = (DropDownList)this.form1.FindControl(id1);
        DropDownList list2 = (DropDownList)this.form1.FindControl(id2);
        list2.Items.Clear();
        if (list1.SelectedValue == "" || list1.SelectedValue=="ALL_RIGS")
            return;
        string sql = "select * from " + id2 + " where " + id1 + "Id=" + list1.SelectedValue + " order by  ";
        if (id2 == "Rig")
            sql += " len(name) , ";
        sql += " name";
        ds = da.GetDataSet(sql);
        list2.DataSource = ds;
        list2.DataTextField = "name";
        list2.DataValueField = "id";
        list2.DataBind();
        list2.Items.Insert(0, "");
    }
    protected void RigDesign_SelectedIndexChanged(object sender, EventArgs e) {
        this.UpdateDropDownList("RigDesign", "Rig");
    }
    protected void WICategory_SelectedIndexChanged(object sender, EventArgs e) {
        this.UpdateDropDownList("WICategory", "JobDescription");
    }
    protected void Clear_Click(object sender, EventArgs e) {
        this.Response.Redirect(this.Request.RawUrl.ToString());
    }
    string GetWhere(){
        string where = "  where 1=1";
        foreach (Control c in Form.Controls) {
            if (c is DropDownList) {
                DropDownList list = (DropDownList)c;
                if (list.SelectedValue != "") {
                    string id = "a." + list.ID;
                    if (list.ID == "WICategory" || list.ID == "EquipmentType")
                        id = list.ID;
                    if (list.ID == "Criticality")
                        where += " and " + id + " in ('" + list.SelectedValue + "')";
                    else if (list.ID == "WorkInstructionType")
                        where += " and WiType  in ('" + list.SelectedValue + "')";
                    else if (list.ID == "Status")
                        where += " ";
                    else if (list.SelectedValue == "ALL_RIGS")
                        where += " and a.WiNo like '%ALL_RIGS%'";
                    else
                        where += " and " + id + "Id in (" + list.SelectedValue + ")";
                }
            }
        }
        where = where.Replace("a.PositionId", "m.ResourcesId");
        where += this.Request.Form["h1"];
        string s = this.SearchText.Text.Trim();
        if (s != "") {
            string like = " like '%" + s + "%'";
            where += " and (j.name " + like + " or a.WiNo " + like + ")";
        }
        return where;
    }
    protected void Submit_Click(object sender, EventArgs e) {
        this.SearchWI( 1);        
    }
    protected void Export_Click(object sender, EventArgs e) {
        this.ExportToExcel(this.Response, null);
    }
    public override void VerifyRenderingInServerForm(Control control) {
        /* Verifies that the control is rendered */
    }
    void ExportToExcel(HttpResponse response, DataSet ds) {
        GridView gv = this.GridView1;
        this.RemoveColumns(gv);
        response.Clear();

        response.Buffer = true;
        response.AddHeader("Content-Disposition", "attachment; filename=WIMS.xls");
        response.ContentType = "application/vnd.ms-excel";
        System.IO.StringWriter writer = new System.IO.StringWriter();
        System.Web.UI.HtmlTextWriter html = new HtmlTextWriter(writer);

        gv.RenderControl(html);
        string s;//=writer.ToString();
        s = HttpUtility.HtmlDecode(this.Request["h1"]);
        Response.Output.Write(s);
        Response.Flush();
        Response.End();
    }
    void RemoveColumns(GridView gv) {
        string [] arr= new string[]{"id", "la"};
        foreach (string name in arr) {
            foreach (DataControlField c in gv.Columns) {
                string s = c.GetType().Name;
            }
        }
    }
    void GridViewToExcel(HttpResponse response, string s) {
        response.Clear();
        response.Buffer = true;
        response.AddHeader("Content-Disposition", "attachment; filename=WIMS.xls");
        response.ContentType = "application/vnd.ms-excel";

        System.IO.StringWriter writer = new System.IO.StringWriter();
        System.Web.UI.HtmlTextWriter html = new HtmlTextWriter(writer);
        html.Write(s);
        response.Write(writer);
        response.End();
    }
    protected void EquipmentType_SelectedIndexChanged(object sender, EventArgs e) {
        this.UpdateDropDownList("EquipmentType", "EquipmentMake");
    }
}