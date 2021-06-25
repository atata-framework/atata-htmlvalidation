using System;
using Atata.HtmlValidation;

namespace Atata
{
    /// <summary>
    /// Provides HTML validation extension method for page object.
    /// </summary>
    public static class PageObjectHtmlValidateExtensions
    {
        /// <summary>
        /// Validates the HTML of current page.
        /// Relies on <see cref="HtmlValidator"/> class
        /// that uses "html-validate" NPM package to execute HTML validation.
        /// If "html-validate" package is not installed, tries to install it.
        /// </summary>
        /// <typeparam name="TPageObject">The type of the page object.</typeparam>
        /// <param name="pageObject">The page object.</param>
        /// <param name="options">The options.</param>
        /// <param name="asWarning">If set to <see langword="true"/>, instead of assertion exception produces warning.</param>
        /// <returns>The same page object.</returns>
        public static TPageObject ValidateHtml<TPageObject>(
            this TPageObject pageObject,
            HtmlValidationOptions options = null,
            bool asWarning = false)
            where TPageObject : PageObject<TPageObject>
        {
            AtataContext.Current.Log.ExecuteSection(
                new LogSection($"Validate: HTML document of {pageObject.ComponentFullName}"),
                () =>
                {
                    string html = null;

                    AtataContext.Current.Log.ExecuteSection(
                        new LogSection("Get page source HTML", LogLevel.Trace),
                        () => { html = pageObject.PageSource; });

                    Validate(
                        html,
                        options ?? HtmlValidationOptions.Default ?? new HtmlValidationOptions(),
                        asWarning);
                });

            return pageObject;
        }

        private static void Validate(string html, HtmlValidationOptions options, bool asWarning = false)
        {
            HtmlValidator validator = new HtmlValidator(
                options ?? HtmlValidationOptions.Default ?? new HtmlValidationOptions());

            HtmlValidationResult validationResult = validator.Validate(html);

            if (!validationResult.IsSuccessful)
            {
                string errorMessage = $"HTML document content, which contains errors:{Environment.NewLine}{validationResult.Output}";

                IVerificationStrategy verificationStrategy = asWarning
                    ? (IVerificationStrategy)new ExpectationVerificationStrategy()
                    : new AssertionVerificationStrategy();

                verificationStrategy.ReportFailure(errorMessage, null);
            }
        }
    }
}
