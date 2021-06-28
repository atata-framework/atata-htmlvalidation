# Atata.HtmlValidation

[![NuGet](http://img.shields.io/nuget/v/Atata.HtmlValidation.svg?style=flat)](https://www.nuget.org/packages/Atata.HtmlValidation/)
[![GitHub release](https://img.shields.io/github/release/atata-framework/atata-htmlvalidation.svg)](https://github.com/atata-framework/atata-htmlvalidation/releases)
[![Build status](https://dev.azure.com/atata-framework/atata-htmlvalidation/_apis/build/status/atata-htmlvalidation-ci?branchName=main)](https://dev.azure.com/atata-framework/atata-htmlvalidation/_build/latest?definitionId=47&branchName=main)
[![Slack](https://img.shields.io/badge/join-Slack-green.svg?colorB=4EB898)](https://join.slack.com/t/atata-framework/shared_invite/zt-5j3lyln7-WD1ZtMDzXBhPm0yXLDBzbA)
[![Atata docs](https://img.shields.io/badge/docs-Atata_Framework-orange.svg)](https://atata.io)
[![Twitter](https://img.shields.io/badge/follow-@AtataFramework-blue.svg)](https://twitter.com/AtataFramework)

**Atata.HtmlValidation** is a .NET library that adds HTML page validation to [Atata](https://github.com/atata-framework/atata) using [html-validate](https://www.npmjs.com/package/html-validate) NPM package.

*Targets .NET Standard 2.0*

## Table of Contents

- [Features](#features)
- [Installation](#installation)
- [Usage](#usage)
  - [Using ValidateHtml Extension Method](#using-validatehtml-extension-method) 
  - [Using HtmlValidator](#using-htmlvalidator)
- [HtmlValidationOptions Properties](#htmlvalidationoptions-properties)
- [HtmlValidationResult Properties](#htmlvalidationresult-properties)
- [Configuration](#configuration)
- [Feedback](#feedback)
- [Thanks](#thanks)
- [SemVer](#semver)
- [License](#license)

## Features

- Uses [CLI of html-validate](https://html-validate.org/usage/cli.html) NPM package.
- Validates HTML of Atata page objects.
- Performs validation offline.
- Supports different result formatters.
- Supports custom validation rule sets by configuration files.
- Saves HTML and validation results to artifact files.
- Can produce either error or warning.

## Installation

### NuGet Package

Install [`Atata.HtmlValidation`](https://www.nuget.org/packages/Atata.HtmlValidation/) NuGet package.

- Package Manager:
  ```
  Install-Package Atata.HtmlValidation
  ```

- .NET CLI:
  ```
  dotnet add package Atata.HtmlValidation
  ```

### NPM Package

Requires [html-validate](https://www.npmjs.com/package/html-validate) NPM package,
as well as NPM itself, to be installed.
By default, when `html-validate` package is missing,
the library automatically tries to install the package,
but in case of permissions lack, installation can fail.
Therefore, it is recommended to preinstall `html-validate` package using NPM command:

```
npm install -g html-validate
```

## Usage

### Using ValidateHtml Extension Method

The primary way to execute validations is using `ValidateHtml` extension method:

```cs
public static TPageObject ValidateHtml<TPageObject>(
    this TPageObject pageObject,
    HtmlValidationOptions options = null,
    bool asWarning = false)
    where TPageObject : PageObject<TPageObject>;
```

Validates the HTML of current page.
Relies on `HtmlValidator` class
that uses "html-validate" NPM package to execute HTML validation.
If "html-validate" package is not installed, tries to install it.
If `asWarning` argument is set to `true`, instead of assertion exception produces warning.

#### Validate by Default

```cs
Go.To<OrdinaryPage>(url: "some/url")
    .ValidateHtml();
```

#### Validate As Warning

```cs
Go.To<OrdinaryPage>(url: "some/url")
    .ValidateHtml(asWarning: true);
```

#### Validate With Custom Options

```cs
var options = new HtmlValidationOptions
{
    OutputFormatter = HtmlValidateFormatter.Names.Text,
    ResultFileFormatter = HtmlValidateFormatter.Names.Json,
    ConfigPath = "some/config.json"
};

Go.To<OrdinaryPage>(url: "some/url")
    .ValidateHtml(options);
```

#### Validate With Options Based on Default Ones

```cs
Go.To<OrdinaryPage>(url: "some/url")
    .ValidateHtml(HtmlValidationOptions.Default.CloneWith(x => x.ConfigPath = "another/config.json"));
```

### Using HtmlValidator

This approach is a bit low-level one.
Can be used without active `AtataContext`.

`HtmlValidator` - uses "html-validate" NPM package to execute HTML validation.
If "html-validate" package is not installed, tries to install it.

```cs
HtmlValidator validator = new HtmlValidator(
    new HtmlValidationOptions()); // HtmlValidationOptions are optional here.

string html = "<html>...</html>";
// string html = webDriver.PageSource;

HtmlValidationResult result = validator.Validate(html);

if (!result.IsSuccessful)
{
    string resultMessage = result.Output;

    // TODO: Handle validation failure case.
}
```

## HtmlValidationOptions Properties

- **`static HtmlValidationOptions Default { get; set; }`**\
  Gets or sets the default options.
- **`Func<string> WorkingDirectoryBuilder { get; set; }`**\
  Gets or sets the working directory builder.
  HTML and result files should be saved in working directory.
  The default builder returns the directory of `Artifacts` of `AtataContext.Current`
  or `AppDomain.CurrentDomain.BaseDirectory` if `AtataContext.Current` is `null`.
- **`string WorkingDirectory { get; set; }`**\
  Gets or sets the working directory where HTML and result files should be saved.
  Gets and sets the value from/to `WorkingDirectoryBuilder` property.
- **`int? MaxWarnings { get; set; }`**\
  Gets or sets the maximum allowed warnings count.
  The default value is `null`, which means that warnings are allowed.
  Use `0` to disallow warnings.
- **`string ConfigPath { get; set; }`**\
  Gets or sets the configuration file path (full or relative to `WorkingDirectory`).
- **`string OutputFormatter { get; set; }`**\
  Gets or sets the output formatter name.
  The default value is `HtmlValidateFormatter.Names.Stylish`.
  See [`HtmlValidateFormatter.Names`](https://github.com/atata-framework/atata-cli-htmlvalidate/blob/04344a8d4452921537dd9c83d806735e5d4427e7/src/Atata.Cli.HtmlValidate/HtmlValidateFormatter.cs#L39) class for options.
- **`string ResultFileFormatter { get; set; }`**\
  Gets or sets the result file formatter name.
  The default value is `HtmlValidateFormatter.Names.Codeframe`.
  See [`HtmlValidateFormatter.Names`](https://github.com/atata-framework/atata-cli-htmlvalidate/blob/04344a8d4452921537dd9c83d806735e5d4427e7/src/Atata.Cli.HtmlValidate/HtmlValidateFormatter.cs#L39) class for options.
- **`string ResultFileExtension`**\
  Gets or sets the result file extension, like ".txt" or ".json".
  The default value is `null`, which means that
  extension should be resolved automatically corresponding to `ResultFileFormatter` value.
- **`bool SaveResultToFile`**\
  Gets or sets a value indicating whether to save validation result output to file.
  The default value is `true`.
- **`bool KeepHtmlFileWhenValid`**\
  Gets or sets a value indicating whether to keep HTML file when it is valid.
  The default value is `false`.
- **`Encoding Encoding`**\
  Gets or sets the encoding to use.
  The default value is `null`,
  which means that default encoding should be used.

### Configure Default Options

It is possible to configure default `HtmlValidationOptions` upon global initialization method.

Change some default values:

```cs
HtmlValidationOptions.Default.OutputFormatter = HtmlValidateFormatter.Names.Codeframe;
```

Reset default options completely:

```cs
HtmlValidationOptions.Default = new HtmlValidationOptions
{
    OutputFormatter = HtmlValidateFormatter.Names.Codeframe,
    SaveResultToFile = false,
    ConfigPath = "some/config.json"
};
```

## HtmlValidationResult Properties

- **`bool IsSuccessful { get; }`**\
  Gets a value indicating whether this result is successful.
- **`string Output { get; }`**\
  Gets the text output of result.
- **`string HtmlFilePath { get; }`**\
  Gets the HTML file path.
- **`string ResultFilePath { get; }`**\
  Gets the result file path.

## Configuration

Check out [html-validate Configuration](https://html-validate.org/usage/index.html#configuration)
documentation page on how to create config files.

## Feedback

Any feedback, issues and feature requests are welcome.

If you faced an issue please report it to [Atata.HtmlValidation Issues](https://github.com/atata-framework/atata-htmlvalidation/issues),
[ask a question on Stack Overflow](https://stackoverflow.com/questions/ask?tags=atata+csharp) using [atata](https://stackoverflow.com/questions/tagged/atata) tag
or use another [Atata Contact](https://atata.io/contact/) way.

## Thanks

The library is implemented thanks to the sponsorship of **[Lombiq Technologies](https://lombiq.com/)**.

## SemVer

Atata Framework follows [Semantic Versioning 2.0](https://semver.org/).
Thus backward compatibility is followed and updates within the same major version
(e.g. from 1.3 to 1.4) should not require code changes.

## License

Atata is an open source software, licensed under the Apache License 2.0.
See [LICENSE](LICENSE) for details.
