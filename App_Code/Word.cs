using System;
using System.Drawing;
using System.Web.UI.HtmlControls;
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
using Novacode;
using Header = Novacode.Header;
using Picture = Novacode.Picture;
using Footer = Novacode.Footer;
using Paragraph = Novacode.Paragraph;
using FontFamily = System.Drawing.FontFamily;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Vml;
using DocumentFormat.OpenXml.Vml.Office;
using DocumentFormat.OpenXml.Vml.Wordprocessing;
using DocumentFormat.OpenXml.Wordprocessing;
using Color = System.Drawing.Color;
using HorizontalAnchorValues = DocumentFormat.OpenXml.Vml.Wordprocessing.HorizontalAnchorValues;
using Lock = DocumentFormat.OpenXml.Vml.Office.Lock;
using VerticalAnchorValues = DocumentFormat.OpenXml.Vml.Wordprocessing.VerticalAnchorValues;
public class Word : PdfAndWord {
    string id;
    string JobDescriptionName;
    string RigList;
    string WICategoryName;
    float TotalWidth = 800f;
    float TotalHeight;
    DocX WordDoc;
    float Scale = 63;
    float Inch = 96f;
    float[] HeaderWidths = new float[] { 1.25f, 5f };
    FontFamily FF=new FontFamily("Arial");
    struct AttachmentImage {
        public string name;
        public int width;
        //public Novacode.Picture pic;
        public object pic;
        public AttachmentImage(string name, int width, object pic) {
            this.name = name;
            this.width = width;
            this.pic = pic;

        }
    }
    public override void Start(System.Web.HttpResponse Response, string userName) {
        Response.Clear();
        Response.AddHeader("content-disposition", "attachment; filename=" + this.GetFileName() + ".docx");
        Response.ContentType = "application/msword";

        this.WordDoc = DocX.Create(Response.OutputStream);
        this.WordDoc.PageLayout.Orientation = Novacode.Orientation.Landscape;
        float margin = .5f * this.Inch;
        this.WordDoc.MarginTop = margin;
        this.WordDoc.MarginLeft = margin;
        this.WordDoc.MarginRight = margin;
        this.WordDoc.MarginBottom = margin;
        this.TotalWidth = this.WordDoc.PageWidth - this.WordDoc.MarginLeft - this.WordDoc.MarginRight;
        this.TotalHeight = this.WordDoc.PageHeight - this.WordDoc.MarginTop - this.WordDoc.MarginBottom-280;
        this.CreateHeaderFooter(this.WordDoc);
        // this.CreatePDFHeader();
        this.CreateResources();
        this.CreatePrecaution();
        this.CreateCriticality();
        this.CreateJobStep();
        this.CreateAttachment();

        using (MemoryStream mem = new MemoryStream()) {
            WordDoc.SaveAs(mem);
            using (var wordDocument = WordprocessingDocument.Open(mem, true)) {
                AddWatermark(wordDocument, "Draft");
                this.ChangeFooterFontStyle(wordDocument);
                wordDocument.MainDocumentPart.Document.Save();
                mem.WriteTo(Response.OutputStream);
                Response.End();
            }
        }
    }
    void junk() {
        Novacode.Table t = this.CreateWordTable(this.HeaderWidths, false);
        Novacode.Paragraph p = t.Rows[0].Cells[0].Paragraphs.First();
        if (!true) {
            p.InsertText("test");
            p.InsertText(Environment.NewLine);
            p.InsertText("test");
            p.InsertText(Environment.NewLine);
        } else {
            p.Append("test");
            p.Append(Environment.NewLine);
            p.Append("test");
            p.Append(Environment.NewLine);
        }
    }
 
