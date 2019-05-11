using System;
using System.Collections.Generic;
using System.Globalization;
using JetBrains.Annotations;

namespace Iso639
{
    /// <summary>
    /// Describes a language defined in the ISO-639 standard.
    /// <seealso href="https://www.iso.org/iso-639-language-codes.html"/>
    /// <seealso href="https://en.wikipedia.org/wiki/ISO_639"/>
    /// </summary>
    public sealed partial class Language : IEquatable<Language>
    {
        /// <summary>
        /// Gets the 3-digit ISO-639-3 language code.
        /// <para>
        /// The ISO-639-3 standard provides the widest definition set, and this property is guaranteed to not be <c>null</c>.
        /// </para>
        /// </summary>
        [NotNull]
        public string Part3 { get; }

        /// <summary>
        /// Gets the obsolete "bibliographic" ISO-639-2/B code, or <c>null</c> if none is defined.
        /// <para>These are legacy values, and very rarely seen anywhere in the wild.</para>
        /// <para>Only 22 languages have a B code defined.</para>
        /// </summary>
        [Obsolete][CanBeNull]
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public string Part2B { get; }
        
        /// <summary>
        /// Gets the 3-digit "terminological" ISO-639-2/T language code, or <c>null</c> if none is defined.
        /// </summary>
        [CanBeNull]
        public string Part2 { get; }
        
        /// <summary>
        /// Gets the 2-digit ISO-639-1 language code, or <c>null</c> if none is defined.
        /// </summary>
        [CanBeNull]
        public string Part1 { get; }
        
        /// <summary>
        /// Gets a brief name/description of the language.
        /// </summary>
        [NotNull]
        public string Name { get; }
        
        /// <summary>
        /// Gets the descriptor for individual language types.
        /// </summary>
        /// <seealso href="https://iso639-3.sil.org/about/types"/>
        public LanguageType Type { get; }
        
        /// <summary>
        /// Gets the scope of the language definition.
        /// </summary>
        /// <seealso href="https://iso639-3.sil.org/about/scope"/>
        public LanguageScope Scope { get; }

        private Language([NotNull] string name, [NotNull] string part3, [CanBeNull] string part2B,
            [CanBeNull] string part2, [CanBeNull] string part1, LanguageType type, LanguageScope scope)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Part3 = part3 ?? throw new ArgumentNullException(nameof(part3));
#pragma warning disable 612
            Part2B = part2B;
#pragma warning restore 612
            Part2 = part2;
            Part1 = part1;
            Type = type;
            Scope = scope;
        }

        /// <summary>
        /// Determines whether the specified <see cref="Language"/> is equal to the current <see cref="Language"/>. 
        /// </summary>
        /// <param name="other">The language to compare to the current instance.</param>
        /// <returns><c>true</c> if the languages are equal, otherwise <c>false</c>.</returns>
        public bool Equals(Language other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(Part3, other.Part3);
        }
        
        /// <inheritdoc cref="Object.Equals(object)"/>
        public override bool Equals(object obj)
        {
            return ReferenceEquals(this, obj) || obj is Language other && Equals(other);
        }

        /// <inheritdoc cref="Object.GetHashCode"/>
        public override int GetHashCode() => Part3.GetHashCode();

        /// <summary>
        /// Impediments the equality operator.
        /// </summary>
        /// <param name="left">The first language to compare.</param>
        /// <param name="right">The second language to compare.</param>
        /// <returns><c>true</c> if the languages are equal, otherwise <c>false</c>.</returns>
        public static bool operator ==(Language left, Language right) => Equals(left, right);
        
        /// <summary>
        /// Impediments the inequality operator.
        /// </summary>
        /// <param name="left">The first language to compare.</param>
        /// <param name="right">The second language to compare.</param>
        /// <returns><c>true</c> if the languages are not equal, otherwise <c>false</c>.</returns>
        public static bool operator !=(Language left, Language right) => !Equals(left, right);
        
        private CultureInfo culture;

        /// <summary>
        /// Gets the culture associated with this language, or <see cref="CultureInfo.InvariantCulture"/> if none is
        /// defined.
        /// </summary>
        [NotNull]
        public CultureInfo Culture            
        {
            get
            {
                if (culture is null)
                {
                    foreach (var info in CultureInfo.GetCultures(CultureTypes.AllCultures))
                    {
                        if (Part1 != null && info.TwoLetterISOLanguageName.Equals(Part1, StringComparison.Ordinal))
                            culture = info;
                        if (Part2 != null && info.ThreeLetterISOLanguageName.Equals(Part2, StringComparison.Ordinal))
                            culture = info;
                    }
                    if (culture is null)
                        culture = CultureInfo.InvariantCulture;
                }
                return culture;
            }
        }
        
