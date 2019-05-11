# ISO-639

Implementation of the [ISO-639](https://www.iso.org/iso-639-language-codes.html) standard for language codes, with database of languages generated from the official tables found at [here](https://iso639-3.sil.org/).

Supports all parts of the standard, including 3-digit codes for `ISO-639-3` and `ISO-639-2` (both "T" and legacy "B" codes), and 2-digit `ISO-639-1` codes. Also includes properties denoting the scope and type of language as a strongly-typed enumerations, as well helper methods for easily finding languages based on their code, name, type, culture, etc.