    void CreateHeaderFooter(Novacode.DocX doc) {
        doc.AddHeaders();
        doc.AddFooters();
        Novacode.Header header = doc.Headers.odd;                
        Footer footer = doc.Footers.odd;
        Novacode.Table t = header.InsertTable(1, 2);
        t.Design = TableDesign.None;
        string path =HttpContext.Current.Server.MapPath("images/ensco.png");
        Novacode.Image img = doc.AddImage(path);
        Picture pic = img.CreatePicture();
        pic.Width = Convert.ToInt16(1.85f * this.Inch);
        pic.Height = Convert.ToInt16(0.93f * this.Inch);
        t.Rows[0].Cells[0].Width = .5 * TotalWidth;
        t.Rows[0].Cells[1].Width = .5 * TotalWidth;
        t.Rows[0].Cells[1].VerticalAlignment = Novacode.VerticalAlignment.Center;
        t.Rows[0].Cells[0].Paragraphs.First().AppendPicture(pic);

        Novacode.Paragraph p = t.Rows[0].Cells[1].Paragraphs.First();
        p.Alignment = Alignment.right;
        p.Append("Work Instruction").FontSize(24).Bold().Color(this.deepblue).Font(this.FF);

        string wiNo = "Work Instruction No.";
        if (this.WiNo.StartsWith("WIT"))
            wiNo = "Work Instruction Template No.";
        DataRow dr = this.ds.Tables[0].Rows[0];
        t = this.CreateHeaderTable(header, this.HeaderWidths, 3 );
        this.AddLabelText(t, 0, 0, wiNo, "");
        this.AddLabelText(t, 0, 1, "", this.WiNo);
        this.AddLabelText(t, 1, 0, "Job Title:", "");
        this.AddLabelText(t, 1, 1, "", dr["jobDescriptionName"].ToString());
        this.AddLabelText(t, 2, 0, "Equipment Type/Make:", "");
        string s = dr["EquipmentTypeName"].ToString() + "/" + dr["EquipmentMakeName"].ToString();
        if (s.Trim() == "/")
            s = "";
        this.AddLabelText(t, 2, 1, "", s);
        //footer
        string footerText = "Confidential and Uncontrolled When Printed";
        //footer.InsertParagraph().Append(footerText);

        t = footer.InsertTable(2, 2);
        t.Design = TableDesign.None;
        t.Rows[0].Cells[0].Width = .5 * TotalWidth;
        t.Rows[0].Cells[1].Width = .5 * TotalWidth;
        t.Rows[0].Cells[0].Paragraphs.First().Append(footerText).Italic().FontSize(8).Font(this.FF);

        t.Rows[0].Cells[1].Paragraphs[0].Italic();
        p = t.Rows[0].Cells[1].Paragraphs.First();

        p.Append("Page ").FontSize(10).Font(this.FF).Italic().AppendPageNumber(PageNumberFormat.normal);
        p.Append(" of ").FontSize(10).Font(this.FF).Italic().AppendPageCount(PageNumberFormat.normal);
        p.Alignment = Alignment.right;
        
        footerText = "Last Updated On:" + ((DateTime)dr["revisionDate"]).ToShortDateString() + " By " + dr["revisedBy"].ToString();
        t.Rows[1].Cells[0].Paragraphs.First().Append(footerText).Italic().FontSize(8).Font(this.FF);
    }
    Novacode.Table CreateHeaderTable(Header header,  float[] widths, int rowCount = 1) {
        int colCount = widths.Length;
        Novacode.Table t = header.InsertTable(rowCount, colCount);
        t.Design = TableDesign.TableGrid;

        float total = 0;
        for (int i = 0; i < colCount; i++) {
            total += widths[i];
        }
        for (int i = 0; i < colCount; i++) {
            t.Rows[0].Cells[i].Width = widths[i] * TotalWidth / total;
            if (i == colCount - 1)
                t.Rows[0].Cells[i].Width = t.Rows[0].Cells[i].Width + 0;
        }        
        return t;
    }
    int  AdjustPicture(Picture pic) {
        int w = pic.Width;
        int h = pic.Height;
        bool isThin = true;
        int w1 =0;
        if (w * 1f / h > this.TotalWidth / this.TotalHeight)
            isThin = false;
        if ( isThin &&  h > this.TotalHeight) {
            pic.Height = Convert.ToInt32(this.TotalHeight);
            w1 = Convert.ToInt32(w * this.TotalHeight / h);
            pic.Width = w1;
        }
        if (!isThin &&  w> this.TotalWidth) {
            w1 = Convert.ToInt32(this.TotalWidth);
            pic.Width = Convert.ToInt32(this.TotalWidth);
            pic.Height = Convert.ToInt32(h* this.TotalWidth / w);
        }
        int min = 200;
        if (w1 < min) w1 = min;
        return w1;
    }
    void CreateAttachment() {
        this.TotalWidth -= 20f;
        DataTable dt = ds.Tables[2];
        Novacode.Paragraph p = this.WordDoc.InsertParagraph();
        int imageNo = 1;
        ArrayList list = new ArrayList();
        int totalWidth = 0;
        int indexGlobal = 0, indexLocal = 0;
        foreach (DataRow dr in dt.Rows) {
            string name = dr["Attachment"].ToString();
            string title = dr["title"].ToString();
            string prompt = dr["prompt"].ToString();
            int w=0;
            object o=null;
            //title = "Figure " + (imageNo++).ToString() + ": " + title;
            title = "Figure " + this.GetImageNumber(ref indexGlobal, ref indexLocal, dr["type"].ToString(), prompt) + ": " + title;
            string[] arr = { prompt, name };
            for (int i = 0; i < 2; i++) {
                if (i == 1) {
                    if (name == "")
                        continue;
                    Novacode.Image img;
                    if (name.ToLower().EndsWith(".pdf"))
                        img = this.WordDoc.AddImage(HttpContext.Current.Server.MapPath(@"images/pdf.png"));
                    else
                        img = this.WordDoc.AddImage(HttpContext.Current.Server.MapPath(@"upload/" + name));
                    Picture pic = img.CreatePicture();
                    w = this.AdjustPicture(pic);
                    o = pic;
                }
                if (i == 0) {
                    if (prompt == "")
                        continue;
                    w = 100;
                    o = prompt;
                }
                if (i == 1 && prompt != "" && name != "")
                    title = "";
                if (totalWidth + w > this.TotalWidth) {
                    if (list.Count == 0) {
                        //pic.Width =(int) this.TotalWidth;
                        AttachmentImage ai = new AttachmentImage();
                        ai.name = title;
                        ai.width = w;// pic.Width;
                        ai.pic = o;

                        list = new ArrayList();
                        this.AddAttachmentList(list, title, w, o);
                        this.DisplayAttachment(list);
                        list = new ArrayList();
                    } else {
                        this.DisplayAttachment(list);
                        list = new ArrayList();
                        this.AddAttachmentList(list, title, w, o);
                        totalWidth = w;
                    }
                } else {
                    totalWidth += w;
                    this.AddAttachmentList(list, title, w, o);
                }
            }
        }
        if (list.Count != 0)
            this.DisplayAttachment(list);
    }
    void CreateAttachment011616() {
        this.TotalWidth -= 20f;
        DataTable dt = ds.Tables[2];
        Novacode.Paragraph p = this.WordDoc.InsertParagraph();
        int imageNo = 1;
        ArrayList list = new ArrayList();
        int totalWidth = 0;
        foreach (DataRow dr in dt.Rows) {
            string name = dr["Attachment"].ToString();
            string title = dr["title"].ToString();
            string prompt = dr["prompt"].ToString();
            int w;
            object o;
            title = "Figure " + (imageNo++).ToString() + ": " + title;
            if (prompt == "") {
                Novacode.Image img;
                if (name.ToLower().EndsWith(".pdf"))
                    img = this.WordDoc.AddImage(HttpContext.Current.Server.MapPath(@"images/pdf.png"));
                else 
                    img = this.WordDoc.AddImage(HttpContext.Current.Server.MapPath(@"upload/" + name));
                Picture pic = img.CreatePicture();
                w = this.AdjustPicture(pic);
                o = pic;
            } else {
                w = 100;
                o = prompt;
            }
            if (totalWidth + w > this.TotalWidth) {
                if (list.Count == 0) {
                    //pic.Width =(int) this.TotalWidth;
                    AttachmentImage ai = new AttachmentImage();
                    ai.name = title;
                    ai.width = w;// pic.Width;
                    ai.pic = o;

                    list = new ArrayList();
                    this.AddAttachmentList(list, title, w, o);
                    this.DisplayAttachment(list);
                    list = new ArrayList();
                } else {
                    this.DisplayAttachment(list);
                    list = new ArrayList();
                    this.AddAttachmentList(list, title, w, o);
                    totalWidth = w;
                }
            } else {
                totalWidth += w;
                this.AddAttachmentList(list, title, w, o);
            }

        }
        if (list.Count != 0)
            this.DisplayAttachment(list);
    }
    void AddAttachmentList(ArrayList list, string name, int width,object pic) {
        AttachmentImage ai = new AttachmentImage(name, width, pic);
        list.Add(ai);
    }
    void DisplayAttachment(ArrayList arr) {
       this.WordDoc.InsertParagraph("");
        Novacode.Table table = WordDoc.AddTable(2, arr.Count);
        table.Alignment = Alignment.center;
        table.Design = TableDesign.LightShadingAccent2;
        Novacode.Table t = this.WordDoc.InsertTable(table);
        t.Design = TableDesign.None;
        for (int i = 0; i < arr.Count; i++) {
            AttachmentImage ai = (AttachmentImage)arr[i];
            t.Rows[0].Cells[i].Width = ai.width;
            t.Rows[0].Cells[i].VerticalAlignment = VerticalAlignment.Bottom;
            t.Rows[0].Cells[i].Paragraphs.First().Alignment = Alignment.center;

            Paragraph p = t.Rows[0].Cells[i].Paragraphs.First();
            if (ai.pic.GetType().Name=="Picture")
                p.AppendPicture((Picture) ai.pic);
            else
                p.Append((string)ai.pic).Italic().Color(Color.Blue);
            p = t.Rows[1].Cells[i].Paragraphs.First();
            p.InsertText(ai.name);
            p.Alignment = Alignment.center;
            t.Rows[1].BreakAcrossPages = false;
        }
    }   
    void CreatePDFHeader() {
        Novacode.Table t = this.CreateWordTable(new float[] { 5, 5 });

        DataRow dr = this.ds.Tables[0].Rows[0];
        this.AddLabelText(t, 0, 0, "Job Description", dr["jobDescriptionName"].ToString());
        this.AddLabelText(t, 0, 1, "Equipment Make/Type", dr["EquipmentMakeName"].ToString() + "/" + dr["EquipmentTypeName"].ToString());
        this.AddLabelText(t, 1, 0, "Status ", dr["Status"].ToString());
        this.AddLabelText(t, 1, 1, "Work Instruction No.", dr["wiNo"].ToString());

    }
    Novacode.Table CreateWordTable( float[] widths, bool insertSpace = true) {
        int colCount = widths.Length;
        if (insertSpace)
            this.WordDoc.InsertParagraph("");

        Novacode.Table table = WordDoc.AddTable(1, colCount);

        table.Alignment = Alignment.center;
        Novacode.Table t = this.WordDoc.InsertTable(table);
        t.Design = TableDesign.TableGrid;

        float total = 0;
        for (int i = 0; i < colCount; i++) {
            total += widths[i];
        }
        for (int i = 0; i < colCount; i++) {
            t.Rows[0].Cells[i].Width = widths[i] * TotalWidth / total;
            if (i == colCount - 1)
                t.Rows[0].Cells[i].Width = t.Rows[0].Cells[i].Width + 0;
        }
        //t.Rows[0].Cells[0].Width = 1;
        return t;
    }
    void CreateResourceLabel() {
        Novacode.Table t = this.CreateWordTable(new float[] { 1}, false);
        t.Design = TableDesign.None;
        this.AddLabelText(t, 0, 0, "Resources", "");
    }
    void CreateResources() {
        this.CreateResourceLabel();
        Novacode.Table t = this.CreateWordTable(this.HeaderWidths, false);
        this.AddJobStepHeader(t, 0, 0, "Type");
        this.AddJobStepHeader(t, 0,1, "Description");
        string[,] arr = new string[,] { { "Minimum Personnel Required", "3" }, { "Personal Protective Equipment", "8" }, { "Equipment/Tools", "4" } };
        for (int i = 0; i < arr.GetLength(0); i++) {
            this.AddLabelText(t, i + 1, 0, arr[i, 0], "");
            this.AddLabelText(t, i + 1, 1, "", this.GetHeader2List(arr[i, 1]));
        }
    }
    void CreatePrecaution() {
        Novacode.Table t = this.CreateWordTable( this.HeaderWidths);
        DataRow dr = this.ds.Tables[0].Rows[0];
        this.AddLabelText(t, 0, 0, "General Precautions", "");
        this.AddLabelTextRich(t, 0, 1, "", dr["GeneralPrecautions"].ToString());
        this.AddLabelText(t, 1, 0, "Local Precautions", "");
        this.AddLabelTextRich(t, 1, 1, "", dr["LocalPrecautions"].ToString());

        Novacode.Border border = new Novacode.Border();
        border.Color = System.Drawing.Color.Red;
        border.Size = BorderSize.one;

        t.SetBorder(TableBorderType.Top, border);
        t.SetBorder(TableBorderType.Bottom, border);
        t.SetBorder(TableBorderType.Left, border);
        t.SetBorder(TableBorderType.Right, border);
        t.SetBorder(TableBorderType.InsideV, border);
        t.SetBorder(TableBorderType.InsideH, border);
    }
    void CreateCriticality() {
        DataRow dr = this.ds.Tables[0].Rows[0];
        Novacode.Table t = this.CreateWordTable(new float[] { 1.25f, 1, 2.5f, 1.5f });

        this.AddLabelText(t, 0, 0, "Job Criticality", dr["Criticality"].ToString());
        this.AddLabelText(t, 0, 1, "Permit Required", dr["Permit"].ToString()=="True"?"Yes":"No");
        this.AssociatedWorkInstruction(t, 0, 2);
        this.AssociatedLink(t, 0, 3);
    }
    void AddJobStepHeader(Novacode.Table t, int i, int j, string name )  {
        if (t.Rows.Count <= i) 
            t.InsertRow();
        Novacode.Paragraph p = t.Rows[i].Cells[j].Paragraphs.First();
        t.Rows[i].Cells[j].MarginLeft = 3;
        t.Rows[i].Cells[j].MarginRight = 0;
        p.Append(name).Bold().FontSize(10).Font(this.FF);
        t.Rows[i].Cells[j].FillColor = Color.LightGray;
    }
    void CreateJobStep() {
        float[] widths = new float[5];
        widths[0] = widths[1]= .6f * this.Inch ;
        widths[3] = 2.26f * this.Inch;
        widths[4] = .6f * this.Inch;
        widths[2] = this.TotalWidth - 2 * widths[0] - widths[3] - widths[4];
        Novacode.Table t = this.CreateWordTable( widths);
        int index = 0;
        foreach (string header in this.Headers)
            this.AddJobStepHeader(t, 0, index++, header);
        DataTable dt = ds.Tables[1];
        int globalIndex = 0, localIndex = 0, standaloneIndex = 0;
        int rowNumber = 1;
        int rowNumber0 = 1;
        int rowSpan = 0;
        foreach (DataRow dr in dt.Rows) {
            rowNumber += this.CreateJobStep2(t, rowNumber, dr, "Warning", dr["type"].ToString());
            if (dr["type"].ToString() == "GlobalStep") {
                globalIndex++;
                localIndex = 0;
                this.AddStepNumber(t, rowNumber, 0,  globalIndex.ToString());
                //this.AddCell(t);
            } else {
                string s = "";
                if (dr["type"].ToString() == "LocalStep") {
                    localIndex++;
                    s= globalIndex.ToString() + "." + localIndex.ToString();
                } else {
                    standaloneIndex++;
                    char ch = (char)(standaloneIndex + 64);
                    s = ch.ToString();
                }
                this.AddStepNumber(t, rowNumber, 1, s) ;
            }
            rowSpan = rowNumber - rowNumber0;
            //this.AddLabelText(t, rowNumber, 2,   "", dr["description"].ToString());
            this.AddImageLabelTextDescriptionAndNote(t, rowNumber, 2, dr);
            this.AddPhoto(t, rowNumber, dr);
            string barrier = dr["barrier"].ToString();
            if (barrier == "Yes") {
                Novacode.Image img = this.WordDoc.AddImage(HttpContext.Current.Server.MapPath(@"images/barrier.png"));

                Picture pic = img.CreatePicture();
                int w = pic.Width;
                int h = pic.Height;
                pic.Height = 40;
                pic.Width = 18;
                t.Rows[rowNumber].Cells[4].MarginLeft = 0.1;
                t.Rows[rowNumber].Cells[4].Paragraphs.First().AppendPicture(pic);
            }
            rowNumber++;
        }
        foreach (Row row in t.Rows) {
            for (int j = 0; j < row.Cells.Count; j++) {
                if (row.Cells.Count == 5)
                    for (int i = 0; i < 5; i++)
                        row.Cells[i].Width = t.Rows[0].Cells[i].Width;
                else {
                    row.Cells[0].Width = t.Rows[0].Cells[0].Width * 2;
                    for (int i = 0; i < 3; i++)
                        row.Cells[i + 1].Width = t.Rows[0].Cells[i + 2].Width;
                }
            }
        }
    }
    void AddStepNumber(Novacode.Table t, int i, int j, string name ) {
        if (t.Rows.Count <= i) t.InsertRow();
        Novacode.Paragraph p = t.Rows[i].Cells[j].Paragraphs.First();
        p.Append(name       ).Font(this.FF).FontSize(10);
        p.Alignment = Alignment.center;
        t.Rows[i].Cells[j].VerticalAlignment = VerticalAlignment.Center;
    }
    void AddPhoto(Novacode.Table t, int rowNumber,  DataRow dr ) {
        string photo = dr["photo"].ToString();
        string prompt = dr["prompt"].ToString();
        Paragraph p = t.Rows[rowNumber].Cells[3].Paragraphs.First();
        if (prompt != "") {
            p.Append(prompt).Font(this.FF).Italic().Color(Color.Blue);
        }else   if (photo != "") {
            Novacode.Image img = this.WordDoc.AddImage(HttpContext.Current.Server.MapPath(@"upload/" + photo));

            Picture pic = img.CreatePicture();
            int w = pic.Width;
            int h = pic.Height;
            float ratio = w * 1f / h;
            if (ratio > 2.2 / 1.5)
                ratio = 2.2f / 1.5f;
            pic.Height = Convert.ToInt32(1.5 * this.Inch);
            pic.Width = Convert.ToInt32(1.5 * this.Inch * ratio);

            p.AppendPicture(pic);
        }
    }
    int CreateJobStep2(Novacode.Table t, int rowNumber, DataRow dr, string name, string stepType) {
        string value = dr[name].ToString();
        string warning = dr["warning"].ToString();
        string caution = dr["Caution"].ToString();
        string hazard = dr["hazard"].ToString();
        int j = 1;
        if (stepType == "GlobalStep")
            j = 0;
        if (warning != "" || caution!="") {
            if (hazard != "") 
                this.AddHazardImage(t, rowNumber,j, hazard);
            this.AddImageLabelTextWarningAndCaution(t, rowNumber, 2, dr);
            return 1;
        } else
            return 0;
    }
    bool IsNoteEmpty(string value) {
        string s;// = @"<ul> <li><span >test</span></li> <li><span >testl</span></li> <li></li> </ul> <pre>test<br />test<br />test</pre> <p></p>";
        s = "<r>" + value + "</r>";
        s = HttpUtility.HtmlDecode(s);
        XDocument doc = XDocument.Parse(s);

        var nodes = doc.DescendantNodes().Where(x => x.NodeType == XmlNodeType.Text);
        if (nodes.Count() == 0)
            return true;

        return false;
    }

