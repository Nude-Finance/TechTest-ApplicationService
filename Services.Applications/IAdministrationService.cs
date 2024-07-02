using Services.Common.Abstractions.Model;

namespace Services.Applications;

public interface IAdministrationService
{
    /// <exception cref="AdministratorException">Thrown if downstream systems indicate failures.</exception>
    Task<CreateInvestorResponse> CreateInvestorAsync(Application application);
}