using System;
using System.Xml;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Collections;
using System.Xml.Linq;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;
using iTextSharp.tool.xml.css;
using iTextSharp.tool.xml.html;
using iTextSharp.tool.xml.parser;
using iTextSharp.tool.xml.pipeline;
using iTextSharp.tool.xml.pipeline.css;
using iTextSharp.tool.xml.pipeline.end;
using iTextSharp.tool.xml.pipeline.html;

namespace XMLWorkerRichText {
    public class UnicodeFontProvider : FontFactoryImp {
        static UnicodeFontProvider() {
            // روش صحيح تعريف فونت   
            var systemRoot = Environment.GetEnvironmentVariable("SystemRoot");
            FontFactory.Register(Path.Combine(systemRoot, "fonts\\tahoma.ttf"));
            // ثبت ساير فونت‌ها در اينجا
            //FontFactory.Register(Path.Combine(Environment.CurrentDirectory, "fonts\\irsans.ttf"));
        }

        public override Font GetFont(string fontname, string encoding, bool embedded, float size, int style, BaseColor color, bool cached) {
            if (string.IsNullOrWhiteSpace(fontname))
                return new Font(Font.FontFamily.UNDEFINED, size, style, color);
            return FontFactory.GetFont(fontname, BaseFont.IDENTITY_H, BaseFont.EMBEDDED, size, style, color);
        }
    }

    /// <summary>
    /// تامين كننده مسير عكس‌هاي ذكر شده در فايل اچ تي ام ال
    /// </summary>
    public class ImageProvider : AbstractImageProvider {
        public override string GetImageRootPath() {
            var path = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            return path + "\\"; // مهم است كه اين مسير به بك اسلش ختم شود تا درست كار كند
        }
    }

    /// <summary>
    /// معادل پي دي افي المان‌هاي اچ تي ام ال را جمع آوري مي‌كند
    /// </summary>
    public class ElementsCollector : IElementHandler {
        private readonly Paragraph _paragraph;

        public ElementsCollector() {
            _paragraph = new Paragraph {
                Alignment = Element.ALIGN_LEFT  // سبب مي‌شود تا در حالت راست به چپ از سمت راست صفحه شروع شود
            };
        }

        /// <summary>
        /// اين پاراگراف حاوي كليه المان‌هاي متن است
        /// </summary>
        public Paragraph Paragraph {
            get { return _paragraph; }
        }

        /// <summary>
        /// بجاي اينكه خود كتابخانه اصلي كار افزودن المان‌ها را به صفحات انجام دهد
        /// قصد داريم آن‌ها را ابتدا جمع آوري كرده و سپس به صورت راست به چپ به صفحات نهايي اضافه كنيم
        /// </summary>
        /// <param name="htmlElement"></param>
        public void Add(IWritable htmlElement) {
            var writableElement = htmlElement as WritableElement;
            if (writableElement == null)
                return;

            foreach (var element in writableElement.Elements()) {
                fixNestedTablesRunDirection(element);
                _paragraph.Add(element);
            }
        }

        /// <summary>
        /// نياز است سلول‌هاي جداول تو در توي پي دي اف نيز راست به چپ شوند
        /// </summary>        
        private void fixNestedTablesRunDirection(IElement element) {
            var table = element as PdfPTable;
            if (table == null)
                return;

            table.RunDirection = PdfWriter.RUN_DIRECTION_RTL;
            foreach (var row in table.Rows) {
                foreach (var cell in row.GetCells()) {
                    cell.RunDirection = PdfWriter.RUN_DIRECTION_RTL;
                    foreach (var item in cell.CompositeElements) {
                        fixNestedTablesRunDirection(item);
                    }
                }
            }
        }
    }

