using PdfCounter.Models;

namespace PdfCounter.Services;

public static class PdfScanner
{
    private static readonly StringComparer PathComparer =
        OperatingSystem.IsWindows()
            ? StringComparer.OrdinalIgnoreCase
            : StringComparer.Ordinal;

    public static ScanResult Scan(IEnumerable<string> droppedPaths)
    {
        var roots = NormalizeRoots(droppedPaths);
        var pdfPaths = CollectPdfPaths(roots);
        var entries = BuildEntries(pdfPaths, roots);
        var folders = BuildFolderSummaries(entries);
        var ok = entries.Where(e => e.IsOk).ToList();

        return new ScanResult
        {
            Files = entries,
            Folders = folders,
            TotalPages = ok.Sum(e => e.PageCount ?? 0),
            TotalFiles = entries.Count,
            ErrorCount = entries.Count(e => !e.IsOk),
        };
    }

    private static List<string> NormalizeRoots(IEnumerable<string> droppedPaths)
    {
        var roots = new List<string>();
        foreach (var path in droppedPaths)
        {
            if (string.IsNullOrWhiteSpace(path))
                continue;

            var full = Path.GetFullPath(path.Trim().Trim('"'));
            if (!File.Exists(full) && !Directory.Exists(full))
                continue;

            roots.Add(full);
        }

        return roots.Distinct(PathComparer).ToList();
    }

    private static List<string> CollectPdfPaths(List<string> roots)
    {
        var result = new HashSet<string>(PathComparer);

        foreach (var root in roots)
        {
            if (File.Exists(root))
            {
                if (IsPdf(root))
                    result.Add(root);
                continue;
            }

            foreach (var file in Directory.EnumerateFiles(root, "*", SearchOption.AllDirectories))
            {
                if (IsPdf(file))
                    result.Add(file);
            }
        }

        return result.OrderBy(p => p, PathComparer).ToList();
    }

    private static List<PdfFileEntry> BuildEntries(List<string> pdfPaths, List<string> roots)
    {
        var entries = new List<PdfFileEntry>(pdfPaths.Count);

        foreach (var fullPath in pdfPaths)
        {
            var (relative, folderKey) = ResolveDisplayPaths(fullPath, roots);
            try
            {
                var pages = PdfPageCounter.CountPages(fullPath);
                entries.Add(new PdfFileEntry
                {
                    FullPath = fullPath,
                    RelativePath = relative,
                    FolderKey = folderKey,
                    PageCount = pages,
                });
            }
            catch (Exception ex)
            {
                entries.Add(new PdfFileEntry
                {
                    FullPath = fullPath,
                    RelativePath = relative,
                    FolderKey = folderKey,
                    Error = ex.Message,
                });
            }
        }

        return entries;
    }

    private static (string RelativePath, string FolderKey) ResolveDisplayPaths(
        string fullPath,
        List<string> roots)
    {
        var directory = Path.GetDirectoryName(fullPath) ?? string.Empty;
        var fileName = Path.GetFileName(fullPath);

        foreach (var root in roots.OrderByDescending(r => r.Length))
        {
            if (File.Exists(root) && PathComparer.Equals(root, fullPath))
                return (fileName, ".");

            if (!Directory.Exists(root))
                continue;

            var rootFull = Path.GetFullPath(root);
            if (!directory.StartsWith(rootFull, PathComparer))
                continue;

            var relativeDir = directory.Length > rootFull.Length
                ? directory[(rootFull.Length + 1)..]
                : string.Empty;

            var relativePath = string.IsNullOrEmpty(relativeDir)
                ? fileName
                : Path.Combine(relativeDir, fileName);

            var folderKey = string.IsNullOrEmpty(relativeDir) ? "." : relativeDir;
            return (relativePath, folderKey);
        }

        return (fullPath, directory);
    }

    private static List<FolderSummary> BuildFolderSummaries(List<PdfFileEntry> entries)
    {
        return entries
            .GroupBy(e => e.FolderKey, PathComparer)
            .OrderBy(g => g.Key, PathComparer)
            .Select(g => new FolderSummary
            {
                FolderPath = g.Key,
                FileCount = g.Count(),
                PageCount = g.Where(e => e.IsOk).Sum(e => e.PageCount ?? 0),
                ErrorCount = g.Count(e => !e.IsOk),
            })
            .ToList();
    }

    private static bool IsPdf(string path) =>
        Path.GetExtension(path).Equals(".pdf", StringComparison.OrdinalIgnoreCase);
}
