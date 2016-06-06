using System;
using System.Net.Mail;
using System.Collections;
using System.IO;
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
public partial class SendWorkflow : WIBase {
    string WiType;
    int Id;
    protected void Page_Load(object sender, EventArgs e) {
        // this.Trace.IsEnabled = true;
        this.WiType = this.Request.QueryString["WiType"];
        this.Id = this.Request.QueryString["id"] == null ? 0 : int.Parse(this.Request.QueryString["id"]);
        if (!this.IsPostBack) {
            this.MyInit();
        } else {
            Post();
        }
        if (this.Request.QueryString["trans"] != null)
            this.EmailTrans();
    }
    void Post() {
        string eventArg = this.Request["__EVENTARGUMENT"];
        switch (this.Request["__EVENTTARGET"]) {
            case "Submit":
                this.Save();
                break;
            default:
                this.Save();
                break;
        }
    }
    void Save() {
        SortedList sl = new SortedList();        
        sl.Add("@id", this.Id);
        sl.Add("@wiType", this.WiType);
        sl.Add("@userId", this.UserId);
        sl.Add("@statusText", "");
        sl.Add("@comment", this.Comment.Text);
        sl.Add("@action", "Feedback");
        ds = this.da.GetDataSet("usp_saveFeedback", sl);
        this.Id =  int.Parse(ds.Tables[0].Rows[0]["id"].ToString());
        this.SendEmail(this.Id, ds.Tables[0].Rows[0]["WINo"].ToString());
    }
    void SendEmail(int id , string WiNo) {
        string from = "application.support@enscoplc.com";
        ds = this.da.GetDataSet("usp_getAdminEmails");
        string toEmail = ds.Tables[0].Rows[0][0].ToString();
        MailMessage m = new MailMessage();
        m.From = new MailAddress(from, "WIMS administrator");
        foreach (string s in toEmail.Split(','))
            if (s.Trim()!="")
                m.To.Add(s);
       // m.To.Add("fwang@enscoplc.com");
        m.Subject = "Feedback on Global Content for "+WiNo;

        HttpFileCollection Files = this.Request.Files;
        string[] arr = Files.AllKeys;  // This will get names of all files into a string array. 
        System.IO.Stream myStream = null;
        for (int i = 0; i < arr.Length; i++) {
            myStream = Files[i].InputStream;
            string name = Files[i].FileName.Split('\\').Last();
            Attachment a = new Attachment(myStream, name);
            m.Attachments.Add(a);
        }
        m.IsBodyHtml = true;
        string url = Request.Url.GetLeftPart(UriPartial.Authority) + Request.ApplicationPath;
        url += "/create.aspx?id=" + id.ToString()+"&wiType=WIT"+ "<br>";
        string comment = this.Comment.Text+"<br>";
        m.Body = "Feedback on Global Content for " + WiNo + " from " + this.UserName + "<br>"+comment + url;
        SmtpClient client = new SmtpClient("smtp.ensco.ws");
        client.Send(m);
        if (myStream != null)
            myStream.Close();
    }
    void EmailTrans() {
        string from = "fwang@enscoplc.com";
        MailMessage m = new MailMessage(from, from);
        m.Subject = "Trans failed";
        m.Body = "Trans failed";
        SmtpClient client = new SmtpClient("smtp.ensco.ws");
        client.Send(m);
    }
    void MyInit() {

    }
}