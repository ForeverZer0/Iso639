namespace Iso639
{
    /// <summary>
    /// Describes a ISO-639 language type.
    /// </summary>
    public enum LanguageType
    {
        /// <summary>
        /// A language that existed in ancient times (i.e. pre-5th century)
        /// </summary>
        /// <seealso href="https://en.wikipedia.org/wiki/Ancient_language"/>
        Ancient,
        
        /// <summary>
        /// An artificially created or invented language, not developed naturally.
        /// </summary>
        /// <seealso href="https://en.wikipedia.org/wiki/Constructed_language"/>
        Constructed,
        
        /// <summary>
        /// A language that no longer exists in spoken form.
        /// </summary>
        /// <seealso href="https://en.wikipedia.org/wiki/Extinct_language"/>
        Extinct,

        /// <summary>
        /// A language that were spoken in a historical period, but differ from their moder form.
        /// </summary>
        /// <seealso href="https://en.wikipedia.org/wiki/Historical_language"/>
        Historical,
        
        /// <summary>
        /// A modern language still in use.
        /// </summary>
        /// <seealso href="https://en.wikipedia.org/wiki/Modern_language"/>
        Living,
        
        /// <summary>
        /// A language with a specific and/or context-dependent type.
        /// </summary>
        Special
    }
}