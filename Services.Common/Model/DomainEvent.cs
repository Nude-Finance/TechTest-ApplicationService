namespace Services.Common.Abstractions.Model;

public abstract record DomainEvent;

public record InvestorCreated(Guid UserId, string InvestorId) : DomainEvent;

public record AccountCreated(string InvestorId, ProductCode Product, string AccountId) : DomainEvent;

public record KycFailed(Guid UserId, Guid ReportId) : DomainEvent;

public record ApplicationCompleted(Guid ApplicationId) : DomainEvent;

public record ApplicationFailed(Guid ApplicationId) : DomainEvent;


