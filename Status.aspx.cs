using System;
using System.Text.RegularExpressions;
using System.IO;
using System.Collections;
using System.Web.UI.DataVisualization.Charting;
using System.Collections.Generic;
using System.Configuration;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Drawing;
using Microsoft.ApplicationBlocks.Data;
using System.Data;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
public partial class Status : WIBase {
    string id;
    bool IsLocal = false;

    protected   System.Web.UI.DataVisualization.Charting.Chart Chart1;
    protected Font seriesFont = new Font("Arial", 7, FontStyle.Regular);
    protected Font xyFont = new Font("Arial", 8, FontStyle.Regular);
    protected Font titleFont = new Font("Arial", 12, FontStyle.Bold);
    protected Font legendFont = new Font("Arial", 7, FontStyle.Bold);
    protected string where = "where 1=1 ";
    protected int markerSize = 10;
    protected int borderWidth = 4;
    protected int screenWdith = 1600;
    Chart ChartExport;
    protected void Page_Load(object sender, EventArgs e) {
       // this.Trace.IsEnabled = true;
        this.id = this.Request.QueryString["id"];
        if (this.Request.QueryString["IsLocal"] != null) {
            this.IsLocal = true;
        }
        if (!this.IsPostBack) {
            this.MyInit();
        } else {
            //this.Post();
        }
    }
     Chart  AddChart(DataTable dt) {
        if (dt.Rows.Count == 0)
            return null;
        Chart c = new Chart();
        this.ChartInit(c);
        this.DivPieChart.Controls.Add(c);

        c.Series.Add("");
        DataRow dr = dt.Rows[dt.Rows.Count - 1]; 
        foreach (Series s in c.Series) {
            s.IsValueShownAsLabel = true;
            s.ChartType = SeriesChartType.Pie;
            double f = double.Parse(dr[2].ToString());
            f/=100;
            string[]  arr= {"New", "Draft", "Pending Approval From OIM", "Pending Approval From Rig Manager", "Pending Approval From Operations Manager", "Approved" };
            if (this.ReportType.SelectedValue=="WIT Status")
                arr =new string[] { "Draft", "Pending Approval", "Approved" };

            foreach (string status in arr) {
                
                int  n = int.Parse(dr["% "+status].ToString());
                s.Points.Add(n / 100.0);
                s.Points[s.Points.Count - 1].LabelFormat = "#.#%";
                s.Points[s.Points.Count - 1].LabelForeColor = Color.White;
                s.Points[s.Points.Count - 1].Font = new Font("Arial", 14, FontStyle.Bold);
                s.Points[s.Points.Count - 1].LegendText = status + " "+dr["# "+status].ToString();
                Color color = Color.Red;
                switch (status) {
                    case "Approved":
                        color = Color.Green;
                        break;
                    case "New":
                        color = Color.Red;
                        break;
                    case "Draft":
                        color = Color.Blue;
                        break;
                    case "Pending Approval From OIM":
                         color = (Color )ColorTranslator.FromHtml("#ffa07a");
                        break;
                    case "Pending Approval From Rig Manager":
                        color = ColorTranslator.FromHtml("#fa8072");
                        break;
                    case "Pending Approval From Operations Manager":
                        color = ColorTranslator.FromHtml("#e9967a");
                        break;
                    default:
                        break;
                }
                s.Points[s.Points.Count - 1].Color = color;
            }
        }

       // c.Titles[0].Text = "Shipping Priority";
        this.SizeChart(c, 50);
        return c;
    }
 
