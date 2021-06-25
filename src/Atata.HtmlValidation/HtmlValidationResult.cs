namespace Atata.HtmlValidation
{
    /// <summary>
    /// Represents the result of HTML validation.
    /// </summary>
    public class HtmlValidationResult
    {
        public HtmlValidationResult(bool isSuccessful, string output, string htmlFilePath, string resultFilePath)
        {
            IsSuccessful = isSuccessful;
            Output = output;
            HtmlFilePath = htmlFilePath;
            ResultFilePath = resultFilePath;
        }

        /// <summary>
        /// Gets a value indicating whether this result is successful.
        /// </summary>
        public bool IsSuccessful { get; }

        /// <summary>
        /// Gets the text output of result.
        /// </summary>
        public string Output { get; }

        /// <summary>
        /// Gets the HTML file path.
        /// </summary>
        public string HtmlFilePath { get; }

        /// <summary>
        /// Gets the result file path.
        /// </summary>
        public string ResultFilePath { get; }
    }
}
