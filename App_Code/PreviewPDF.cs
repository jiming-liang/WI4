using System;
using mUtilities.Data;
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

using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.collection;
using iTextSharp.text.html;
using iTextSharp.text.html.simpleparser;
using XMLWorkerRichText;
public class PreviewPDF:PdfAndWord  {
    float TotalWidth;
    float TotalHeight;
    iTextSharp.text.Font PdfFont = FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.NORMAL);
    iTextSharp.text.Font PdfFontBold = FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.BOLD);
    PdfWriter Writer;
    Document PdfDoc;
    float Scale = 63;
    List<float> RowList = new List<float>();
    int CurrentRowIndex = 0;
    float CurrentY = 0;
    bool PrePDF = true;
    bool IsNoteBlank = true;
    protected int IconSize = 40;
    public string BaseImagePath;

    public string ExportPath;
    public void Copy(string cat, string userName) {
        string s = this.ExportPath + this.Rig;
        if (!Directory.Exists(s))
            Directory.CreateDirectory(s);
        s += @"\" + cat;
        if (!Directory.Exists(s))
            Directory.CreateDirectory(s);
        s += @"\" + this.GetFileName() + ".pdf";

        if (this.ds.Tables[0].Rows.Count == 0)
            return;
        this.RenderPDF(null, s, userName);
    }
    public override void Start(System.Web.HttpResponse response, string userName) {
        // this.Junk(); return;
        response.Buffer = true;
        response.Clear();
        response.AddHeader("Content-Disposition", "attachment; filename=" + this.GetFileName() + ".pdf");
        response.ContentType = "application/pdf";
        if (this.ds.Tables[0].Rows.Count == 0)
            return;
        this.RenderPDF(response, "", userName);
    }
    void RenderPDF(System.Web.HttpResponse response, string f, string userName) {
        float margin = 0.5f * this.Scale;
        float headerMargin = 132, footerMargin = 60;
        DataRow dr = this.ds.Tables[0].Rows[0];
        this.PdfDoc = new Document(PageSize.A4.Rotate(), margin, margin, headerMargin, footerMargin);
        this.TotalWidth = this.PdfDoc.PageSize.Width - this.PdfDoc.LeftMargin - this.PdfDoc.RightMargin;
        this.TotalHeight = this.PdfDoc.PageSize.Height - this.PdfDoc.TopMargin - this.PdfDoc.BottomMargin;
        if (response != null)
            Writer = PdfWriter.GetInstance(this.PdfDoc, response.OutputStream);
        else
            Writer = PdfWriter.GetInstance(this.PdfDoc, new FileStream(f, FileMode.Create));

        HeaderFooter PageEventHandler = new HeaderFooter();
        PageEventHandler.BaseUrl = this.BaseUrl;
        PageEventHandler.Dr = dr;
        PageEventHandler.Margin = margin;
        PageEventHandler.Scale = this.Scale;
        PageEventHandler.LastUpdatedBy = dr["revisedBy"].ToString();
        PageEventHandler.UserName = userName;
        PageEventHandler.WiNo = this.WiNo;
        PageEventHandler.LastUpdatedDate = ((DateTime)dr["revisionDate"]).ToShortDateString();
        PageEventHandler.SelectedStatus = SelectedStatus;
        Writer.PageEvent = PageEventHandler;

       // HTMLWorker worker = new HTMLWorker(this.PdfDoc);
        this.PdfDoc.Open();
        this.CreateTable(this.PdfDoc);
        this.PdfDoc.Close();
    }
    void CreateTable(Document document) {
        iTextSharp.text.Paragraph p = new iTextSharp.text.Paragraph("Resources", this.PdfFontBold);
        this.PdfDoc.Add(p);

        this.CreateResources();
        this.CreateGeneralPrecautions();
        this.CreateJobCriticality();
        this.CreateJobStep();
        this.CreateAttachment("type<>'local' or type is null ");
        // this.CreateAttachment("type='local'");
    }

    void SetTableProperties(PdfPTable t, float[] widths) {
        t.SpacingBefore = 10;
        t.DefaultCell.FixedHeight = 70;
        t.DefaultCell.PaddingLeft = 11;

        t.TotalWidth = this.TotalWidth;
        //t.KeepTogether = true;
        //t.SplitRows = false;
        // t.SplitLate = false;
        t.LockedWidth = true;
        if (widths != null)
            t.SetWidths(widths);
        t.HorizontalAlignment = 0;
    }
    void CreatePDFHeader() {
        PdfPTable t = new PdfPTable(3);
        float[] widths = new float[] { 4f, 1f, 5f };
        this.SetTableProperties(t, widths);
        DataRow dr = this.ds.Tables[0].Rows[0];
        this.AddLabelText(t, "Job Description", dr["jobDescriptionName"].ToString());
        this.AddLabelText(t, "Facility", dr["Facility"].ToString());
        this.AddLabelText(t, "Equipment Type/Make", dr["EquipmentTypeName"].ToString() + "/" + dr["EquipmentMakeName"].ToString());
        this.AddLabelText(t, "Status ", dr["Status"].ToString());
        string WiNo;
        if (this.Rig != null)
            WiNo = "WIT-" + this.Rig + "-" + dr["WICategoryName"] + "-" + dr["jobDescriptionNumber"];
        else
            WiNo = dr["wiNo"].ToString();
        this.AddLabelText(t, "Work Instruction No.", WiNo, 2);

        this.PdfDoc.Add(t);
    }

    void CreateResources() {
        PdfPTable t = new PdfPTable(2);

        float[] widths = new float[] { 1.25f, 5f };
        this.SetTableProperties(t, widths);
        this.AddLabelText(t, "Type", "");
        this.AddLabelText(t, "Description ", "");
        string[,] arr = new string[,] { { "Minimum Personnel Required", "3" }, { "Personal Protective Equipment", "8" }, { "Equipment/Tools", "4" } };
        for (int i = 0; i < arr.GetLength(0); i++) {
            this.AddLabelText(t, "", arr[i, 0]);
            this.AddLabelText(t, "", this.GetHeader2List(arr[i, 1]));
        }
        this.SetBorderColor(t, true);
        this.SetHeaderGray(t);
        t.SpacingBefore = 4;
        this.PdfDoc.Add(t);
    }
    void SetMargins() {
        this.PdfDoc.SetMargins(15, 15, 110 + 20, 60);
    }
    void CreateGeneralPrecautions() {
        // this.SetMargins();
        PdfPTable t = new PdfPTable(2);
        float[] widths = new float[] { 1.25f, 5f };
        this.SetTableProperties(t, widths);
        DataRow dr = this.ds.Tables[0].Rows[0];
        this.AddLabelText(t, "General Precautions", "");
        this.AddLabelTextRich(t, "", this.HtmlDecodeRichText(dr["GeneralPrecautions"].ToString(), true), 1, true);
        this.AddLabelText(t, "Local Precautions", "");
        this.AddLabelTextRich(t, "", this.HtmlDecodeRichText(dr["LocalPrecautions"].ToString(), true));
        for (int i = 0; i < 2; i++) {
            for (int j = 0; j < 2; j++) {
                PdfPCell c = t.Rows[i].GetCells()[j];
                c.BorderWidthLeft = 1;
                c.BorderWidthTop = 1;
                c.BorderWidthRight = 1;
                c.BorderWidthBottom = 1;
                if (i == 0)
                    c.BorderWidthBottom = 0;
                if (j == 0)
                    c.BorderWidthRight = 0;
                c.BorderColorLeft = BaseColor.RED;
                c.BorderColorTop = BaseColor.RED;
                c.BorderColorBottom = BaseColor.RED;
                c.BorderColorRight = BaseColor.RED;
            }
        }
        this.PdfDoc.Add(t);
    }
    void AddLabelTextCriticality(PdfPTable t, string name, string value, float[] widths) {
        Phrase ph = new Phrase(name, this.PdfFontBold);
        PdfPTable t1 = new PdfPTable(widths);
        // t1.DefaultCell.FixedHeight = 1;
        PdfPCell c = new PdfPCell(ph);
        c.PaddingLeft = 0;
        c.Border = 0;
        t1.AddCell(c);
        ph = new Phrase(value, this.PdfFont);
        c = new PdfPCell(ph);
        c.Border = 0;
        t1.AddCell(c);
        t.DefaultCell.PaddingLeft = 0;
        t.DefaultCell.PaddingTop = 0;
        t.AddCell(t1);
        t.DefaultCell.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
    }
    void CreateJobCriticality() {
        PdfPTable t = new PdfPTable(4);
        float[] widths = new float[] { 1.25f, .9f, 2.8f, 1.2f };
        this.SetTableProperties(t, widths);
        t.DefaultCell.FixedHeight = 20;
        DataRow dr = this.ds.Tables[0].Rows[0];

        this.AddLabelTextCriticality(t, "Job Criticality: ", dr["Criticality"].ToString(), new float[] { 5.6f, 6 });
        this.AddLabelTextCriticality(t, "Permit Required: ", (dr["Permit"].ToString() == "True" ? "Yes" : "No"), new float[] { 8f, 2.5f });
        iTextSharp.text.Image img = iTextSharp.text.Image.GetInstance(this.BaseUrl + "images/AssociatedWI_Icon.gif");

        this.AssociatedWorkInstruction(t, this.GetHeader2List("5"), img);
        this.AssociatedLink(t, "10", img);
        this.SetBorderColor(t, true);
        this.PdfDoc.Add(t);
    }
    PdfPTable SetJobStepTableProperties(bool hasSpace = false) {
        PdfPTable t = new PdfPTable(5);
        float[] widths = new float[5];//{ 1.4f, 1.4f, 14.1f, 2.3f, 1.9f };
        widths[0] = .7f * this.Scale;
        widths[1] = widths[0];
        widths[3] = 2.26f * this.Scale;
        widths[4] = .8f * this.Scale;
        widths[2] = this.TotalWidth - 2 * widths[0] - widths[3] - widths[4];
        if (hasSpace)
            t.SpacingBefore = 10;
        // t.DefaultCell.FixedHeight = 70;
        //t.DefaultCell.PaddingTop = 30;

        t.TotalWidth = this.TotalWidth;
        //t.KeepTogether = true;
        //t.SplitRows = false;
        t.SplitLate = false;
        t.LockedWidth = true;
        if (widths != null)
            t.SetWidths(widths);
        t.HorizontalAlignment = 0;
        return t;
    }
    void AddJobStepHeader(PdfPTable t, bool hasSpace = false) {
        foreach (string header in this.Headers)
            if (header.Contains("Barriers"))
                this.AddLabelText(t, "Barrier (Initial)", "");
            else
                this.AddLabelText(t, header, "");
        this.SetBorderColor(t, true);
        this.SetHeaderGray(t);
    }
    void CreateJobStep() {
        PdfPTable t = this.SetJobStepTableProperties(true);
        t.HeaderRows = 1;

        this.AddJobStepHeader(t, true);
        DataTable dt = ds.Tables[1];
        int globalIndex = 0, localIndex = 0, standaloneIndex = 0;
        foreach (DataRow dr in dt.Rows) {
            bool isGlobal = dr["type"].ToString() == "GlobalStep" ? true : false;
            this.AddWarningAndCaution(t, dr);
            if (isGlobal) {
                globalIndex++;
                localIndex = 0;
                //this.AddLabelText(t, "", globalIndex.ToString());
                this.AddStepNumber(t, globalIndex.ToString());
                this.AddCell(t);
            } else {
                this.AddCell(t);
                if (dr["type"].ToString() == "LocalStep") {
                    localIndex++;
                    //this.AddLabelText(t, "", globalIndex.ToString() + "." + localIndex.ToString());
                    this.AddStepNumber(t, globalIndex.ToString() + "." + localIndex.ToString());
                } else {
                    standaloneIndex++;
                    char ch = (char)(standaloneIndex + 64);
                    //this.AddLabelText(t, "", ch.ToString());
                    this.AddStepNumber(t, ch.ToString());
                }
            }
            this.AddDescriptionAndNote(t, dr["description"].ToString(), dr["note"].ToString());
            this.AddPhoto(dr, t);
            string barrier = dr["barrier"].ToString();
            if (barrier == "Yes") {
                this.CreateBarrier(t);
            } else
                this.AddCell(t);
            this.SetBorderColor(t);
        }
        this.PdfDoc.Add(t);
    }
    void CreateBarrier(PdfPTable t) {
        PdfPTable subTable = new PdfPTable(2);
        subTable.SplitLate = false;
        subTable.SetWidths(new float[] { 10, 1 });
        subTable.DefaultCell.FixedHeight = 400;

        iTextSharp.text.Image img = iTextSharp.text.Image.GetInstance(this.BaseUrl + "images/barrier.png");
        img.ScaleToFit(img.Width * 30 / img.Height, 30);
        Phrase ph = new Phrase();
        ph.Add(new Chunk(img, -3, -10));
        PdfPCell c = new PdfPCell();
        c.AddElement(ph);
        c.Border = 0;
        subTable.AddCell(c);
        c = new PdfPCell(new Phrase(Environment.NewLine + Environment.NewLine));
        c.Border = 0;
        subTable.AddCell(c);
        t.AddCell(subTable);
    }
    PdfPCell AddPhotoCell(string name, float h0) {
        PdfPCell c;
        iTextSharp.text.Image img = iTextSharp.text.Image.GetInstance(this.BaseUrl + "images/" + name);

        float h = h0 * this.Scale;
        float w = img.Width * h0 / img.Height;
        if (w > 2.26f * this.Scale)
            w = 2.26f * this.Scale;
        img.ScaleAbsoluteWidth(h);
        img.ScaleAbsoluteHeight(w);
        c = new PdfPCell(img);
        c.Border = 0;
        return c;
    }
    void CreatePDFPhoto(PdfPTable t, string photo) {
        PdfPCell c;
        PdfPTable subTable = new PdfPTable(1);
        subTable.SplitLate = true;
        subTable.SetWidths(new float[] { 10 });
        subTable.AddCell(this.AddPhotoCell("empty.png", 0.5f));
        iTextSharp.text.Image img = iTextSharp.text.Image.GetInstance(this.BaseUrl + "upload/" + photo);
        //string path = HttpContext.Current.Server.MapPath("upload")+@"\";
        //using (FileStream fs = new FileStream(path + photo, FileMode.Open)) 
        //    img = iTextSharp.text.Image.GetInstance(System.Drawing.Image.FromStream(fs), System.Drawing.Imaging.ImageFormat.Png);

        float h = 1.5f * this.Scale;
        float w = img.Width * h / img.Height;
        if (w > 2.2f * this.Scale)
            w = 2.2f * this.Scale;
        img.ScaleAbsoluteWidth(w);
        img.ScaleAbsoluteHeight(h);
        //img.ScaleToFit(100, 144);
        //Phrase ph = new Phrase();
        //ph.Add(new Chunk(img, -3, -10));
        c = new PdfPCell(img);
        c.Border = 0;
        c.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
        subTable.AddCell(c);
        t.AddCell(subTable);
    }
    void AddPhoto(DataRow dr, PdfPTable t) {
        string photo = dr["photo"].ToString();
        string prompt = dr["prompt"].ToString();
        PdfPCell cell = new PdfPCell();
        if (prompt != "") {
            cell.AddElement(this.GetPrompt(prompt));
            t.AddCell(cell);
        } else if (photo != "") {
            this.CreatePDFPhoto(t, photo);
        } else
            this.AddCell(t);
    }

    void CheckPageBreak() {
        if (this.PrePDF)
            return;
        float y = this.Writer.GetVerticalPosition(true);

        if (y - this.RowList[this.CurrentRowIndex] < 62 || (y > this.CurrentY && this.CurrentY != 0)) {
            this.PdfDoc.NewPage();
            //this.AddJobStepHeader();
        } else {

        }
        this.CurrentY = this.Writer.GetVerticalPosition(true);
        this.CurrentRowIndex++;
    }
    void TableAdded(PdfPTable t) {
        if (this.PrePDF)
            this.RowList.Add(t.GetRowHeight(0));
    }
    void AddWarningAndCaution(PdfPTable t, DataRow dr) {
        bool added = false;
        string[] arr = { "Warning", "Caution" };
        PdfPTable subTable = new PdfPTable(2);
        subTable.SplitLate = false;
        subTable.SetWidths(new float[] { 1, 14 });
        subTable.DefaultCell.FixedHeight = 200;
        int count = 0;
        foreach (string name in arr) {
            string value = dr[name].ToString();
            if (value != "") {
                added = true;
                string url = this.BaseUrl + "images/" + name + ".png";
                iTextSharp.text.Image img = iTextSharp.text.Image.GetInstance(url);
                img.ScaleToFit(this.IconSize - 10, this.IconSize - 10);
                PdfPCell c = new PdfPCell();
                Phrase ph = new Phrase();
                ph.Add(new Chunk(img, -3, -10));
                c.AddElement(img);
                c.Border =0;
                c.Padding =.5f;
                c.VerticalAlignment = PdfPCell.ALIGN_CENTER;
               // c.SetLeading(1000, 1);
                subTable.AddCell(c);
                c = new PdfPCell();

                Paragraph p = new Paragraph();
                p.SetLeading(11, 0);
                ph = new Phrase();
                string[] arr1 = value.Split('\n');
                bool firstLne = true;
                foreach (string s in arr1) {
                    if (firstLne) {
                        //p.Add(Environment.NewLine);
                        firstLne = false;
                    }
                    if (s.Trim() != "") {
                        ph = new iTextSharp.text.Phrase(name + ": ", this.PdfFontBold);
                        p.Add(ph);
                        p.Add(this.AddText(s));
                    }
                    p.Add(Environment.NewLine);
                }
                if (arr1.Length == 1)
                    p.Add("\n");
                if (arr1.Length == 1) {
                    // p.Add(Environment.NewLine);
                    count += 2;
                } else
                    count += arr1.Length;

                c.AddElement(p);
                c.Border = 0;
                // c.PaddingTop = 14;
                subTable.AddCell(c);
            }
        }
        if (subTable.Rows.Count != 0) {
            if (dr["hazard"].ToString() == "")
                this.AddCell(t, 2);
            else {
                bool isLocal = true;
                if (dr["type"].ToString().Contains("Global"))
                    isLocal = false;
                if (isLocal)
                    this.AddCell(t, 1);
                this.AddHazardImage(t, dr["hazard"].ToString(), count + 4);
                if (!isLocal)
                    this.AddCell(t, 1);
            }

            t.AddCell(subTable);

            this.AddCell(t, 2);
        }
        if (added) {
            this.CheckPageBreak();
        }
    }
    void AddImageAndTextRich2(PdfPTable t, string name, string value, string url) {
        iTextSharp.text.Image img = iTextSharp.text.Image.GetInstance(url);
        img.ScaleToFit(this.IconSize - 10, this.IconSize - 10);
        PdfPCell c = new PdfPCell();
        iTextSharp.text.Paragraph p = new iTextSharp.text.Paragraph();
        Phrase ph = new Phrase();
        ph.Add(new Chunk(img, -3, -28));
        PdfPTable subTable = new PdfPTable(new float[] { 100 });


        t.AddCell(subTable);
    }
    void AddImageAndTextRich(PdfPTable t, string name, string value, string url) {
        iTextSharp.text.Image img = iTextSharp.text.Image.GetInstance(url);
        img.ScaleToFit(this.IconSize - 10, this.IconSize - 10);
        PdfPCell c = new PdfPCell();
        iTextSharp.text.Paragraph p = new iTextSharp.text.Paragraph();
        Phrase ph = new Phrase();
        ph.Add(new Chunk(img, -3, -28));
        PdfPTable subTable = new PdfPTable(new float[] { 6, 100 });
        subTable.AddCell(new PdfPCell(ph) { Border = 0 });
        value = HttpUtility.HtmlDecode(value);
        value = this.ReplaceFont(value);
        List<IElement> htmlArrayList = iTextSharp.text.html.simpleparser.HTMLWorker.ParseToList(new StringReader(value), null);
        p.InsertRange(0, htmlArrayList);
        c = new PdfPCell() { Border = 0 };

        c.AddElement(p);
        subTable.AddCell(c);//new PdfPCell(p) { Border = 0 });

        t.AddCell(subTable);
    }
    void AddImageAndText(PdfPTable t, string name, string value, string url) {
        iTextSharp.text.Image img = iTextSharp.text.Image.GetInstance(url);
        img.ScaleToFit(this.IconSize - 10, this.IconSize - 10);
        PdfPCell c = new PdfPCell();
        iTextSharp.text.Paragraph p = new iTextSharp.text.Paragraph();
        Phrase ph = new Phrase();
        ph.Add(new Chunk(img, -3, -28));
        PdfPTable subTable = new PdfPTable(new float[] { 6, 100 });
        subTable.AddCell(new PdfPCell(ph) { Border = 0 });
        string[] arr = value.Split('\n');
        foreach (string s in arr) {
            if (s.Trim() != "") {
                ph = new iTextSharp.text.Phrase(name + ": ", this.PdfFontBold);
                p.Add(ph);
                p.Add(this.AddText(s));
            }
            p.Add(Environment.NewLine);
        }
        subTable.AddCell(new PdfPCell(p) { Border = 0 });

        t.AddCell(subTable);
    }
    void AddImageFromUrl(PdfPTable t, string url) {
        iTextSharp.text.Image img = iTextSharp.text.Image.GetInstance(url);
        float size = 30;
        img.ScaleToFit(size, size);
        PdfPCell c = new PdfPCell();
        // c.Border = 0;
        c.AddElement(img);
        c.Colspan = 2;
        t.AddCell(c);
    }
    PdfPTable GetPdfPTable(int colSpan) {
        PdfPTable t = new PdfPTable(colSpan);
        t.DefaultCell.Border = 0;// getDefaultCell().setBorder(0); 
        float[] widths = new float[] { 1, 1, 1, 1, 1 };
        this.SetTableProperties(t, null);
        t.KeepTogether = true;
        return t;
    }

    void CreateAttachment(string filter) {
        DataTable dt = ds.Tables[2];
        int colSpan = 5;
        PdfPTable t = this.GetPdfPTable(colSpan);
        iTextSharp.text.Paragraph p = new iTextSharp.text.Paragraph("");
        List<Tuple<float, float, object, string>> list = new List<Tuple<float, float, object,  string>>();
        int indexGlobal = 0, indexLocal = 0;
        float totalWidth = 0;
        float pageWidth = this.PdfDoc.PageSize.Width - this.PdfDoc.LeftMargin - this.PdfDoc.RightMargin;
        float pageHeight = this.PdfDoc.PageSize.Height - this.PdfDoc.TopMargin - this.PdfDoc.BottomMargin;
        pageHeight -= 50;
        foreach (DataRow dr in dt.Select()) {
            string name = dr["Attachment"].ToString();
            string title = dr["title"].ToString();
            string prompt = dr["prompt"].ToString();
            title = HttpUtility.HtmlDecode(title);
            prompt = HttpUtility.HtmlDecode(prompt);
            title = "Figure " + this.GetImageNumber(ref indexGlobal, ref indexLocal, dr["type"].ToString(), prompt) + ": " + title;
            float w = 0, h = 0;
            iTextSharp.text.Image img = null;
            string[] arr = { prompt, name };
            for (int i = 0; i < 2; i++) {
                if (i == 1) {
                    if (name == "")
                        continue;
                    if (name.ToLower().IndexOf(".pdf") == -1) {
                        img = iTextSharp.text.Image.GetInstance(this.BaseUrl + @"upload/" + name);
                        w = img.Width;
                        h = img.Height;
                        float ratio = this.Scale / 110;
                        w *= ratio;
                        h *= ratio;
                        this.AdjustImageSize(img, ref w, ref h);
                        img.ScaleToFit(w, h);
                    } else {
                        w = 100;
                        h = 10;
                    }
                }
                if (i == 0) {
                    if (prompt == "")
                        continue;
                    w = 100;
                    h = 10;
                }
                totalWidth += w;
                if (totalWidth > this.TotalWidth) {
                    this.RenderAttachment(list);
                    totalWidth = w;
                    list = new List<Tuple<float, float, object, string>>();
                }
                object o = arr[i];
                if (i==1 ) {
                    o = img;
                    if (prompt != "")
                        title = "";
                }
                list.Add(new Tuple<float, float, object, string>(w, h, o, title));
            }
        }
        if (list.Count() != 0)
            this.RenderAttachment(list);
    }
    void CreateAttachment011616(string filter) {
        DataTable dt = ds.Tables[2];
        int colSpan = 5;
        PdfPTable t = this.GetPdfPTable(colSpan);
        iTextSharp.text.Paragraph p = new iTextSharp.text.Paragraph("");
        List<Tuple<float, float, object, string>> list = new List<Tuple<float, float, object,  string>>();
        int indexGlobal = 0, indexLocal = 0;
        float totalWidth = 0;
        float pageWidth = this.PdfDoc.PageSize.Width - this.PdfDoc.LeftMargin - this.PdfDoc.RightMargin;
        float pageHeight = this.PdfDoc.PageSize.Height - this.PdfDoc.TopMargin - this.PdfDoc.BottomMargin;
        pageHeight -= 50;
        foreach (DataRow dr in dt.Select()) {
            string name = dr["Attachment"].ToString();
            string title = dr["title"].ToString();
            string prompt = dr["prompt"].ToString();
            title = HttpUtility.HtmlDecode(title);
            prompt = HttpUtility.HtmlDecode(prompt);
            title = "Figure " + this.GetImageNumber(ref indexGlobal, ref indexLocal, dr["type"].ToString(), prompt) + ": " + title;
            float w = 0, h = 0;
            iTextSharp.text.Image img = null;
            if (prompt == "") {
                if (name.ToLower().IndexOf(".pdf") == -1) {
                    img = iTextSharp.text.Image.GetInstance(this.BaseUrl + @"upload/" + name);
                    w = img.Width;
                    h = img.Height;
                    float ratio = this.Scale / 110;
                    w *= ratio;
                    h *= ratio;
                    //if (w > pageWidth)
                    //    w = pageWidth;
                    //if (h > pageHeight)
                    //    h = pageHeight;
                    this.AdjustImageSize(img, ref w, ref h);
                    img.ScaleToFit(w, h);
                } else {
                    w = 100;
                    h = 10;

                }
            } else {
                w = 100;
                h = 10;
            }
            totalWidth += w;
            if (totalWidth > this.TotalWidth) {
                this.RenderAttachment(list);
                totalWidth = w;
                list = new List<Tuple<float, float, object, string>>();
            }
            object o = prompt;
            if (prompt == "")
                if (img == null)
                    o = name;
                else
                    o = img;
            list.Add(new Tuple<float, float, object, string>(w, h, o,  title));
        }
        if (list.Count() != 0)
            this.RenderAttachment(list);
    }
    void AdjustImageSize(iTextSharp.text.Image img, ref float w, ref float h) {
        float f = 50;
        float height = this.TotalHeight - f;
        if (w / h > this.TotalWidth / height) {
            if (w > this.TotalWidth) {
                h *= this.TotalWidth / w;
                w = this.TotalWidth;
            }
        } else {
            if (h > height) {
                w *= height / h;
                h = height;
            }
        }
        if (w < 100)
            w = 100;
    }
    Phrase GetPrompt(string prompt) {
        Phrase ph = new Phrase(prompt);
        ph.Font.Color = BaseColor.BLUE;
        BaseColor grey = new BaseColor(128, 128, 128);
        Font font1 = FontFactory.GetFont("Arial", 10, Font.ITALIC, grey);
        ph.Font = font1;
        return ph;
    }
    void RenderAttachment(List<Tuple<float, float, object, string>> list) {
        PdfPTable t = new PdfPTable(list.Count());
        t.DefaultCell.Border = 0;// getDefaultCell().setBorder(0); 
        float y = this.Writer.GetVerticalPosition(true);
        float[] widths = new float[list.Count()];
        for (int i = 0; i < widths.Length; i++) {
            widths[i] = list[i].Item1;
        }
        t.SetWidths(widths);
        this.SetTableProperties(t, null);
        t.KeepTogether = true;
        int indexEmbeded = 1;
        for (int i = 0; i < widths.Length; i++) {
            PdfPCell c = new PdfPCell();
            c.VerticalAlignment = PdfPCell.ALIGN_BOTTOM;
            c.Border = 0;
           // c.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
            string text = list[i].Item3.ToString();
            if (list[i].Item3 is iTextSharp.text.Image) {
                iTextSharp.text.Image img = (iTextSharp.text.Image)list[i].Item3;
                //img.Alignment = PdfPCell.ALIGN_CENTER;
                c.AddElement(img);
            } else if (text.IndexOf(".pdf") != -1) {
                c.AddElement(this.GetEmbeded(text, indexEmbeded++));
            } else
                c.AddElement(this.GetPrompt(text));
            t.AddCell(c);
        }
        for (int i = 0; i < widths.Length; i++) {
            PdfPCell c = new PdfPCell();
            iTextSharp.text.Paragraph p = new iTextSharp.text.Paragraph(list[i].Item4);
           // p.Alignment = PdfPCell.ALIGN_CENTER;
            c.AddElement(p);
            c.Border = 0;
            t.AddCell(c);
        }
        this.PdfDoc.Add(t);
    }
    Chunk GetEmbeded(string name, int index) {
        string f = HttpContext.Current.Server.MapPath(@"upload\" + name);
        PdfFileSpecification fs = PdfFileSpecification.FileEmbedded(this.Writer, f, name, null);
        fs.AddDescription(name, false);
        this.Writer.AddFileAttachment(fs);

        PdfTargetDictionary target = new PdfTargetDictionary(true);
        target.EmbeddedFileName = name;
        PdfDestination dest = new PdfDestination(PdfDestination.FIT);
        dest.AddFirst(new PdfNumber(index));
        PdfAction action = PdfAction.GotoEmbedded(null, target, dest, true);
        PdfPCell c = new PdfPCell();

        BaseColor grey = new BaseColor(128, 128, 128);
        Chunk chunk = new Chunk(" (Click to open the embeded PDF)", FontFactory.GetFont("Arial", 10, Font.ITALIC, grey));
        //ph.Font.Color = BaseColor.BLUE;
        chunk.SetAction(action);
        return chunk;
    }
    public static float TotalRowHeights(
  Document document, PdfContentByte content,
  PdfPTable table, params int[] wantedRows) {
        float height = 0f;
        ColumnText ct = new ColumnText(content);
        // respect current Document.PageSize    
        ct.SetSimpleColumn(
          document.Left, document.Bottom,
          document.Right, document.Top
        );
        ct.AddElement(table);
        // **simulate** adding the PdfPTable to calculate total height
        ct.Go(true);
        foreach (int i in wantedRows) {
            height += table.GetRowHeight(i);
        }
        return height;
    }
    PdfPTable CheckNewPage(PdfPTable t, int colSpan) {
        float y = this.Writer.GetVerticalPosition(false);
        int[] wantedRows = { 0, 1 };
        float y1;//= CalculatePdfPTableHeight(t);
        y1 = TotalRowHeights(this.PdfDoc, this.Writer.DirectContent, t, wantedRows);
        //if (y - y1 < this.PdfDoc.Bottom)
        //    this.PdfDoc.NewPage();
        this.PdfDoc.Add(t);
        PdfPTable t1 = new PdfPTable(colSpan);
        // t1.KeepTogether = true;
        t1.DefaultCell.Border = 0;
        this.SetTableProperties(t1, null);
        return t1;
    }
    public float CalculatePdfPTableHeight(PdfPTable table) {
        using (MemoryStream ms = new MemoryStream()) {
            using (Document doc = new Document(PageSize.TABLOID)) {
                using (PdfWriter w = PdfWriter.GetInstance(doc, ms)) {
                    doc.Open();

                    table.WriteSelectedRows(0, table.Rows.Count, 0, 0, w.DirectContent);

                    doc.Close();
                    return table.TotalHeight;
                }
            }
        }
    }
    void AddAttachmentTitle(List<Tuple<int, string>> list, PdfPTable t, int emptyCell = 0) {
        for (int i = 0; i < list.Count; i++) {
            PdfPCell c = new PdfPCell();
            int j = list[i].Item1;
            string text = list[i].Item2;
            iTextSharp.text.Paragraph p = new iTextSharp.text.Paragraph(text);
            c.Colspan = j;
            c.Border = 0;
            p.Alignment = PdfPCell.ALIGN_CENTER;
            c.AddElement(p);
            t.AddCell(c);
        }
        //this.AddCell(t, emptyCell);
        while (emptyCell-- > 0) {
            PdfPCell c = new PdfPCell();
            c.AddElement(new Phrase(""));
            c.Border = 0;
            t.AddCell(c);
        }
        list.Clear();
    }
    void AddEmptyImage(PdfPTable t, int total) {
        for (int i = 0; i < total; i++) {
            iTextSharp.text.Image img = iTextSharp.text.Image.GetInstance(this.BaseUrl + @"images/empty.png");
            PdfPCell c = new PdfPCell(img);
            c.Border = 0;
            t.AddCell(c);
        }
    }
    void AddCell(PdfPTable t, int i = 1) {
        while (i-- > 0) {
            this.AddLabelText(t, "", "");  //t.AddCell(" ");
        }
    }
    void AddHazardImage(PdfPTable t, string value, int count = 0) {
        PdfPTable subTable = new PdfPTable(3);
        subTable.SplitLate = false;
        subTable.SetWidths(new float[] { 11, 50, 11 });
        subTable.DefaultCell.FixedHeight = 200;

        iTextSharp.text.Paragraph p = new iTextSharp.text.Paragraph();
        string[] arr = value.Split(',');
        if (value != "")
            foreach (string str in arr) {
                PdfPCell c = new PdfPCell();
                c.Border = 0;
                // c.AddElement(new Phrase(Environment.NewLine + Environment.NewLine));
                subTable.AddCell(c);

                iTextSharp.text.Image img = iTextSharp.text.Image.GetInstance(this.BaseUrl + @"images/hazard/" + str);
                img.ScaleToFit(this.IconSize - 10, this.IconSize - 10);

                Phrase ph = new Phrase();
                ph.Add(new Chunk(img, -3, -10));
                c = new PdfPCell();
                c.Border = 0;
                c.PaddingTop = 2;
                c.PaddingBottom = 12;
                c.AddElement(ph);
                c.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                c.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                subTable.AddCell(c);
                c = new PdfPCell();
                c.Border = 0;
                subTable.AddCell(c);
            }
        t.AddCell(subTable);
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
        string s = "";
        foreach (DataRow dr in ds.Tables[i].Rows) {
            s = "" + dr["name"];
            if (dr.Table.Columns.Contains("other") && dr["other"].ToString().Trim() != "undefined" && dr["other"].ToString().Trim() != "") {
                //s += "(" + dr["other"] + ")";
                s = dr["other"] + "";
            }
            list.Add(s);
        }
        string[] arr = (string[])list.ToArray(Type.GetType("System.String"));
        return string.Join(", ", arr);
    }
    void GetHeader2ListLink(PdfPTable t, string index) {
        int i = int.Parse(index);
        PdfPCell cell = new PdfPCell();
        iTextSharp.text.Paragraph p = new iTextSharp.text.Paragraph();
        p.Add(this.AddLabel("Associated Link "));
        ArrayList list = new ArrayList();
        foreach (DataRow dr in ds.Tables[i].Rows) {
            string title = dr["title"].ToString();
            string link = dr["link"].ToString();
            Chunk anchor = new Chunk(title);
            Uri uri;
            if (Uri.TryCreate(link, UriKind.Absolute, out uri)) {
                anchor.SetAnchor(uri);
                anchor.SetUnderline(1, -1);
                p.Add(anchor);
            }
            p.Add(" ");
        }
        cell.AddElement(p);
        t.AddCell(cell);
    }
    void AddStepNumber(PdfPTable t, string name) {
        PdfPCell cell;
        cell = new PdfPCell(this.AddText(name));
        cell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
        cell.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
        cell.PaddingLeft = 2;
        t.AddCell(cell);
    }
    void AddLabelText(PdfPTable t, string name, string value, int colSpan = 1) {
        PdfPCell cell;
        if (name == "")
            cell = new PdfPCell(this.AddText(value));
        else if (value == "")
            cell = new PdfPCell(this.AddLabel(name));
        else {
            //Paragraph p = new Paragraph();
            //p.Add(this.AddLabel(name));
            //p.Add(this.AddText(value));
            //cell = new PdfPCell();
            //cell.PaddingLeft=0;
            Phrase ph = new Phrase(name, this.PdfFontBold);
            PdfPTable t1 = new PdfPTable(new float[] { 5.6f, 6 });
            // t1.DefaultCell.FixedHeight = 1;
            PdfPCell c = new PdfPCell(ph);
            c.PaddingLeft = 0;
            c.Border = 0;
            t1.AddCell(c);
            ph = new Phrase(value, this.PdfFont);
            c = new PdfPCell(ph);
            c.Border = 0;
            t1.AddCell(c);
            t.DefaultCell.PaddingLeft = 0;
            t.AddCell(t1);
            t.DefaultCell.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
            return;
        }
        float f = 0;
        if (float.TryParse(value, out f)) {
            cell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
            cell.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
        }
        cell.PaddingLeft = 2;
        cell.PaddingTop = 0;
        // cell.Border = 0;
        t.AddCell(cell);
    }
    void AssociatedWorkInstructionBak(PdfPTable tParent, string value, iTextSharp.text.Image img, int colSpan = 1) {
        string name = " Associated Work Instruction";
        if (this.Prefix == "WIT-")
            name += " Template";
        Paragraph p = new Paragraph();
        img.ScaleToFit(0.19f * this.Scale, 0.1f * this.Scale);
        PdfPCell c = new PdfPCell(img);
        p.SetLeading(2, 1);

        p.Add(new Chunk(img, 0, 0));
        p.Add(new Phrase(name + "\n", PdfFontBold));
        foreach (DataRow dr in ds.Tables[5].Rows) {
            Phrase ph = new Phrase(dr["name"].ToString() + "\n");
            ph.Font.Size = 8;
            ph.Font.Color = BaseColor.BLUE;
            p.Add(ph);
        }
        //p.Add("\n");
        c.AddElement(p);
        c.PaddingBottom = 2;
        c.PaddingLeft = 2;
        tParent.AddCell(c);
    }
    void AssociatedWorkInstruction(PdfPTable tParent, string value, iTextSharp.text.Image img, int colSpan = 1) {
        string name = " Associated Work Instruction";
        if (this.Prefix == "WIT-")
            name += " Template";
        Paragraph p = new Paragraph();
        img.ScaleToFit(0.19f * this.Scale, 0.1f * this.Scale);
        PdfPCell c = new PdfPCell(img);
        p.SetLeading(2, 1);

        p.Add(new Chunk(img, 0, 0));
        p.Add(new Phrase(name + "\n", PdfFontBold));
        foreach (DataRow dr in ds.Tables[5].Rows) {
            string title = dr["name"].ToString();
            string id = dr["id"].ToString();
            string url= HttpContext.Current.Request.Url.ToString();
            string[] arr = url.Split(new char[]{'?'}  );
            string wiType = title.Substring(0, 3);
            url = arr[0] + "?id=" + id + "&WiType=" + wiType.Replace("-", "");
            Chunk anchor = new Chunk(title + "\n");
            Uri uri;
            if (Uri.TryCreate(url, UriKind.Absolute, out uri)) {
                anchor.SetAnchor(uri);
                anchor.SetUnderline(1, -1);
                anchor.Font.Size = 8;
                anchor.Font.Color = BaseColor.BLUE;
                p.Add(anchor);
            }
        }
        //p.Add("\n");
        c.AddElement(p);
        c.PaddingBottom = 2;
        c.PaddingLeft = 2;
        tParent.AddCell(c);
    }

    void AssociatedLink(PdfPTable tParent, string index, iTextSharp.text.Image img) {
        PdfPCell cellOut = new PdfPCell();
        Paragraph p = new Paragraph();
        img.ScaleToFit(0.19f * this.Scale, 0.1f * this.Scale);
        PdfPCell c = new PdfPCell(img);
        p.SetLeading(2, 1);

        p.Add(new Chunk(img, 0, 0));
        p.Add(new Phrase(" Associated Link \n", PdfFontBold));
        int i = int.Parse(index);
        foreach (DataRow dr in ds.Tables[i].Rows) {
            string title = dr["title"].ToString();
            string link = dr["link"].ToString();
            string newLink = HttpContext.Current.Server.UrlDecode(link);
            if ((!newLink.StartsWith("http://")) && (!newLink.StartsWith("https://"))) {
                newLink = "http://" + newLink;
            }
            Chunk anchor = new Chunk(title + "\n");
            Uri uri;
            if (Uri.TryCreate(newLink, UriKind.Absolute, out uri)) {
                anchor.SetAnchor(uri);
                anchor.SetUnderline(1, -1);
                anchor.Font.Size = 8;
                anchor.Font.Color = BaseColor.BLUE;
                p.Add(anchor);
            }
        }
        c.AddElement(p);
        c.PaddingBottom = 2;
        c.PaddingLeft = 2;
        tParent.AddCell(c);
    }
    void AssociatedWorkInstruction101215(PdfPTable tParent, string name, string value, iTextSharp.text.Image img, int colSpan = 1) {
        PdfPCell cellOut = new PdfPCell();      // tParent - cellOut - table - cellIn - paragraph
        PdfPTable table = new PdfPTable(1);
        table.SetWidths(new float[] { 10 });
        PdfPCell cellx = new PdfPCell();
        Paragraph p = new Paragraph();

        table.HorizontalAlignment = 0;
        cellx.BorderWidthBottom = 0f;
        cellx.BorderWidthTop = 0f;
        cellx.BorderWidthRight = 0f;
        cellx.BorderWidthLeft = 0f;
        img.ScaleToFit(0.19f * this.Scale, 0.1f * this.Scale);
        p.Add(new Chunk(img, 0, 0));
        p.Add(new Phrase(name + "\n", PdfFontBold));
        //p.Add(new Phrase(value, PdfFont));
        p.Add(new Phrase(value.Replace(",", ", "), PdfFont));
        p.Font.Size = 8;
        p.Font.Color = BaseColor.BLUE;
        cellx.AddElement(p);
        table.AddCell(cellx);
        cellOut.AddElement(table);
        tParent.AddCell(cellOut);
    }
    void SetHeaderGray(PdfPTable t) {
        foreach (PdfPRow row in t.Rows) {
            foreach (PdfPCell cell in row.GetCells())
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            return;
        }
    }
    void SetBorderColor(PdfPTable t, bool showTop = false) {
        // return;
        bool firstRow = true;
        foreach (PdfPRow row in t.Rows) {
            bool firstCell = true;
            foreach (PdfPCell cell in row.GetCells()) {
                // cell.BorderColor = BaseColor.GRAY;
                cell.BorderColorBottom = BaseColor.LIGHT_GRAY;
                cell.BorderColorLeft = BaseColor.LIGHT_GRAY;
                cell.BorderColorRight = BaseColor.LIGHT_GRAY;
                cell.BorderColorTop = BaseColor.LIGHT_GRAY;

                cell.BorderWidthRight = 1f;
                //cell.BorderWidthBottom = 1f;
                //if (firstRow)
                //    cell.BorderWidthTop = 1f;
                //else
                //    cell.BorderWidthTop = 1f;
                if (firstCell)
                    cell.BorderWidthLeft = 1f;
                else
                    cell.BorderWidthLeft = 0f;
                if (!showTop)
                    cell.BorderWidthTop = .5f;
                cell.BorderWidthTop = .5f;
                cell.BorderWidthBottom = .5f;
                cell.UseVariableBorders = true;

                firstCell = false;
            }
            firstRow = false;
        }
    }
    string ReplaceFont(string v) {
        string value = v;
        value = value.Replace("font-size: xx-small", "font-size:10pt ");
        value = value.Replace("font-size: x-small", "font-size:10pt ");
        value = value.Replace("font-size: small", "font-size:10pt");
        value = value.Replace("font-size: Medium", "font-size:10pt");
        value = value.Replace("font-size: large", "font-size:10pt");
        value = value.Replace("font-size: x-large", "font-size:10pt");
        value = value.Replace("font-size: xx-large", "font-size:10pt ");
        value = value.Replace(@" style=""small", @" style=""font-size:10pt ");
        //value = value.Replace(@"style=""margin: 0px;""", @"style=""margin: 0px;font-size:10pt""");
        //value = value.Replace(@"style=""", @"style=""font-size:10pt;");
        value = value.Replace("font-family: arial,helvetica,sans-serif; small", "font-family: arial;font-size: 10pt");
        value = @"<body style=""font-size:10pt;"">" + value + @"</body>";
        return value;
    }
    StyleSheet GetStyleSheet(bool isUnicode) {
        //if (!isUnicode)
        //    return null;
        FontFactory.Register("c:/windows/fonts/ARIALUNI.TTF");
        StyleSheet style = new StyleSheet();
        style.LoadTagStyle("body", "face", "times Unicode MS");
        style.LoadTagStyle("body", "encoding", BaseFont.IDENTITY_H);
        style.LoadTagStyle("body", "line - height", "110px");
        style.LoadTagStyle("td", "line - height", "110px");
        //style.LoadTagStyle("td", "style", "line-height:125pt");
        //style.LoadTagStyle("body", "style", "line-height:125pt");

        return style;
    }
    void AddLabelTextRich(PdfPTable t, string name, string value, int colSpan = 1, bool isUnicode = false) {


        PdfPCell c = PdfXML.GetRichCell(value);
        c.Colspan = colSpan;
        t.AddCell(c);
    }
    void AddLabelTextRichbak(PdfPTable t, string name, string value, int colSpan = 1, bool isUnicode = false) {
        PdfPCell cell = new PdfPCell();
        iTextSharp.text.Paragraph p = new iTextSharp.text.Paragraph();
        cell.Colspan = colSpan;

        value = this.HtmlDecodeRichText(value);
        List<IElement> htmlArrayList = iTextSharp.text.html.simpleparser.HTMLWorker.ParseToList(new StringReader(value), this.GetStyleSheet(isUnicode));
        p.SetLeading(0.1f, 0f);
        p.InsertRange(0, htmlArrayList);
        p.Add(Environment.NewLine);
        cell.AddElement(p);
        cell.PaddingLeft = 4;
        t.AddCell(cell);
    }
    void CheckNote(XElement node) {
        if (node.NodeType == XmlNodeType.Text && node.Value.Trim() != "")
            this.IsNoteBlank = false;
        foreach (XNode node2 in node.Nodes()) {
            if (node2.NodeType == XmlNodeType.Text && node2.ToString().Trim() != "")
                this.IsNoteBlank = false;
            if (node2.NodeType == XmlNodeType.Element)
                this.CheckNote((XElement)node2);

        }
    }
    string HtmlDecodeRichText(string value, bool bDecode = false) {
        if (bDecode)
            value = HttpUtility.HtmlDecode(value);
        value = this.ReplaceFont(value);
        value = value.Replace(this.LessThanEncode, "&lt;");
        value = value.Replace(this.AmpEncode, "&amp;");
        value = value.Replace((char)160, ' ');
        return value;
    }
    void AddDescriptionAndNote(PdfPTable t, string description, string note) {
        PdfPCell cell = new PdfPCell();
        iTextSharp.text.Paragraph p = new iTextSharp.text.Paragraph("", this.PdfFont);

        description = this.HtmlDecodeRichText(description, true);
        note = this.HtmlDecodeRichText(note, true);
        note = "<r>" + note + "</r>";
        XDocument root = XDocument.Parse(note);
        this.IsNoteBlank = true;
        this.CheckNote((XElement)root.FirstNode);

        PdfPTable subTable = new PdfPTable(2);
        subTable.DefaultCell.FixedHeight = 250;
        subTable.SetWidths(new float[] { 1, 20 });
        subTable.WidthPercentage = 100;

        List<IElement> htmlArrayList = iTextSharp.text.html.simpleparser.HTMLWorker.ParseToList(new StringReader(description), null);
        // p.InsertRange(0, htmlArrayList);
        cell = PdfXML.GetRichCell(description);
        if (!this.IsNoteBlank) {
            string url = this.BaseUrl + "images/note.png";
            iTextSharp.text.Image img = iTextSharp.text.Image.GetInstance(url);
            int noteSize = 20;
            img.ScaleToFit(noteSize, noteSize);
            PdfPCell c = new PdfPCell();

            Phrase ph = new Phrase();
            ph.Add(new Chunk(img, -1, -5));
            c.AddElement(ph);
            c.Border = 0;
            c.Padding = 3;
            c.VerticalAlignment = PdfPCell.ALIGN_TOP;
            subTable.AddCell(c);
            c = PdfXML.GetRichCell(note);
            subTable.AddCell(c);
        }
        cell.AddElement(subTable);
        t.AddCell(cell);
    }
    void AddDescriptionAndNoteBak(PdfPTable t, string description, string note) {
        PdfPCell cell = new PdfPCell();
        iTextSharp.text.Paragraph p = new iTextSharp.text.Paragraph("", this.PdfFont);

        description = this.HtmlDecodeRichText(description, true);
        note = this.HtmlDecodeRichText(note, true);
        note = "<r>" + note + "</r>";
        XDocument root = XDocument.Parse(note);
        this.IsNoteBlank = true;
        this.CheckNote((XElement)root.FirstNode);

        PdfPTable subTable = new PdfPTable(2);
        subTable.DefaultCell.FixedHeight = 250;
        subTable.SetWidths(new float[] { 1, 20 });
        subTable.WidthPercentage = 100;

        List<IElement> htmlArrayList = iTextSharp.text.html.simpleparser.HTMLWorker.ParseToList(new StringReader(description), null);
        p.InsertRange(0, htmlArrayList);
        cell.AddElement(p);
        if (!this.IsNoteBlank) {
            string url = this.BaseUrl + "images/note.png";
            iTextSharp.text.Image img = iTextSharp.text.Image.GetInstance(url);
            int noteSize = 20;
            img.ScaleToFit(noteSize, noteSize);
            PdfPCell c = new PdfPCell();

            Phrase ph = new Phrase();
            ph.Add(new Chunk(img, -1, -5));
            c.AddElement(ph);
            c.Border = 0;
            c.Padding = 0;
            c.VerticalAlignment = PdfPCell.ALIGN_TOP;
            subTable.AddCell(c);

            htmlArrayList = iTextSharp.text.html.simpleparser.HTMLWorker.ParseToList(new StringReader(note), this.GetStyleSheet(true));
            p = new iTextSharp.text.Paragraph("", this.PdfFont);
            //List<IElement> htmlArrayList0 = iTextSharp.text.html.simpleparser.HTMLWorker.ParseToList(new StringReader("<br/>"), null);

            p = new Paragraph();
            //p.SetLeading(30, 0);
            p.SetLeading(0.1f, 0);

            p.InsertRange(0, htmlArrayList);
            c = new PdfPCell();
            c.VerticalAlignment = PdfPCell.ALIGN_TOP;
            // p.Add(Environment.NewLine);
            c.AddElement(p);
            c.Padding = 0;
            c.PaddingTop = 0;

            c.Border = 0;
            subTable.AddCell(c);
        }
        cell.AddElement(subTable);
        t.AddCell(cell);
    }
    iTextSharp.text.Phrase AddLabel(string s) {
        return new iTextSharp.text.Phrase(s + "  ", this.PdfFontBold);
    }
    iTextSharp.text.Phrase AddText(string s) {
        return new iTextSharp.text.Phrase(s, this.PdfFont);
    }
}

