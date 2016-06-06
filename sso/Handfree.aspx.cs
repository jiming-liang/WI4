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
using mUtilities.Data;
using iTextSharp.tool.xml.html;
public partial class Handfree : Page {
    //public DataAccessor da = new DataAccessor("user id=kpi_read;password=KPIiadc123;Data Source=DDc-OASQL02;  Initial Catalog=kpi_dataMart_1" );
    public DataAccessor da = new DataAccessor(ConfigurationManager.AppSettings["ConnectionStringWI"]);

    public DataSet ds;

    protected void Page_Load(object sender, EventArgs e) {
        //  this.BaseUrl = ConfigurationManager.AppSettings["BaseUrl"];
        this.Init();
    }
    void Init() {
        SortedList sl = new SortedList();
        ds = this.da.GetDataSet("select id, Path, Email from FileWatchList order by id");
        this.GridView1.DataSource = ds;
        this.GridView1.DataBind();
    }


}
