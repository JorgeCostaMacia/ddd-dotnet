using FluentValidation;
using JorgeCostaMacia.DDD.ValueObject.Domain;

namespace JorgeCostaMacia.DDD.ValueObject.Tests.Domain;

public sealed class PageSizeValueObjectValidatorTests
{
    private static readonly PageSizeValueObjectValidator _validator = PageSizeValueObjectValidator.Create();

    [Theory]
    [InlineData(1)]
    [InlineData(50)]
    [InlineData(10000)]
    public void Validate_WithAPositiveSize_Passes(int value)
        => Assert.True(_validator.Validate(PageSizeValueObject.From(value)).IsValid);

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validate_WithZeroOrANegativeSize_Fails(int value)
        => Assert.False(_validator.Validate(PageSizeValueObject.From(value)).IsValid);

    [Fact]
    public void ValidateAndThrow_WithZero_ThrowsTheTypedException()
        => Assert.Throws<PageSizeValueObjectValidationException>(
            () => _validator.ValidateAndThrow(PageSizeValueObject.From(0)));
}
