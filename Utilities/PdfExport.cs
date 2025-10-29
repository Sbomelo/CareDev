using System.IO;
using CareDev.Heplers;
using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;


public static class PdfExport
{
    public static byte[] CreatePersonalPdf(PersonalExportPackage p)
    {
        using var doc = new PdfDocument();
        var font = new XFont("Arial", 12, XFontStyle.Regular);
        var headerFont = new XFont("Arial", 14, XFontStyle.Bold);

        //void AddHeaderPage(string title)
        //{
        //    var page = doc.AddPage();
        //    var gfx = XGraphics.FromPdfPage(page);
        //    gfx.DrawString(title, headerFont, XBrushes.Black, new XRect(40, 40, page.Width - 80, 30), XStringFormats.TopLeft);
        //}

        //// Cover
        //AddHeaderPage($"Export for {p.Patient.Name} - {DateTime.UtcNow:yyyy-MM-dd}");

        // Patient section
        var page1 = doc.AddPage();
        var g1 = XGraphics.FromPdfPage(page1);
        int y = 40;
        //g1.DrawString($"Patient Profile For {p.Patient.Name} {p.Patient.SurName} ", headerFont, XBrushes.Black, new XRect(40, y, page1.Width - 80, 20), XStringFormats.TopLeft);
        //y += 30;
        g1.DrawString($"Patient ID: {p.Patient.PatientId}", font, XBrushes.Black, new XRect(40, y, page1.Width - 80, 20), XStringFormats.TopLeft); y += 18;
        g1.DrawString($"Name: {p.Patient.Name}", font, XBrushes.Black, new XRect(40, y, page1.Width - 80, 20), XStringFormats.TopLeft); y += 18;
        g1.DrawString($"SurName: {p.Patient.SurName}", font, XBrushes.Black, new XRect(40, y, page1.Width - 80, 20), XStringFormats.TopLeft); y += 18;
        g1.DrawString($"Age: {p.Patient.Age}", font, XBrushes.Black, new XRect(40, y, page1.Width - 80, 20), XStringFormats.TopLeft); y += 18;
        g1.DrawString($"Gender: {p.Patient.Gender}", font, XBrushes.Black, new XRect(40, y, page1.Width - 80, 20), XStringFormats.TopLeft); y += 18;
        g1.DrawString($"Contact: {p.Patient.PhoneNumber}", font, XBrushes.Black, new XRect(40, y, page1.Width - 80, 20), XStringFormats.TopLeft); y += 18;
        g1.DrawString($"Adress: {p.Patient.Address}", font, XBrushes.Black, new XRect(40, y, page1.Width - 80, 20), XStringFormats.TopLeft); y += 18;
        g1.DrawString($"City: {p.Patient.City}", font, XBrushes.Black, new XRect(40, y, page1.Width - 80, 20), XStringFormats.TopLeft); y += 18;
        g1.DrawString($"Allergy: {p.Patient.Allergy}", font, XBrushes.Black, new XRect(40, y, page1.Width - 80, 20), XStringFormats.TopLeft); y += 18;
        g1.DrawString($"Medication: {p.Patient.Medications}", font, XBrushes.Black, new XRect(40, y, page1.Width - 80, 20), XStringFormats.TopLeft); y += 18;
        g1.DrawString($"ChronicCondition: {p.Patient.ChronicCondition}", font, XBrushes.Black, new XRect(40, y, page1.Width - 80, 20), XStringFormats.TopLeft); y += 18;

        // Admissions table (simple list)
        var page2 = doc.AddPage();
        var g2 = XGraphics.FromPdfPage(page2);
        y = 40;
        g2.DrawString("Admissions History", headerFont, XBrushes.Black, new XRect(40, y, page2.Width - 80, 20), XStringFormats.TopLeft); y += 26;

        foreach (var a in p.Admissions)
        {
            if (y > page2.Height - 60) { page2 = doc.AddPage(); g2 = XGraphics.FromPdfPage(page2); y = 40; }
            y += 18;
            g2.DrawString($"Admission Number: {a.AdmissionId}", font, XBrushes.Black, new XRect(40, y, page2.Width - 80, 18), XStringFormats.TopLeft); y += 18;
            g2.DrawString($"Admitted At: {a.AdmittedAt:yyyy-MM-dd HH:mm}", font, XBrushes.Black, new XRect(40, y, page2.Width - 80, 18), XStringFormats.TopLeft); y += 18;
            g2.DrawString($"In Ward: {a.Ward}", font, XBrushes.Black, new XRect(40, y, page2.Width - 80, 18), XStringFormats.TopLeft); y += 18;
            g2.DrawString($"In Bed: {a.Bed}", font, XBrushes.Black, new XRect(40, y, page2.Width - 80, 18), XStringFormats.TopLeft); y += 18;

        }

        // Discharges
        var page3 = doc.AddPage();
        var g3 = XGraphics.FromPdfPage(page3);
        y = 40;
        g3.DrawString("Discharges", headerFont, XBrushes.Black, new XRect(40, y, page3.Width - 80, 20), XStringFormats.TopLeft); y += 26;
        foreach (var d in p.Discharges)
        {
            if (y > page3.Height - 60) { page3 = doc.AddPage(); g3 = XGraphics.FromPdfPage(page3); y = 40; }
            g3.DrawString($"DischargeId: {d.DischargeId} | At: {d.DischargedAt:yyyy-MM-dd HH:mm} | Summary: {Truncate(d.Summary, 120)}", font, XBrushes.Black, new XRect(40, y, page3.Width - 80, 18), XStringFormats.TopLeft);
            y += 16;
        }

        using var ms = new MemoryStream();
        doc.Save(ms);
        return ms.ToArray();
    }

    static string Truncate(string s, int length) => string.IsNullOrEmpty(s) ? "" : (s.Length <= length ? s : s.Substring(0, length) + "...");
}
