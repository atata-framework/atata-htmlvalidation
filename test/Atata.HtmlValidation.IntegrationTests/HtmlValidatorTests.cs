namespace Atata.HtmlValidation.IntegrationTests;

public class HtmlValidatorTests
{
    [Test]
    public void Validate_WithManyErrors()
    {
        string workingDirectory = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            "artifacts",
            Path.GetRandomFileName());

        HtmlValidator sut = new(new HtmlValidationOptions
        {
            OutputFormatter = HtmlValidateFormatter.Names.Json,
            WorkingDirectory = workingDirectory
        });

        StringBuilder htmlBuilder = new(
            """
            <!DOCTYPE html>
            <html lang="en" xmlns="http://www.w3.org/1999/xhtml">
            <head>
                <meta charset="utf-8">
                <title></title>
            </head>
            <body>
            """);

        for (int i = 0; i < 400; i++)
        {
            htmlBuilder.AppendLine("<span><div>!</div></span>");
        }

        htmlBuilder.Append(
            """
            </body>
            </html>
            """);

        var result = sut.Validate(htmlBuilder.ToString());

        result.ToResultSubject()
            .ValueOf(x => x.IsSuccessful).Should.BeFalse()
            .ValueOf(x => x.Output.Length).Should.BeGreater(100_000)
            .ValueOf(x => x.HtmlFilePath).Should.StartWith(workingDirectory)
            .ValueOf(x => x.ResultFilePath).Should.StartWith(workingDirectory);

        new FileSubject(result.ResultFilePath).Length.Should.BeGreater(100_000);

        Directory.Delete(workingDirectory, true);
    }
}
