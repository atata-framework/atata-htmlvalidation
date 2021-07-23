namespace Atata.HtmlValidation
{
    /// <summary>
    /// The trigger attribute that indicates that the page HTML should be validated on the specified event.
    /// By default occurs upon the page object initialization.
    /// Invokes <see cref="PageObjectHtmlValidateExtensions.ValidateHtml{TPageObject}(TPageObject, HtmlValidationOptions, bool)"/> method
    /// using <see cref="HtmlValidationOptions.Default"/> options.
    /// </summary>
    public class ValidateHtmlAttribute : TriggerAttribute
    {
        public ValidateHtmlAttribute(TriggerEvents on = TriggerEvents.Init, TriggerPriority priority = TriggerPriority.Medium)
            : base(on, priority)
        {
        }

        /// <summary>
        /// Gets or sets a value indicating whether to produce a warning instead of assertion exception on validation failure.
        /// The default value is <see langword="false"/>.
        /// </summary>
        public bool AsWarning { get; set; }

        protected override void Execute<TOwner>(TriggerContext<TOwner> context)
        {
            context.Component.Owner.ValidateHtml(asWarning: AsWarning);
        }
    }
}