    void AddImageLabelTextDescriptionAndNote(Novacode.Table t, int i, int j, DataRow dr, int colSpan = 1) {
        if (t.Rows.Count <= i) t.InsertRow();
        if (colSpan > 1)
            t.Rows[i].MergeCells(j, j + colSpan - 1);
        int rowCount = 1;
        t.Rows[i].Cells[j].MarginLeft = 0;
        t.Rows[i].Cells[j].MarginRight = 0;
        bool noteEmpty = this.IsNoteEmpty(dr["Note"].ToString().Trim());
        if (!noteEmpty)
            rowCount++;
        Novacode.Table t2 = t.Rows[i].Cells[j].InsertTable(0, rowCount, 1);
        t2.Rows[0].Cells[0].MarginLeft = 5;
        t2.Rows[0].Cells[0].MarginTop = 5;
        t2.Design = TableDesign.None;

        string[] arr = { "Description", "Note" };
        int k = 0;
        foreach (string label in arr) {
            string value = dr[label].ToString();
            //if (value.Trim() == "") continue;
            if (k == 1 && noteEmpty) continue;
            t2.Rows[k].Cells[0].Width = t.Rows[0].Cells[2].Width;

            Novacode.Paragraph p = t2.Rows[k].Cells[0].Paragraphs.First();
            if (k == 0) {
                this.ParseRichText(t2.Rows[k].Cells[0], value, true );
                //p.Append(value);
                k++;
                continue;
            }
            t2.Rows[1].Cells[0].MarginLeft = 2;
            t2.Rows[1].Cells[0].MarginRight = 0;
            Novacode.Table t3 = t2.Rows[1].Cells[0].InsertTable(0, 1,2);
           t3.Design = TableDesign.None;
            t3.Rows[0].Cells[0].Width = t.Rows[0].Cells[2].Width * 0.01;
            t3.Rows[0].Cells[1].Width = t.Rows[0].Cells[2].Width * 0.99;
            //t3.Rows[0].Cells[0].MarginLeft = 2;
            //t3.Rows[0].Cells[0].MarginTop = 2;
            //t3.Rows[0].Cells[0].MarginRight = 0;
            //t3.Rows[0].Cells[1].MarginLeft = 0;
            //t3.Rows[0].Cells[1].MarginRight = 0;
            this.SetTableIndent(t3);
            Novacode.Image img = this.WordDoc.AddImage(HttpContext.Current.Server.MapPath(@"images/" + label + ".png"));
            Picture pic = img.CreatePicture();
            int size = 40;
            pic.Width = size;
            pic.Height = size;
            p = t3.Rows[0].Cells[0].Paragraphs.First();
            p.AppendPicture(pic);
            p = t3.Rows[0].Cells[1].Paragraphs.First();
            this.ParseRichText(t3.Rows[0].Cells[1], value, true);
        }
    }
    void AddImageLabelTextDescriptionAndNote___112215(Novacode.Table t, int i, int j, DataRow dr, int colSpan = 1) {
        if (t.Rows.Count <= i)  t.InsertRow();
        if (colSpan > 1)
            t.Rows[i].MergeCells(j, j + colSpan - 1);
        int rowCount = 1;

        bool noteEmpty = this.IsNoteEmpty(dr["Note"].ToString().Trim());
        if (!noteEmpty)
            rowCount++;
        Novacode.Table t2 = t.Rows[i].Cells[j].InsertTable(0, rowCount, 2);
        t2.Design = TableDesign.None;

        string[] arr = { "Description", "Note" };
        int k = 0;
        foreach (string label in arr) {
            string value = dr[label].ToString();
            if (k == 1 && noteEmpty) continue;
            t.Rows[i].Cells[j].MarginLeft = 0;
            t.Rows[i].Cells[j].MarginRight = 0;
            t2.Rows[k].Cells[0].Width = t.Rows[0].Cells[2].Width * 0.01;
            t2.Rows[k].Cells[1].Width = t.Rows[0].Cells[2].Width * 0.9;

            Novacode.Paragraph p = t2.Rows[k].Cells[0].Paragraphs.First();
            Novacode.Formatting labelFormatting = new Novacode.Formatting();

            if (k == 0) {
                this.ParseRichText(t2.Rows[k].Cells[0], value, true);
                t2.Rows[0].MergeCells(0, 1);
                t2.Rows[k].Cells[0].Width = t.Rows[0].Cells[2].Width;
                k++;
                continue;
            }
            labelFormatting.Bold = true;
            Novacode.Image img = this.WordDoc.AddImage(HttpContext.Current.Server.MapPath(@"images/" + label + ".png"));
            Picture pic = img.CreatePicture();
            int size = 40;
            pic.Width = size;
            pic.Height = size;
            p.AppendPicture(pic);

           // p = t2.Rows[k++].Cells[1].Paragraphs.First();
            this.ParseRichText(t2.Rows[k++].Cells[1], value, true);
        }
    }
    void SetIndent(Novacode.Table t, int i, int j) {
        t.Rows[i].Cells[j].MarginLeft = .5f;
    }
    void SetTableIndent(Novacode.Table t) {
        int i = 2;
        foreach(Row row in t.Rows) {
            foreach(Cell cell in row.Cells) {
                cell.MarginLeft =i;
                cell.MarginRight = 0;
                cell.MarginTop = i;
                cell.MarginBottom = 0;
            }
        }
    }
    void AddImageLabelTextWarningAndCaution(Novacode.Table t, int i, int j, DataRow dr, int colSpan = 1) {
        if (t.Rows.Count <= i) 
            t.InsertRow();
        this.SetIndent(t, i, j);
        if (colSpan > 1)
            t.Rows[i].MergeCells(j, j + colSpan - 1);
        int rowCount = 0;
        if (dr["Warning"].ToString().Trim() != "")
            rowCount++;
        if (dr["Caution"].ToString().Trim() != "")
            rowCount++;
        if (rowCount == 0) return;
        Novacode.Table t2 = t.Rows[i].Cells[j].InsertTable(0, rowCount, 2);
        t2.Design = TableDesign.None;
        this.SetTableIndent(t2);
        string[] arr = { "Warning", "Caution" };
        int k = 0;
        foreach (string label in arr) {
            string value = dr[label].ToString();
            if (value.Trim() == "") continue;
            t.Rows[i].Cells[j].MarginLeft = 0;
            t.Rows[i].Cells[j].MarginRight = 0;
            t2.Rows[k].Cells[0].Width = t.Rows[0].Cells[2].Width * 0.01;
            t2.Rows[k].Cells[1].Width = t.Rows[0].Cells[2].Width * 0.9;

            Novacode.Paragraph p = t2.Rows[k].Cells[0].Paragraphs.First();
            Novacode.Formatting labelFormatting = new Novacode.Formatting();
            labelFormatting.Bold = true;
            Novacode.Image img = this.WordDoc.AddImage(HttpContext.Current.Server.MapPath(@"images/" + label + ".png"));
            Picture pic = img.CreatePicture();
            int size = 40;
            pic.Width = size;
            pic.Height = size;
            p.AppendPicture(pic);

            p = t2.Rows[k++].Cells[1].Paragraphs.First();
            string[] arr1 = value.Split('\n');
            foreach (string s in arr1) {
                if (s.Trim() != "") {
                    p.Append(label + ": ").Bold().FontSize(10).Font(this.FF);
                    p.Append(s).Font(this.FF).FontSize(10);
                }
                p.Append(Environment.NewLine);
            }
        }
    }
    void AddImageLabelText(Novacode.Table t, int i, int j, string label, string value, int colSpan = 1) {
        if (t.Rows.Count <= i) {
            t.InsertRow();
        }
        if (colSpan > 1)
            t.Rows[i].MergeCells(j, j + colSpan - 1);

        Novacode.Table t2 = t.Rows[i].Cells[j].InsertTable(0, 1, 2);
        t.Rows[i].Cells[j].MarginLeft = 0;
        t.Rows[i].Cells[j].MarginRight = 0;
        t2.Design = TableDesign.None;
        t2.Rows[0].Cells[0].Width = t.Rows[0].Cells[2].Width * 0.01;
        t2.Rows[0].Cells[1].Width = t.Rows[0].Cells[2].Width * 0.9;

        Novacode.Paragraph p = t2.Rows[0].Cells[0].Paragraphs.First();
        Novacode.Formatting labelFormatting = new Novacode.Formatting();
        labelFormatting.Bold = true;
        Novacode.Image img = this.WordDoc.AddImage(HttpContext.Current.Server.MapPath(@"images/" + label + ".png"));
        Picture pic = img.CreatePicture();
        int size = 40;
        pic.Width = size;
        pic.Height = size;
        p.AppendPicture(pic);

        p = t2.Rows[0].Cells[1].Paragraphs.First();
        string[] arr = value.Split('\n');
        foreach (string s in arr) {
            if (s.Trim() != "") {
                p.Append(label + ": ").Bold();
                p.Append(s);
            }
            p.Append(Environment.NewLine);
        }

    }
    void AddLabelTextRich(Novacode.Table t, int i, int j, string label, string value, int colSpan = 1) {
        if (t.Rows.Count <= i) {
            t.InsertRow();
        }
        if (colSpan > 1)
            t.Rows[i].MergeCells(j, j + colSpan - 1);
        Novacode.Paragraph p = t.Rows[i].Cells[j].Paragraphs.First();
        if (label == "Type") {
            p.Color(System.Drawing.Color.LightGray);

        }
        if (label != "") {
            p.Append(label).Bold();
            p.InsertText("  ");
        }
        this.ParseRichText(t.Rows[i].Cells[j], value, true);
    }
    void ParseRichText(Cell cell,  string value, bool bDecode=false) {
        string s;
        cell.MarginLeft = 1;
        Novacode.Paragraph p = cell.Paragraphs.First();
        if (value.Trim() == "")
            return;
        s = "<r>" + value + "</r>";
        string lt = "$fdsfds$$%%%";
        string amp = "$fds$$%fds%%";
        if (!bDecode) {
            s = s.Replace("&lt;", lt);
            s = s.Replace("&amp;", amp);
        }
        s = HttpUtility.HtmlDecode(s);
        if (!bDecode) {
            s = s.Replace(lt, "&lt;");
            s = s.Replace(amp, "&amp;");
        }
        s = s.Replace(this.LessThanEncode, "&lt;");
        s = s.Replace(this.AmpEncode, "&amp;");
        s = s.Replace("<br />", Environment.NewLine);
        XDocument doc = XDocument.Parse(s);
        if (doc.Root.Elements().Count() == 0)
            p.Append(s).Font(this.FF).FontSize(10);
        else
            this.ParseRichTextRecursive(cell, doc.Root);
    }
    void ParseRichTextRecursive(Cell cell, XElement el) {
        Novacode.Paragraph p = cell.Paragraphs.First();
        foreach (XNode node in el.Nodes()) {
            if (node.NodeType.ToString() != "Element") {
                //  p.InsertText(node.ToString());
                //+ Environment.NewLine);
            } else {
                XElement x = XElement.Parse(node.ToString());
                switch (x.Name.ToString()) {
                    case "p":
                    case "pre":
                    case "div":
                        var nodes = x.DescendantNodes().Where(xx => xx.NodeType == XmlNodeType.Text);
                        //if (nodes.Count() != 0)
                        //    p.InsertText(Environment.NewLine);
                        if (x.Name.ToString() == "p" && p.Text.Trim() != "" && nodes.Count()>0)
                            p.Append("\n");

                        foreach (XNode n in nodes) {
                            string s = n.ToString();
                            s = HttpUtility.HtmlDecode(s);
                            p.Append(s).Font(this.FF).FontSize(10);
                        }
                        this.ParseRichTextRecursive(cell, x);
                        break;
                    case "ul":
                    case "ol":
                        this.GetLi(p, x);
                        break;
                    case "table":
                        this.GetTable(cell, x);
                        break;
                    default:
                        var nodes2 = x.Nodes().Where(xx => xx.NodeType == XmlNodeType.Text);
 
                        break;
                }
            }
        }
    }
    void GetTable(Cell cell,  XElement el) {
        int i = 0, colCount = 0;
        foreach (XElement tr in el.Elements("tbody").ElementAt(0).Elements("tr")) {
            int j = 0;
            foreach (XElement td in tr.Elements("td")) {
                j++;
            }
            if (colCount < j)
                colCount = j;
        }
        cell.RemoveParagraphAt(0);
        Novacode.Table t = cell.InsertTable (1,  colCount);
        t.Design = TableDesign.None ;

        foreach (XElement tr in el.Elements("tbody").ElementAt(0).Elements("tr")) {
            t.InsertRow();
            int j=0;
            foreach (XElement td in tr.Elements("td")) {
                var nodes = td.DescendantNodes().Where(xx => xx.NodeType == XmlNodeType.Text);
                foreach (XNode n in nodes) {
                    string s = n.ToString();
                    s = HttpUtility.HtmlDecode(s) + colCount.ToString();
                    t.Rows[i].Cells[j].Width = 2000;
                    Novacode.Paragraph p2 =t.Rows[i].Cells[j].Paragraphs.First();
                    p2.Append(s).Font(this.FF).FontSize(10);
                }
                j++;
            }
            i++;
        }
    }
    void GetPre(Novacode.Paragraph p, XElement el) {        
        foreach (XNode e in el.Nodes()) {
            if (e.NodeType.ToString() != "Element")
                p.InsertText(e.ToString() + Environment.NewLine);
        }
    }
    void GetLi(Novacode.Paragraph p, XElement el) {
        int index = 1;
        string s = "";
        foreach (XElement e in el.Elements("li")) {
            p.InsertText(Environment.NewLine);
            if (el.Name.ToString() == "ul")
                p.Append("\x2022").Append(" ").FontSize(10);
            if (el.Name.ToString() == "ol")
                p.Append((index++).ToString() + ". ").Font(this.FF).FontSize(10);
            this.GetStrong(p, e);

        }
        p.InsertText(Environment.NewLine);

    }
    void GetStrong(Novacode.Paragraph p, XElement el) {

        foreach (XNode e in el.Nodes()) {
            if (e.NodeType.ToString() != "Element")
                p.Append(e.ToString()).Font(this.FF).FontSize(10);
                //p.InsertText(e.ToString());
            else {
                string value = XElement.Parse(e.ToString()).Value;
                switch (XElement.Parse(e.ToString()).Name.ToString()) {
                    case "strong":
                        p.Append(value).Bold().Font(this.FF).FontSize(10);
                        break;
                    case "em":
                        p.Append(value).Italic().Font(this.FF).FontSize(10);
                        break;

                    default:
                        p.Append(value).Font(this.FF).FontSize(10);
                        //p.InsertText(value);
                        break;
                }
            }
        }
    }
    void AddLabelText(Novacode.Table t, int i, int j, string label, string value, int colSpan = 1) {
        if (t.Rows.Count <= i) {
            t.InsertRow();
        }
        if (colSpan > 1)
            t.Rows[i].MergeCells(j, j + colSpan - 1);
        Novacode.Paragraph p = t.Rows[i].Cells[j].Paragraphs.First();
        if (label == "Type") {
            p.Color(System.Drawing.Color.LightGray);

        }
        if (label != "") {
            p.Append(label).Bold().Font(this.FF).FontSize(10) ;
            p.InsertText("  ");
        }
        p.Append(value).Font(this.FF).FontSize(10).SetLineSpacing(LineSpacingType.Before, 0.2f);
        t.Rows[i].Cells[j].MarginLeft = 1.4f;

    }
    void AddHazardImage(Novacode.Table t, int rowNumber, int colNumber, string value) {
        if (t.Rows.Count <= rowNumber) t.InsertRow();
        string[] arr = value.Split(',');
        int index = 0;
        Novacode.Paragraph p = t.Rows[rowNumber].Cells[colNumber].Paragraphs.First();
        foreach (string str in arr) {
            Novacode.Image img = this.WordDoc.AddImage(HttpContext.Current.Server.MapPath(@"images/hazard/" + str));
            Picture pic = img.CreatePicture();
            int size = 40;
            pic.Width = size;
            pic.Height = size;
            if (index++ % 2 == 0) {
                //p = t.Rows[rowNumber].Cells[colNumber].InsertParagraph();
            }
            p.AppendPicture(pic);
        }
        t.Rows[rowNumber].Cells[colNumber].VerticalAlignment = VerticalAlignment.Center;        
        //double w = t.Rows[rowNumber].Cells[0].Width;
        //t.Rows[rowNumber].MergeCells(0, 1);
        //t.Rows[rowNumber].Cells[0].Width = 2 * w;   
    }
    void AddImage(Novacode.Table t, int rowNumber, int colNumber, string value) {
        if (t.Rows.Count <= rowNumber)
            t.InsertRow();

        Novacode.Image img = this.WordDoc.AddImage(HttpContext.Current.Server.MapPath(@"images/" + value));
        Picture pic = img.CreatePicture();
        t.Rows[rowNumber].Cells[colNumber].Paragraphs.First().AppendPicture(pic);
    }
    string GetHeader2ListResoruces(int index) {
        ArrayList list = new ArrayList();
        ArrayList list0 = new ArrayList();
        string s = "";
        int prevRowId = -1, curRowId = 0;
        foreach (DataRow dr in ds.Tables[index].Rows) {
            curRowId = int.Parse(dr["row"].ToString());
            if (curRowId != prevRowId) {
                if (prevRowId != -1) {
                    string[] arr0 = (string[])list.ToArray(Type.GetType("System.String"));
                    list0.Add(string.Join(" / ", arr0));
                    list.Clear();

                }
                prevRowId = curRowId;
            }
            s = "" + dr["name"];
            if (dr.Table.Columns.Contains("other") && dr["other"].ToString().Trim() != "undefined" && dr["other"].ToString().Trim() != "") {
                s = dr["other"] + "";
            }
            s += "(" + dr["number"] + ")";
            list.Add(s);
        }
        if (list.Count > 0)
            list0.Add(string.Join(" / ", (string[])list.ToArray(Type.GetType("System.String"))));

        string[] arr = (string[])list0.ToArray(Type.GetType("System.String"));
        return string.Join(", ", arr);
    }
    string GetHeader2List(string index) {
        int i = int.Parse(index);
        if (i == 3)
            return this.GetHeader2ListResoruces(i);
        ArrayList list = new ArrayList();
        string  s = "";
        foreach (DataRow dr in ds.Tables[i].Rows) {
            s = "" + dr["name"];
            if (dr.Table.Columns.Contains("other") && dr["other"].ToString().Trim() != "undefined" && dr["other"].ToString().Trim() != "") {
                s = dr["other"].ToString() ;
            }
            if (dr.Table.Columns.Contains("number")) {
                s += "(" + dr["number"] + ")";
            }
            list.Add(s);
        }
        string[] arr = (string[])list.ToArray(Type.GetType("System.String"));
        return string.Join(", ", arr);
    }
    void AssociatedWorkInstruction(Novacode.Table t, int i, int j)  {
        Novacode.Paragraph p = t.Rows[i].Cells[j].Paragraphs.First();
        Novacode.Image img = this.WordDoc.AddImage(HttpContext.Current.Server.MapPath(@"images/AssociatedWI_Icon.gif"));
        Picture pic = img.CreatePicture();
        p.InsertPicture(pic);
        p.Append(" Associated Work Instruction: ").Bold().Font(this.FF).FontSize(10);
        foreach (DataRow dr in ds.Tables[5].Rows) {
            string title = dr["name"].ToString();
            string id = dr["id"].ToString();
            string url = HttpContext.Current.Request.Url.ToString();
            string[] arr = url.Split(new char[] { '?' });
            string wiType = title.Substring(0, 3);
            url = arr[0] + "?id=" + id + "&WiType=" + wiType.Replace("-", "");

            Novacode.Hyperlink h = this.WordDoc.AddHyperlink(title, new Uri(url));
            p.Append("\n");
            p.AppendHyperlink(h).Font(this.FF).FontSize(8).Color(Color.Blue);
        }
    }
    void AssociatedLink(Novacode.Table t, int i, int j) {
        Novacode.Paragraph p = t.Rows[i].Cells[j].Paragraphs.First();
        Novacode.Image img = this.WordDoc.AddImage(HttpContext.Current.Server.MapPath(@"images/AssociatedWI_Icon.gif"));
        Picture pic = img.CreatePicture();
        p.InsertPicture(pic);
        p.Append(" Associated Link: ").Bold().Font(this.FF).FontSize(10);
        foreach (DataRow dr in ds.Tables[10].Rows) {
            string title = dr["title"].ToString();
            string link = dr["link"].ToString();
            Novacode.Hyperlink h = this.WordDoc.AddHyperlink(title, new Uri(link));
            p.Append("\n");
            p.AppendHyperlink(h).Font(this.FF).FontSize(8).Color(Color.Blue) ;
        }
    }

