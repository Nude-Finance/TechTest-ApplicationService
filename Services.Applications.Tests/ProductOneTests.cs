using Moq;
using NUnit.Framework;
using Services.Common.Abstractions.Abstractions;
using Services.Common.Abstractions.Model;

namespace Services.Applications.Tests;

[TestFixture]
public class ProductOneTests
{
    [Test]
    public async Task ApplicationFailed_is_published_when_Applicant_is_too_young()
    {
        Application application = CreateApplication(applicantAge: 17);
        
        var mock = new Mock<IBus>();
        IBus bus = mock.Object;
        
        await new ApplicationProcessor(bus).Process(application);

        mock.Verify(b => b.PublishAsync(It.IsAny<ApplicationFailed>()), Times.Once());
    }

    [Test]
    public async Task ApplicationCompleted_is_published_when_All_checks_pass()
    {
        Application application = CreateApplication(applicantAge: 30);
        
        var mock = new Mock<IBus>();
        IBus bus = mock.Object;
        
        await new ApplicationProcessor(bus).Process(application);

        mock.Verify(b => b.PublishAsync(It.IsAny<ApplicationCompleted>()), Times.Once());
    }

    private static Application CreateApplication(int applicantAge)
    {
        var now = DateTime.Now;
        var dob = new DateOnly(now.Year - applicantAge, now.Month, now.Day);
        var application = new Application
        {
            ProductCode = ProductCode.ProductOne,
            Applicant = new User { DateOfBirth = dob }
        };
        return application;
    }
}