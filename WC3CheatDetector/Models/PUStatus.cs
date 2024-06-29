namespace WC3CheatDetector.Models
{
    /// <summary>
    /// Represents the return status code from ProcessUtility.
    /// </summary>
    public class PUStatus
    {
        /// <summary>
        /// The Error Code. 0 if there is no error.
        /// </summary>
        public uint ErrorCode { get; private set; }

        /// <summary>
        /// An error message relating to the Error Code. Empty string if there is no error.
        /// </summary>
        public string ErrorMessage { get; private set; }

        public PUStatus(uint errorCode, string errorMessage)
        {
            ErrorCode = errorCode;
            ErrorMessage = errorMessage;
        }
    }
}
