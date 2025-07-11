﻿using Atata.HtmlValidation;

namespace Atata;

/// <summary>
/// Provides HTML validation extension method for page object.
/// </summary>
public static class PageObjectHtmlValidateExtensions
{
    /// <summary>
    /// <para>
    /// Validates the HTML of current page.
    /// Relies on <see cref="HtmlValidator"/> class
    /// that uses "html-validate" NPM package to execute HTML validation.
    /// </para>
    /// <para>
    /// Before the execution of validation, the check of installed "html-validate" package version performs.
    /// The required package version is defined in <see cref="HtmlValidationOptions.HtmlValidatePackageVersion"/> property of <paramref name="options"/>.
    /// The required version will be installed if "html-validate" package is not installed or the installed version differs from the required one.
    /// </para>
    /// <para>
    /// By default, when validation fails, throws an assertion exception with a message containing a list of HTML errors.
    /// Produces a warning instead of assertion exception if <paramref name="asWarning"/> argument is set to <see langword="true"/>.
    /// </para>
    /// </summary>
    /// <typeparam name="TPageObject">The type of the page object.</typeparam>
    /// <param name="pageObject">The page object.</param>
    /// <param name="options">The options.</param>
    /// <param name="asWarning">If set to <see langword="true"/>, instead of assertion exception produces warning.</param>
    /// <returns>The same page object.</returns>
    public static TPageObject ValidateHtml<TPageObject>(
        this TPageObject pageObject,
        HtmlValidationOptions? options = null,
        bool asWarning = false)
        where TPageObject : PageObject<TPageObject>
    {
        pageObject.Session.Log.ExecuteSection(
            new LogSection($"Validate: {pageObject.ComponentFullName} HTML document"),
            () =>
            {
                string? html = null;

                pageObject.Session.Log.ExecuteSection(
                    new LogSection("Get page source HTML", LogLevel.Trace),
                    () => { html = pageObject.PageSource; });

                Validate(
                    pageObject,
                    html!,
                    options,
                    asWarning);
            });

        return pageObject;
    }

    private static void Validate(UIComponent pageObject, string html, HtmlValidationOptions? options, bool asWarning)
    {
        options ??= HtmlValidationOptions.Default ?? new();

        HtmlValidator validator = new(options, pageObject.Session);

        HtmlValidationResult validationResult = validator.Validate(html);

        if (!validationResult.IsSuccessful)
        {
            string errorMessage = $"{pageObject.ComponentFullName} HTML document, which contains errors:{Environment.NewLine}{validationResult.Output}";

            if (asWarning)
                pageObject.Session.RaiseAssertionWarning(errorMessage);
            else
                pageObject.Session.RaiseAssertionError(errorMessage);
        }
    }
}
