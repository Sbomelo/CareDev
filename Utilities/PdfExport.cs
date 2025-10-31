// Utilities/PdfExport.cs
using CareDev.Heplers;
using CareDev.Models;
using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;
using System;
using System.Collections.Generic;
using System.IO;

public static class PdfExport
{
    // Public: create a PDF for a PersonalExportPackage (patient + admissions + discharges)
    public static byte[] CreatePersonalPdf(PersonalExportPackage p)
    {
        if (p == null) throw new ArgumentNullException(nameof(p));

        using var doc = new PdfDocument();
        var headerFont = new XFont("Arial", 14, XFontStyle.Bold);
        var font = new XFont("Arial", 11, XFontStyle.Regular);

        // Cover / title page
        var cover = doc.AddPage();
        var gfxCover = XGraphics.FromPdfPage(cover);
        gfxCover.DrawString("Exported Patient Data", headerFont, XBrushes.Black, new XRect(40, 40, cover.Width - 80, 30), XStringFormats.TopLeft);
        gfxCover.DrawString($"Export time (UTC): {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} ", font, XBrushes.Black, new XRect(40, 80, cover.Width - 80, 20), XStringFormats.TopLeft);

        // Patient section (if available)
        if (p.Patient != null)
        {
            var page = doc.AddPage();
            var gfx = XGraphics.FromPdfPage(page);
            int y = 40;
            gfx.DrawString("Patient Profile", headerFont, XBrushes.Black, new XRect(40, y, page.Width - 80, 20), XStringFormats.TopLeft);
            y += 30;

            // safe access - handle properties that might be null
            gfx.DrawString($"Patient ID: {SafeToString(p.Patient.PatientId)}", font, XBrushes.Black, new XRect(40, y, page.Width - 80, 20), XStringFormats.TopLeft); y += 18;
            gfx.DrawString($"Name: {SafeToString(p.Patient.Name)}", font, XBrushes.Black, new XRect(40, y, page.Width - 80, 20), XStringFormats.TopLeft); y += 18;
            if (p.Patient.DateOfBirth != null && p.Patient.DateOfBirth != default)
                gfx.DrawString($"DOB: {p.Patient.DateOfBirth:yyyy-MM-dd}", font, XBrushes.Black, new XRect(40, y, page.Width - 80, 20), XStringFormats.TopLeft);
            y += 18;
            gfx.DrawString($"Phone: {SafeToString(p.Patient.PhoneNumber)}", font, XBrushes.Black, new XRect(40, y, page.Width - 80, 20), XStringFormats.TopLeft);
        }

        // Admissions section
        if (p.Admissions != null)
            RenderAdmissionsSection(doc, p.Admissions, headerFont, font);

        // Discharges section
        if (p.Discharges != null)
            RenderDischargesSection(doc, p.Discharges, headerFont, font);

        // Audits section (optional)
        if (p.Audits != null)
            RenderAuditsSection(doc, p.Audits, headerFont, font);

        using var ms = new MemoryStream();
        doc.Save(ms);
        return ms.ToArray();
    }

    // Public: create a PDF for a list of admissions (admin filtered export)
    public static byte[] CreateAdmissionsPdf(IEnumerable<AdmissionDto> admissions)
    {
        using var doc = new PdfDocument();
        var headerFont = new XFont("Arial", 14, XFontStyle.Bold);
        var font = new XFont("Arial", 10, XFontStyle.Regular);

        RenderAdmissionsSection(doc, admissions, headerFont, font);

        using var ms = new MemoryStream();
        doc.Save(ms);
        return ms.ToArray();
    }

