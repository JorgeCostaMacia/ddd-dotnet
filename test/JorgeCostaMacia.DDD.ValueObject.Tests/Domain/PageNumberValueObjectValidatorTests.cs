using FluentValidation;
using JorgeCostaMacia.DDD.ValueObject.Domain;

namespace JorgeCostaMacia.DDD.ValueObject.Tests.Domain;

public sealed class PageNumberValueObjectValidatorTests
{
    private static readonly PageNumberValueObjectValidator _validator = PageNumberValueObjectValidator.Create();

    [Theory]
    [InlineData(1)]
    [InlineData(5)]
    [InlineData(100)]
    public void Validate_WithAPositiveNumber_Passes(int value)
        => Assert.True(_validator.Validate(PageNumberValueObject.From(value)).IsValid);

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-100)]
    public void Validate_WithZeroOrANegativeNumber_Fails(int value)
        => Assert.False(_validator.Validate(PageNumberValueObject.From(value)).IsValid);

    [Fact]
    public void ValidateAndThrow_WithZero_ThrowsTheTypedException()
        => Assert.Throws<PageNumberValueObjectValidationException>(
            () => _validator.ValidateAndThrow(PageNumberValueObject.From(0)));
}
