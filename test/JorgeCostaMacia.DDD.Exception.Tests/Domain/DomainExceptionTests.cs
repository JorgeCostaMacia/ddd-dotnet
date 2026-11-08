using JorgeCostaMacia.DDD.Exception.Domain;

namespace JorgeCostaMacia.DDD.Exception.Tests.Domain;

file sealed class TestDomainException : DomainException
{
    public TestDomainException(
        string? message = null,
        Guid? id = null,
        string? type = null
    ) : base(
        id,
        type,
        null,
        null,
        null,
        message,
        null
    )
    { }
}

file sealed class TestExplicitDomainException : DomainException
{
    public TestExplicitDomainException(
        Guid id,
        string type,
        Guid code,
        int httpCode,
        DateTime occurredAt,
        string? message,
        System.Exception? innerException
    ) : base(
        id,
        type,
        code,
        httpCode,
        occurredAt,
        message,
        innerException
    )
    { }
}

// Test Data Builder: defaults the metadata noise so each test states only the field it varies.
file sealed class DomainExceptionBuilder
{
    private string? _message = "m";
    private System.Exception? _innerException = null;

    public DomainExceptionBuilder WithMessage(string? message)
    {
        _message = message;

        return this;
    }

    public DomainExceptionBuilder WithInnerException(System.Exception innerException)
    {
        _innerException = innerException;

        return this;
    }

    public TestExplicitDomainException Build()
        => new TestExplicitDomainException(
            Guid.NewGuid(),
            "T",
            Guid.NewGuid(),
            500,
            DateTime.UtcNow,
            _message,
            _innerException);
}

public sealed class DomainExceptionTests
{
    [Fact]
    public void Ctor_WithMetadataOmitted_AppliesTheDefaults()
    {
        TestDomainException exception = new TestDomainException();

        Assert.NotEqual(Guid.Empty, exception.AggregateId);
        Assert.Equal(DomainExceptionDefaults.AGGREGATE_TYPE, exception.AggregateType);
        Assert.Equal(DomainExceptionDefaults.AGGREGATE_CODE, exception.AggregateCode);
        Assert.Equal(500, exception.AggregateHttpCode);
        Assert.True(exception.AggregateOccurredAt <= DateTime.UtcNow);
        Assert.True(exception.AggregateOccurredAt > DateTime.UtcNow.AddMinutes(-1));
    }

    [Fact]
    public void Message_WithASuppliedMessage_IncludesTheSameIdAndType()
    {
        TestDomainException exception = new TestDomainException("boom");

        // The id embedded in the message must be the exact same value as AggregateId.
        Assert.Equal($"{exception.AggregateId}/DomainException => boom", exception.Message);
    }

    [Fact]
    public void Message_WithNoMessage_OmitsTheArrow()
    {
        TestDomainException exception = new TestDomainException();

        Assert.Equal($"{exception.AggregateId}/DomainException", exception.Message);
    }

    [Fact]
    public void Message_WithAPaddedMessage_TrimsTheSuppliedMessage()
    {
        TestDomainException exception = new TestDomainException("   boom   ");

        Assert.Equal($"{exception.AggregateId}/DomainException => boom", exception.Message);
    }

    [Fact]
    public void Ctor_WithASuppliedId_PreservesItInIdAndMessage()
    {
        Guid id = Guid.NewGuid();

        TestDomainException exception = new TestDomainException(
            "x",
            id);

        Assert.Equal(id, exception.AggregateId);
        Assert.StartsWith($"{id}/DomainException", exception.Message);
    }

    [Fact]
    public void ExplicitCtor_WithAllMetadataSupplied_StoresItVerbatim()
    {
        Guid id = Guid.NewGuid();
        Guid code = Guid.NewGuid();
        DateTime occurredAt = new DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        TestExplicitDomainException exception = new TestExplicitDomainException(
            id,
            "My.Custom.Type",
            code,
            418,
            occurredAt,
            "msg",
            null);

        Assert.Equal(id, exception.AggregateId);
        Assert.Equal("My.Custom.Type", exception.AggregateType);
        Assert.Equal(code, exception.AggregateCode);
        Assert.Equal(418, exception.AggregateHttpCode);
        Assert.Equal(occurredAt, exception.AggregateOccurredAt);
    }

    [Fact]
    public void InnerException_WithAnInnerExceptionSupplied_IsPropagated()
    {
        InvalidOperationException inner = new InvalidOperationException("cause");

        TestExplicitDomainException exception = new DomainExceptionBuilder().WithInnerException(inner).Build();

        Assert.Same(inner, exception.InnerException);
    }

    [Fact]
    public void Ctor_WithNoArguments_IsAssignableToSystemException()
        => Assert.IsAssignableFrom<System.Exception>(new TestDomainException());

    [Fact]
    public void Message_WithADottedType_UsesItsLastSegment()
    {
        TestDomainException exception = new TestDomainException(
            "boom",
            type: "A.B.C.Widget");

        Assert.Equal($"{exception.AggregateId}/Widget => boom", exception.Message);
    }

    [Fact]
    public void Message_WhitespaceOnly_KeepsArrowWithEmptyTail()
    {
        // Documents the current asymmetry: "" omits the arrow, but whitespace is not empty so a trailing "=> " remains.
        TestDomainException exception = new TestDomainException("   ");

        Assert.Equal($"{exception.AggregateId}/DomainException => ", exception.Message);
    }

    [Fact]
    public void Message_FromTheExplicitCtor_IsRawAndUnprefixed()
    {
        TestExplicitDomainException exception = new DomainExceptionBuilder().WithMessage("raw message").Build();

        Assert.Equal("raw message", exception.Message);
    }
}