    private static void RenderAdmissionsSection(PdfDocument doc, IEnumerable<AdmissionDto> admissions, XFont headerFont, XFont font)
    {
        if (admissions == null) return;

        PdfPage page = null;
        XGraphics gfx = null;
        int y = 0;
        const int lineHeight = 14;
        const int marginTop = 40;
        const int bottomMargin = 40;

        void NewPage()
        {
            page = doc.AddPage();
            gfx = XGraphics.FromPdfPage(page);
            y = marginTop;
        }

        NewPage();
        gfx.DrawString("Admissions", headerFont, XBrushes.Black, new XRect(40, y, page.Width - 80, 20), XStringFormats.TopLeft);
        y += 30;

        // Table header
        gfx.DrawString("ID  Patient  AdmittedAt               DischargedAt            Ward   Bed   Doctor", font, XBrushes.Black, new XRect(40, y, page.Width - 80, 20), XStringFormats.TopLeft);
        y += 18;

        foreach (var a in admissions)
        {
            if (y > page.Height - bottomMargin)
            {
                NewPage();
            }

            var line = $"{SafeToString(a.AdmissionId),-4} {Truncate(SafeToString(a.PatientName), 18),-18} {SafeDate(a.AdmittedAt),-22} {SafeDate(a.DischargedAt),-22} {Truncate(SafeToString(a.Ward), 6),-6} {Truncate(SafeToString(a.Bed), 5),-5} {Truncate(SafeToString(a.DoctorName), 16),-16}";
            gfx.DrawString(line, font, XBrushes.Black, new XRect(40, y, page.Width - 80, lineHeight), XStringFormats.TopLeft);
            y += lineHeight;
        }
    }

    private static void RenderDischargesSection(PdfDocument doc, IEnumerable<DischargeDto> discharges, XFont headerFont, XFont font)
    {
        if (discharges == null) return;

        PdfPage page = doc.AddPage();
        var gfx = XGraphics.FromPdfPage(page);
        int y = 40;
        gfx.DrawString("Discharges", headerFont, XBrushes.Black, new XRect(40, y, page.Width - 80, 20), XStringFormats.TopLeft);
        y += 30;

        foreach (var d in discharges)
        {
            if (y > page.Height - 60)
            {
                page = doc.AddPage();
                gfx = XGraphics.FromPdfPage(page);
                y = 40;
            }

            var text = $"DischargeId: {SafeToString(d.DischargeId)} | Admission: {SafeToString(d.AdmissionId)} | At: {SafeDate(d.DischargedAt)} | Summary: {Truncate(SafeToString(d.Summary), 120)}";
            gfx.DrawString(text, font, XBrushes.Black, new XRect(40, y, page.Width - 80, 18), XStringFormats.TopLeft);
            y += 16;
        }
    }

    private static void RenderAuditsSection(PdfDocument doc, IEnumerable<AuditEntry> audits, XFont headerFont, XFont font)
    {
        if (audits == null) return;

        PdfPage page = doc.AddPage();
        var gfx = XGraphics.FromPdfPage(page);
        int y = 40;
        gfx.DrawString("Audit entries (last items)", headerFont, XBrushes.Black, new XRect(40, y, page.Width - 80, 20), XStringFormats.TopLeft);
        y += 30;

        foreach (var a in audits)
        {
            if (y > page.Height - 60)
            {
                page = doc.AddPage();
                gfx = XGraphics.FromPdfPage(page);
                y = 40;
            }

            var text = $"{SafeDate(a.CreatedAt)} | {SafeToString(a.UserName)} | {SafeToString(a.Action)} | {Truncate(SafeToString(a.TableName), 40)}";
            gfx.DrawString(text, font, XBrushes.Black, new XRect(40, y, page.Width - 80, 18), XStringFormats.TopLeft);
            y += 16;
        }
    }

    // Helper for safe string conversion
    private static string SafeToString(object obj)
    {
        if (obj == null) return string.Empty;
        return obj.ToString();
    }

    private static string SafeDate(DateTime? dt)
    {
        if (dt == null || dt == default) return "";
        return dt.Value.ToString("yyyy-MM-dd HH:mm");
    }

    private static string Truncate(string s, int max)
    {
        if (string.IsNullOrEmpty(s)) return "";
        return s.Length <= max ? s : s.Substring(0, max - 3) + "...";
    }
}
