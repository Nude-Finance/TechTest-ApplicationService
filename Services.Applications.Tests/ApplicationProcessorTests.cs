using Moq;
using NUnit.Framework;
using Services.Common.Abstractions.Abstractions;
using Services.Common.Abstractions.Model;
using static Services.Common.Abstractions.Model.ProductCode;

namespace Services.Applications.Tests;

[TestFixture]
public class ApplicationProcessorTests
{
    private ApplicationProcessor _applicationProcessor = null!;
    private Mock<IBus> _busMock = null!;
    private Mock<IKycService> _kycMock = null!;
    private Mock<IAdministrationService> _administratorMock = null!;
    private Application _validApplication = null!;
    private const string Reference = "reference";
    private const string InvestorId = "investorId";
    private const string AccountId = "accountId";
    private const string PaymentId = "paymentId";

    [SetUp]
    public void Setup()
    {
        _busMock = new Mock<IBus>();

        _kycMock = new Mock<IKycService>();
        _kycMock.Setup(k => k.GetKycReportAsync(It.IsAny<User>()))
            .ReturnsAsync(Result.Success(new KycReport(Guid.NewGuid(), true)));

        IKycService kycService = new KycReportCache(_kycMock.Object);

        _administratorMock = new Mock<IAdministrationService>();
        
        _administratorMock.Setup(a => a.CreateInvestorAsync(It.IsAny<Application>()))
            .ReturnsAsync(new CreateInvestorResponse(Reference, InvestorId, AccountId,
                PaymentId));
        
        _applicationProcessor = new ApplicationProcessor(
            _administratorMock.Object,
            kycService,
            _busMock.Object);
        
        _validApplication = CreateApplication(applicantAge: 30, ProductOne);
    }
    
    [Test]
    public async Task CreateInvestor_called_when_Verification_successful()
    {
        await _applicationProcessor.Process(_validApplication);

        _administratorMock.Verify(
            a => a.CreateInvestorAsync(_validApplication),
            Times.Once);
    }

    [Test]
    public async Task ApplicationFailed_is_published_when_Applicant_is_too_young()
    {
        Application application = CreateApplication(applicantAge: 17, ProductOne);
        
        await _applicationProcessor.Process(application);

        _busMock.Verify(
            b => b.PublishAsync(new ApplicationFailed(application.Id, new ApplicantTooYoung())),
            Times.Once());
    }

    [Test]
    public async Task ApplicationFailed_is_published_when_Applicant_is_too_old()
    {
        Application application = CreateApplication(applicantAge: 70, ProductOne);
        
        await _applicationProcessor.Process(application);

        _busMock.Verify(
            b => b.PublishAsync(new ApplicationFailed(application.Id, new ApplicantTooOld())),
            Times.Once());
    }

    [Test]
    public async Task ApplicationCompleted_is_published_when_All_checks_pass()
    {
        await _applicationProcessor.Process(_validApplication);

        _busMock.Verify(b => b.PublishAsync(new ApplicationCompleted(_validApplication.Id)), Times.Once());
    }

    [Test]
    public async Task ApplicationCompleted_is_published_when_All_checks_pass_ProductTwo()
    {
        Application application = CreateApplication(applicantAge: 42, ProductTwo);
        
        await _applicationProcessor.Process(application);

        _busMock.Verify(b => b.PublishAsync(new ApplicationCompleted(application.Id)), Times.Once());
    }

    [Test]
    public async Task GetKycReport_called_once_when_Process_called_twice_for_same_user()
    {
        await _applicationProcessor.Process(_validApplication);
        await _applicationProcessor.Process(_validApplication);

        _kycMock.Verify(s => s.GetKycReportAsync(_validApplication.Applicant), Times.Once);
    }

    private static Application CreateApplication(
        int applicantAge,
        ProductCode productCode) =>
        new()
        {
            Id = Guid.NewGuid(),
            ProductCode = productCode,
            Applicant = new User
            {
                DateOfBirth = GetDob(applicantAge),
                IsVerified = true
            },
            Payment = new Payment(new BankAccount(), new Money("GBP", 100m))
        };

    private static DateOnly GetDob(int applicantAge)
    {
        var now = DateTime.Now;
        var dob = new DateOnly(now.Year - applicantAge, now.Month, now.Day);
        return dob;
    }
}
