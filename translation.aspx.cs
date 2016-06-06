using System;
using System.Web.UI.HtmlControls;
using System.DirectoryServices.AccountManagement;
using System.Threading;
using System.Security;
using System.Security.Principal;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.IO;
using System.Web;
using System.Web.UI;

using System.Web.UI.WebControls;
using System.Diagnostics;
using System.DirectoryServices;
using Newtonsoft.Json;
using System.Configuration;
public partial class translation : WIBase {

    protected void Page_Load(object sender, EventArgs e) {
        if (!this.IsPostBack) {
            var items = new List<string> {
"1",
"2",
"3"
};
            this.DropDownList1.DataSource = items;
            this.DropDownList1.DataBind();

            var items2 = new List<string> {
"EN",
"PT",
"AR"
};
            this.DropDownList2.DataSource = items2;
            this.DropDownList2.DataBind();
            this.Render();
        }

    }
    void Render() {
        string index = this.DropDownList1.SelectedValue;
        string lang = this.DropDownList2.SelectedValue;
        string sql = "select * from test_"+ lang+index;

        ds = this.da.GetDataSet(sql);
        this.GridView1.DataSource = ds;
        this.GridView1.DataBind();

        foreach (GridViewRow row in this.GridView1.Rows) {
            //CheckBox ck = row.Cells[13].Controls[0] as CheckBox;

            foreach (TableCell cell in row.Cells)
                cell.Text = cell.Text.Replace("&nbsp;", "");
            this.DecodeColumn(row, 1);
            this.DecodeColumn(row, 2);
            this.DecodeColumn(row, 0);
            this.DecodeColumn(row, 0);
            this.DecodeColumn(row, 3);
            this.DecodeColumn(row, 3);
        }
    }
    void DecodeColumn(GridViewRow row, int  ii) {
        int index = -1;
        for (int i = 0; i < this.GridView1.HeaderRow.Cells.Count; i++)
            if (i== ii)
                index = i;
        if (index == -1)
            index = index;
        string s = HttpUtility.HtmlDecode(row.Cells[index].Text);
        row.Cells[index].Text = s;
    }
    void DecodeColumn(GridViewRow row, string name) {
        int index = -1;
        for (int i = 0; i < this.GridView1.HeaderRow.Cells.Count; i++)
            if (this.GridView1.HeaderRow.Cells[i].Text.ToLower() == name.ToLower())
                index = i;
        if (index == -1)
            index = index;
        string s = HttpUtility.HtmlDecode(row.Cells[index].Text);
        row.Cells[index].Text = s;
    }


    protected void Export_Click(object sender, EventArgs e) {
        this.ExportToExcel(this.Response);
    }
    public override void VerifyRenderingInServerForm(Control control) {
        /* Verifies that the control is rendered */
    }
    void ExportToExcel(HttpResponse response) {
        GridView gv = this.GridView1;
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
    protected void DropDownList1_SelectedIndexChanged(object sender, EventArgs e) {
        this.Render();
    }

    protected void DropDownList2_SelectedIndexChanged(object sender, EventArgs e) {
        this.Render();
    }
}


