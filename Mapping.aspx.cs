using System;
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
public partial class Mapping : WIBase {
    DataSet ds;
    string id;
    public string MappingXml {
        get {
            if (this.Session["MappingXml"] == null)
                return "";
            else
                return this.Session["MappingXml"].ToString();
        }
        set {
            this.Session["MappingXml"] = value;
        }
    }
    protected void Page_Load(object sender, EventArgs e) {
       // this.Trace.IsEnabled = true;
        this.id = this.Request.QueryString["id"];
        if (this.Request.QueryString["Action"] != null && this.Request.QueryString["Action"] == "Duplicate")
            this.Action.Value = "Duplicate";
        if (!this.IsPostBack) {
            this.MyInit();
        } else {
            this.Post();
        }
    }
    void Post() {
        string eventArg = this.Request["__EVENTARGUMENT"];
        switch (this.Request["__EVENTTARGET"]) {
            case "Create":
                this.Create(eventArg);
                break;
            default:
                break;
        }      
    }
    void Create(string xml) {
           //string xml = this.Request.QueryString["xml"];
           this.Session["xml"] = xml;
           this.Response.Redirect("Create.aspx");
    }
    void MyInit() {
        this.TreeView();
        if (this.Request.QueryString["mappingXml"] != null)
            this.PopulateMappingSelected();
        else {
            this.WICategory.DataSource = ds.Tables[1];
            this.WICategory.DataTextField = "name";
            this.WICategory.DataValueField = "id";
            this.WICategory.DataBind();
        }
    }
    void TreeView() {
        XDocument doc;
        XElement root;
        ds = this.da.GetDataSet("usp_getRigMapping");
        var RigTypes = (from r in ds.Tables[0].AsEnumerable()
                        select new {
                            RigTypeId = r.Field<int>("RigTypeId"),
                            RigRootId = r.Field<int>("RigRootId"),
                            Code = r.Field<string>("RigTypeCode"),
                            RigTypeName = r.Field<string>("RigTypeName")

                        }).Distinct();
        string s = "";//<img src=images/plus.gif /> <span ></span><span >";
        string xml = "";
        foreach (var RigType in RigTypes) {

            var RigDesigns = (from r in ds.Tables[0].AsEnumerable().Where(r => ((int)r["RigTypeId"]) == RigType.RigTypeId && (int)r["RigDesignId"] != 0)
                              select new {
                                  RigDesignId = r.Field<int>("RigDesignId"),
                                  RigDesignName = r.Field<string>("RigDesignName"),
                                  Code = r.Field<string>("RigDesignCode"),

                              }).Distinct();
            //  doc = XDocument.Parse(@"<RigType />");
            root = this.GetRoot("RigType");
            this.AddAttribute(root, "id", RigType.RigTypeId.ToString());
            this.AddAttribute(root, "name", RigType.RigTypeName);
            this.AddAttribute(root, "code", RigType.Code);
            if(s!="")
             s += " <br>";
            s+=this.Indent(5) + @"<img src=images/plus.gif />";
          s+=" <span Code='" + RigType.Code + "' RigRootId=" + RigType.RigRootId.ToString() + " xml=" + this.GetXml(root) + @"  >" + RigType.RigTypeName + "</span><span >";

            foreach (var RigDesign in RigDesigns) {
                var Rigs = (from r in ds.Tables[0].AsEnumerable().Where(r => ((int)r["RigDesignId"]) == RigDesign.RigDesignId)
                            select new {
                                RigId = r.Field<int>("RigId"),
                                RigName = r.Field<string>("RigName"),
                                Code = r.Field<string>("RigCode")

                            }).Distinct();
                root = this.GetRoot("RigDesign");
                this.AddAttribute(root, "id", RigDesign.RigDesignId.ToString());
                this.AddAttribute(root, "name", RigDesign.RigDesignName);
                this.AddAttribute(root, "code", RigDesign.Code);
                //s += " <br>" + this.Indent(14) + @"<img src=images/plus.gif /><span Code='" + RigDesign.Code + "'  xml=" + this.GetXml(root) + @" >" + RigDesign.RigDesignName + "</span>  <span >";
                s += " <br>" + this.Indent(14) + @"<img src=images/plus.gif />";
              s+=" <span Code='" + RigDesign.Code + "'  xml=" + this.GetXml(root) + @" >" + RigDesign.RigDesignName + "</span>  <span >";
               //s+=this.CreateNode("span",RigDesign.RigDesignName, new string[]{"Code", RigDesign.Code, "xml", this.GetXml(root )}   )+"<span>";
                foreach (var Rig in Rigs) {
                    root = this.GetRoot("Rig");
                    this.AddAttribute(root, "id", Rig.RigId.ToString());
                    this.AddAttribute(root, "name", Rig.RigName);
                    this.AddAttribute(root, "code", Rig.Code);
                    s += " <br>" + this.Indent(20) + "<span  Code='" + Rig.Code + "'  xml=" + this.GetXml(root) + @"> " + Rig.RigName + " </span>";
                   // s += " <br>" + this.Indent(20) + this.CreateNode("span", Rig.RigName, new string[] { "Code", Rig.Code, "xml", this.GetXml(root) });
                }
                s += "</span>";
            }
            s += "</span>";
        }
        s += "</span>";
        this.RigMapping.InnerHtml = s;
    }
    void PopulateMappingSelected() {
        string mappingXml;//= this.MappingXml;
        mappingXml = this.Request.QueryString["mappingXml"];
        mappingXml = HttpUtility.UrlDecode(mappingXml);
        mappingXml = HttpUtility.HtmlDecode(mappingXml);
        mappingXml = "<MappingXml >" + mappingXml + "</MappingXml >";
        mappingXml = mappingXml.Replace("&", "&amp;");//

        XDocument doc = XDocument.Parse(mappingXml);
        string s = "";
        foreach (XElement element in doc.Descendants("MappingXml").Descendants()) {
            string value = element.Attribute("name").Value.Replace("&amp;", "&");
            string xml= "<" + element.Name + @" id=""" + element.Attribute("id").Value + @""" code=""" + element.Attribute("code").Value + @""" name=""" + element.Attribute("name").Value + @""" />";
          //  xml = this.CreateNode(element.Name.ToString(), "", new string[] { "id", element.Attribute("id").Value, "code", element.Attribute("code").Value, "name", value });
            xml = HttpUtility.HtmlEncode(xml);
            xml = HttpUtility.UrlEncode(xml);
            xml = @"""" + xml + @"""";
            s += "<div code='" + element.Attribute("code").Value + "' xml=" + xml + " >" + element.Attribute("name").Value + " </div>";
           // s += this.CreateNode("div",value, new string[] { "code", element.Attribute("code").Value, "xml", xml });
        }
        this.MappingSelected.InnerHtml = s;
    }
    string CreateNode2(string elementName , string elementValue, string[] arr) {
        //elementValue = elementValue.Replace("&amp;", "&");
        XDocument doc = new XDocument(new XDeclaration("1.0", "utf-8", "yes"), new XElement(elementName, elementValue));
        for (int i = 0; i < arr.Length ; i+=2) {
            doc.Element( elementName).Add(new XAttribute( arr[i], arr[i+1]));
        }
        return doc.ToString();
    }
    string GetXml(XElement root) {
        string xml = root.ToString();
        xml = HttpUtility.HtmlEncode(xml);
        xml = HttpUtility.UrlEncode(xml);
        xml = @"""" + xml + @"""";
        return xml;
    }
    void AddAttribute(XElement element, string name, string value) {
        if (value!=null)
            element.Add(new XAttribute(name, value));
    }
    XElement GetRoot(string name) {
        XDocument doc = XDocument.Parse(@"<"+name+@" />");
       return  doc.Element(name);
    }
    string  Indent(int i) {
        while (i-- > 0) {
            return "&nbsp" + this.Indent(i - 1);
        }
         return "";
    }

    protected void Submit_Click(object sender, EventArgs e) {

    }
}