using Atata.Cli.HtmlValidate;

namespace Atata.HtmlValidation.IntegrationTests;

[TestFixture]
public class PageObjectHtmlValidateExtensionsTests : UITestFixture
{
    [Test]
    public void ValidateHtml_WithoutErrors() =>
        GoToTestPage("Errors0.html")
            .ValidateHtml();

    [Test]
    public void ValidateHtml_WithErrors_WithOptions()
    {
        var options = new HtmlValidationOptions
        {
            OutputFormatter = HtmlValidateFormatter.Names.Json
        };

        var sut = GoToTestPage("Errors1.html");

        var exception = Assert.Throws<NUnit.Framework.AssertionException>(
            () => sut.ValidateHtml(options));

        exception.ToResultSubject()
            .ValueOf(x => x.Message).Should.Contain("\"errorCount\"");
    }

    [Test]
    public void ValidateHtml_WithErrors_AsWarning()
    {
        GoToTestPage("Errors1.html")
            .ValidateHtml(asWarning: true);

        var assertionResults = TestExecutionContext.CurrentContext.CurrentResult.AssertionResults;

        assertionResults.ToSubject(nameof(assertionResults))
            .ValueOf(x => x.Count).Should.Equal(1)
            .ValueOf(x => x[0].Status).Should.Equal(NUnit.Framework.Interfaces.AssertionStatus.Warning);

        assertionResults.Clear();
    }

    private static OrdinaryPage GoToTestPage(string name)
    {
        string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestPages", name);
        string url = "file:///" + filePath;

        return Go.To<OrdinaryPage>(url: url);
    }
}
