﻿# Atata.HtmlValidation

[![NuGet](http://img.shields.io/nuget/v/Atata.HtmlValidation.svg?style=flat)](https://www.nuget.org/packages/Atata.HtmlValidation/)
[![GitHub release](https://img.shields.io/github/release/atata-framework/atata-htmlvalidation.svg)](https://github.com/atata-framework/atata-htmlvalidation/releases)
[![Build status](https://dev.azure.com/atata-framework/atata-htmlvalidation/_apis/build/status/atata-htmlvalidation-ci?branchName=main)](https://dev.azure.com/atata-framework/atata-htmlvalidation/_build/latest?definitionId=47&branchName=main)
[![Slack](https://img.shields.io/badge/join-Slack-green.svg?colorB=4EB898)](https://join.slack.com/t/atata-framework/shared_invite/zt-5j3lyln7-WD1ZtMDzXBhPm0yXLDBzbA)
[![Atata docs](https://img.shields.io/badge/docs-Atata_Framework-orange.svg)](https://atata.io)
[![Twitter](https://img.shields.io/badge/follow-@AtataFramework-blue.svg)](https://twitter.com/AtataFramework)

**Atata.HtmlValidation** is a .NET library that adds HTML page validation to [Atata](https://github.com/atata-framework/atata) using [html-validate](https://www.npmjs.com/package/html-validate) NPM package.

*The package targets .NET Standard 2.0, which supports .NET 5+, .NET Framework 4.6.1+ and .NET Core/Standard 2.0+.*

## Table of Contents

- [Features](#features)
- [Installation](#installation)
- [Usage](#usage)
  - [Using ValidateHtml Extension Method](#using-validatehtml-extension-method) 
  - [Using ValidateHtmlAttribute Trigger](#using-validatehtmlattribute-trigger)
  - [Using HtmlValidator](#using-htmlvalidator)
- [Configuration](#configuration)
- [HtmlValidationOptions Properties](#htmlvalidationoptions-properties)
- [HtmlValidationResult Members](#htmlvalidationresult-members)
- [Validation Results](#validation-results)
  - [Exception](#exception)
  - [Result File](#result-file)
  - [Log](#log)
- [Sample Project](#sample-project)
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
Therefore, it is recommended when possible to preinstall `html-validate` package using NPM command:

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

The method validates the HTML of current page.
Relies on `HtmlValidator` class
that uses "html-validate" NPM package to execute HTML validation.

Before the execution of validation, the check of installed "html-validate" package version performs.
The required package version is defined in `HtmlValidatePackageVersion` property of `options`.
The required version will be installed if "html-validate" package is not installed or the installed version differs from the required one.

By default, when validation fails, throws an assertion exception with a message containing a list of HTML errors.
Produces a warning instead of assertion exception if `asWarning` argument is set to `true`.

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

### Using ValidateHtmlAttribute Trigger

`ValidateHtmlAttribute` - the trigger attribute that indicates that the page HTML should be validated on the specified event.
By default occurs upon the page object initialization.
Invokes `ValidateHtml` method using `HtmlValidationOptions.Default` options.

Has `public bool AsWarning { get; set; }` property that gets or sets a value indicating whether to produce a warning instead of assertion exception on validation failure.
The default value is `false`.

#### Apply to Certain Page Object

```cs
[ValidateHtml]
public class SomePage : Page<_>
{
}
```

#### Apply to All Page Objects

```cs
AtataContext.GlobalConfiguration
    .Attributes.Global.Add(new ValidateHtmlAttribute { TargetType = typeof(PageObject<>) });
```

### Using HtmlValidator

This approach is a bit low-level one.
Can be used without active `AtataContext`.

`HtmlValidator` - uses "html-validate" NPM package to execute HTML validation.
If required version of "html-validate" package is not installed, installs it.

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

## Configuration

Check out [html-validate Configuration](https://html-validate.org/usage/index.html#configuration)
documentation page on how to create config files.

Mostly, you can create standard `.htmlvalidate.json` file in the root of a test project with
"Copy to Output Directory" property set to "Copy if newer".

## HtmlValidationOptions Properties

- **`static HtmlValidationOptions Default { get; set; }`**\
  Gets or sets the default options.
- **`Func<AtataContext, string> WorkingDirectoryBuilder { get; set; }`**\
  Gets or sets the working directory builder.
  HTML and result files should be saved in working directory.
  The default builder returns the directory of `Artifacts` property of `AtataContext` argument
  or `AppDomain.CurrentDomain.BaseDirectory` if `AtataContext` argument is `null`.
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
- **`HtmlSaveCondition SaveHtmlToFile`**\
  Gets or sets a value indicating the condition of HTML file saving.
  The default value is `HtmlSaveCondition.Invalid`,
  meaning that HTML file should be saved only when it is not valid.
  Other available options are: `HtmlSaveCondition.Never` and `HtmlSaveCondition.Always`.
- **`Encoding Encoding`**\
  Gets or sets the encoding to use.
  The default value is `null`,
  which means that default encoding should be used.
- **`ICliCommandFactory CliCommandFactory`**\
  Gets or sets the CLI command factory.
  The default value is `null`,
  which means that default CLI command factory should be used,
  which is `ProgramCli.DefaultShellCliCommandFactory`.
- **`string HtmlValidatePackageVersion`**\
  Gets or sets the required version of "html-validate" NPM package.
  The required version will be installed if "html-validate" package is not installed or the installed version differs from the required one.
  The default value is `"7.13.2"`.
  Set `null` to disable the version check and use any pre-installed version.

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

## HtmlValidationResult Members

### Properties

- **`bool IsSuccessful { get; }`**\
  Gets a value indicating whether this result is successful.
- **`string Output { get; }`**\
  Gets the text output of result.
- **`string HtmlFilePath { get; }`**\
  Gets the HTML file path.
- **`string ResultFilePath { get; }`**\
  Gets the result file path.

### Methods

- **`HtmlValidationResult MoveFilesToDirectory(string directory)`**\
  Moves the HTML and result files to another directory.

## Validation Results

The results of failed validation using `ValidateHtml` method can be found in few places.

Additionally original HTML snapshot, which was validated, is saved to Atata Artifacts directory as a file.

### Exception

An `AssertionException` is thrown with the message similar to:

```
Wrong "<app>" page HTML document, which contains errors:
785a0e99-359a-4905-b490-3a62c61fbf37.html
  69:22  error  <th> is missing required "scope" attribute     element-required-attributes
  70:22  error  <th> is missing required "scope" attribute     element-required-attributes
  71:22  error  <th> is missing required "scope" attribute     element-required-attributes
  72:22  error  <th> is missing required "scope" attribute     element-required-attributes
  81:26  error  <button> is missing required "type" attribute  element-required-attributes
  82:26  error  <button> is missing required "type" attribute  element-required-attributes
  83:26  error  <button> is missing required "type" attribute  element-required-attributes

✖ 7 problems (7 errors, 0 warnings)

More information:
  https://html-validate.org/rules/element-required-attributes.html
```

### Result File

By default, the result file that is saved to Atata Artifacts directory is generated using "codeframe" formatter,
which provides nice detailed report.

```
error: <th> is missing required "scope" attribute (element-required-attributes) at 785a0e99-359a-4905-b490-3a62c61fbf37.html:69:22:
  67 |             <thead>
  68 |                 <tr>
> 69 |                     <th>Name</th>
     |                      ^^
  70 |                     <th>Price</th>
  71 |                     <th>Amount</th>
  72 |                     <th></th>
Details: https://html-validate.org/rules/element-required-attributes.html


error: <th> is missing required "scope" attribute (element-required-attributes) at 785a0e99-359a-4905-b490-3a62c61fbf37.html:70:22:
  68 |                 <tr>
  69 |                     <th>Name</th>
> 70 |                     <th>Price</th>
     |                      ^^
  71 |                     <th>Amount</th>
  72 |                     <th></th>
  73 |                 </tr>
Details: https://html-validate.org/rules/element-required-attributes.html

...
```

### Log

Additional details of validation execution can be found in Atata log.

```
...
2021-07-23 19:06:22.7088  INFO > Validate: "<app>" page HTML document
2021-07-23 19:06:22.7102 TRACE - > Get page source HTML
2021-07-23 19:06:22.7319 TRACE - < Get page source HTML (0.021s)
2021-07-23 19:06:22.7434 TRACE - HTML saved to file "D:\Development\atata-sample-app-tests\test\AtataSampleApp.UITests\bin\Debug\netcoreapp3.1\artifacts\2021-07-23 19_05_57\HtmlPageValidationTests\Validate(products)\785a0e99-359a-4905-b490-3a62c61fbf37.html"
2021-07-23 19:06:23.3315 TRACE - > Execute html-validate CLI command for "785a0e99-359a-4905-b490-3a62c61fbf37.html" with "stylish" formatter
2021-07-23 19:06:24.2595 TRACE - < Execute html-validate CLI command for "785a0e99-359a-4905-b490-3a62c61fbf37.html" with "stylish" formatter (0.927s) >> { IsSuccessful = False }
2021-07-23 19:06:24.2615 TRACE - > Execute html-validate CLI command for "785a0e99-359a-4905-b490-3a62c61fbf37.html" with "codeframe" formatter
2021-07-23 19:06:25.1164 TRACE - < Execute html-validate CLI command for "785a0e99-359a-4905-b490-3a62c61fbf37.html" with "codeframe" formatter (0.854s) >> { IsSuccessful = False }
2021-07-23 19:06:25.1203  INFO - HTML validation report saved to file "D:\Development\atata-sample-app-tests\test\AtataSampleApp.UITests\bin\Debug\netcoreapp3.1\artifacts\2021-07-23 19_05_57\HtmlPageValidationTests\Validate(products)\785a0e99-359a-4905-b490-3a62c61fbf37.txt"
2021-07-23 19:06:31.4848  INFO < Validate: "<app>" page HTML document (8.775s) >> NUnit.Framework.AssertionException: Wrong "<app>" page HTML document, which contains errors...
2021-07-23 19:06:31.5586 ERROR Wrong "<app>" page HTML document, which contains errors:
785a0e99-359a-4905-b490-3a62c61fbf37.html
  69:22  error  <th> is missing required "scope" attribute     element-required-attributes
  70:22  error  <th> is missing required "scope" attribute     element-required-attributes
  71:22  error  <th> is missing required "scope" attribute     element-required-attributes
  72:22  error  <th> is missing required "scope" attribute     element-required-attributes
  81:26  error  <button> is missing required "type" attribute  element-required-attributes
  82:26  error  <button> is missing required "type" attribute  element-required-attributes
  83:26  error  <button> is missing required "type" attribute  element-required-attributes

✖ 7 problems (7 errors, 0 warnings)

More information:
  https://html-validate.org/rules/element-required-attributes.html
...
```

## Sample Project

Check out [atata-framework/atata-sample-app-tests](https://github.com/atata-framework/atata-sample-app-tests) repository, which contains [`HtmlPageValidationTests`](https://github.com/atata-framework/atata-sample-app-tests/blob/master/test/AtataSampleApp.UITests/HtmlPageValidationTests.cs) test class that validates HTML of some pages.
It also contains a sample [`.htmlvalidate.json`](https://github.com/atata-framework/atata-sample-app-tests/blob/master/test/AtataSampleApp.UITests/.htmlvalidate.json) configuration file.

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
