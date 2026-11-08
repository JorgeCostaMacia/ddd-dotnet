using FluentValidation;
using FluentValidation.Results;
using JorgeCostaMacia.DDD.ValueObject.Domain;

namespace JorgeCostaMacia.DDD.ValueObject.Tests.Domain;

public sealed class UriValueObjectValidatorTests
{
    private static readonly UriValueObjectValidator _validator = UriValueObjectValidator.Create();

    // The base takes any URI: absolute of any scheme, or relative. Narrowing it is a derived Value Object's job.
    [Theory]
    [InlineData("https://example.com")]
    [InlineData("http://localhost:8080/path")]
    [InlineData("ftp://files.example.com")]
    [InlineData("urn:isbn:0451450523")]
    [InlineData("/rooted/path")]
    [InlineData("relative/path")]
    public void Validate_WithAnyAbsoluteOrRelativeUri_Passes(string value)
        => Assert.True(_validator.Validate(UriValueObject.From(new Uri(value, UriKind.RelativeOrAbsolute))).IsValid);

    [Fact]
    public void Validate_WithANullUri_FailsTheNotNullRuleOnly()
    {
        ValidationResult result = _validator.Validate(new UriValueObject(null!));

        Assert.Equal("NotNullValidator", Assert.Single(result.Errors).ErrorCode);
    }

    [Fact]
    public void ValidateAndThrow_WithANullUri_ThrowsTheTypedException()
        => Assert.Throws<UriValueObjectValidationException>(
            () => _validator.ValidateAndThrow(new UriValueObject(null!)));

    // The derived Value Object narrows the rule: this is how an http(s)-only URL is built on the base.
    [Theory]
    [InlineData("https://example.com", true)]
    [InlineData("http://example.com/a", true)]
    [InlineData("ftp://example.com", false)]
    [InlineData("/rooted/path", false)]
    public void Validate_ThroughADerivedWebUrlValidator_NarrowsToAWebUrl(
        string value,
        bool valid
    )
        => Assert.Equal(
            valid,
            new WebUrlValidator().Validate(new WebUrl(new Uri(value, UriKind.RelativeOrAbsolute))).IsValid);

    public sealed record WebUrl : UriValueObject
    {
        public WebUrl(Uri value) : base(value) { }
    }

    public sealed class WebUrlValidator : AbstractValidator<WebUrl>
    {
        public WebUrlValidator()
        {
            Include(UriValueObjectValidator.Create());

            // IsAbsoluteUri first: Uri.Scheme throws on a relative URI.
            RuleFor(v => v.Value)
                .Must(v => v is not null
                    && v.IsAbsoluteUri
                    && (v.Scheme == Uri.UriSchemeHttp || v.Scheme == Uri.UriSchemeHttps))
                .WithErrorCode("WebUrlValidator")
                .WithMessage("{PropertyName} must be an http(s) Url");
        }
    }
}
