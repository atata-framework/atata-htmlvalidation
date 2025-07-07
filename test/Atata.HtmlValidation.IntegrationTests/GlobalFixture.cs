[assembly: SetCulture("en-US")]
[assembly: Parallelizable(ParallelScope.Fixtures)]

namespace Atata.HtmlValidation.IntegrationTests;

[SetUpFixture]
public sealed class GlobalFixture : AtataGlobalFixture
{
    protected override void ConfigureAtataContextGlobalProperties(AtataContextGlobalProperties globalProperties) =>
        globalProperties.UseRootNamespaceOf<GlobalFixture>();

    protected override void ConfigureAtataContextBaseConfiguration(AtataContextBuilder builder) =>
        builder.Sessions.AddWebDriver(x => x
            .UseStartScopes(AtataContextScopes.Test)
            .UseChrome(x => x
                .WithArguments(
                    "window-size=1200,800",
                    "headless=new",
                    "disable-search-engine-choice-screen")));
}
