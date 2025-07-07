namespace Atata.HtmlValidation.IntegrationTests;

public sealed class PageObjectHtmlValidateExtensionsTests : AtataTestSuite
{
    [Test]
    public void ValidateHtml_WithoutErrors() =>
        GoToTestPage("Errors0.html")
            .ValidateHtml();

    [Test]
    public void ValidateHtml_WithErrors_WithOptions()
    {
        // Arrange
        HtmlValidationOptions options = new()
        {
            OutputFormatter = HtmlValidateFormatter.Names.Json,
            WorkingDirectory = "HtmlValidation"
        };

        var sut = GoToTestPage("Errors1.html");

        // Act
        var act = sut.ToSutSubject()
            .Invoking(x => x.ValidateHtml(options, false));

        // Assert
        act.Should.Throw<global::NUnit.Framework.AssertionException>()
            .ValueOf(x => x.Message).Should.Contain("\"errorCount\"");

        Context.Artifacts.Directories["HtmlValidation"].Files.Should.HaveCount(2);

        var assertionResults = TestExecutionContext.CurrentContext.CurrentResult.AssertionResults;

        assertionResults.ToSubject(nameof(assertionResults))
            .ValueOf(x => x.Count).Should.Be(1)
            .ValueOf(x => x[0].Status).Should.Be(global::NUnit.Framework.Interfaces.AssertionStatus.Failed);

        assertionResults.Clear();
    }

    [Test]
    public void ValidateHtml_WithErrors_AsWarning()
    {
        // Arrange
        var sut = GoToTestPage("Errors1.html");

        // Act
        sut.ValidateHtml(asWarning: true);

        // Assert
        var assertionResults = TestExecutionContext.CurrentContext.CurrentResult.AssertionResults;

        assertionResults.ToSubject(nameof(assertionResults))
            .ValueOf(x => x.Count).Should.Be(1)
            .ValueOf(x => x[0].Status).Should.Be(global::NUnit.Framework.Interfaces.AssertionStatus.Warning);

        assertionResults.Clear();
    }

    private static OrdinaryPage GoToTestPage(string name)
    {
        string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestPages", name);

        string urlPrefix = RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Windows)
            ? "file:///"
            : "file://";

        string url = urlPrefix + filePath;

        return Go.To<OrdinaryPage>(url: url);
    }
}
