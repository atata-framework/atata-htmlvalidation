# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Changed

- Upgrade Atata package reference to v4.0.0-beta.7.
- Upgrade Atata.Cli.HtmlValidate package reference to v4.0.0-beta.2.
- Enable nullable reference types feature.
- Change `HtmlValidationOptions.HtmlValidatePackageVersion` property default value to `"10.0.0"`.

## [3.2.0] - 2025-01-28

### Changed

- Upgrade Atata package reference to v3.4.0.
- Change `HtmlValidationOptions.HtmlValidatePackageVersion` property default value to `"8.29.0"`.

## [3.1.0] - 2024-05-18

### Changed

- Upgrade Atata package reference to v3.1.0.

### Fixed

- Validation result output cuts off when greater than 65536 characters on non-Windows OSs (#9).

## [3.0.0] - 2024-04-16

### Changed

- Upgrade Atata package to v3.0.0.
- Change `HtmlValidationOptions.HtmlValidatePackageVersion` property default value to `"8.18.1"`.

### Removed

- Remove obsolete `HtmlValidationOptions` properties: `KeepHtmlFileWhenValid` and `RecommendedHtmlValidatePackageVersion`.
- Remove `Func<AtataContext, string> WorkingDirectoryBuilder` property of `HtmlValidationOptions` in favor of using `string WorkingDirectory` property.

## [2.5.0] - 2024-01-12

### Changed

- Upgrade Atata package to v2.14.0.
- Change `HtmlValidationOptions.HtmlValidatePackageVersion` property default value to `"8.9.1"`.

## [2.4.0] - 2023-09-01

### Changed

- Change `HtmlValidationOptions.HtmlValidatePackageVersion` property default value to `"8.3.0"`.
- Upgrade Atata package to v2.9.0.
- Upgrade Atata.Cli.HtmlValidate package to v2.3.0.

## [2.3.0] - 2023-02-20

### Changed

- Change `HtmlValidationOptions.HtmlValidatePackageVersion` property default value to `"7.13.2"`.
- Upgrade Atata package to v2.7.0.

## [2.2.0] - 2022-10-04

### Changed

- Upgrade Atata.Cli.HtmlValidate package to v2.2.0.
- Upgrade Atata package to v2.2.0.
- Change `HtmlValidationOptions.HtmlValidatePackageVersion` property default value to `"7.5.0"`.

## [2.1.0] - 2022-07-21

### Changed

- Upgrade Atata.Cli.HtmlValidate package to v2.1.0.
- Upgrade Atata package to v2.1.0.
- Change `HtmlValidationOptions.HtmlValidatePackageVersion` property default value to `"7.1.2"`.

## [2.0.0] - 2022-05-11

### Added

- Add optional `AtataContext atataContext` parameter to `HtmlValidator` constructor.

### Changed

- Change `HtmlValidationOptions.HtmlValidatePackageVersion` property default value to `"7.0.0"`.
- Get rid of `AtataContext.Current` usages.
- Change `HtmlValidationOptions.WorkingDirectoryBuilder` property type from `Func<string>` to `Func<AtataContext, string>`.
- Upgrade Atata.Cli.HtmlValidate package to v2.0.0.
- Upgrade Atata package to v2.0.0.

## [1.5.0] - 2022-03-26

### Added

- Add `public ICliCommandFactory CliCommandFactory { get; set; }` property to `HtmlValidationOptions`.

### Changed

- Upgrade Atata.Cli.HtmlValidate package to v1.4.0.
- Upgrade Atata package to v1.14.0.
- Change `HtmlValidationOptions.HtmlValidatePackageVersion` property default value to `"5.5.0"`.

## [1.4.0] - 2021-07-24

### Changed

- Change `HtmlValidationOptions.HtmlValidatePackageVersion` property default value to `"5.2.0"`.

## [1.3.0] - 2021-07-23

### Added

- Add `ValidateHtmlAttribute` trigger attribute.

### Changed

- Add `ComponentFullName` of page object to error message in `PageObjectHtmlValidateExtensions.ValidateHtml` method.
- Reformat main log message in `PageObjectHtmlValidateExtensions.ValidateHtml` method.
- Upgrade Atata.Cli.HtmlValidate package to v1.3.0.

## [1.2.0] - 2021-07-21

### Changed

- Upgrade Atata.Cli.HtmlValidate package to v1.2.0.
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
- Upgrade Atata.Cli.HtmlValidate package to v1.1.0.
- Improve logging of `HtmlValidator`.

## [1.0.0] - 2021-06-28

Initial version release.
