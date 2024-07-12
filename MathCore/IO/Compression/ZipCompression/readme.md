<style>
mark{
    color:red;
}

#zip{
    color:red;
}
</style>

# Структура файла Zip



<mark>what is DataBase</mark>

Файл состоит из набора структур (файлов).

Каждый файл представляет собой конструкцию
- `LocalFileHeader`
- `Тело файла`
- Опциональная структура `Data descriptor`.

`LocalFileHeader` хранит информацию о файле:
- размер сжатый/не сжатый
- имя
- crc32:`0xEDB88320`
- о наличии опциональной стркутуры `Data descriptor` после завершения тела файла

`Data descriptor` хранит информацию о:
- размере сжатом/не сжатом
- crc32:`0xEDB88320`

Данная структура появляется лишь в том случае, если архив записывается 
в однопоточном режиме, и отсутствует возможность вернуться в `LocalFileHeader`
и дописать туда данные, которые становятся известны лишь по завершении
процесса сжатия потока байт файла. К примеру, когда архив формируется
"на лету" записываясь сразу в выходной поток сетевого соединения.

## Структура упаковки файлов

- LocalFileHeader
    + Data
    + Data descriptor (crc32, compressed, uncompressed)
- LocalFileHeader
    + Data
    + Data descriptor (crc32, compressed, uncompressed)
- ...
- LocalFileHeader
    + Data
    + Data descriptor (crc32, compressed, uncompressed)

- Archive description header
- Archive extra data record
- Central directory
    + File entry 1
    + File entry 2
    + ...
    + File entry N

## `LocalFileHeader`

Запись файла начинается с 4 байт сигнатуры `0x04034b50`

Off | Len   | Description
---:|:------|------------
 0  | 4     | Signature 0x04034b50
 4  | 2     | Версия для извлечения 0x1404
 6  | 2     | Бит общего назначения (0x0800 - UTF8; 0 - иначе)
 8  | 2     | Метод сжатия (0 - без сжатия; 8 - deflate)
10  | 2     | Время модификации файла
12  | 2     | Дата модификации файла
14  | 4     | CRC32(0xEDB88320)
18  | 4     | Сжатый размер файла (либо 0xFFFFFFFF если не известен на начало записи потока в архив)
22  | 4     | Несжатый размер файла (либо 0xFFFFFFFF если не известен на начало записи потока в архив)
26  | 4     | Длина массива байт имени файла
30  | 4     | Длина поля с дополнительными данными

### Flags

#### Бит общего назначения (off:6;len:2)

- Bit  0: encrypted file
- Bit  1: compression option
- Bit  2: compression option
- Bit  3: data descriptor
- Bit  4: enhanced deflation
- Bit  5: compressed patched data
- Bit  6: strong encryption
- Bit  7-10: unused
- Bit 11: language encoding
- Bit 12: reserved
- Bit 13: mask header values
- Bit 14-15: reserved

### Метод сжатия

- 00: no compression
- 01: shrunk
- 02: reduced with compression factor 1
- 03: reduced with compression factor 2
- 04: reduced with compression factor 3
- 05: reduced with compression factor 4
- 06: imploded
- 07: reserved
- 08: deflated
- 09: enhanced deflated
- 10: PKWare DCL imploded
- 11: reserved
- 12: compressed using BZIP2
- 13: reserved
- 14: LZMA
- 15-17: reserved
- 18: compressed using IBM TERSE
- 19: IBM LZ77 z
- 98: PPMd version I, Rev 1

### Время модификации файла

для хранения времени используется MS-DOS формат:
- Bits 00-04: секунды, делёные на 2
- Bits 05-10: минуты
- Bits 11-15: часы

0x7d1c = 0111110100011100
- часы = (01111)10100011100 = 15
- минуты = 01111(101000)11100 = 40
- секунды = 01111101000(11100) = 28 = 56 секунд

15:40:56

### Дата модификации файла