    protected void SizeChart(Chart c, int w) {
        //c.Width = Unit.Pixel(this.screenWdith * w * 95 / 10000 );
        c.Width = Unit.Pixel(this.screenWdith * w / 100 - 20);
    }
    protected void ChartInit(Chart c) {
        Legend l = new Legend("");

        ChartArea ca = new ChartArea();
        c.ChartAreas.Add(ca);
        c.Titles.Add(new Title());
        c.Titles[0].Font = this.titleFont;
        c.ChartAreas[0].AxisY.MajorGrid.Enabled = true;
        c.ChartAreas[0].AxisY.MajorGrid.LineDashStyle = ChartDashStyle.Dot;
        c.ChartAreas[0].AxisX.MajorGrid.LineDashStyle = ChartDashStyle.Dot;
        c.ChartAreas[0].AxisY2.MajorGrid.Enabled = false;
        c.Legends.Add(l);
        l.LegendItemOrder = LegendItemOrder.ReversedSeriesOrder;
        c.Legends[0].Docking = Docking.Bottom;
        c.Legends[0].Alignment = StringAlignment.Center;
        c.Legends[0].Font = this.legendFont;

        c.ChartAreas[0].AxisX.LabelStyle.Font = this.xyFont;
        c.ChartAreas[0].AxisY.LabelStyle.Font = this.xyFont;

        c.ChartAreas[0].AxisX.Minimum = 0;
        c.ChartAreas[0].AxisX.Interval = 1;

        c.Width = Unit.Pixel(1100);
        c.Height = Unit.Pixel(500);
        c.SuppressExceptions = true;
        c.ChartAreas[0].AxisY.LabelStyle.Format = "{0:#,##0.##}";
        c.ChartAreas[0].AxisY2.LabelStyle.Format = "{0:#,##0.##}";

        //c.PostPaint += new EventHandler<ChartPaintEventArgs>(c_PostPaint);
        //this.ScInit();
    }
    void Post() {  
    }
    void SearchWI( string where ) {
        SortedList sl = new SortedList();
        sl.Add("@where",  where);
        sl.Add("@reportType", this.ReportType.SelectedValue);
            ds = da.GetDataSet("[usp_statusReport]", sl);

        this.GridView1.DataSource = ds;
        this.GridView1.DataBind();
        this.GridView1.HeaderRow.BackColor = this.deepblue;

        if (this.ReportType.SelectedValue!="WI Printed")
           this.ChartExport= this.AddChart(ds.Tables[0]);
    }
    void FormatGridView() {
        foreach (GridViewRow row in this.GridView1.Rows) {
        }
    }

