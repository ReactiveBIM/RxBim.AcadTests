# RxBim.AcadTests

Пример команд для запуска тестов Revit 2025 локально

1. `nuke SetVersion2025` - обязательно перед выполнением тестов для подключения правильных версий пакетов
2. `nuke IntegrationTests --testToolName revit --onlySelectedProjects --version 2025`

Опции:

- `--testToolVersion 1.0.2-dev001` - установить заданную версию Test Tool CLI (если не задать, устанавливается последняя
  опубликованная релизная версия)
- `--skipUpdateTool` - не обновлять Test Tool CLI: использовать версию, установленную в предыдущем запуске (или вручную)
- `--isDebug` - запуск в режиме отладки. Выводятся в консоль отладочные данные запускаемых приложений. Запускается
  заданный в системе отладчик (VS или Rider) при выполнении команд.
- `--testToolName revit` или `--testToolName acad` - задаёт приложение для теста (Revit или AutoCAD)
- `--version 2025` - задаёт версию приложения для тестов: 2019, 2020, 2022-2025...
- `--onlySelectedProjects` - добавляет возможность выбора проекта для запуска теста. Если не задать эту опцию, будут
  запущены все тестовые проекты в решении. Игнорируется, если задан параметр `--projectNames`.
- `--projectNames RxBim.Example.Revit.IntegrationTests` - задаёт проекты для запуска тестов по названиям. Несколько проектов можно указать через запятую. 

## Внимание!
Для корректной работы тестовых проектов в версиях >= 2025 в свойствах проекта требуется задать:
`<CopyLocalLockFileAssemblies>true</CopyLocalLockFileAssemblies>`

## Установка Test Tool CLI локально (на примере Revit):

Автоматически:
1. Выполнить таргет: `nuke UpdateToolsLocal --testToolName revit`

Или вручную:
1. Поднять версию: Directory.Build.props -> Version
2. Открыть консоль (CMD, PowerShell, Bash ...) в папке проекта RxBim.RevitTests.Console.
3. Упаковка в nupkg `dotnet pack --output ./nupkg`
4. Установка инструмента `dotnet tool install --global --add-source ./nupkg RxBim.RevitTests.Console --prerelease`. Если версия стабильная, опция `--prerelease` не нужна.

## Удаление установленной версии Test Tool CLI:

- `dotnet tool uninstall -g RxBim.RevitTests.Console` - для Revit
- `dotnet tool uninstall -g RxBim.AutocadTests.Console` - для AutoCAD