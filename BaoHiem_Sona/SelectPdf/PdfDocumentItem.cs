using System.IO;

namespace SelectPdf
{
    internal class PdfDocumentItem : PdfDocument
    {
        private MemoryStream tempStream;
        private string v;

        public PdfDocumentItem(MemoryStream tempStream, string v)
        {
            this.tempStream = tempStream;
            this.v = v;
        }
    }
}