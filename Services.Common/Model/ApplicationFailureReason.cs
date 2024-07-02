namespace Services.Common.Abstractions.Model;

public abstract record ApplicationFailureReason;

public record ApplicantTooYoung : ApplicationFailureReason;
public record ApplicantTooOld : ApplicationFailureReason;
public record UserNotVerified : ApplicationFailureReason;
public record KycServiceError(Error Error) : ApplicationFailureReason;
public record KycVerificationFailed : ApplicationFailureReason;
public record InsufficientPaymentAmount : ApplicationFailureReason;
