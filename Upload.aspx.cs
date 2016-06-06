using System;
using System.Collections;
using System.Net;
using System.Collections.Generic;
using System.Configuration;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.ApplicationBlocks.Data;
using System.Data;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Data.SqlClient;
public partial class Upload : WIBase {
    protected void Page_Load(object sender, EventArgs e) {
        this.UploadPhoto();
    }
    void UploadPhoto() {
        HttpFileCollection Files = this.Request.Files;
        string[] arr = Files.AllKeys;  // This will get names of all files into a string array. 
        string s = this.Request.RawUrl;
        for (int i = 0; i < arr.Length; i++) {
            if (this.Request.QueryString["thumbnail"] == "0")
                this.SaveShrink(arr[i], Files[i].FileName);
            else {
                Files[i].SaveAs(this.Server.MapPath(@"upload/") + arr[i]);
                this.SaveThumbnail(arr[i]);
            }
            this.Guid.Value = arr[i];
        }
    }
    void UploadPhotoBak() {
        HttpFileCollection Files = this.Request.Files;
        string[] arr = Files.AllKeys;  // This will get names of all files into a string array. 
        for (int i = 0; i < arr.Length; i++) {
            Files[i].SaveAs(this.Server.MapPath(@"upload/") + arr[i]);
            if (this.Request.QueryString["thumbnail"] != "0")
                this.SaveThumbnail(arr[i]);
            this.Guid.Value = arr[i];
        }
    }
    public bool ThumbnailCallback() {
        return false;
    }
    void SaveShrink(string name, string path  ) {
        System.Drawing.Image image = System.Drawing.Image.FromFile(path);
        int w = image.Width;
        int h = image.Height;
        int newWidth = 150;
        image = (System.Drawing.Image)(new Bitmap(image, new Size(newWidth, newWidth*h/w)));

        string s = Server.MapPath("Upload/" + name);
        image.Save(s);
        image.Dispose();
    }
    void SaveThumbnail(string name) {
        string s = Server.MapPath("Upload/" + name);
        System.Drawing.Image.GetThumbnailImageAbort myCallBack = new System.Drawing.Image.GetThumbnailImageAbort(ThumbnailCallback);
        Bitmap myBitmap = new Bitmap(s);
        System.Drawing.Image myThumbnail = myBitmap.GetThumbnailImage(50, 50, myCallBack, IntPtr.Zero);
        s = Server.MapPath("Upload/" + name.Replace(".", "thumb."));
        myThumbnail.Save(s);
        myThumbnail.Dispose();
        myBitmap.Dispose();
    }
}