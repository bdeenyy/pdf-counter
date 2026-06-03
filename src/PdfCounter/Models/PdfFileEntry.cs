namespace PdfCounter.Models;

public sealed class PdfFileEntry
{
    public required string FullPath { get; init; }
    public required string RelativePath { get; init; }
    public required string FolderKey { get; init; }
    public int? PageCount { get; init; }
    public string? Error { get; init; }

    public bool IsOk => PageCount.HasValue && string.IsNullOrEmpty(Error);
}

public sealed class FolderSummary
{
    public required string FolderPath { get; init; }
    public string DisplayPath => FolderPath == "." ? "(корень)" : FolderPath;
    public int FileCount { get; init; }
    public int PageCount { get; init; }
    public int ErrorCount { get; init; }
}

public sealed class ScanResult
{
    public required IReadOnlyList<PdfFileEntry> Files { get; init; }
    public required IReadOnlyList<FolderSummary> Folders { get; init; }
    public int TotalPages { get; init; }
    public int TotalFiles { get; init; }
    public int ErrorCount { get; init; }
}