                /// <summary>
        /// Searches for a language using the specified 2-digit <paramref name="code"/> defined in ISO-639-1.
        /// </summary>
        /// <param name="code">A 2-digit language code, not case-sensitive.</param>
        /// <returns>The specified <see cref="Language"/>, or <c>null</c> if none was found.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="code"/> is <c>null</c>.</exception>
        [CanBeNull]
        public static Language FromPart1([NotNull] string code)
        {
            if (code is null)
                throw new ArgumentNullException(nameof(code), "Language code cannot be null.");
            if (code.Length != 2)
                return null;
            foreach (var language in database)
            {
                if (language.Part1 is null)
                    continue;
                if (language.Part1.Equals(code, StringComparison.OrdinalIgnoreCase))
                    return language;
            }
            return null;
        }
        
        /// <summary>
        /// Searches for a language using the specified 3-digit <paramref name="code"/> defined in ISO-639-2/T.
        /// </summary>
        /// <param name="code">A 3-digit language code, not case-sensitive.</param>
        /// <returns>The specified <see cref="Language"/>, or <c>null</c> if none was found.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="code"/> is <c>null</c>.</exception>
        [CanBeNull]
        public static Language FromPart2([NotNull] string code)
        {
            if (code is null)
                throw new ArgumentNullException(nameof(code), "Language code cannot be null.");
            if (code.Length != 3)
                return null;
            foreach (var language in database)
            {
                if (language.Part2 is null)
                    continue;
                if (language.Part2.Equals(code, StringComparison.OrdinalIgnoreCase))
                    return language;
            }
            return null;
        }

        /// <summary>
        /// Searches for a language using the specified 3-digit <paramref name="code"/> defined in ISO-639-3.
        /// </summary>
        /// <param name="code">A 3-digit language code, not case-sensitive.</param>
        /// <returns>The specified <see cref="Language"/>, or <c>null</c> if none was found.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="code"/> is <c>null</c>.</exception>
        [CanBeNull]
        public static Language FromPart3([NotNull] string code)
        {
            if (code is null)
                throw new ArgumentNullException(nameof(code), "Language code cannot be null.");
            if (code.Length != 3)
                return null;
            foreach (var language in database)
            {
                if (language.Part3.Equals(code, StringComparison.OrdinalIgnoreCase))
                    return language;
            }
            return null;
        }

        /// <summary>
        /// Returns a collection of all languages that are in specified <paramref name="type"/>.
        /// </summary>
        /// <param name="type">The type of the languages to retrieve.</param>
        /// <returns>A collection of languages with specified type.</returns>
        [NotNull]
        public static IEnumerable<Language> FromType(LanguageType type)
        {
            foreach (var language in database)
            {
                if (language.Type == type)
                    yield return language;
            }
        }

        /// <summary>
        /// Returns a collection of all languages that are in specified <paramref name="scope"/>.
        /// </summary>
        /// <param name="scope">The scope of the languages to retrieve.</param>
        /// <returns>A collection of languages with specified scope.</returns>
        [NotNull]
        public static IEnumerable<Language> FromScope(LanguageScope scope)
        {
            foreach (var language in database)
            {
                if (language.Scope == scope)
                    yield return language;
            }
        }

        /// <summary>
        /// Returns a collection of all languages that contain the specified <paramref name="name"/> within their
        /// name/description.
        /// </summary>
        /// <param name="name">The name/description string to search for.</param>
        /// <param name="ignoreCase"><c>true</c> to search in case-insensitive manner, otherwise <c>false</c>.</param>
        /// <returns>A collection of languages whose name matches the specified name.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="name"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="name"/> is empty or only whitespace.</exception>
        [NotNull]
        public static IEnumerable<Language> FromName([NotNull] string name, bool ignoreCase)
        {
            if (name is null)
                throw new ArgumentNullException(nameof(name), "Search string cannot be null.");
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Search string cannot be empty or only whitespace", nameof(name));
            foreach (var language in database)
            {
                if (ignoreCase)
                {
                    if (language.Name.IndexOf(name, StringComparison.CurrentCultureIgnoreCase) >= 0)
                        yield return language;
                }
                else if (language.Name.Contains(name))
                    yield return language;
            }
        }
    }
}