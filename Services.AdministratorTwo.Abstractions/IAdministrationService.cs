using Services.Common.Abstractions.Model;

namespace Services.AdministratorTwo.Abstractions;

public interface IAdministrationService
{
    Task<Result<Guid>> CreateInvestorAsync(User user);
    
    Task<Result<Guid>> CreateAccountAsync(Guid investorId, ProductCode productCode);
    
    Task<Result<Guid>> ProcessPaymentAsync(Guid accountId, Payment payment);
}