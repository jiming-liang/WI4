using System;
using System.Collections.Generic;
using System.Threading;
using System.Security;
using System.Security.Principal;
using System.DirectoryServices.AccountManagement;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.ApplicationBlocks.Data;
using mUtilities.Data;
using System.Configuration;
using System.Web.UI.DataVisualization.Charting;
using System.Drawing;
using System.Collections;
using System.Data.SqlClient;
using System.Data;
public partial class WIBase : System.Web.UI.Page {
    public DataAccessor da = new DataAccessor(ConfigurationManager.AppSettings["ConnectionStringWI"]);
    char[] delimiterChars = { '\\', '\t' };
    public DataSet ds;
    public string[] StatusList = new string[] { "In Progress", "In Testing", "Under Review", "Approved" };

    protected string UserId {
        get {
            if (this.Session["UserId"] == null)
                return "0";
            else
                return (string)this.Session["UserId"];
        }
        set {
            this.Session["UserId"] = value;
        }
    }
    protected string UserName {
        get {
            if (this.Session["UserName"] == null)
                return "0";
            else
                return (string)this.Session["UserName"];
        }
        set {
            this.Session["UserName"] = value;
        }
    }
    protected Color orange = Color.FromArgb(232, 109, 31);
    protected Color deepblue = Color.FromArgb(0, 70, 127);
    protected Color lightblue = Color.FromArgb(61, 183, 228);
    protected int IconSize = 40;
    protected string Role {
        get {
            if (this.Session["Role"] == null)
                return "0";
            else
                return (string)this.Session["Role"];
        }
        set {
            this.Session["Role"] = value;
        }
    }
    protected bool IsAuthorized {
        get {
            if (this.Session["IsAuthorized"] == null)
                return false;
            else
                return (bool)this.Session["IsAuthorized"];
        }
        set {
            this.Session["IsAuthorized"] = value;
        }
    }
    protected ArrayList FunctionList = new ArrayList();
    protected string Function {
        set {
            string f = value;
            return;
            if (!this.FunctionList.Contains(f)) {
                this.Response.Write("<script> alert('Access Denied.'); window.location='home.aspx'</script>");
                //this.Response.End();
            }
        }
    }
    protected string UserAccessRigIdList {
        get {
            return (string)this.Session["UserAccessRigIdList"];
        }
        set {
            this.Session["UserAccessRigIdList"] = value;
        }
    }
    protected void Page_Init(object sender, EventArgs e) {
        //this.Trace.IsEnabled = true;
        this.GetUserId();
        //if (this.IsAuthorized) return;
        SortedList sl = new SortedList();
        sl.Add("@userId", this.UserId);
        ds = this.da.GetDataSet("usp_user_access", sl);
        DataTable dt = ds.Tables[0];
        string s = "<script> var Roles=[";
        ArrayList roleList = new ArrayList();
        foreach (DataRow dr in dt.Rows) {
            roleList.Add("['" + dr["role"].ToString() + "','" + dr["rigid"].ToString() + "']");
           // this.FunctionList.Add(dr["action"].ToString());
        }
        s += String.Join(",", roleList.ToArray()) + "] \n";
        s+=" var UserId='"+this.UserId+@"', UserName="""+this.UserName+@"""    </script>";
        //this.IsAuthorized = true;
        this.Response.Write(s);
        //this.Response.Write("<input id=UserName type=hidden value='" + this.UserName + "' />");
        //this.Response.Write("var UserId='"+this.UserId+"', UserName='"+this.UserName+"'");
        // this.Response.Write("<input id=UserAccessRigIdList type=hidden value='" + this.UserAccessRigIdList + "' />");
        if (!s.Contains("Admin") && !s.Contains("Global") && this.Request.Url.ToString().ToLower().Contains("home.aspx"))
            this.Response.Redirect("search.aspx?isLocal=1");

        this.InitBreadCrumb();
    }
protected void InitBreadCrumb() {
        string[] arr = { "mapping.aspx?", "jobstep", "popawi", "popup", "upload", "sendworkflow", "HistoryLog" };
        string url = this.Request.RawUrl.ToLower();
        foreach (string s in arr) {
            if (url.Contains(s.ToLower()) && !url.Contains("wisid=0") )
                return;
        }
        var form1 = Form.FindControl("form1");
        if (form1 != null) {
            BreadCrumbControl breadcrumbCtrl = (BreadCrumbControl)LoadControl("./BreadCrumbControl.ascx");
            breadcrumbCtrl.UserName = this.UserName;
            form1.Controls.AddAt(0, breadcrumbCtrl);
        }
    }

    public string GetUserId() {
        string id;
        if (!String.IsNullOrEmpty(Page.User.Identity.Name)) {
            id = Page.User.Identity.Name.ToString().Split(delimiterChars)[1];
        } else {
            id = System.Web.HttpContext.Current.Request.LogonUserIdentity.Name.Split(delimiterChars)[1];
        }
        Thread.GetDomain().SetPrincipalPolicy(PrincipalPolicy.WindowsPrincipal);
        WindowsPrincipal principal = (WindowsPrincipal)Thread.CurrentPrincipal;
        // or, if you're in Asp.Net with windows authentication you can use: // WindowsPrincipal principal = (WindowsPrincipal)User;
        using (PrincipalContext pc = new PrincipalContext(ContextType.Domain)) {
            UserPrincipal up = UserPrincipal.FindByIdentity(pc, id);
            this.UserId = id;
            this.UserName = up.DisplayName;
            return this.UserId;
        }
    }
}
