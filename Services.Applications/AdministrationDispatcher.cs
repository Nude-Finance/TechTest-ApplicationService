using System.ComponentModel;
using Services.Common.Abstractions.Model;

namespace Services.Applications;

/// <summary>
/// Dispatches CreateInvestor() calls to either administrator
/// based on the application ProductCode. 
/// </summary>
public class AdministrationDispatcher(
    IAdministrationService administratorOne,
    IAdministrationService administratorTwo) : IAdministrationService
{
    public async Task<CreateInvestorResponse> CreateInvestorAsync(Application application)
    {
        IAdministrationService service = GetAdministrationService(application.ProductCode);

        return await service.CreateInvestorAsync(application);
    }

    private IAdministrationService GetAdministrationService(ProductCode productCode)
    {
        return productCode switch
        {
            ProductCode.ProductOne => administratorOne,
            ProductCode.ProductTwo => administratorTwo,
            
            _ => throw new InvalidEnumArgumentException(
                nameof(productCode),
                (int) productCode,
                typeof(ProductCode))
        };
    }
}