namespace StarterProject.Shared.Configuration;

public interface IApplicationSettings
{
    string CompanyName { get; }    
    string ApplicationName { get; }
    string FullApplicationName { get; }
}
