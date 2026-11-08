using FluentValidation;
using JorgeCostaMacia.DDD.ValueObject.Domain;

namespace JorgeCostaMacia.DDD.ValueObject.Tests.Domain;

public sealed class GroupByValueObjectValidatorTests
{
    private static readonly GroupByValueObjectValidator _validator = GroupByValueObjectValidator.Create();

    [Theory]
    [InlineData("category")]
    [InlineData("country_code")]
    public void Validate_WithANonEmptyName_Passes(string value)
        => Assert.True(_validator.Validate(GroupByValueObject.From(value)).IsValid);

    [Fact]
    public void Validate_WithAnEmptyName_Fails()
        => Assert.False(_validator.Validate(GroupByValueObject.From("")).IsValid);

    [Fact]
    public void ValidateAndThrow_WithAnEmptyName_ThrowsTheTypedException()
        => Assert.Throws<GroupByValueObjectValidationException>(
            () => _validator.ValidateAndThrow(GroupByValueObject.From("")));
}
