namespace ApplicationInsightsRepro.IntegrationTests.Fixtures;

public class DefaultTestFixture
{
    internal TestWebApplicationFactory Factory { get; }
    public HttpClient ServerClient { get; }

    public DefaultTestFixture()
    {
        Factory = new TestWebApplicationFactory();

        ServerClient = Factory.CreateClient();
        ServerClient.Timeout = TimeSpan.FromMinutes(5);
    }
}
