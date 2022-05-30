using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TourPlanner.Client.UI.Services.Reporting
{
    public class PdfBackgroundHelper : PdfPageEventHelper
    {

        private PdfContentByte? _cb;
        private readonly List<PdfTemplate> _templates;

        public PdfBackgroundHelper()
        {
            this._templates = new List<PdfTemplate>();
        }

        public override void OnEndPage(PdfWriter writer, Document document)
        {
            base.OnEndPage(writer, document);

            _cb = writer.DirectContentUnder;
            PdfTemplate templateM = _cb.CreateTemplate(50, 50);
            _templates.Add(templateM);

            int pageN = writer.CurrentPageNumber;
            string pageText = $"Page {pageN} of ";
            BaseFont bf = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
            float len = bf.GetWidthPoint(pageText, 10);
            _cb.BeginText();
            _cb.SetFontAndSize(bf, 10);
            _cb.SetTextMatrix(document.LeftMargin, document.PageSize.GetBottom(document.BottomMargin - 20));
            _cb.ShowText(pageText);
            _cb.EndText();
            _cb.AddTemplate(templateM, document.LeftMargin + len, document.PageSize.GetBottom(document.BottomMargin - 20));
        }

        public override void OnCloseDocument(PdfWriter writer, Document document)
        {
            base.OnCloseDocument(writer, document);
            BaseFont bf = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
            foreach (PdfTemplate item in _templates)
            {
                item.BeginText();
                item.SetFontAndSize(bf, 10);
                item.SetTextMatrix(0, 0);
                item.ShowText($"{writer.PageNumber}");
                item.EndText();
            }

        }
    }
}
