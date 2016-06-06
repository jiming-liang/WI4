using System;
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
public partial class JobStep: WIBase {
    DataSet ds;
    protected void Page_Load(object sender, EventArgs e) {
       // this.Trace.IsEnabled = true;
        if (!this.IsPostBack) {
            this.MyInit();
           // this.Post();

        } else {
          //  this.Post();
        }
    }
    void Post() {
        string eventArg = this.Request["__EVENTARGUMENT"];
        switch (this.Request["__EVENTTARGET"]) {
            case "Submit":
                this.UploadPhoto();
                break;

            default:
                break;
        }      
    }
    void UploadPhoto() {
        HttpFileCollection Files = this.Request.Files;
        string[] arr = Files.AllKeys;  // This will get names of all files into a string array. 
        for (int i = 0; i < arr.Length; i++) {
            Files[i].SaveAs(this.Server.MapPath(@"upload/") + arr[i]);
           // this.SaveThumbnail(arr[i]);
          //  this.Guid.Value = arr[i];
        }
    }
    void MyInit() {        
        DirectoryInfo di = new DirectoryInfo(this.Server.MapPath("~/images/hazard"));
        string s = "<table width=90% >";
        int i =0;
        foreach (FileInfo fi in di.GetFiles()) {
            if (i++ % 2== 0)
                s += "<tr>";
            s += "<td> <input type=checkbox  value=" + fi.Name + " /></td><td style='width1:20%;text-align:left'><img width=40px src=images/hazard/" + fi.Name + "  />";
                s+="<br>"+fi.Name.Replace(".png", "") + "</td>";
        }
        s += "</table>";
        this.holder.InnerHtml = s;
    }


}