    void MyInit() {
        DateTime dt = new DateTime(2015, 1, 1);
        this.FromDate.Text = dt.ToString("d-MMM-yy");
        //  this.ToDate.Text = String.Format("{0:MM/dd/yyyy}", DateTime.Now);
        this.ToDate.Text = DateTime.Now.ToString("d-MMM-yy");
        string sql="select * from rigType order by name select * from WICategory order by name select * from EquipmentType order by name";
        sql += " select '' 'name', '' 'id' union select 'Rig-Specific Work Instruction','WIS' union select 'Work Instruction with Global Content','WI'";
        ds = this.da.GetDataSet(sql);
        this.RigType.DataSource = ds.Tables[0] ;
        this.RigType.DataTextField = "name";
        this.RigType.DataValueField = "id";
        this.RigType.DataBind();
        this.RigType.Items.Insert(0, "");

        this.WICategory.DataSource = ds.Tables[1];
        this.WICategory.DataTextField = "name";
        this.WICategory.DataValueField = "id";
        this.WICategory.DataBind();
        this.WICategory.Items.Insert(0, "");

        string[] arr = { "WIT Status", "WI Status", "WI Printed" };
        this.ReportType.DataSource = arr;
        this.ReportType.DataBind();


        this.WiType.DataSource = ds.Tables[3];
        this.WiType.DataTextField = "Name";
        this.WiType.DataValueField = "id";
        this.WiType.DataBind();
    }
    protected void RigType_SelectedIndexChanged(object sender, EventArgs e) {
        this.Rig.Items.Clear();
        this.UpdateDropDownList("RigType", "RigDesign");
    }
    void UpdateDropDownList(string id1, string id2) {
        DropDownList list1 = (DropDownList)this.form1.FindControl(id1);
        DropDownList list2 = (DropDownList)this.form1.FindControl(id2);
        list2.Items.Clear();
        if (list1.SelectedValue == "")
            return;
        ds = da.GetDataSet("select * from "+id2+" where "+id1+"Id=" + list1.SelectedValue);
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
    protected void Submit_Click(object sender, EventArgs e) {
        string where = "  where 1=1";
        //if (this.ReportType.SelectedValue=="WI printed")

        foreach (Control c in Form.Controls) {
            if (c is DropDownList) {
                DropDownList list = (DropDownList)c;
                if (list.SelectedValue != "" && list.ID != "ReportType") {
                    if (!(this.ReportType.SelectedValue != "WI Printed" && (list.ID == "WICategory" || list.ID == "JobDescription")))
                        if (list.ID == "WiType")
                            if (this.ReportType.SelectedValue == "WIT Status")
                                where += "";
                            else
                                where += " and " + list.ID + " in ('" + list.SelectedValue + "')";
                        else 
                            where += " and " + list.ID + "Id in (" + list.SelectedValue + ")";
                }
            }
        }
        where += " and a.revisiondate between '" + this.FromDate.Text + "' and '" + this.ToDate.Text + "'";
        this.SearchWI(where);        
    }
    protected void Export_Click1(object sender, EventArgs e) {
        this.GridViewToExcel(this.Response, null);
    }
    public override void VerifyRenderingInServerForm(Control control) {
        /* Verifies that the control is rendered */
    }
    void GridViewToExcel(HttpResponse response, DataSet ds) {
        GridView gv = this.GridView1;
        response.Clear();

        response.Buffer = true;
        response.AddHeader("Content-Disposition", "attachment; filename=WIMS.xls");
        response.ContentType = "application/vnd.ms-excel";
        System.IO.StringWriter writer = new System.IO.StringWriter();
        System.Web.UI.HtmlTextWriter html = new HtmlTextWriter(writer);
        //this.DataGrid1.RenderControl(html);

        //gv.DataSource = ds;
        //gv.DataBind();
        foreach (TableCell cell in gv.HeaderRow.Cells)
            if (cell.Text == "c")
                cell.Text = "Total Count";
        gv.RenderControl(html);
        Response.Output.Write(writer.ToString());
        Response.Flush();
        Response.End();

        //response.Write(writer);
        //response.End();
    }
    protected void EquipmentType_SelectedIndexChanged(object sender, EventArgs e) {
        this.UpdateDropDownList("EquipmentType", "EquipmentMake");
    }

    protected void Export_Click(object sender, EventArgs e) {
        this.Submit_Click(null, null);
        if (this.ChartExport == null) {
            this.GridViewToExcel(this.Response, null);
            return;
        }
        Response.Clear();
        Response.Buffer = true;
        Response.AddHeader("content-disposition", "attachment;filename=ChartExport.xls");
        Response.ContentType = "application/vnd.ms-excel";
        Response.Charset = "";
        StringWriter sw = new StringWriter();
        HtmlTextWriter hw = new HtmlTextWriter(sw);
        this.ChartExport.RenderControl(hw);
        string src = Regex.Match(sw.ToString(), "<img.+?src=[\"'](.+?)[\"'].+?>", RegexOptions.IgnoreCase).Groups[1].Value;
        string img = string.Format("<img src = '{0}{1}' />", Request.Url.GetLeftPart(UriPartial.Authority), src);

        Table table = new Table();
        TableRow row = new TableRow();
        row.Cells.Add(new TableCell());
        row.Cells[0].Width = 200;
        row.Cells[0].RowSpan = 26;
        row.Cells[0].HorizontalAlign = HorizontalAlign.Center;
        // row.Cells[0].Controls.Add(new Label { Text = "Fruits Distribution (India)", ForeColor = Color.Red });
        row.Cells[0].Controls.Add(new Literal { Text = img });
        table.Rows.Add(row);
        //row = new TableRow();
        //row.Cells.Add(new TableCell());
        //row.Cells[0].Controls.Add(new Literal { Text = img });
        //table.Rows.Add(row);

        sw = new StringWriter();
        hw = new HtmlTextWriter(sw);
        table.RenderControl(hw);

        System.IO.StringWriter writer = new System.IO.StringWriter();
        System.Web.UI.HtmlTextWriter html = new HtmlTextWriter(writer);
        GridView gv = this.GridView1;
        foreach (TableCell cell in gv.HeaderRow.Cells)
            if (cell.Text == "c")
                cell.Text = "Total Count";
        gv.Rows[gv.Rows.Count - 1].Cells[0].Text = "Total";
        foreach (TableCell cell in gv.Rows[gv.Rows.Count - 1].Cells)
            cell.Font.Bold = true;
        gv.RenderControl(html);
        string s=writer.ToString();

        Response.Write(sw.ToString()+s);
        Response.Flush();
        Response.End();
    }
}