    private void AddWatermark(WordprocessingDocument doc, string txtWatermark) {
        if (doc.MainDocumentPart.HeaderParts.Count() == 0) {
            doc.MainDocumentPart.DeleteParts(doc.MainDocumentPart.HeaderParts);
            var newHeaderPart = doc.MainDocumentPart.AddNewPart<HeaderPart>();
            var rId = doc.MainDocumentPart.GetIdOfPart(newHeaderPart);
            var headerRef = new HeaderReference();
            headerRef.Id = rId;
            var sectionProps = doc.MainDocumentPart.Document.Body.Elements<SectionProperties>().LastOrDefault();
            if (sectionProps == null) {
                sectionProps = new SectionProperties();
                doc.MainDocumentPart.Document.Body.Append(sectionProps);
            }
            sectionProps.RemoveAllChildren<HeaderReference>();
            sectionProps.Append(headerRef);

            newHeaderPart.Header = MakeHeader();
            newHeaderPart.Header.Save();
        }

        foreach (HeaderPart headerPart in doc.MainDocumentPart.HeaderParts) {
            var sdtBlock1 = new SdtBlock();
            var sdtProperties1 = new SdtProperties();
            var sdtId1 = new SdtId() { Val = 87908844 };
            var sdtContentDocPartObject1 = new SdtContentDocPartObject();// DocPartObjectSdt();
            var docPartGallery1 = new DocPartGallery() { Val = "Watermarks" };
            var docPartUnique1 = new DocPartUnique();
            sdtContentDocPartObject1.Append(docPartGallery1);
            sdtContentDocPartObject1.Append(docPartUnique1);
            sdtProperties1.Append(sdtId1);
            sdtProperties1.Append(sdtContentDocPartObject1);

            var sdtContentBlock1 = new SdtContentBlock();
            var paragraph2 = new DocumentFormat.OpenXml.Wordprocessing.Paragraph() {
                RsidParagraphAddition = "00656E18",
                RsidRunAdditionDefault = "00656E18"
            };
            var paragraphProperties2 = new ParagraphProperties();
            var paragraphStyleId2 = new ParagraphStyleId() { Val = "Header" };
            paragraphProperties2.Append(paragraphStyleId2);
            var run1 = new DocumentFormat.OpenXml.Wordprocessing.Run();
            var runProperties1 = new RunProperties();
            var noProof1 = new NoProof();
            var languages1 = new Languages() { EastAsia = "zh-TW" };
            runProperties1.Append(noProof1);
            runProperties1.Append(languages1);
            var picture1 = new DocumentFormat.OpenXml.Wordprocessing.Picture();
            var shapetype1 = new Shapetype() {
                Id = "_x0000_t136",
                CoordinateSize = "21600,21600",
                OptionalNumber = 136,
                Adjustment = "10800",
                EdgePath = "m@7,l@8,m@5,21600l@6,21600e"
            };
            var formulas1 = new Formulas();
            var formula1 = new Formula() { Equation = "sum #0 0 10800" };
            var formula2 = new Formula() { Equation = "prod #0 2 1" };
            var formula3 = new Formula() { Equation = "sum 21600 0 @1" };
            var formula4 = new Formula() { Equation = "sum 0 0 @2" };
            var formula5 = new Formula() { Equation = "sum 21600 0 @3" };
            var formula6 = new Formula() { Equation = "if @0 @3 0" };
            var formula7 = new Formula() { Equation = "if @0 21600 @1" };
            var formula8 = new Formula() { Equation = "if @0 0 @2" };
            var formula9 = new Formula() { Equation = "if @0 @4 21600" };
            var formula10 = new Formula() { Equation = "mid @5 @6" };
            var formula11 = new Formula() { Equation = "mid @8 @5" };
            var formula12 = new Formula() { Equation = "mid @7 @8" };
            var formula13 = new Formula() { Equation = "mid @6 @7" };
            var formula14 = new Formula() { Equation = "sum @6 0 @5" };

            formulas1.Append(formula1);
            formulas1.Append(formula2);
            formulas1.Append(formula3);
            formulas1.Append(formula4);
            formulas1.Append(formula5);
            formulas1.Append(formula6);
            formulas1.Append(formula7);
            formulas1.Append(formula8);
            formulas1.Append(formula9);
            formulas1.Append(formula10);
            formulas1.Append(formula11);
            formulas1.Append(formula12);
            formulas1.Append(formula13);
            formulas1.Append(formula14);
            var path1 = new DocumentFormat.OpenXml.Vml.Path() {
                AllowTextPath = true,
                ConnectionPointType = ConnectValues.Custom,
                ConnectionPoints = "@9,0;@10,10800;@11,21600;@12,10800",
                ConnectAngles = "270,180,90,0"
            };
            var textPath1 = new TextPath() {
                On = true,
                FitShape = true
            };
            var shapeHandles1 = new DocumentFormat.OpenXml.Vml.ShapeHandles();

            var shapeHandle1 = new DocumentFormat.OpenXml.Vml.ShapeHandle() {
                Position = "#0,bottomRight",
                XRange = "6629,14971"
            };

            shapeHandles1.Append(shapeHandle1);

            var lock1 = new Lock {
                Extension = ExtensionHandlingBehaviorValues.Edit,
                TextLock = true,
                ShapeType = true
            };

            shapetype1.Append(formulas1);
            shapetype1.Append(path1);
            shapetype1.Append(textPath1);
            shapetype1.Append(shapeHandles1);
            shapetype1.Append(lock1);
            var shape1 = new Shape() {
                Id = "PowerPlusWaterMarkObject357476642",
                Style = "position:absolute;left:0;text-align:left;margin-left:0;margin-top:0;width:527.85pt;height:131.95pt;rotation:315;z-index:-251656192;mso-position-horizontal:center;mso-position-horizontal-relative:margin;mso-position-vertical:center;mso-position-vertical-relative:margin",
                OptionalString = "_x0000_s2049",
                AllowInCell = false,
                FillColor = "silver",
                Stroked = false,
                Type = "#_x0000_t136"
            };


            var fill1 = new Fill() { Opacity = ".5" };
            TextPath textPath2 = new TextPath() {
                Style = "font-family:\"Arial\";font-size:1pt",
                String = txtWatermark // "DRAFT"
            };

            var textWrap1 = new TextWrap() {
                AnchorX = HorizontalAnchorValues.Margin,
                AnchorY = VerticalAnchorValues.Margin
            };

            shape1.Append(fill1);
            shape1.Append(textPath2);
            shape1.Append(textWrap1);
            picture1.Append(shapetype1);
            picture1.Append(shape1);
            run1.Append(runProperties1);
            run1.Append(picture1);
            paragraph2.Append(paragraphProperties2);
            paragraph2.Append(run1);
            sdtContentBlock1.Append(paragraph2);
            sdtBlock1.Append(sdtProperties1);
            sdtBlock1.Append(sdtContentBlock1);
            headerPart.Header.Append(sdtBlock1);
            headerPart.Header.Save();
        }
    }
    private DocumentFormat.OpenXml.Wordprocessing.Header MakeHeader() {
        var header = new DocumentFormat.OpenXml.Wordprocessing.Header();
        var paragraph = new DocumentFormat.OpenXml.Wordprocessing.Paragraph();
        var run = new DocumentFormat.OpenXml.Wordprocessing.Run();
        var text = new Text();
        text.Text = "";
        run.Append(text);
        paragraph.Append(run);
        header.Append(paragraph);
        return header;
    }
    private StringValue GetFooterPageSize(WordprocessingDocument doc) {
        StringValue stringValue = "16";
        foreach (FooterPart footer in doc.MainDocumentPart.FooterParts) {
            //foreach (DocumentFormat.OpenXml.Wordprocessing.Paragraph currentParagraph in
            //      Foot.RootElement.Descendants<DocumentFormat.OpenXml.Wordprocessing.Paragraph>())
            IEnumerable<DocumentFormat.OpenXml.Wordprocessing.Paragraph> paragraphs = footer.RootElement.Descendants<DocumentFormat.OpenXml.Wordprocessing.Paragraph>();
            foreach (DocumentFormat.OpenXml.Wordprocessing.Paragraph paragraph in paragraphs) {
                string innerText = paragraph.InnerText;
                IEnumerable<DocumentFormat.OpenXml.Wordprocessing.Run> runs = paragraph.Descendants<DocumentFormat.OpenXml.Wordprocessing.Run>();
                foreach (DocumentFormat.OpenXml.Wordprocessing.Run run in runs) {
                    string runText = run.InnerText;
                    if (runText.Trim().ToLower() == "page") {
                        DocumentFormat.OpenXml.Wordprocessing.RunProperties runProperties = run.RunProperties;
                        return (runProperties.FontSize.Val);
                    }
                }
            }
        }

        return stringValue;

    }

