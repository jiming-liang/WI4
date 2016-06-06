using System;
using System.Web.Script.Services;
using System.Diagnostics;
using System.Threading;
using System.Security;
using System.Security.Principal;
using System.IO;
using System.Collections;
using System.Web.Services;
using System.Collections.Generic;
using System.Configuration;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.ApplicationBlocks.Data;
using System.DirectoryServices.AccountManagement;
using System.Data;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.DirectoryServices;
using System.Web.Script.Serialization;
public partial class Users : WIBase {
    string id;
    public struct UserInfo {
        public string Id;
        public string Name;
        public string Email;

    }
    protected void Page_Load(object sender, EventArgs e) {
        this.Function = "AdminFunction";
        this.id = this.Request.QueryString["id"];
        if (!this.IsPostBack) {
            this.MyInit();
        } else {
            this.Post();
        }
    }
    void MyInit() {
        string[] roles = { "Admin", "Global", "Local", "User", "OIM", "Rig Manager", "Operations Manager" };
        this.AllRole.DataSource = roles;
        this.AllRole.DataBind();
        ds = this.da.GetDataSet("select * from Rig order by name");
        this.AllRig.DataSource = ds;
        this.AllRig.DataTextField = "name";
        this.AllRig.DataValueField = "id";
        this.AllRig.DataBind();
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
    public static UserInfo GetUserInfo(string id) {
        UserInfo ui = new UserInfo();
        ui.Id = id;
        Thread.GetDomain().SetPrincipalPolicy(PrincipalPolicy.WindowsPrincipal);
        WindowsPrincipal principal = (WindowsPrincipal)Thread.CurrentPrincipal;
        // or, if you're in Asp.Net with windows authentication you can use: // WindowsPrincipal principal = (WindowsPrincipal)User;
        using (PrincipalContext pc = new PrincipalContext(ContextType.Domain)) {
            UserPrincipal up = UserPrincipal.FindByIdentity(pc, id);
            if (up != null) {
                ui.Name = up.DisplayName.Replace("'", "''");
                string s=up.Guid.ToString();
                ui.Email = up.EmailAddress.Replace("'", "''");
            } else {
                ui.Name = "";
                ui.Email = "";
            }
        }
        return ui;
    }
    [WebMethod]
    public static string ValidateUser(string userId) {
        //string userId = this.Request.QueryString["id"];
        //Users u = new Users();
        //u.GetUserInfo()
        UserInfo ui = Users.GetUserInfo(userId);
        ArrayList arr = new ArrayList();
        arr.Add(ui.Id);
        arr.Add(ui.Name);
        arr.Add(ui.Email);
        //return String.Join(",", arr.ToArray());
        if (ui.Name == "")
            return "";
        List<UserInfo> list = new List<UserInfo> { ui };
        JavaScriptSerializer serializer = new JavaScriptSerializer();
        return serializer.Serialize(list);
    }
    protected void SyncUser_Click(object sender, EventArgs e) {
        string s = "";
        ds = this.da.GetDataSet("select * from users where isnull( name, '')='' or isnull(email, '')='' ");
        foreach (DataRow dr in ds.Tables[0].Rows) {
            id = dr["userId"].ToString();
            UserInfo ui = Users.GetUserInfo(id);
            s += " update users set name='" + ui.Name + "', email='" + ui.Email + "' where UserId='" + id + "'";
        }
        this.da.GetDataSet(s);
    }
    protected void SearchAD_Click(object sender, EventArgs e) {
        string[] arr = new string[] { "sAMAccountName", "givenName", "sn", "mail" };
        DirectoryEntry entry = new DirectoryEntry("LDAP://ensco", "sa-ppointnp", "d3^el0pmint");// CN=users,DC=enscoplc,DC=com");
        DirectorySearcher searcher1 = new DirectorySearcher(entry);
        string name = this.SearchText.Text.Trim();
        searcher1.Filter = "(&(objectClass=user)(|(givenName="+name+"*)(sn="+name+"*)(sAMAccountName="+name+"*)))";
        SearchResultCollection results = searcher1.FindAll();
        string s = "<br><table border=1 id=TableSearchAD width=50% cellspacing=0 cellpadding=0 style='border-collapse:collapse;'>";
        foreach (SearchResult r in results) {
            DirectoryEntry de = r.GetDirectoryEntry();
            s += "<tr><td><a href=#>Add</a>";
            foreach (string key in arr)
                s += "<td>" + de.Properties[key].Value;
        }
        s += "</table>";
        this.l.Text = s;
    }
}