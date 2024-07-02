using Services.Common.Abstractions.Model;

namespace Services.Applications;

public class AdministratorTwoAdapter(
    Services.AdministratorTwo.Abstractions.IAdministrationService administrationService)
    : IAdministrationService
{
    public async Task<CreateInvestorResponse> CreateInvestorAsync(Application application)
    {
        Guid investorId = Unwrap(await administrationService.CreateInvestorAsync(
            application.Applicant));

        Guid accountId = Unwrap(await administrationService.CreateAccountAsync(
            investorId,
            application.ProductCode));

        Guid paymentId = Unwrap(await administrationService.ProcessPaymentAsync(
            accountId,
            application.Payment));

        return new CreateInvestorResponse(
            Reference: "", // TODO: what goes in here?
            investorId.ToString(),
            accountId.ToString(),
            paymentId.ToString());
    }
    
    static T Unwrap<T>(Result<T> result)
    {
        return result.IsSuccess ? result.Value : throw new AdministratorException(result.Error);
    }
}