namespace Atata.HtmlValidation.IntegrationTests;

[SetUpFixture]
public sealed class GlobalFixture
{
    [OneTimeSetUp]
    public void GlobalSetUp()
    {
        AtataContext.GlobalConfiguration
            .UseChrome()
                .WithArguments(
                    "window-size=1200,800",
                    "headless=new",
                    "disable-search-engine-choice-screen")
            .UseCulture("en-US")
            .UseAllNUnitFeatures();

        AtataContext.GlobalConfiguration.AutoSetUpDriverToUse();
    }
}
