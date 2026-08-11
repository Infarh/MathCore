$ErrorActionPreference = 'Stop'
$log = "d:/src/lib/MathCore/MathCore/build_cs1591.log"
$out = "d:/src/lib/MathCore/MathCore/XMLDocumentationTracker.md"

$lines = @(Get-Content $log | Select-String "CS1591")
$groups = @(
    $lines | ForEach-Object {
        if ($_.Line -match '\\MathCore\\(MathCore\\.+?\.cs)\(\d+,\d+\)') { $matches[1] }
    } | Group-Object
)

$NL = [Environment]::NewLine
$sb = New-Object System.Text.StringBuilder
[void]$sb.AppendLine('# Трекинг-файл: XML-комментарии публичного API')
[void]$sb.AppendLine('')
[void]$sb.AppendLine('> Список файлов исходного кода проекта **MathCore**, требующих **добавления** или **доработки** XML-комментариев публичного API.')
[void]$sb.AppendLine('>')
[void]$sb.AppendLine('> Данные получены автоматически из предупреждений компилятора **CS1591** (отсутствие XML-комментария) при сборке `MathCore/MathCore.csproj` в конфигурации `Release`, framework `net10.0`.')
[void]$sb.AppendLine('')
[void]$sb.AppendLine('## Общая статистика')
[void]$sb.AppendLine('')
[void]$sb.AppendLine(('- **Всего предупреждений CS1591:** ' + $lines.Count))
[void]$sb.AppendLine(('- **Файлов с недостающими комментариями:** ' + $groups.Count))
[void]$sb.AppendLine('- **Методика:** сборка с генерацией XML-документации; предупреждение CS1591 выдаётся на каждый публичный тип/член без XML-комментария')
[void]$sb.AppendLine('')
[void]$sb.AppendLine('## Условные обозначения статуса')
[void]$sb.AppendLine('')
[void]$sb.AppendLine('- **⬜ Нужно добавить** — публичное API в файле не документировано')
[void]$sb.AppendLine('- **🟡 Нужно доработать** — комментарии неполные или отсутствуют для части членов')
[void]$sb.AppendLine('- **✅ Готово** — комментарии добавлены')
[void]$sb.AppendLine('')
[void]$sb.AppendLine('## Перечень файлов (по убыванию числа недостающих комментариев)')
[void]$sb.AppendLine('')
[void]$sb.AppendLine('| # | Файл | Кол-во CS1591 | Статус |')
[void]$sb.AppendLine('|---|------|:---:|------|')
$i = 0
foreach ($g in ($groups | Sort-Object Count -Descending)) {
    $i++
    $rel = ('MathCore/' + ($g.Name -replace '^MathCore\\MathCore\\', ''))   # путь относительно корня репозитория
    $rel = $rel -replace '\\', '/'
    [void]$sb.AppendLine(('| ' + $i + ' | ' + $rel + ' | ' + $g.Count + ' | ⬜ Нужно добавить |'))
}
[void]$sb.AppendLine('')

# Запись в UTF-8 BOM, чтобы Windows PowerShell корректно читал кириллицу
$utf8Bom = New-Object System.Text.UTF8Encoding($true)
[System.IO.File]::WriteAllText($out, $sb.ToString(), $utf8Bom)
Write-Output ('OK строк:' + ($sb.ToString().Split("`n").Count))
