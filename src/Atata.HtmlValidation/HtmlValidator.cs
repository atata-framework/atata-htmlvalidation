using System;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using Atata.Cli;
using Atata.Cli.HtmlValidate;
using Atata.Cli.Npm;

namespace Atata.HtmlValidation
{
    /// <summary>
    /// Represents the HTML validator.
    /// Uses "html-validate" NPM package to execute HTML validation.
    /// If "html-validate" package is not installed, tries to install it.
    /// </summary>
    public class HtmlValidator
    {
        private readonly HtmlValidationOptions _options;

        /// <summary>
        /// Initializes a new instance of the <see cref="HtmlValidator"/> class
        /// using <see cref="HtmlValidationOptions.Default"/> options.
        /// </summary>
        public HtmlValidator()
            : this(HtmlValidationOptions.Default)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HtmlValidator"/> class
        /// with the specified options.
        /// </summary>
        /// <param name="options">The options.</param>
        public HtmlValidator(HtmlValidationOptions options)
        {
            _options = options.CheckNotNull(nameof(options));
        }

        /// <summary>
        /// Validates the specified HTML content.
        /// </summary>
        /// <param name="html">The HTML content.</param>
        /// <returns>The validation result.</returns>
        public HtmlValidationResult Validate(string html)
        {
            string workingDirectory = ResolveWorkingDirectory(_options);

            string htmlFileName = $"{Guid.NewGuid()}.html";
            string htmlFilePath = Path.Combine(workingDirectory, htmlFileName);

            WriteToFile(htmlFilePath, html);
            AtataContext.Current?.Log.Trace($"HTML saved to file \"{htmlFilePath}\"");

            var result = ExecuteCliCommand(workingDirectory, htmlFileName, _options.OutputFormatter);
            string resultFilePath = null;

            if (!result.IsSuccessful && _options.SaveResultToFile)
            {
                string resultFileExtension = _options.ResultFileExtension ?? ResolveFormatterFileExtension(_options.ResultFileFormatter);
                resultFilePath = Path.Combine(
                    workingDirectory,
                    Path.GetFileNameWithoutExtension(htmlFilePath) + resultFileExtension);

                string resultFileOutput = _options.ResultFileFormatter == _options.OutputFormatter
                    ? result.Output
                    : ExecuteCliCommand(workingDirectory, htmlFileName, _options.ResultFileFormatter).Output;

                WriteToFile(resultFilePath, resultFileOutput);
                AtataContext.Current?.Log.Info($"HTML validation report saved to file \"{resultFilePath}\"");
            }

            if (!ShouldSaveHtmlFile(result.IsSuccessful, _options.SaveHtmlToFile))
            {
                File.Delete(htmlFilePath);
                htmlFilePath = null;
            }

            return new HtmlValidationResult(result.IsSuccessful, result.Output, htmlFilePath, resultFilePath);
        }

        /// <inheritdoc cref="Validate(string)"/>
        public async Task<HtmlValidationResult> ValidateAsync(string html) =>
            await Task.Run(() => Validate(html));

        private static string ResolveWorkingDirectory(HtmlValidationOptions settings)
        {
            string workingDirectory = settings.WorkingDirectoryBuilder?.Invoke()
                ?? AppDomain.CurrentDomain.BaseDirectory;

            workingDirectory = AtataContext.Current?.FillTemplateString(workingDirectory) ?? workingDirectory;

            if (!Directory.Exists(workingDirectory))
                Directory.CreateDirectory(workingDirectory);

            return workingDirectory;
        }

        private static string ResolveFormatterFileExtension(string formatter)
        {
            switch (formatter)
            {
                case HtmlValidateFormatter.Names.Json:
                    return ".json";
                case HtmlValidateFormatter.Names.Checkstyle:
                    return ".xml";
                default:
                    return ".txt";
            }
        }

        private static bool ShouldSaveHtmlFile(bool isValid, HtmlSaveCondition saveCondition)
        {
            switch (saveCondition)
            {
                case HtmlSaveCondition.Never:
                    return false;
                case HtmlSaveCondition.Invalid:
                    return !isValid;
                case HtmlSaveCondition.Always:
                    return true;
                default:
                    throw new InvalidEnumArgumentException(nameof(saveCondition), (int)saveCondition, typeof(HtmlSaveCondition));
            }
        }

        private void EnsureCliIsInstalled() =>
            new NpmCli()
                .EnsureItIsInstalled()
                .InstallIfMissing(HtmlValidateCli.Name, _options.RecommendedHtmlValidatePackageVersion, global: true);

        private void WriteToFile(string path, string contents)
        {
            if (_options.Encoding is null)
                File.WriteAllText(path, contents);
            else
                File.WriteAllText(path, contents, _options.Encoding);
        }

        private HtmlValidateResult ExecuteCliCommand(string workingDirectory, string htmlFileName, string formatter)
        {
            var cliOptions = CreateCliOptions(formatter);

            HtmlValidateResult Execute()
            {
                var cli = HtmlValidateCli.InDirectory(workingDirectory);
                cli.Encoding = _options.Encoding;

                try
                {
                    return cli.Validate(htmlFileName, cliOptions);
                }
                catch (CliCommandException)
                {
                    EnsureCliIsInstalled();
                    return cli.Validate(htmlFileName, cliOptions);
                }
            }

            return AtataContext.Current is null
                ? Execute()
                : AtataContext.Current.Log.ExecuteSection(
                    new LogSection($"Execute html-validate CLI command for {htmlFileName}", LogLevel.Trace),
                    Execute);
        }

        private HtmlValidateOptions CreateCliOptions(string formatter) =>
            new HtmlValidateOptions
            {
                MaxWarnings = _options.MaxWarnings,
                Config = _options.ConfigPath,
                Formatter = new HtmlValidateFormatter(formatter)
            };
    }
}
