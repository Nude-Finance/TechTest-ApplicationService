using Services.Common.Abstractions.Abstractions;
using Services.Common.Abstractions.Model;

namespace Services.Applications;

public class ApplicationProcessor(
    IAdministrationService administrationService,
    IKycService kycService,
    IBus bus) : IApplicationProcessor
{
    public async Task Process(Application application)
    {
        ApplicationFailureReason? failureReason = 
            Verify(application) ?? await RunKyc(application.Applicant);

        if (failureReason is not null)
        {
            await bus.PublishAsync(new ApplicationFailed(application.Id, failureReason));
            return;
        }

        CreateInvestorResponse r =
            await administrationService.CreateInvestorAsync(application);

        await bus.PublishAsync(new InvestorCreated(application.Applicant.Id, r.InvestorId));
        await bus.PublishAsync(new AccountCreated(r.InvestorId, application.ProductCode, r.AccountId));
        await bus.PublishAsync(new ApplicationCompleted(application.Id));
    }

    private async Task<ApplicationFailureReason?> RunKyc(User applicant)
    {
        Result<KycReport> kycReport = await kycService.GetKycReportAsync(applicant);

        if (!kycReport.IsSuccess)
        {
            return new KycServiceError(kycReport.Error);
        }
        
        if (!kycReport.Value.IsVerified)
        {
            await bus.PublishAsync(new KycFailed(applicant.Id, kycReport.Value.Id));
            return new KycVerificationFailed();
        }

        return null;
    }

    private static ApplicationFailureReason? Verify(Application application)
    {
        if (application.Applicant.IsVerified != true)
        {
            return new UserNotVerified();
        }
        
        int applicantAge = GetFullYearsSince(application.Applicant.DateOfBirth);

        Product product = Products[application.ProductCode];
        
        var ageLimits = product.AgeLimits;
        
        if (applicantAge < ageLimits.Min)
        {
            return new ApplicantTooYoung();
        }

        if (applicantAge > ageLimits.Max)
        {
            return new ApplicantTooOld();
        }

        (string? currency, decimal amount) = application.Payment.Amount;

        // TODO: Verify currency.
        
        if (amount < product.MinimumPaymentAmount)
        {
            return new InsufficientPaymentAmount();
        }
        
        return null;
    }

    record AgeLimits(int Min, int Max);

    record Product(AgeLimits AgeLimits, decimal MinimumPaymentAmount);
    
    // TODO: move to config
    private static readonly Dictionary<ProductCode, Product> Products = new()
    {
        { ProductCode.ProductOne, new Product(new AgeLimits(18, 39), MinimumPaymentAmount: .99m) },
        { ProductCode.ProductTwo, new Product(new AgeLimits(18, 50), MinimumPaymentAmount: .99m) },
    };

    private static int GetFullYearsSince(DateOnly date)
    {
        DateTime today = DateTime.Now.Date;
        int age = today.Year - date.Year;

        if (date.Month < today.Month || (date.Month == today.Month && date.Day < today.Day))
        {
            --age;
        }

        return age;
    }
}
