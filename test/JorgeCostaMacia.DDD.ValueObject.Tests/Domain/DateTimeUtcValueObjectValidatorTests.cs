using FluentValidation;
using JorgeCostaMacia.DDD.ValueObject.Domain;

namespace JorgeCostaMacia.DDD.ValueObject.Tests.Domain;

public sealed class DateTimeUtcValueObjectValidatorTests
{
    private static readonly DateTimeUtcValueObjectValidator _validator = DateTimeUtcValueObjectValidator.Create();

    [Fact]
    public void Validate_WithADateWithinRange_Passes()
        => Assert.True(
            _validator.Validate(
                DateTimeUtcValueObject.From(new DateTime(2020, 6, 15, 0, 0, 0, DateTimeKind.Utc))).IsValid);

    [Fact]
    public void Validate_WithADateBefore1900_Fails()
        => Assert.False(
            _validator.Validate(
                DateTimeUtcValueObject.From(new DateTime(1800, 1, 1, 0, 0, 0, DateTimeKind.Utc))).IsValid);

    // The zeros other systems write when they have no date: .NET's default, SQL Server's datetime minimum,
    // Delphi/OLE/Excel's zero and SQL Server's CAST(0 AS datetime).
    [Theory]
    [InlineData(1, 1, 1)]
    [InlineData(1753, 1, 1)]
    [InlineData(1899, 12, 30)]
    [InlineData(1900, 1, 1)]
    public void Validate_WithTheZeroDateOfAnotherSystem_Fails(
        int year,
        int month,
        int day
    )
        => Assert.False(
            _validator.Validate(
                DateTimeUtcValueObject.From(new DateTime(year, month, day, 0, 0, 0, DateTimeKind.Utc))).IsValid);

    [Fact]
    public void ValidateAndThrow_WithADateBefore1900_ThrowsTheTypedException()
        => Assert.Throws<DateTimeUtcValueObjectValidationException>(
            () => _validator.ValidateAndThrow(
                DateTimeUtcValueObject.From(new DateTime(1800, 1, 1, 0, 0, 0, DateTimeKind.Utc))));

    // The floor is strict: midnight on 1900-01-01 is a zero, any later instant of that day is a date.
    [Fact]
    public void Validate_WithADateJustAfterTheFloor_Passes()
        => Assert.True(
            _validator.Validate(
                DateTimeUtcValueObject.From(new DateTime(1900, 1, 1, 0, 0, 1, DateTimeKind.Utc))).IsValid);

    [Fact]
    public void Validate_WithADateAtTheMaxBoundary_Passes()
        => Assert.True(_validator.Validate(DateTimeUtcValueObject.From(DateTime.UtcNow.Date.AddYears(100))).IsValid);

    [Fact]
    public void Validate_WithADateAfterTheMaxBoundary_Fails()
        => Assert.False(
            _validator.Validate(DateTimeUtcValueObject.From(DateTime.UtcNow.Date.AddYears(100).AddDays(1))).IsValid);

    // The ceiling binds every date: a far-future sentinel does not pass.
    [Fact]
    public void Validate_WithAnOpenEndSentinel_Fails()
        => Assert.False(
            _validator.Validate(
                DateTimeUtcValueObject.From(new DateTime(9999, 12, 31, 0, 0, 0, DateTimeKind.Utc))).IsValid);
}
