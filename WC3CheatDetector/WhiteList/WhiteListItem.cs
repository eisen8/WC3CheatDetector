namespace WC3CheatDetector.WhiteList
{ 
    /// <summary>
    /// Represents an item in the whitelist
    /// </summary>
    public class WhiteListItem
    {
        /// <summary>
        /// The MD5 hash of the white listed map
        /// </summary>
        public string Hash;

        /// <summary>
        /// An identifier for this item (such as map name and version)
        /// </summary>
        public string Identifier;
    }
}
