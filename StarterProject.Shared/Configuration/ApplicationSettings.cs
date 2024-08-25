namespace StarterProject.Shared.Configuration;

public class ApplicationSettings : IApplicationSettings
{
    public string CompanyName => "Company Name";

    public string ApplicationName => "Starter Project";

    public string FullApplicationName => $"{CompanyName} {ApplicationName}".Trim();
}