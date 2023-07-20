using System.ComponentModel;

namespace Atata.HtmlValidation;

/// <summary>
/// Represents the HTML validator.
/// Uses "html-validate" NPM package to execute HTML validation.
/// If required version of "html-validate" package is not installed, installs it.
/// </summary>
public class HtmlValidator
{
    private static readonly object s_cliInstallationSyncObject = new();

    private static string s_installedCliVersion;

    private readonly HtmlValidationOptions _options;

    private readonly AtataContext _atataContext;

    /// <summary>
    /// Initializes a new instance of the <see cref="HtmlValidator"/> class
    /// using <see cref="HtmlValidationOptions.Default"/> options and <see cref="AtataContext.Current"/>.
    /// </summary>
    public HtmlValidator()
        : this(HtmlValidationOptions.Default)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="HtmlValidator" /> class
    /// using <see cref="HtmlValidationOptions.Default" /> options and the specified <paramref name="atataContext" />.
    /// </summary>
    /// <param name="atataContext">The context, which can be <see langword="null"/>.</param>
    public HtmlValidator(AtataContext atataContext)
        : this(HtmlValidationOptions.Default, atataContext)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="HtmlValidator"/> class
    /// with the specified <paramref name="options"/> and using <see cref="AtataContext.Current"/>.
    /// </summary>
    /// <param name="options">The options.</param>
    public HtmlValidator(HtmlValidationOptions options)
        : this(options, AtataContext.Current)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="HtmlValidator"/> class.
    /// with the specified <paramref name="options"/> and <paramref name="atataContext"/>.
    /// </summary>
    /// <param name="options">The options.</param>
    /// <param name="atataContext">The context, which can be <see langword="null"/>.</param>
    public HtmlValidator(HtmlValidationOptions options, AtataContext atataContext)
    {
        _options = options.CheckNotNull(nameof(options));
        _atataContext = atataContext;
    }

    /// <summary>
    /// Ensures the "html-validate" CLI is installed with the specified <paramref name="version"/>.
    /// </summary>
    /// <param name="version">The version.</param>
    public static void EnsureCliIsInstalled(string version)
    {
        version.CheckNotNullOrWhitespace(nameof(version));

        var options = HtmlValidationOptions.Default.CloneWith(x => x.HtmlValidatePackageVersion = version);
        var validator = new HtmlValidator(options);
        validator.EnsureCliIsInstalled();
    }

    /// <summary>
    /// <para>
    /// Validates the specified HTML content.
    /// </para>
    /// <para>
    /// Before the execution of validation, the check of installed "html-validate" package version performs.
    /// The required package version is defined in <see cref="HtmlValidationOptions.HtmlValidatePackageVersion"/> property of
    /// <c>options</c> that can be passed into the constructor.
    /// The required version will be installed if "html-validate" package is not installed or the installed version differs from the required one.
    /// </para>
    /// </summary>
    /// <param name="html">The HTML content.</param>
    /// <returns>The validation result.</returns>
    public HtmlValidationResult Validate(string html)
    {
        string workingDirectory = ResolveWorkingDirectory(_options);

        string htmlFileName = $"{Guid.NewGuid()}.html";
        string htmlFilePath = Path.Combine(workingDirectory, htmlFileName);

        WriteToFile(htmlFilePath, html);
        _atataContext?.Log.Trace($"HTML saved to file \"{htmlFilePath}\"");

        EnsureCliIsInstalled();

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
            _atataContext?.Log.Info($"HTML validation report saved to file \"{resultFilePath}\"");
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

    private static string ResolveFormatterFileExtension(string formatter) =>
        formatter switch
        {
            HtmlValidateFormatter.Names.Json => ".json",
            HtmlValidateFormatter.Names.Checkstyle => ".xml",
            _ => ".txt",
        };

    private static bool ShouldSaveHtmlFile(bool isValid, HtmlSaveCondition saveCondition) =>
        saveCondition switch
        {
            HtmlSaveCondition.Never => false,
            HtmlSaveCondition.Invalid => !isValid,
            HtmlSaveCondition.Always => true,
            _ => throw new InvalidEnumArgumentException(nameof(saveCondition), (int)saveCondition, typeof(HtmlSaveCondition)),
        };

    private string ResolveWorkingDirectory(HtmlValidationOptions settings)
    {
        string workingDirectory = settings.WorkingDirectoryBuilder?.Invoke(_atataContext)
            ?? AppDomain.CurrentDomain.BaseDirectory;

        workingDirectory = _atataContext?.FillTemplateString(workingDirectory) ?? workingDirectory;

        if (!Directory.Exists(workingDirectory))
            Directory.CreateDirectory(workingDirectory);

        return workingDirectory;
    }

    private void ExecuteAction(string sectionMessage, Action action)
    {
        if (_atataContext is null)
            action.Invoke();
        else
            _atataContext.Log.ExecuteSection(new LogSection(sectionMessage, LogLevel.Trace), action);
    }

    private TResult ExecuteFunction<TResult>(string sectionMessage, Func<TResult> function) =>
        _atataContext is null
            ? function.Invoke()
            : _atataContext.Log.ExecuteSection(new LogSection(sectionMessage, LogLevel.Trace), function);

    private void EnsureCliIsInstalled()
    {
        string version = _options.HtmlValidatePackageVersion;

        if (version != null && version != s_installedCliVersion)
        {
            lock (s_cliInstallationSyncObject)
            {
                if (version != s_installedCliVersion)
                {
                    ExecuteAction(
                        $"Check {HtmlValidateCli.Name} NPM package installed version to be {version} and install it in case it's not installed",
                        () => new HtmlValidateCli().RequireVersion(version));

                    s_installedCliVersion = version;
                }
            }
        }
    }

    private void WriteToFile(string path, string contents)
    {
        // TODO: v3. Use _atataContext.AddArtifact method to write file.
        if (_options.Encoding is null)
            File.WriteAllText(path, contents);
        else
            File.WriteAllText(path, contents, _options.Encoding);
    }

    private HtmlValidateResult ExecuteCliCommand(string workingDirectory, string htmlFileName, string formatter)
    {
        var cliOptions = CreateCliOptions(formatter);

        var cli = HtmlValidateCli.InDirectory(workingDirectory);
        cli.Encoding = _options.Encoding;

        if (_options.CliCommandFactory != null)
            cli.CliCommandFactory = _options.CliCommandFactory;

        return ExecuteFunction(
            $"Execute html-validate CLI command for \"{htmlFileName}\" with \"{cliOptions.Formatter.Name}\" formatter",
            () => cli.Validate(htmlFileName, cliOptions));
    }

    private HtmlValidateOptions CreateCliOptions(string formatter) =>
        new()
        {
            MaxWarnings = _options.MaxWarnings,
            Config = _options.ConfigPath,
            Formatter = new HtmlValidateFormatter(formatter)
        };
}
