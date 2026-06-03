using UglyToad.PdfPig;

namespace PdfCounter.Services;

public static class PdfPageCounter
{
    public static int CountPages(string filePath)
    {
        using var document = PdfDocument.Open(filePath);
        return document.NumberOfPages;
    }
}
