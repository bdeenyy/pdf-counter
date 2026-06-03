using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using PdfCounter.Models;
using PdfCounter.Services;

namespace PdfCounter;

public partial class MainWindow : Window
{
    private readonly ObservableCollection<FolderSummary> _folders = new();
    private readonly ObservableCollection<PdfFileEntry> _files = new();
    private bool _isDropHighlighted;

    public MainWindow()
    {
        InitializeComponent();
        Title = $"PDF Counter v{AppVersion.Current}";
        FoldersGrid.ItemsSource = _folders;
        FilesGrid.ItemsSource = _files;
    }

    private void DropZone_DragEnter(object sender, DragEventArgs e)
    {
        if (HasFileDrop(e))
        {
            e.Effects = DragDropEffects.Copy;
            SetDropHighlight(true);
        }
        else
        {
            e.Effects = DragDropEffects.None;
        }

        e.Handled = true;
    }

    private void DropZone_DragOver(object sender, DragEventArgs e)
    {
        e.Effects = HasFileDrop(e) ? DragDropEffects.Copy : DragDropEffects.None;
        e.Handled = true;
    }

    private void DropZone_DragLeave(object sender, DragEventArgs e)
    {
        SetDropHighlight(false);
    }

    private async void DropZone_Drop(object sender, DragEventArgs e) =>
        await HandleDropAsync(e);

    private void Window_DragEnter(object sender, DragEventArgs e) =>
        DropZone_DragEnter(DropZone, e);

    private void Window_DragOver(object sender, DragEventArgs e) =>
        DropZone_DragOver(DropZone, e);

    private void Window_DragLeave(object sender, DragEventArgs e) =>
        SetDropHighlight(false);

    private async void Window_Drop(object sender, DragEventArgs e) =>
        await HandleDropAsync(e);

    private async Task HandleDropAsync(DragEventArgs e)
    {
        SetDropHighlight(false);

        if (!e.Data.GetDataPresent(DataFormats.FileDrop))
            return;

        var paths = (string[]?)e.Data.GetData(DataFormats.FileDrop);
        if (paths is null or { Length: 0 })
            return;

        await RunScanAsync(paths);
    }

    private async Task RunScanAsync(string[] paths)
    {
        StatusText.Text = "Сканирование…";
        TotalFilesText.Text = string.Empty;
        TotalPagesText.Text = string.Empty;
        IsEnabled = false;

        try
        {
            var result = await Task.Run(() => PdfScanner.Scan(paths));
            ApplyResult(result);
        }
        catch (Exception ex)
        {
            StatusText.Text = $"Ошибка: {ex.Message}";
        }
        finally
        {
            IsEnabled = true;
        }
    }

    private void ApplyResult(ScanResult result)
    {
        _folders.Clear();
        _files.Clear();

        foreach (var folder in result.Folders)
            _folders.Add(folder);

        foreach (var file in result.Files)
            _files.Add(file);

        if (result.TotalFiles == 0)
        {
            StatusText.Text = "PDF не найдено";
            TotalFilesText.Text = string.Empty;
            TotalPagesText.Text = string.Empty;
            return;
        }

        var errorNote = result.ErrorCount > 0 ? $", ошибок: {result.ErrorCount}" : string.Empty;
        StatusText.Text = $"Обработано за {DateTime.Now:HH:mm:ss}{errorNote}";
        TotalFilesText.Text = $"Файлов: {result.TotalFiles}";
        TotalPagesText.Text = $"Всего листов: {result.TotalPages}";
    }

    private static bool HasFileDrop(DragEventArgs e) =>
        e.Data.GetDataPresent(DataFormats.FileDrop);

    private void SetDropHighlight(bool active)
    {
        if (_isDropHighlighted == active)
            return;

        _isDropHighlighted = active;
        DropZone.Background = active
            ? (Brush)FindResource("DropHoverBrush")
            : (Brush)FindResource("SurfaceBrush");
        DropZone.BorderBrush = active
            ? (Brush)FindResource("AccentBrush")
            : (Brush)FindResource("BorderBrush");
    }
}
