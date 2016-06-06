using System;
using System.IO;
using System.Collections;
using System.Web.UI;
using System.Xml.Linq;
using System.Collections.Generic;
using System.Configuration;
using System.Security.Permissions;
using System.Net.Mail;
using Tools;
using System.Data;
using mUtilities.Data;
public partial class FileWatch :Page {
    public DataAccessor da = new DataAccessor(ConfigurationManager.AppSettings["ConnectionStringWI"]);
    public DataSet ds;
    protected void Page_Load(object sender, EventArgs e) {
        this.MyInit();
    }
    void MyInit() {
        string userId, pwd;
        userId =ConfigurationManager.AppSettings["HandsfreeUserId"];
        pwd =  ConfigurationManager.AppSettings["HandsfreePwd"];
        using (new Impersonator(userId, "ensco", pwd)) {
            ds = this.da.GetDataSet("select * from FileWatchList");
            foreach (DataRow dr in ds.Tables[0].Rows) {
                string path = dr["path"].ToString();
                string id = dr["id"].ToString();
                string email = dr["email"].ToString();
                this.da.GetDataSet("update FileWatchList set updatedOn='" + DateTime.Now.ToString() + "' where id=" + id);
                DirectoryInfo di = new DirectoryInfo(path);
                foreach (FileInfo fi in di.GetFiles()) {
                    //this.Response.Write(fi.LastAccessTime.ToString() + " " + fi.CreationTime.ToString() + fi.Name + "<br>");continue;
                    if ( !dr.IsNull("updatedOn") &&  DateTime.Parse(dr["updatedOn"].ToString()) >fi.CreationTime)
                        continue;
                    this.Sendmail(fi.Name, di.FullName, email );
                }
            }
            //  File.Copy(@"c:\aa\test.txt", @"\\ensco\apps\handsfree\ReviewHandsFreeVideos\test.txt");
        }
    }
    void Sendmail(string name, string path, string email) {
        string from = "application.support@enscoplc.com";
        string subject = "Review Hands Free Video";
        string body = "There is a Hands free video {0} at {1}. Please review and take action";
        if (!path.ToLower().StartsWith(@"\\ensco")) {
            subject = "Approved HandsFree Video ";
            body = "There is an approved Hands free video {0} at {1}. ";
        }
        body = string.Format(body, name, path);
        MailMessage m = new MailMessage();
        m.From = new MailAddress(from, "Application Administrator");
        m.To.Add(email );
        m.Subject = subject;
        m.Body = body;
        SmtpClient client = new SmtpClient("smtp.ensco.ws");
        client.Send(m);
    }


}
