namespace Atata.HtmlValidation;

/// <summary>
/// The enumeration of conditions of HTML saving.
/// </summary>
public enum HtmlSaveCondition
{
    /// <summary>
    /// Never save.
    /// </summary>
    Never,

    /// <summary>
    /// Save when HTML is invalid.
    /// </summary>
    Invalid,

    /// <summary>
    /// Always save.
    /// </summary>
    Always
}
