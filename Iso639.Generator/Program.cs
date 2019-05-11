using System;
using System.CodeDom.Compiler;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Iso639.Generator
{
    internal class Program
    {
        private const string SOURCE = "https://iso639-3.sil.org/sites/iso639-3/files/downloads/iso-639-3.tab";

        private const string PATTERN =
            @"(?<p3>[a-z]{3})\t(?<p2b>[a-z]{3})?\t(?<p2t>[a-z]{3})?\t(?<p1>[a-z]{2})?\t(?<scope>[A-Z])\t(?<type>[A-Z])\t(?<name>.+)\t(?<comment>.*)";

        private static async Task Main(string[] args)
        {
            var path = Path.Combine(Environment.CurrentDirectory, Path.GetFileName(SOURCE));
            var uri = new Uri(SOURCE);

            if (args.Contains("--force") || !File.Exists(path))
            {
                using (var client = new WebClient())
                {
                    await client.DownloadFileTaskAsync(uri, path);
                }
            }
            
            var regex = new Regex(PATTERN);
            var total = 0;

            using (var streamWriter = new StreamWriter("Language.Generated.cs", false, Encoding.UTF8))
            {
                using (var writer = new IndentedTextWriter(streamWriter, new string(' ', 4)))
                {
                    writer.WriteLine($"// Generated from ISO-639-3 tables at {uri.Host}");
                    writer.WriteLine();
                    writer.WriteLine("namespace Iso639");
                    writer.WriteLine("{");
                    writer.WriteLine();
                    writer.Indent++;
                    writer.WriteLine("public partial class Language");
                    writer.WriteLine("{");
                    writer.Indent++;
                    writer.WriteLine();
                    writer.WriteLine("/// <summary>");
                    writer.WriteLine("/// Gets a read-only collection of all defined ISO-639 languages.");
                    writer.WriteLine("/// </summary>");
                    writer.WriteLine(
                        "public static System.Collections.Generic.IReadOnlyList<Language> Database { get; }");
                    writer.WriteLine();
                    writer.WriteLine("private static readonly System.Collections.Generic.List<Language> database;");
                    writer.WriteLine();
                    writer.WriteLine("static Language()");
                    writer.WriteLine("{");
                    writer.Indent++;
                    writer.WriteLine("database = new System.Collections.Generic.List<Language>");
                    writer.WriteLine("{");
                    writer.Indent++;
                    writer.WriteLine("#if ISO639");
                    const string NULL = "null";
                    using (var reader = new StreamReader(path, Encoding.UTF8))
                    {
                        string line;
                        while ((line = reader.ReadLine()) != null)
                        {
                            var match = regex.Match(line);
                            if (!match.Success)
                            {
                                if (total > 0)
                                    throw new InvalidDataException();
                                continue;
                            }

                            var name = match.Groups["name"].Value;
                            var part3 = match.Groups["p3"].Value;
                            var part2B = match.Groups["p2b"].Value;
                            var part2T = match.Groups["p2t"].Value;
                            var part1 = match.Groups["p1"].Value;
                            var scope = GetLanguageScope(match.Groups["scope"].Value[0]);
                            var type = GetLanguageType(match.Groups["type"].Value[0]);

                            writer.Write($"new Language(\"{name}\", ");
                            writer.Write(string.IsNullOrWhiteSpace(part3) ? NULL : $"\"{part3}\"");
                            writer.Write(", ");
                            writer.Write(string.IsNullOrWhiteSpace(part2B) ? NULL : $"\"{part2B}\"");
                            writer.Write(", ");
                            writer.Write(string.IsNullOrWhiteSpace(part2B) ? NULL : $"\"{part2T}\"");
                            writer.Write(", ");
                            writer.Write(string.IsNullOrWhiteSpace(part2B) ? NULL : $"\"{part1}\"");
                            writer.WriteLine($", LanguageType.{type}, LanguageScope.{scope}),");
                            total++;
                        }

                        writer.WriteLine("#endif // ISO639");
                        writer.Indent--;
                        writer.WriteLine("};");
                        writer.WriteLine(
                            "Database = new System.Collections.ObjectModel.ReadOnlyCollection<Language>(database);");
                        do
                        {
                            writer.Indent--;
                            writer.WriteLine("}");
                        } while (writer.Indent > 0);
                    }
                }
            }
        }

        private static LanguageScope GetLanguageScope(char chr)
        {
            switch (chr)
            {
                case 'C': return LanguageScope.Collective;
                case 'I': return LanguageScope.Individual;
                case 'L': return LanguageScope.Local;
                case 'M': return LanguageScope.MacroLanguage;
                case 'S': return LanguageScope.Special;
                default:
                    throw new ArgumentException("Invalid character.", nameof(chr));
            }
        }

        private static LanguageType GetLanguageType(char chr)
        {
            switch (chr)
            {
                case 'L': return LanguageType.Living;
                case 'E': return LanguageType.Extinct;
                case 'C': return LanguageType.Constructed;
                case 'A': return LanguageType.Ancient;
                case 'H': return LanguageType.Historical;
                case 'S': return LanguageType.Special;
                default:
                    throw new ArgumentException("Invalid character.", nameof(chr));
            }
        }
    }
}