для хранения даты используется MS-DOS фломат:
- Bits 00-04: день
- Bits 05-08: месяц
- Bits 09-15: год, начиная от 1980

0x354b = 0011010101001011
- год = (0011010)101001011 = 26 :: 26 + 1980 = 2006
- месяц = 0011010(1010)01011 = 10
- день = 00110101010(01011) = 11

11.10.2006

## `Central directory file header`

Запись файла начинается с 4 байт сигнатуры `0x02014b50`

Off | Len   | Description
---:|:------|------------
 0  | 4     | Сигнатура `0x02014b50`
 4  | 2     | Версия
 6  | 2     | Версия для распаковки
 8  | 2     | Флаги
10  | 2     | Метод сжатия
12  | 2     | Время модификации
14  | 2     | Дата модификации
16  | 4     | CRC32(0xEDB88320)
20  | 4     | Сжатый размер
24  | 4     | Несжатый размер
28  | 2     | Длина имени файла
30  | 2     | Длина дополнительного поля данных
32  | 2     | Длина комментария к файлу
34  | 2     | Номер текущего диска
36  | 2     | Внутренние атрибуты файла
38  | 4     | Внешние атрибуты файла
42  | 4     | Смещение от начала файла архива до места расположения данных упакованного файла. Там должна быть сигнатура `0x04034b50`.

### Версия

старый байт:
- 0 - MS-DOS and OS/2 (FAT / VFAT / FAT32 file systems)
- 1 - Amiga
- 2 - OpenVMS
- 3 - UNIX
- 4 - VM/CMS
- 5 - Atari ST
- 6 - OS/2 H.P.F.S.
- 7 - Macintosh
- 8 - Z-System
- 9 - CP/M
- 10 - Windows NTFS
- 11 - MVS (OS/390 - Z/OS)
- 12 - VSE
- 13 - Acorn Risc
- 14 - VFAT
- 15 - alternate MVS
- 16 - BeOS
- 17 - Tandem
- 18 - OS/400
- 19 - OS/X (Darwin)
- 20 - 255: unused

младший байт:
- Версия спецификации zip

### General propose bit
- Bit 00: encrypted file
- Bit 01: compression option
- Bit 02: compression option
- Bit 03: data descriptor
- Bit 04: enhanced deflation
- Bit 05: compressed patched data
- Bit 06: strong encryption
- Bit 07-10: unused
- Bit 11: language encoding
- Bit 12: reserved
- Bit 13: mask header values
- Bit 14-15: reserved

### Compression method
- 00: no compression
- 01: shrunk
- 02: reduced with compression factor 1
- 03: reduced with compression factor 2
- 04: reduced with compression factor 3
- 05: reduced with compression factor 4
- 06: imploded
- 07: reserved
- 08: deflated
- 09: enhanced deflated
- 10: PKWare DCL imploded
- 11: reserved
- 12: compressed using BZIP2
- 13: reserved
- 14: LZMA
- 15-17: reserved
- 18: compressed using IBM TERSE
- 19: IBM LZ77 z
- 98: PPMd version I, Rev 1

### Internal attr
- Bit 0: apparent ASCII/text file
- Bit 1: reserved
- Bit 2: control field records precede logical records
- Bits 3-16: unused

## Завершающая структура индекса архива

End of central directory record

В конце файла расположена структура окончания индекса файла

Завершающая запись начинается с сигнатуры `0x06054b50`

Длина записи 22 байта

Off | Len   | Description
---:|:------|------------
 0  | 4     | Сигнатура `0x06054b50`
 4  | 2     | Номер тома, либо `0xffff` для `zip64`
 6  | 2     | Номер тома с главным индексом
 8  | 2     | Количество записей в данном томе
10  | 2     | Общее количество записей
12  | 4     | Размер главного индекса
16  | 4     | Смещение главного индекса от начала файла
20  | 2     | Размер комментария n
22  | n     | Комментарий