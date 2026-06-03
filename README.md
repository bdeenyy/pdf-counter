# PDF Counter

[![Build Windows](https://github.com/bdeenyy/pdf-counter/actions/workflows/build.yml/badge.svg)](https://github.com/bdeenyy/pdf-counter/actions/workflows/build.yml)

Минималистичное Windows-приложение для подсчёта листов в PDF.

## Возможности

- Перетащите **один PDF**, **несколько файлов** или **папку** (включая подпапки)
- Сводка **по папкам** и детализация **по файлам**
- Итого: количество файлов и суммарное число листов

## Скачать готовый exe (без сборки)

После каждого push в `main` GitHub Actions собирает portable `PdfCounter.exe`:

1. Откройте вкладку [Actions](https://github.com/bdeenyy/pdf-counter/actions) → workflow **Build Windows** → последний успешный запуск.
2. Внизу страницы скачайте артефакт **PdfCounter-win-x64** (внутри один файл `PdfCounter.exe`).

Релиз с прикреплённым exe (по желанию):

```bash
git tag v1.0.0
git push origin v1.0.0
```

На [Releases](https://github.com/bdeenyy/pdf-counter/releases) появится версия с `PdfCounter.exe` и заметками.

## Требования

- Windows 10/11
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) — только для локальной сборки

## Сборка

```powershell
cd pdf-counter
dotnet build -c Release
```

Запуск из исходников:

```powershell
dotnet run --project src/PdfCounter/PdfCounter.csproj
```

## Публикация (один exe)

```powershell
dotnet publish src/PdfCounter/PdfCounter.csproj -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true
```

Готовый файл: `src/PdfCounter/bin/Release/net8.0-windows/win-x64/publish/PdfCounter.exe`

Для 32-bit систем замените `win-x64` на `win-x86`.

## Использование

1. Запустите `PdfCounter.exe`
2. Перетащите PDF или папку в зону вверху окна
3. Смотрите вкладки «По папкам» / «По файлам» и итог внизу