    private void ChangeFooterFontStyle(WordprocessingDocument doc) {
        string theSize = GetFooterPageSize(doc).ToString();
        try {
            foreach (FooterPart footer in doc.MainDocumentPart.FooterParts) {
                IEnumerable<DocumentFormat.OpenXml.Wordprocessing.Paragraph> paragraphs = footer.RootElement.Descendants<DocumentFormat.OpenXml.Wordprocessing.Paragraph>();
                foreach (DocumentFormat.OpenXml.Wordprocessing.Paragraph paragraph in paragraphs) {
                    string innerText = paragraph.InnerText;
                    IEnumerable<DocumentFormat.OpenXml.Wordprocessing.Run> runs = paragraph.Descendants<DocumentFormat.OpenXml.Wordprocessing.Run>();
                    foreach (DocumentFormat.OpenXml.Wordprocessing.Run run in runs) {
                        string runText = run.InnerText;
                        int result;
                        bool ret = Int32.TryParse(runText, out result);
                        if (ret) {
                            DocumentFormat.OpenXml.Wordprocessing.RunProperties runProperties = run.RunProperties;

                            DocumentFormat.OpenXml.Wordprocessing.FontSize fontSize = new DocumentFormat.OpenXml.Wordprocessing.FontSize();
                            fontSize.Val = new StringValue(theSize);
                            DocumentFormat.OpenXml.Wordprocessing.FontSizeComplexScript script = new DocumentFormat.OpenXml.Wordprocessing.FontSizeComplexScript();
                            script.Val = new StringValue(theSize);
                            DocumentFormat.OpenXml.Wordprocessing.RunFonts runFont = new RunFonts();
                            runFont.Ascii = "Arial";

                            Italic italic = new Italic();
                            italic.Val = OnOffValue.FromBoolean(true);
                            runProperties.FontSize = fontSize;
                            runProperties.RunFonts = runFont;
                            runProperties.Italic = italic;
                        }
                    }
                }
                footer.Footer.Save();
            }
        } catch (Exception ex) {
            string aaa = ex.Message;
        }
    }
}
