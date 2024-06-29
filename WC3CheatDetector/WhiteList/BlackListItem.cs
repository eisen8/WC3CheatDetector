namespace WC3CheatDetector.WhiteList
{
    /// <summary>
    /// Represents an item in the blacklist
    /// </summary>
    public class BlackListItem
    {
        /// <summary>
        /// The MD5 hash of the black listed map
        /// </summary>
        public string Hash;

        /// <summary>
        /// The name of the cheatpack (if known)
        /// </summary>
        public string CheatPack;

        /// <summary>
        /// The activator text or method (if known)
        /// </summary>
        public string Activator;
    }
}
