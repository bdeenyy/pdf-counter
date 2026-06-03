# PDF Counter

[![Build Windows](https://github.com/bdeenyy/pdf-counter/actions/workflows/build.yml/badge.svg)](https://github.com/bdeenyy/pdf-counter/actions/workflows/build.yml)

Минималистичное Windows-приложение для подсчёта листов в PDF.

## Возможности

- Перетащите **один PDF**, **несколько файлов** или **папку** (включая подпапки)
- Сводка **по папкам** и детализация **по файлам**
- Итого: количество файлов и суммарное число листов

## Версия и релизы

Текущая версия хранится в файле [`VERSION`](VERSION) (сейчас **1.0.0**). Она попадает в заголовок окна приложения и в сборку CI.

### Скачать exe без релиза

После push в `main`: [Actions](https://github.com/bdeenyy/pdf-counter/actions) → **Build Windows** → артефакт `PdfCounter-<версия>-win-x64`.

### Опубликовать GitHub Release

**Вариант 1 — тег (рекомендуется):**

```bash
# обновите VERSION при необходимости, затем:
git add VERSION && git commit -m "Release v1.0.0"
git tag v1.0.0
git push origin main
git push origin v1.0.0
```

**Вариант 2 — скрипт (Windows PowerShell):**

```powershell
.\scripts\release.ps1 -Version 1.0.0
```

**Вариант 3 — вручную в GitHub:** Actions → **Build Windows** → **Run workflow** → укажите версию → включите **Создать GitHub Release**.

На [Releases](https://github.com/bdeenyy/pdf-counter/releases) появится `PdfCounter-<версия>-win-x64.exe`.

> Job **release** запускается только при push тега `v*` или при ручном запуске с флагом релиза. Обычный push в `main` собирает только артефакт — это ожидаемо.

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