    public static class XMLWorkerUtils {
        /// <summary>
        /// نحوه تعريف يك فايل سي اس اس خارجي
        /// </summary>
        public static ICssFile GetCssFile(string filePath) {
            using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)) {
                return XMLWorkerHelper.GetCSS(stream);
            }
        }
    }

    class PdfXML {
        /// <summary>
        ///  XMLWorker RTL sample.
        /// </summary>
        /// <param name="args"></param>
        /// 
        public static PdfPCell GetRichCell(string html) {
            var cell = new PdfPCell {
                Border = 0,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
                iTextSharp.text.Paragraph p = new iTextSharp.text.Paragraph();
                TextReader sr = new StringReader(html);

                // XMLWorkerHelper.GetInstance().ParseXHtml(elementsHandler, sr);
                System.Collections.Generic.List<IElement> htmlArrayList = iTextSharp.text.html.simpleparser.HTMLWorker.ParseToList(new StringReader(html), null);
                p.SetLeading(0.1f, 0f);
                p.InsertRange(0, htmlArrayList);
                cell.AddElement(p);
                cell.PaddingLeft = 4;
            return cell;
        }
//        public static PdfPCell GetRichCellBak(string html) {
//            var cssResolver = new StyleAttrCSSResolver();
//            // cssResolver.AddCss(XMLWorkerUtils.GetCssFile(@"c:\path\pdf.css"));
//            // html=@"<table border=1 width=100% ><tr><td>tet</td><td>test2</td></tr></table>";
//            cssResolver.AddCss(@"body 
//                                                 {
//                                                    padding: 2px 4px;
//                                                    color1: #d14;
//                                                    white-space: nowrap;                             
//                                                    border: 1px solid ;
//            line-height:14px;
//                                                 }",
//                                 "utf-8", true);
//            var elementsHandler = new ElementsCollector();

//            var htmlContext = new HtmlPipelineContext(new CssAppliersImpl(new UnicodeFontProvider()));
//            htmlContext.SetImageProvider(new ImageProvider());
//            htmlContext.CharSet(Encoding.UTF8);
//            htmlContext.SetAcceptUnknown(true).AutoBookmark(true).SetTagFactory(Tags.GetHtmlTagProcessorFactory());
//            var pipeline = new CssResolverPipeline(cssResolver,
//                                                   new HtmlPipeline(htmlContext, new ElementHandlerPipeline(elementsHandler, null)));
//            var worker = new XMLWorker(pipeline, parseHtml: true);
//            var cell = new PdfPCell {
//                Border = 0,
//                HorizontalAlignment = Element.ALIGN_LEFT
//            };
//            var parser = new XMLParser();
//            parser.AddListener(worker);
//            try {
//                parser.Parse(new StringReader(html));



//                cell.AddElement(elementsHandler.Paragraph);
//            } catch (Exception ex) {
//                //iTextSharp.text.Paragraph p = new iTextSharp.text.Paragraph();
//                //TextReader sr = new StringReader(html);

//                //// XMLWorkerHelper.GetInstance().ParseXHtml(elementsHandler, sr);
//                //System.Collections.Generic.List<IElement> htmlArrayList = iTextSharp.text.html.simpleparser.HTMLWorker.ParseToList(new StringReader(html), null);
//                //p.SetLeading(0.1f, 0f);
//                //p.InsertRange(0, htmlArrayList);
//                //p.Add(Environment.NewLine);
//                //cell.AddElement(p);
//                //cell.PaddingLeft = 4;
//                //cell.AddElement(p);

//                Sgml.SgmlReader sgmlReader = new Sgml.SgmlReader();
//                sgmlReader.DocType = "HTML";
//                sgmlReader.WhitespaceHandling = WhitespaceHandling.All;
//                sgmlReader.CaseFolding = Sgml.CaseFolding.ToLower;
//                TextReader sr = new StringReader(html);

//                sgmlReader.InputStream = sr;
//                // create document
//                XmlDocument doc = new XmlDocument();
//                doc.PreserveWhitespace = true;
//                doc.XmlResolver = null;
//                doc.Load(sgmlReader);

//                html = doc.OuterXml;
//                 parser = new XMLParser();
//                parser.AddListener(worker);
//                //html = "<p>test</p>";
//                parser.Parse(new StringReader(html));
//                cell.AddElement(elementsHandler.Paragraph);
//            }
//            return cell;
//        }
    }
}