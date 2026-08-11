"""Построение трекинг-файла TestCoverageTracker.md (система модульного тестирования MathCore)."""
import re
from pathlib import Path

ROOT = Path(r"D:\src\lib\MathCore\MathCore")
SRC_DIR = ROOT / "MathCore"
TEST_DIR = ROOT / "Tests" / "MathCore.Tests"
OUT = ROOT / "TestCoverageTracker.md"

# Класс с атрибутом [TestClass]: атрибут на предыдущих строках, объявление class/record-класса
TEST_CLASS_RE = re.compile(
    r'\[TestClass\]\s*'
    r'(?:(?:static|sealed|abstract|partial|public|internal|private|protected)\s+)*'
    r'(?:class|record\s+class|record\s+struct)\s+([A-Za-z_][A-Za-z0-9_]*)',
    re.MULTILINE)
METHOD_RE = re.compile(r'\[(?:TestMethod|DataTestMethod|DataRow|DynamicData)\b')

# ================= 1. Тестовые файлы и классы =================
test_files = []          # (rel, n_methods, is_strict_tests)
test_classes = set()
for path in TEST_DIR.rglob("*.cs"):
    if "\\bin\\" in str(path) or "\\obj\\" in str(path):
        continue
    rel = path.relative_to(ROOT).as_posix()
    text = path.read_text(encoding="utf-8", errors="ignore")
    n_methods = len(METHOD_RE.findall(text))
    is_strict = path.name.endswith("Tests.cs")
    has_test_class = bool(TEST_CLASS_RE.search(text))
    for m in TEST_CLASS_RE.finditer(text):
        test_classes.add(m.group(1))
    # считаем файл тестовым, если он строгий тест ИЛИ содержит test-класс с методами
    if n_methods > 0 and (is_strict or has_test_class):
        test_files.append((rel, n_methods))

# Покрываемые имена типов из тестовых классов
covered_type_names = set()
for tc in test_classes:
    covered_type_names.add(re.sub(r'Tests$', '', tc))
    covered_type_names.add(tc)

MANUAL_COVERED = {
    'Complex', 'Matrix', 'MatrixArray', 'MatrixComplex', 'MatrixDecimal', 'MatrixFloat',
    'MatrixInt', 'MatrixLong', 'MatrixN', 'Polynom', 'StringPtr', 'Randomizer',
    'RandomNormal', 'RungeKutta', 'RungeKuttaVector2', 'RungeKuttaVector3',
    'RungeKuttaMatrix', 'RungeKuttaComplex', 'Euler', 'EulerModified',
    'Levenshtein', 'NamedLock', 'Messenger', 'Maybe', 'FList', 'EnumExtensions',
}
covered_type_names |= MANUAL_COVERED

# ================= 2. Исходные модули =================
PUBLIC_CLASS_RE = re.compile(
    r'^\s*public\s+(?:static\s+|sealed\s+|abstract\s+|readonly\s+|partial\s+)*'
    r'(?:class|struct|interface|enum)\s+([A-Za-z_][A-Za-z0-9_]*)', re.MULTILINE)
RECORD_RE = re.compile(
    r'^\s*public\s+(?:sealed\s+|abstract\s+|partial\s+)*record\s+(?:class\s+|struct\s+)?'
    r'([A-Za-z_][A-Za-z0-9_]*)', re.MULTILINE)
TRIVIAL = re.compile(r'(Attribute|Annotations|\.tt\.cs)$|^(Service/|Properties/)', re.I)

src_modules = {}  # logical -> (rel, types)
for path in SRC_DIR.rglob("*.cs"):
    if "\\bin\\" in str(path) or "\\obj\\" in str(path):
        continue
    rel = path.relative_to(ROOT).as_posix()
    if TRIVIAL.search(rel):
        continue
    text = path.read_text(encoding="utf-8", errors="ignore")
    types = [m.group(1) for m in PUBLIC_CLASS_RE.finditer(text)] + [m.group(1) for m in RECORD_RE.finditer(text)]
    if not types:
        continue
    if all('Attribute' in t for t in types):
        continue
    logical = path.stem.split('.')[0]
    src_modules[logical] = (rel, types)

def covered(logical, types):
    if logical in covered_type_names:
        return True
    for t in types:
        base = t.split('<')[0]
        if base in covered_type_names:
            return True
        if (base + 'Tests') in test_classes:
            return True
    return False

covered_mods = []
uncovered_mods = []
for logical, (rel, types) in src_modules.items():
    (covered_mods if covered(logical, types) else uncovered_mods).append((logical, rel, types))

# ================= 3. Генерация =================
total_methods = sum(x[1] for x in test_files)
test_files_sorted = sorted(test_files, key=lambda x: (x[1], x[0].lower()))
for i, (rel, n) in enumerate(test_files_sorted, 1):
    pass

sb = []
sb.append('# Трекинг-файл: система модульного тестирования MathCore')
sb.append('')
sb.append('> Перечень файлов тестовой системы проекта **MathCore**, требующих **добавления** или **доработки**.')
sb.append('>')
sb.append('> Сводка построена автоматически по проекту `Tests/MathCore.Tests` (MSTest). Учтены файлы `*Tests.cs` и файлы с тест-классами `[TestClass]`.')
sb.append('')
sb.append('## Общая статистика')
sb.append('')
sb.append(f'- **Тестовых файлов:** {len(test_files)}')
sb.append(f'- **Всего тест-методов / атрибутов тестов:** {total_methods}')
sb.append(f'- **Логических модулей с публичным API:** {len(src_modules)}')
sb.append(f'  - покрыто тестами: {len(covered_mods)}')
sb.append(f'  - **без тестов:** {len(uncovered_mods)}')
sb.append('')
sb.append('## Условные обозначения статуса')
sb.append('')
sb.append('- **🟡 Нужно доработать** — тестовый файл существует, но покрытие недостаточно (мало тест-методов)')
sb.append('- **⬜ Нужно создать** — для логического модуля тесты отсутствуют')
sb.append('- **✅ Готово** — тесты покрывают модуль (заполняется вручную)')
sb.append('')

sb.append('## Раздел A. Существующие тестовые файлы, требующие доработки')
sb.append('')
sb.append('> По возрастанию числа тест-методов: чем меньше, тем выше приоритет доработки.')
sb.append('')
sb.append('| # | Тестовый файл | Тест-методов | Статус |')
sb.append('|---|---------------|:---:|--------|')
for i, (rel, n) in enumerate(test_files_sorted, 1):
    disp = rel.replace('Tests/MathCore.Tests/', '')
    sb.append(f'| {i} | `{disp}` | {n} | 🟡 Нужно доработать |')

sb.append('## Раздел B. Логические модули без тестов (нужно создать)')
sb.append('')
sb.append(f'> {len(uncovered_mods)} логических модулей с публичным API без тестов в `Tests/MathCore.Tests`.')
sb.append('')
sb.append('| # | Логический модуль | Исходный файл (пример) | Статус |')
sb.append('|---|--------------------|------------------------|--------|')
for i, (logical, rel, types) in enumerate(sorted(uncovered_mods, key=lambda x: x[0].lower()), 1):
    disp_rel = rel.replace('MathCore/', '')
    sb.append(f'| {i} | `{logical}` | `{disp_rel}` | ⬜ Нужно создать |')
sb.append('')

OUT.write_text('\n'.join(sb), encoding='utf-8')
print('OK')
print('Тестовых файлов:', len(test_files))
print('Тестовых классов:', len(test_classes))
print('Модулей покрыто:', len(covered_mods), '| без тестов:', len(uncovered_mods))
