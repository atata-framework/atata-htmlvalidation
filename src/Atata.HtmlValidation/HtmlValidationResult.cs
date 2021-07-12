using System.IO;

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

        /// <summary>
        /// Moves the HTML and result files to another directory.
        /// </summary>
        /// <param name="directory">The destination directory.</param>
        /// <returns>The new <see cref="HtmlValidationResult"/> instance with updated file path property values.</returns>
        public HtmlValidationResult MoveFilesToDirectory(string directory)
        {
            directory.CheckNotNullOrWhitespace(nameof(directory));

            return new HtmlValidationResult(
                IsSuccessful,
                Output,
                MoveFileToDirectory(HtmlFilePath, directory),
                MoveFileToDirectory(ResultFilePath, directory));
        }

        private static string MoveFileToDirectory(string filePath, string directory)
        {
            if (!string.IsNullOrEmpty(filePath))
            {
                string newFilePath = Path.Combine(directory, Path.GetFileName(filePath));
                File.Move(filePath, newFilePath);

                return newFilePath;
            }
            else
            {
                return filePath;
            }
        }
    }
}
