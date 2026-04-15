
using System;
using System.IO;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows;

namespace ElectricVehicleCms.Helpers
{
    public static class RtfHelper
    {
        public static void SaveRichTextBoxToRtf(RichTextBox richTextBox, string relativePath)
        {
            string absolutePath = PathHelper.GetAbsolutePath(relativePath);
            string folderPath = Path.GetDirectoryName(absolutePath);

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            TextRange textRange = new TextRange(richTextBox.Document.ContentStart, richTextBox.Document.ContentEnd);

            using (FileStream fileStream = new FileStream(absolutePath, FileMode.Create))
            {
                textRange.Save(fileStream, DataFormats.Rtf);
            }
        }

        public static void LoadRtfIntoRichTextBox(RichTextBox richTextBox, string relativePath)
        {
            richTextBox.Document = new FlowDocument();

            string absolutePath = PathHelper.GetAbsolutePath(relativePath);

            if (!File.Exists(absolutePath))
            {
                return;
            }

            TextRange textRange = new TextRange(richTextBox.Document.ContentStart, richTextBox.Document.ContentEnd);

            using (FileStream fileStream = new FileStream(absolutePath, FileMode.Open))
            {
                textRange.Load(fileStream, DataFormats.Rtf);
            }
        }

        public static FlowDocument CreateFlowDocumentFromRtf(string relativePath)
        {
            FlowDocument document = new FlowDocument();
            string absolutePath = PathHelper.GetAbsolutePath(relativePath);

            if (!File.Exists(absolutePath))
            {
                return document;
            }

            TextRange textRange = new TextRange(document.ContentStart, document.ContentEnd);

            using (FileStream fileStream = new FileStream(absolutePath, FileMode.Open))
            {
                textRange.Load(fileStream, DataFormats.Rtf);
            }

            return document;
        }

        public static string GetPlainText(RichTextBox richTextBox)
        {
            return new TextRange(richTextBox.Document.ContentStart, richTextBox.Document.ContentEnd).Text;
        }

        public static bool HasText(RichTextBox richTextBox)
        {
            return !string.IsNullOrWhiteSpace(GetPlainText(richTextBox));
        }

        public static int CountWords(RichTextBox richTextBox)
        {
            string text = GetPlainText(richTextBox);

            return text
                .Split(new[] { ' ', '\n', '\r', '\t' }, StringSplitOptions.RemoveEmptyEntries)
                .Count();
        }

        public static Brush GetSelectionBrush(RichTextBox richTextBox)
        {
            object selectedBrush = richTextBox.Selection.GetPropertyValue(TextElement.ForegroundProperty);

            if (selectedBrush is Brush)
            {
                return (Brush)selectedBrush;
            }

            return Brushes.Black;
        }
    }
}
