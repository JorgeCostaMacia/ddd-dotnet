using FluentValidation;
using JorgeCostaMacia.DDD.ValueObject.Domain;

namespace JorgeCostaMacia.DDD.ValueObject.Tests.Domain;

public sealed class OrderTypeValueObjectValidatorTests
{
    private static readonly OrderTypeValueObjectValidator _validator = OrderTypeValueObjectValidator.Create();

    [Theory]
    [InlineData("ASC")]
    [InlineData("DESC")]
    [InlineData("asc")]     // From upper-cases, so lowercase input is valid
    [InlineData("desc")]
    public void Validate_WithAscOrDescInAnyCase_Passes(string value)
        => Assert.True(_validator.Validate(OrderTypeValueObject.From(value)).IsValid);

    [Theory]
    [InlineData("")]        // empty
    [InlineData("UP")]      // not a valid direction
    [InlineData("ASCENDING")]
    public void Validate_WithAnythingButAscOrDesc_Fails(string value)
        => Assert.False(_validator.Validate(OrderTypeValueObject.From(value)).IsValid);

    [Fact]
    public void ValidateAndThrow_WithAnUnknownDirection_ThrowsTheTypedException()
        => Assert.Throws<OrderTypeValueObjectValidationException>(
            () => _validator.ValidateAndThrow(OrderTypeValueObject.From("UP")));
}
