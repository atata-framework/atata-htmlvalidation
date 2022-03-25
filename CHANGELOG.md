# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Changed

- Upgrade `Atata.Cli.HtmlValidate` package to v1.4.0.
- Upgrade `Atata` package to v1.14.0.

## [1.4.0] - 2021-07-24

### Changed

- Change `HtmlValidationOptions.HtmlValidatePackageVersion` property default value to `5.2.0`.

## [1.3.0] - 2021-07-23

### Added

- Add `ValidateHtmlAttribute` trigger attribute.

### Changed

- Add `ComponentFullName` of page object to error message in `PageObjectHtmlValidateExtensions.ValidateHtml` method.
- Reformat main log message in `PageObjectHtmlValidateExtensions.ValidateHtml` method.
- Upgrade `Atata.Cli.HtmlValidate` package to v1.3.0.

## [1.2.0] - 2021-07-21

### Changed

- Upgrade `Atata.Cli.HtmlValidate` package to v1.2.0.
- Replace `RecommendedHtmlValidatePackageVersion` property of `HtmlValidationOptions` with new `HtmlValidatePackageVersion`.

## [1.1.0] - 2021-07-14

### Added

- Add method to `HtmlValidationResult`:
  ```cs
  public HtmlValidationResult MoveFilesToDirectory(string directory);
  ```
- Add property to `HtmlValidationOptions`:
  ```cs
  public string RecommendedHtmlValidatePackageVersion { get; set; } = "5.x";
  ```

### Changed

- Make `HtmlValidationOptions` `bool KeepHtmlFileWhenValid` property obsolete
  and replace it with new `HtmlSaveCondition SaveHtmlToFile` property.
- Upgrade `Atata.Cli.HtmlValidate` package to v1.1.0.
- Improve logging of `HtmlValidator`.

## [1.0.0] - 2021-06-28

Initial version release.