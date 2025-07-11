﻿namespace Atata.HtmlValidation;

/// <summary>
/// Represents the set of options for HTML validation.
/// </summary>
public class HtmlValidationOptions
{
    /// <summary>
    /// Gets or sets the default options.
    /// </summary>
    public static HtmlValidationOptions Default { get; set; } = new();

    /// <summary>
    /// Gets or sets the working directory where HTML and result files should be saved.
    /// The default value is <see langword="null"/>, meaning to use <see cref="AtataContext.ArtifactsPath"/>.
    /// The other value should be relative to Artifacts directory, for example, a name of subdirectory.
    /// The path supports <see cref="AtataContext"/> template variables.
    /// </summary>
    public string? WorkingDirectory { get; set; }

    /// <summary>
    /// Gets or sets the maximum allowed warnings count.
    /// The default value is <see langword="null"/>, which means that warnings are allowed.
    /// Use <c>0</c> to disallow warnings.
    /// </summary>
    public int? MaxWarnings { get; set; }

    /// <summary>
    /// Gets or sets the configuration file path (full or relative to <see cref="WorkingDirectory"/>).
    /// The default value is <see langword="null"/>.
    /// </summary>
    public string? ConfigPath { get; set; }

    /// <summary>
    /// Gets or sets the output formatter name.
    /// The default value is <see cref="HtmlValidateFormatter.Names.Stylish"/>.
    /// See <see cref="HtmlValidateFormatter.Names"/> class for options.
    /// </summary>
    public string OutputFormatter { get; set; } = HtmlValidateFormatter.Names.Stylish;

    /// <summary>
    /// Gets or sets the result file formatter name.
    /// The default value is <see cref="HtmlValidateFormatter.Names.Codeframe"/>.
    /// See <see cref="HtmlValidateFormatter.Names"/> class for options.
    /// </summary>
    public string ResultFileFormatter { get; set; } = HtmlValidateFormatter.Names.Codeframe;

    /// <summary>
    /// Gets or sets the result file extension, like ".txt" or ".json".
    /// The default value is <see langword="null"/>, which means that
    /// extension should be resolved automatically corresponding to <see cref="ResultFileFormatter"/> value.
    /// </summary>
    public string? ResultFileExtension { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to save validation result output to file.
    /// The default value is <see langword="true"/>.
    /// </summary>
    public bool SaveResultToFile { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating the condition of HTML file saving.
    /// The default value is <see cref="HtmlSaveCondition.Invalid"/>,
    /// meaning that HTML file should be saved only when it is not valid.
    /// Other available options are: <see cref="HtmlSaveCondition.Never"/> and <see cref="HtmlSaveCondition.Always"/>.
    /// </summary>
    public HtmlSaveCondition SaveHtmlToFile { get; set; } = HtmlSaveCondition.Invalid;

    /// <summary>
    /// Gets or sets the encoding to use.
    /// The default value is <see langword="null"/>,
    /// which means that default encoding should be used.
    /// </summary>
    public Encoding? Encoding { get; set; }

    /// <summary>
    /// Gets or sets the CLI command factory.
    /// The default value is <see langword="null"/>,
    /// which means that default CLI command factory should be used,
    /// which is <see cref="ProgramCli.DefaultShellCliCommandFactory"/>.
    /// </summary>
    public ICliCommandFactory? CliCommandFactory { get; set; }

    /// <summary>
    /// Gets or sets the required version of "html-validate" NPM package.
    /// The required version will be installed if "html-validate" package is not installed or the installed version differs from the required one.
    /// The default value is <c>"8.29.0"</c>.
    /// Set <see langword="null"/> to disable the version check and use any pre-installed version.
    /// </summary>
    public string HtmlValidatePackageVersion { get; set; } = "8.29.0";

    /// <summary>
    /// Clones this instance with executing the action that can change some properties of the copy.
    /// </summary>
    /// <param name="action">The action.</param>
    /// <returns>The copy of this instance.</returns>
    public HtmlValidationOptions CloneWith(Action<HtmlValidationOptions> action)
    {
        Guard.ThrowIfNull(action);

        HtmlValidationOptions copy = Clone();
        action.Invoke(copy);

        return copy;
    }

    /// <summary>
    /// Clones this instance.
    /// </summary>
    /// <returns>The copy of this instance.</returns>
    public HtmlValidationOptions Clone() =>
        (HtmlValidationOptions)MemberwiseClone();
}
