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
using System.Data;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Diagnostics;

public partial class Create : WIBase {
    string id;
    public string RowVersion;
    public string RigId;
    string[] Headers = { "Global Step #", "Local Step #", "Warning / Caution / Note / Job Step ", "Photo", "Barrier" };
    string WiType = "WIT";
    string BaseUrl = ConfigurationManager.AppSettings["BaseUrl"];
    string FacilityList;
    float TotalWidth = 800f;
    public string  ActionRequired;
    public bool IsApprover;
    protected string Action = "";
    public string Dict;
    public string MappingXml {
        get {
            if (this.Session["MappingXml"] == null)
                return "";
            else
                return this.Session["MappingXml"].ToString();
        }
        set {
            this.Session["MappingXml"] = value;
            this.MappingXmlHidden.Value = value;
        }
    }
    public string Rig;
    bool IsNoteBlank = true;
    protected string LessThanEncode = "!23!@#1";
    protected string AmpEncode = "q2sfd@";
    protected void Page_Load(object sender, EventArgs e) {
        this.GetParameters();
        if (this.Action  == "preview") {
            this.Preview();
            return;
        }
        this.WiRole.Value = this.Role;
        if (!this.IsPostBack) {
            this.Post();
            this.MyInit();
        }else
            this.Post();
    }
    void  GetParameters() {
        this.id = this.Request.QueryString["id"];
        this.RigId = this.Request.QueryString["RigId"];
        if (this.RigId == null) this.RigId = "0";
        this.WiType = this.Request.QueryString["wiType"];
        this.WiType = this.WiType.ToUpper();
        if (this.id == null) this.id = "0";
        this.Action = this.Request.QueryString["action"];
        this.Action = this.Action == null ? "" : this.Action.ToLower();
        this.Rig = this.Request.QueryString["Rig"];
    }
    void MyInit() {
        this.resources.Attributes.Add("xml", "");
        this.PersonalProtectiveEquipment.Attributes.Add("xml", "");
        this.tool.Attributes.Add("xml", "");
        this.WI.Attributes.Add("xml", "");

        if (  (this.WiType=="WIT" &&  this.id == "0")|| (this.WiType=="WIS" &&  this.id == "0"))  {
            this.JobDescriptionId.Value = this.Request.Cookies["JobDescriptionId"].Value;
            this.JobDescription.Attributes.Add("WICategoryName", this.Request.Cookies["WICategoryName"].Value);
            this.JobDescription.Attributes.Add("JobDescriptionId", this.Request.Cookies["JobDescriptionId"].Value);
            this.JobDescription.Attributes.Add("JobDescriptionNumber", this.Request.Cookies["JobDescriptionNumber"].Value);
            this.JobDescription.Text = this.Request.Cookies["JobDescriptionName"].Value;

            this.Facility.Text = this.GetFacility();
            this.WiNo.Text = this.GetWiNo();
        }
        this.ShowWI();  
    }
    void ShowWI() {
        this.GetDs();
        this.Criticality.DataSource = new string[] { "", "Low", "Medium", "High" };
        this.Criticality.DataBind();
        this.Status.DataSource = new string[] { "In Progress", "Under Review", "Approved" };
        this.Status.DataBind();
        this.Status.SelectedValue = "In Progress";

        this.EquipmentType.DataSource = ds.Tables[6];
        this.EquipmentType.DataTextField = "name";
        this.EquipmentType.DataValueField = "id";
        this.EquipmentType.DataBind();
        this.EquipmentType.Items.Insert(0, "");

        this.EquipmentMake.DataSource = ds.Tables[7];
        this.EquipmentMake.DataTextField = "name";
        this.EquipmentMake.DataValueField = "id";
        this.EquipmentMake.DataBind();
        if (ds.Tables[0].Rows.Count > 0) {
            DataRow dr = ds.Tables[0].Rows[0];
            this.JobDescriptionId.Value = dr["JobDescriptionId"].ToString();

            this.JobDescription.Attributes.Add("WICategoryName", dr["WICategoryName"].ToString());
            this.JobDescription.Attributes.Add("JobDescriptionId", dr["JobDescriptionId"].ToString());
            this.JobDescription.Attributes.Add("JobDescriptionNumber", dr["JobDescriptionNumber"].ToString());
            this.JobDescription.Text = dr["JobDescriptionName"].ToString();

            this.GeneralPrecautions.InnerHtml = this.HtmlDecodeRichText(dr["GeneralPrecautions"].ToString());
            this.LocalPrecautions.InnerHtml = this.HtmlDecodeRichText(dr["LocalPrecautions"].ToString());
            this.Criticality.SelectedValue = dr["Criticality"].ToString().Trim();
            this.Permit.Checked = dr["Permit"].ToString() == "True" ? true : false;

            this.ShowList("resources", 3);
            this.ShowList("tool", 4);
            this.ShowList("PersonalProtectiveEquipment", 8);
            this.ShowAssociatedWI(ds.Tables[5]);
            this.ShowAssociatedLink(ds.Tables[10]);
            string xml = "";//<MappingXml>";
            foreach (DataRow dr1 in ds.Tables[9].Rows) {
                xml += dr1["xml"].ToString();
            }
            // xml += "</MappingXml>";
            xml = HttpUtility.HtmlEncode(xml);
            xml = HttpUtility.UrlEncode(xml);
            this.MappingXml = xml;
            this.Facility.Text = this.GetFacility();
            this.WiNo.Text = this.GetWiNo();

            //this.Response.Cookies["wiid"].Value = ds.Tables[0].Rows[0]["id"].ToString();
            this.wiid.Value = ds.Tables[0].Rows[0]["id"].ToString();
            this.EquipmentType.SelectedValue = dr["EquipmentTypeId"].ToString();
            this.EquipmentMake.SelectedValue = dr["EquipmentMakeId"].ToString();
            this.Status.SelectedValue = dr["status"].ToString();
        }
        //this.ShowHeader();
        //this.GridViewAttachment.DataSource = ds.Tables[2];
        //this.GridViewAttachment.DataBind();
        //this.GridViewAttachment.HeaderRow.BackColor = this.deepblue;
        //this.FormatGridViewAttachment();
        this.RenderJobStep();
        this.RenderAttachment();
        if (this.WiType!="WIT" && this.Action!="approved") {
            this.Status.Visible = false;
            if (this.WiType=="WIS" && this.id=="0")
                this.StatusText.InnerText = "Draft";
            else {
                this.StatusText.InnerText = ds.Tables[0].Rows[0]["Status"].ToString();

                SortedList sl = new SortedList();
                sl.Add("@userId", this.UserId);
                //if (this.WiType=="WI")
                    sl.Add("@id", this.id);
                //else
                //    sl.Add("@id", this.WisId);
                sl.Add("@WiType", this.WiType);
                DataSet ds1 = this.da.GetDataSet("usp_getApprover", sl);
                if (ds1.Tables[0].Rows.Count>0)
                this.IsApprover = true;
            }
        }
    }
    string HtmlDecodeRichText(string value) {
            value = HttpUtility.HtmlDecode(value);
        value = value.Replace(this.LessThanEncode, "&lt;");
        value = value.Replace(this.AmpEncode, "&amp;");
        return value;
    }
    string GetFacility() {
        string xml = this.MappingXml; //;= eventArg;// @"<MappingXml >" + HttpUtility.HtmlDecode(eventArg) + @"</MappingXml >";
        xml = HttpUtility.UrlDecode(xml);
        xml = HttpUtility.HtmlDecode(xml);
        xml = xml.Replace("&", "&amp;");//
        xml = @"<MappingXml >" + xml + @"</MappingXml >";
        XDocument doc = XDocument.Parse(xml);//this.MappingXml);
        ArrayList arrFacility = new ArrayList();
        foreach (XElement xe in doc.Descendants("MappingXml").Descendants()) {
            arrFacility.Add(xe.Attribute("code").Value);
        }
        arrFacility.Sort();
        return string.Join("/", arrFacility.ToArray()).Replace("&amp;", "&");
    }
    string GetWiNo() {
        string cat = this.JobDescription.Attributes["WICategoryName"];
        string number = this.JobDescription.Attributes["JobDescriptionNumber"];

        string fac = this.GetFacility();
        fac = fac.Replace(",", "/");
        string prefix = "WIT";
        if (this.WiType == "WIS")
            prefix = "WIS";
        if (this.WiType=="WI") {
            fac = ds.Tables[0].Rows[0]["rig"].ToString();
            prefix = "WI";// ds.Tables[0].Rows[0]["WiNo"].ToString();
        }
        return prefix+"-" + fac + "-" + cat + "-" + number;
    }
    void ShowHeader() {
        //this.JobDescription.Text = this.JobDescriptionName;
        this.EquipmentType.DataSource = ds.Tables[6];
        this.EquipmentType.DataTextField = "name";
        this.EquipmentType.DataValueField = "id";
        this.EquipmentType.DataBind();
        this.EquipmentType.Items.Insert(0, "");

        this.EquipmentMake.DataSource = ds.Tables[7];
        this.EquipmentMake.DataTextField = "name";
        this.EquipmentMake.DataValueField = "id";
        this.EquipmentMake.DataBind();

        if (ds.Tables[0].Rows.Count > 0) {
            // this.Response.Cookies["wiid"].Value = ds.Tables[0].Rows[0]["id"].ToString();

            HttpCookie ck = Request.Cookies["wiid"];
            if (ck.Value == "undefined" || string.IsNullOrEmpty(ck.Value)) {
                ck.Value = ds.Tables[0].Rows[0]["id"].ToString();
                Response.Cookies.Set(ck);
            }

            this.JobDescription.Attributes.Add("JobDescriptionId", ds.Tables[0].Rows[0]["JobDescriptionId"].ToString());
            this.EquipmentType.SelectedValue = ds.Tables[0].Rows[0]["EquipmentTypeId"].ToString();
            this.EquipmentMake.SelectedValue = ds.Tables[0].Rows[0]["EquipmentMakeId"].ToString();

            string status = ds.Tables[0].Rows[0]["Status"].ToString();
            string[] statusList = new string[] { "In Progress", "Under Review", "Approved" };
            for (int i = 0; i < statusList.Length; i++) {
                if (statusList[i] == status)
                    this.Status.Items[i].Selected = true;
            }
        }
        if (this.Role != "Admin") {
            this.Status.Enabled = false;
        }
    }
    void Post() {
        string eventArg = this.Request["__EVENTARGUMENT"];
        switch (this.Request["__EVENTTARGET"]) {
            case "Create":
                this.MappingXml = eventArg;// @"<MappingXml >" + HttpUtility.HtmlDecode(eventArg) + @"</MappingXml >";
                this.MyInit();
                break;
            case "Save":
                if (eventArg != "")
                    this.MappingXml = eventArg;// "<MappingXml >" + HttpUtility.HtmlDecode(eventArg) + "</MappingXml>"; ;
                if (this.WiType == "WI")
                    this.SaveLocal();
                else if (this.WiType == "WIS")
                    this.Save();
                else
                    Save();
                break;
            case "Photo":
                DisplayAttachment(eventArg);
                break;
            case "Preview":
                this.Action = "Preview";
                this.Preview();
                break;
            case "PreviewWord":
                this.Action = "Preview";
                this.PreviewWord();
                break;
            case "Convert":
                this.Convert();
                break;
            default:
                break;
        }
    }
    void DisplayAttachment(string id) {
        string name = "", contentType = "";

        this.Response.Clear();
        this.Response.Buffer = true;
        this.Response.AddHeader("Content-Disposition", "attachment; filename=" + name);
        this.Response.ContentType = contentType; // "application/pdf";//vnd.ms-excel";

        FileStream MyFileStream;
        long FileSize;

        MyFileStream = new FileStream("sometext.txt", FileMode.Open);
        FileSize = MyFileStream.Length;

        byte[] Buffer = new byte[(int)FileSize];
        MyFileStream.Read(Buffer, 0, (int)FileSize);
        MyFileStream.Close();

        Response.BinaryWrite(Buffer);

        this.Response.End();
    }
    void Save() {
        this.Function = "EditGlobal";
        string xml = HttpUtility.HtmlDecode(this.Request["h1"]);
        xml = xml.Replace("&nbsp;", " ");
        XDocument doc = XDocument.Parse(xml);
        XElement root = doc.Element("wi");
        if (true) {//this.id == "0") {
            string mappingXml = this.MappingXml;
            mappingXml = HttpUtility.UrlDecode(mappingXml);
            mappingXml = HttpUtility.HtmlDecode(mappingXml);
            mappingXml = mappingXml.Replace("&", "&amp;");//.Replace("<", "&lt;").Replace(">", "&gt;").Replace("\"", "&quot;").Replace("'", "&apos;");

            mappingXml = "<MappingXml >" + mappingXml + "</MappingXml>";
            XDocument doc1 = XDocument.Parse(mappingXml);
            XElement root1 = doc1.Element("MappingXml");
            root.Add(root1);
        }
        string status = this.Status.Text;
        if (this.Duplicate.Value == "1") {
            this.id = "0";
            if (this.WiType == "WIS")
                status = "Draft";
        }
            this.AddAttribute(root, "id", this.id);
        string cat = this.JobDescription.Attributes["WICategoryName"];
        string number = this.JobDescription.Attributes["JobDescriptionNumber"];
        this.AddAttribute(root, "Name", cat + "-" + number);

        this.AddAttribute(root, "Facility", this.GetFacility());
        this.AddAttribute(root, "WiNo", this.GetWiNo());
        this.AddAttribute(root, "Criticality", this.Criticality.SelectedValue);
        this.AddAttribute(root, "Permit", this.Permit.Checked.ToString());
        this.AddAttribute(root, "EquipmentMake", this.Request["EquipmentMake"] == null ? "" : this.Request["EquipmentMake"]);
        //if (this.Duplicate.Value == "1")
        //    this.AddAttribute(root, "Status", "In Progress");
        //else
            this.AddAttribute(root, "Status",status );
        this.AddAttribute(root, "UserId", this.UserName);
        // if (this.id == "0") {
        this.AddAttribute(root, "JobDescriptionId", this.JobDescriptionId.Value);
        //}
        SortedList sl = new SortedList();
        sl.Add("@xml", doc.ToString());
        ds = this.da.GetDataSet("usp_saveXML", sl);
        this.Upload();
        this.id = ds.Tables[0].Rows[0][0].ToString();
        if (this.id == "-1") {
            string s = @"<script type=text/javascript src=jquery-1.10.2.js></script><script type=text/javascript src=WI.js></script>    ";
            this.Response.Write(s);
            this.Response.Write("<br><br><br><br><br><br>The Work Instruction already exists for this mapping.");
            this.Response.End();
        }
        this.Response.Redirect("Create.aspx?id=" + this.id+"&WiType="+this.WiType);    }
    void SaveLocal() {
        this.Function = "EditLocal";
        string xml = HttpUtility.HtmlDecode(this.Request["h1"]);
        xml = xml.Replace("&nbsp;", " ");
        XDocument doc = XDocument.Parse(xml);
        XElement root = doc.Element("wi");

        this.AddAttribute(root, "id", this.id);
        this.AddAttribute(root, "RigId", this.RigId);
       // this.AddAttribute(root, "LocalId", this.LocalId);
        this.AddAttribute(root, "UserId", this.UserName);
       // this.AddAttribute(root, "Status", this.Status.Text);
        SortedList sl = new SortedList();
        sl.Add("@xml", doc.ToString());
        ds = this.da.GetDataSet("usp_saveXMLlocal", sl);
        this.Upload();
        this.id = ds.Tables[0].Rows[0][0].ToString();

        this.Response.Redirect("Create.aspx?id=" + this.id +"&wiType=WI");
    }
    void AddAttribute(XElement element, string name, string value) {
        element.Add(new XAttribute(name, value));
    }
    void AddElement(XElement element, string elementName, string name, string value) {
        element.Add(new XElement(elementName, new XAttribute(name, value)));
    }
    string AddNameValue(string name, string value) {
        return " " + name + @"=""" + value + @""" ";
    }
    void Upload() {
        HttpFileCollection Files = this.Request.Files;
        string[] arr = Files.AllKeys;  // This will get names of all files into a string array. 
        for (int i = 0; i < arr.Length; i++) {
            //string ext=Path.GetExtension(Files[i].FileName);
            Files[i].SaveAs(this.Server.MapPath(@"upload/") + arr[i]);
            int size = Files[i].ContentLength;
        }
    }

    void ShowResources(string name, int index) {
        DataTable dt = ds.Tables[index];
        ArrayList list = new ArrayList();
        ArrayList list0 = new ArrayList();
        string xml = "", s = "", s0 = "";
        int prevRowId = -1, curRowId = 0;
        bool addXml = false;
        int i = 0;
        foreach (DataRow dr in dt.Rows) {
            if (dt.Columns.Contains("row")) {
                curRowId = int.Parse(dr["row"].ToString());
                if (curRowId != prevRowId) {
                    if (prevRowId != -1) {
                        string[] arr0 = (string[])list.ToArray(Type.GetType("System.String"));
                        list0.Add(string.Join(" / ", arr0));
                        list.Clear();
                        xml += "</row>";
                    }
                    xml += @"<row id="""+i.ToString()+@""" >";
                    i++;
                    prevRowId = curRowId;
                    addXml = true;
                }
            }
            xml += @"<" + name + @" id=""" + dr["id"] + @"""" + @" name=""" + dr["name"] + @"""";
            s = "" + dr["name"];
            if (dr.Table.Columns.Contains("other") && dr["other"].ToString().Trim() != "undefined" && dr["other"].ToString().Trim() != "") {
                string other = dr["other"].ToString();
                other = HttpUtility.HtmlEncode(other);
                s = other;// dr["other"] + "";
                xml += @" other=""" + other + @""" ";
            }
            if (dr.Table.Columns.Contains("number")) {
                s += "(" + dr["number"] + ")";
                xml += @" number=""" + dr["number"] + @""" ";
            }
           list.Add(s);
            xml += @"  />";

        }
        if (addXml) {
            list0.Add(string.Join(" / ", (string[])list.ToArray(Type.GetType("System.String"))));
            xml += "</row>";
        }

        string[] arr = (string[])list0.ToArray(Type.GetType("System.String"));
        HtmlTableCell td = (HtmlTableCell)this.FindControl(name);
        td.InnerHtml = string.Join(", ", arr);
        td.Attributes.Add("xml", xml);
    }
    void ShowList(string name, int index) {
        ArrayList list = new ArrayList();
        string xml = "", s = "";
        if (name == "resources") {
            this.ShowResources(name, index);
            return;
        }
        foreach (DataRow dr in ds.Tables[index].Rows) {
            xml += @"<" + name + @" id=""" + dr["id"] + @"""" + @" name=""" + dr["name"] + @"""";
            s = "" + dr["name"];
            if (dr.Table.Columns.Contains("other") && dr["other"].ToString().Trim() != "undefined" && dr["other"].ToString().Trim() != "") {
                string other = dr["other"].ToString();
                other = HttpUtility.HtmlEncode(other);
                s =  dr["other"] + "";
                xml += @" other=""" + other + @""" ";
            }
            if (dr.Table.Columns.Contains("number")) {
                s += "(" + dr["number"] + ")";
                xml += @" number=""" + dr["number"] + @""" ";
            }
            list.Add(s);
            xml += @"  />";
        }
        string[] arr = (string[])list.ToArray(Type.GetType("System.String"));
        HtmlTableCell td = (HtmlTableCell)this.FindControl(name);
        td.InnerText = string.Join(", ", arr);
        td.Attributes.Add("xml", xml);
    }
    void ShowAssociatedWI(DataTable dt) {
        string s = "", xml = "";
        foreach (DataRow dr in dt.Rows) {
            XmlElement root = this.GetXmlRoot("WI");
            root.SetAttribute("id", dr["id"].ToString());
            root.SetAttribute("type", dr["type"].ToString());
            root.SetAttribute("name", HttpUtility.HtmlEncode(dr["name"]));
            xml += root.OuterXml;
            s += "<a target=_new href=Create.aspx?";
            string wiType = dr["name"].ToString().Split('-')[0];
            s += "id=" + dr["id"] + "&wiType=" + wiType + " >" + dr["name"] + "</a> <br>";
        }
        this.WI.InnerHtml = s;
        this.WI.Attributes.Add("xml", xml);
    }
    XmlElement GetXmlRoot(string name) {
        XmlDocument doc = new XmlDocument();
        doc.LoadXml("<"+name+"/>");
        return  doc.DocumentElement;
    }
    void ShowAssociatedWI010816(DataTable dt) {
        string s = "", xml = "";
        foreach (DataRow dr in dt.Rows) {
            xml += @"<WI id=""" + dr["id"] + @"""  name=""" + HttpUtility.HtmlEncode(dr["name"]) + @"""  />";
            s += "<a target=_new href=Create.aspx?";
            if (this.WiType == "WIT")
                s += "id=" + dr["id"] + " >" + dr["name"] + "</a> <br>";
            else {
                if (dr["name"].ToString().StartsWith("WIS"))
                    s += "wisId=" + dr["id"];
                else if (dr.IsNull(dr.Table.Columns["localId"]))
                    s += "id=" + dr["id"] + "&rigId=" + dr["rigId"];
                else
                    s += "localId=" + dr["localId"];
                s += " >" + dr["name"] + "</a> <br>";
            }
        }
        this.WI.InnerHtml = s;
        this.WI.Attributes.Add("xml", xml);
    }
    void ShowAssociatedLink(DataTable dt) {
        string s = "", xml = "";
       // string t1 = @"<WILink title=""{0}""  link=""{1}"" />";
        string t2 = @"<a target=_new href='{1}' >{0}</a> &nbsp";
        foreach (DataRow dr in dt.Rows) {
            XmlElement root = this.GetXmlRoot("WILink");
            root.SetAttribute("title", dr["title"].ToString());
            root.SetAttribute("type", dr["type"].ToString());
            root.SetAttribute("link", dr["link"].ToString());
            xml += root.OuterXml;
           // xml += string.Format(t1, dr["title"], dr["link"]);
            s += string.Format(t2, dr["title"], dr["link"].ToString().Replace("@!123", "&"));
        }
        this.WILink.InnerHtml = s;
        this.WILink.Attributes.Add("xml", xml);
    }
    void GetDs() {
        SortedList sl = new SortedList();
        this.Action = this.Request.QueryString["action"];
        this.Action = this.Action == null ? "" : this.Action.ToLower();
        if (this.WiType == "WI") {//&& this.LocalId!="0") {
            sl.Add("@id", this.id);
            if (this.Action.ToLower() == "approved")
                ds = this.da.GetDataSet("usp_getWIlocalApproved", sl);
            else {
                ds = this.da.GetDataSet("usp_getWIlocal", sl);
                if (ds.Tables[0].Rows.Count > 0 && ds.Tables[0].Columns.Contains("ActionRequired"))
                    this.ActionRequired = ds.Tables[0].Rows[0]["ActionRequired"].ToString();
            }
        } else {
            sl.Add("@id", this.id);
            //sl.Add("@rigId", "0");
            if (this.Action.ToLower() == "approved")
                ds = this.da.GetDataSet("usp_getWIApproved", sl);
            else
                ds = this.da.GetDataSet("usp_getWI", sl);
        }
        if (ds.Tables.Count == 0 && ds.Tables[0].Rows.Count == 0) {
            this.Response.Write("<script> alert('The record does not exist.');window.location='home.aspx'</script> ");
            this.Response.End();
        }
        if (ds.Tables[0].Rows.Count > 0)
            this.RowVersion = ds.Tables[0].Rows[0]["r"].ToString();
        if (this.RigId == "0" && this.ds.Tables[0].Columns.Contains("rigId"))
            this.RigId = ds.Tables[0].Rows[0]["RigId"].ToString();

        if (this.WiType == "WIS") {
            if (ds.Tables.Count > 11 && ds.Tables[11].Rows.Count > 0)
                this.RigId = ds.Tables[11].Rows[0][0].ToString();
        }
    }
    void RenderJobStep() {
        DataTable dt = ds.Tables[1];
        this.GridView1.DataSource = dt;
        this.GridView1.DataBind();
        this.GridView1.HeaderRow.BackColor = this.deepblue;
        StringBuilder sb = new StringBuilder();
        ArrayList al0 = new ArrayList();
        foreach (GridViewRow row in this.GridView1.Rows) {
            //CheckBox ck = row.Cells[13].Controls[0] as CheckBox;

            foreach (TableCell cell in row.Cells)
                cell.Text = cell.Text.Replace("&nbsp;", "");
            this.DecodeColumn(row, "Warning");
            this.DecodeColumn(row, "Caution");
            this.DecodeColumn(row, "Description");
            this.DecodeColumn(row, "Description");
            this.DecodeColumn(row, "Note");
            this.DecodeColumn(row, "Note");
            ArrayList al = new ArrayList();
            string id = row.Cells[0].Text;
            string name = row.Cells[2].Text;
            string[] cols = { "id", "Type", "Description", "Warning", "Caution","Note", "Hazard", "Photo", "Barrier", "Prompt" };
            foreach (string col in cols) {
                this.AddKeyValue(row, col);
            }
            string guid = Guid.NewGuid().ToString();
            row.Cells[3].Text = this.DecodeStep(row.Cells[3].Text);
            string img = row.Cells[8].Text;
            if (img.Trim() != "")
                img = "<img width=144px height=144px  src=upload/" + img + " />";
            row.Cells[8].Text = img;
            row.Cells[6].Text = this.DecodeStep(row.Cells[6].Text);

            row.Cells[0].Attributes.Add("Key", guid);
            // row.Cells[0].Attributes.Add("StepId", row.Cells[0].Text);
            string s = this.AddLink("Edit");
            if (row.Cells[13].Text=="")
            //    s += this.AddLink("Delete", "disabled");
            //else
                s += this.AddLink("Delete");
            row.Cells[0].Text = s+ this.AddLink("Insert Above");
            row.Cells[0].HorizontalAlign = HorizontalAlign.Right;
            row.Cells[0].Wrap = false;
            for (int i = 1; i < 6; i++)
                if (i == 3) {
                    row.Cells[i].VerticalAlign = VerticalAlign.Top;
                } else {
                    row.Cells[i].Width = Unit.Percentage(5);
                    row.Cells[i].HorizontalAlign = HorizontalAlign.Center;
                }
        }
        for (int i = 0; i < 6; i++) {
            if (i != 0)
                this.GridView1.HeaderRow.Cells[i].Text = this.Headers[i - 1];
            this.GridView1.HeaderRow.Cells[1].Text = "Global<br> Step #";
            this.GridView1.HeaderRow.Cells[2].Text = "Local<br> Step #";
           // this.GridView1.HeaderRow.Cells[5].Text = "Barriers<br> (Initial)";
            if (i != 3) {
                this.GridView1.HeaderRow.Cells[i].Width = Unit.Percentage(5);
                this.GridView1.HeaderRow.Cells[i].Wrap = false;
                this.GridView1.HeaderRow.Cells[i].HorizontalAlign = HorizontalAlign.Center;
            }
        }
        this.GridView1.HeaderRow.Cells[0].Text = "";
    }
    void AddKeyValue(GridViewRow row, string name) {
        if (name == "id")
            row.Cells[0].Attributes.Add("StepId", this.GetValue(row, name));
        else
            row.Cells[0].Attributes.Add(name, this.GetValue(row, name));
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
    string GetValue(GridViewRow row, string name) {
        int index = -1;
        for (int i = 0; i < this.GridView1.HeaderRow.Cells.Count; i++)
            if (this.GridView1.HeaderRow.Cells[i].Text == name)
                index = i;
        //if (index == -1)
        //    return "";
        string s = row.Cells[index].Text;
       // s=s.Replace(this.)
        if (s == "&nbsp;")
            return "";
        return s;
    }
    string DecodeStep(string value) {
        return HttpUtility.HtmlDecode(HttpUtility.HtmlDecode(value));
    }
    string EncodeNewline(string name, string value) {
        string line = "";
        // string s = value.Replace("\r\n", replaceWith).Replace("\n", replaceWith).Replace("\r", replaceWith);
        string[] arr = value.Split('\n');
        foreach (string s in arr) {
            if (s.Trim() != "") {
                if (name != "Name")
                    line += "<b>" + name + ":</b>";
                line += s;
            }
            line += "<br>";
        }
        return line;
    }
    void RenderAttachment() {
        this.GridViewAttachment.DataSource = this.ds.Tables[2];
        this.GridViewAttachment.DataBind();
        this.GridViewAttachment.HeaderRow.BackColor = this.deepblue;
    }
    string GetFigureNo(string type, int index, int indexLocal) {
        if (type == "Local") {
           return  ((char) (indexLocal+64)).ToString();
        }
        return index.ToString();
    }
    void FormatGridViewAttachment() {
        return;
        //foreach (TableRow tr in this.GridView1.Rows) {
        //    string delete="<input type=button onclick=on_AttachmentDelete(this) value=Delete />&nbsp";
        //    string edit="<input type=button onclick=on_UploadAttachment(this) value=Edit />";
        //    tr.Cells[0].Text = delete + edit;
        //    tr.Cells[0].HorizontalAlign = HorizontalAlign.Right;
        //    string title = tr.Cells[1].Text;
        //    string name = tr.Cells[2].Text;
        //    string prompt = tr.Cells[4].Text;
        //    prompt = HttpUtility.HtmlDecode(prompt).Trim();
        //    if (this.IsLocal && tr.Cells[3].Text != "Local") {
        //        tr.Cells[0].Text = "";
        //        if (prompt != "")
        //            tr.Cells[0].Text =delete+ edit;
        //    }
        //    string img;
        //    //  img = "<a onmouseover='on_help(this);return' onmouseout='on_help_out(this);return' href=# photo='" + name + "' >Photo</a>";
        //    string thumb = name.Replace(".", "thumb.");
        //    img = "<img  onmouseover='on_mouseover(this);return' onmouseout='on_mouseout(this);return'  photo='" + name + "' src=upload/" + thumb + " />";
        //    tr.Cells[0].Attributes.Add("photo", name);
        //    tr.Cells[0].Attributes.Add("prompt", prompt);
        //    tr.Cells[1].Text = title;
        //    if ( prompt == "")
        //        tr.Cells[2].Text = img;
        //    else
        //        tr.Cells[2].Text = prompt;
        //    tr.Cells.RemoveAt(3);
        //    tr.Cells.RemoveAt(3);
        //}
        //this.GridViewAttachment.HeaderRow.Cells.RemoveAt( 3);
        //this.GridViewAttachment.HeaderRow.Cells.RemoveAt(3);
    }
    string AddLink(string name, string disabled="") {
        string s = "&nbsp<input type=button value='{0}' onclick=on_{0}(this)  "+disabled+" />";
        if (name == "Insert Above")
            return string.Format(s, name).Replace("on_Insert Above", "on_Insert");
        return String.Format(s, name);
    }
    void Preview2() {
       this.GetDs();
       PreviewPDF preview = new PreviewPDF();
        preview.ds =  ds;
       preview.Rig = this.Rig;
       switch (this.WiType) {
           case "WI":
               preview.Prefix = "WI-";
               preview.Rig = ds.Tables[0].Rows[0]["rig"].ToString();
               break;
           case "WIS":
               preview.Prefix = "WIS-";
               preview.Rig = ds.Tables[0].Rows[0]["facility"].ToString();
               break;
       }
       preview.Headers = this.Headers;
       preview.BaseUrl = this.BaseUrl;
       preview.LessThanEncode = this.LessThanEncode;
       preview.AmpEncode = this.AmpEncode;
       preview.SelectedStatus = this.Status.SelectedValue;
       preview.Start( this.Response, this.UserName);
       if (this.WiType != "WIT") {
            SortedList sl = new SortedList();
            sl.Add("@id", this.id);
            sl.Add("@WiType", this.WiType);
            this.da.GetDataSet("usp_updateWiPrinted", sl);
        }
    }
    void Preview() {
        this.PreviewStart("PDF");
    }
    void PreviewStart(string type) {
        PdfAndWord common;
        if (type == "PDF")
            common = new PreviewPDF();
        else
            common = new Word();
        this.GetDs();
        common.ds = ds;
        common.Rig = this.Rig;

        switch (this.WiType) {
            case "WI":
                common.Prefix = "WI-";
                common.Rig = ds.Tables[0].Rows[0]["rig"].ToString();
                break;
            case "WIS":
                common.Prefix = "WIS-";
                common.Rig = ds.Tables[0].Rows[0]["facility"].ToString();
                break;
        }
        common.Headers = this.Headers;
        common.BaseUrl = this.BaseUrl;
        common.LessThanEncode = this.LessThanEncode;
        common.AmpEncode = this.AmpEncode;
        common.SelectedStatus = this.Status.SelectedValue;
        common.Start(this.Response, this.UserName);
    }
    void PreviewWord() {
        this.PreviewStart("Word");
    }
    void Convert() {
        SortedList sl = new SortedList();
        sl.Add("@id", this.id);
        ds = this.da.GetDataSet("usp_convert", sl);
        this.id = ds.Tables[0].Rows[0][0].ToString();
        if (this.WiType == "WI")
            this.WiType = "WIS";
        else
            this.WiType = "WIT";
        string url = "create.aspx?id=" + this.id + "&WiType=" + this.WiType;
        this.Response.Redirect( url);
    }
}
