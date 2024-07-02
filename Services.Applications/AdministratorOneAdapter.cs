using Services.AdministratorOne.Abstractions.Model;
using Services.Common.Abstractions.Model;

namespace Services.Applications;

public class AdministratorOneAdapter(
    Services.AdministratorOne.Abstractions.IAdministrationService administrationService)
    : IAdministrationService
{
    public Task<CreateInvestorResponse> CreateInvestorAsync(Application application)
    {
        User applicant = application.Applicant;
        Address? applicantAddress = applicant.Addresses.FirstOrDefault();

        // TODO: or application.Payment.BankAccount?
        BankAccount? bankAccount = applicant.BankAccounts.FirstOrDefault();

        var createInvestorRequest = new CreateInvestorRequest
        {
            FirstName = applicant.Forename,
            LastName = applicant.Surname,
            Addressline1 = applicantAddress?.Addressline1 ?? "",
            Addressline2 = applicantAddress?.Addressline2 ?? "",
            Addressline3 = applicantAddress?.Addressline3 ?? "",
            Addressline4 = "",
            PostCode = applicantAddress?.PostCode ?? "",
            Nino = applicant.Nino,
            AccountNumber = bankAccount?.AccountNumber ?? "",
            SortCode = bankAccount?.SortCode ?? "",

            // TODO: what goes in here?
            Reference = "",
            Email = "",
            MobileNumber = "",

            // TODO: check the expected format
            DateOfBirth = applicant.DateOfBirth.ToShortDateString(),
            Product = application.ProductCode.ToString(),
            InitialPayment = (int) (application.Payment.Amount.Amount * 100),
        };

        try
        {
            // TODO: Run this on a separate thread if this call is not CPU-bound.
            AdministratorOne.Abstractions.Model.CreateInvestorResponse r =
                administrationService.CreateInvestor(createInvestorRequest);

            var createInvestorResponse = new CreateInvestorResponse(
                r.Reference, r.InvestorId, r.AccountId, r.PaymentId);
            
            return Task.FromResult(createInvestorResponse);
        }
        catch (Services.AdministratorOne.Abstractions.Model.AdministratorException exception)
        {
            // Convert AdministratorOne exceptions to Services.Applications exceptions
            Error error = GetError(exception);
            throw new AdministratorException(error, inner: exception);
        }
    }

    private static Error GetError(AdministratorOne.Abstractions.Model.AdministratorException e)
    {
        // TODO: Assuming that "AdministratorOne" a valid name for a "system".
        string system = nameof(AdministratorOne);

        // TODO: Assuming AdministratorOne and the rest of the system share the same set of error codes.
        string code = e.Code;

        // TODO: The message might require translation.
        string message = e.Message;

        return new Error(system, code, message);
    }
}