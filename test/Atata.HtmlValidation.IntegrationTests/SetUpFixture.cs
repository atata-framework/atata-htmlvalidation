namespace Atata.HtmlValidation.IntegrationTests;

[SetUpFixture]
public class SetUpFixture
{
    [OneTimeSetUp]
    public void GlobalSetUp()
    {
        AtataContext.GlobalConfiguration
            .UseChrome()
                .WithArguments("start-maximized")
            .UseCulture("en-US")
            .UseAllNUnitFeatures();

        AtataContext.GlobalConfiguration.AutoSetUpDriverToUse();
    }
}
