namespace Iso639
{
    /// <summary>
    /// Describes the scope of a language definition.
    /// </summary>
    /// <seealso href="https://iso639-3.sil.org/about/scope"/>
    public enum LanguageScope
    {
        /// <summary>
        /// Represents a group or parent of a individual languages.
        /// </summary>
        Collective,
        
        /// <summary>
        /// A distinct individual language.
        /// </summary>
        Individual,
        
        /// <summary>
        /// A language that is defined locally, and may be different outside of that context.
        /// </summary>
        Local,
        
        /// <summary>
        /// A super-set definition of a more basic defined language (i.e. same language, different locale).
        /// </summary>
        MacroLanguage,
        
        /// <summary>
        /// A generic non-specific scope that is context-dependent.
        /// </summary>
        Special
    }
}