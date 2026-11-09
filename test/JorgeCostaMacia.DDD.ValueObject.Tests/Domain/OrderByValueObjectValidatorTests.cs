using FluentValidation;
using JorgeCostaMacia.DDD.ValueObject.Domain;

namespace JorgeCostaMacia.DDD.ValueObject.Tests.Domain;

public sealed class OrderByValueObjectValidatorTests
{
    private static readonly OrderByValueObjectValidator _validator = OrderByValueObjectValidator.Create();

    [Theory]
    [InlineData("name")]
    [InlineData("created_at")]
    public void Validate_WithANonEmptyName_Passes(string value)
        => Assert.True(_validator.Validate(OrderByValueObject.From(value)).IsValid);

    [Fact]
    public void Validate_WithAnEmptyName_Fails()
        => Assert.False(_validator.Validate(OrderByValueObject.From("")).IsValid);

    [Fact]
    public void ValidateAndThrow_WithAnEmptyName_ThrowsTheTypedException()
        => Assert.Throws<OrderByValueObjectValidationException>(
            () => _validator.ValidateAndThrow(OrderByValueObject.From("")));
}
