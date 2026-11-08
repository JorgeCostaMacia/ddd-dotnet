using JorgeCostaMacia.DDD.ValueObject.Domain;

namespace JorgeCostaMacia.DDD.ValueObject.Tests.Domain;

public sealed class ByteValueObjectTests
{
    [Fact]
    public void Ctor_WithAByteArray_HydratesTheSameInstance()
    {
        byte[] data = { 1, 2, 3 };
        Assert.Same(data, new ByteValueObject(data).Value);
    }

    [Fact]
    public void From_WithAByteArray_KeepsTheValue()
    {
        byte[] data = { 1, 2, 3 };
        Assert.Equal(data, ByteValueObject.From(data).Value);
    }

    // The validator has no rules, so Create never throws.
    [Fact]
    public void Create_WithAByteArray_KeepsTheValue()
    {
        byte[] data = { 1, 2, 3 };
        Assert.Equal(data, ByteValueObject.Create(data).Value);
    }

    [Fact]
    public void ImplicitOperator_WithAByteArray_ReturnsTheUnderlyingValue()
    {
        byte[] data = { 1, 2, 3 };
        byte[] value = ByteValueObject.Create(data);
        Assert.Equal(data, value);
    }

    [Fact]
    public void ToString_WithUtf8EncodedBytes_DecodesThemAsUtf8()
        => Assert.Equal("hi", ByteValueObject.Create(new byte[] { 104, 105 }).ToString());

    // The protected Convert family is the toolbox for derived VOs in consuming contexts —
    // exercised through a derived test type, like a real derived VO would.
    [Fact]
    public void Convert_FromBase64String_Decodes()
        // "hi" => bytes { 104, 105 } => base64 "aGk="
        => Assert.Equal(new byte[] { 104, 105 }, TestByte.Convert("aGk="));

    [Fact]
    public void Convert_FromInvalidBase64_Throws()
        => Assert.Throws<FormatException>(() => TestByte.Convert("not-base64!"));

    public sealed record TestByte : ByteValueObject
    {
        public TestByte(byte[] value) : base(value) { }

        public static new byte[] Convert(string value) => ByteValueObject.Convert(value);
    }
    // The OrNull pair carries the field's optionality: absence in, absence out — never a second
    // policy for invalid input.
    [Fact]
    public void FromOrNull_WithNoValue_ShortCircuitsToNull()
        => Assert.Null(ByteValueObject.FromOrNull(null));

    [Fact]
    public void FromOrNull_WithAValue_MaterializesIt()
        => Assert.NotNull(ByteValueObject.FromOrNull(new byte[] { 1, 2 }));

    [Fact]
    public void CreateOrNull_WithNoValue_ShortCircuitsWithoutValidating()
        => Assert.Null(ByteValueObject.CreateOrNull(null));

    [Fact]
    public void CreateOrNull_WithAValue_ReturnsTheValueObject()
        => Assert.NotNull(ByteValueObject.CreateOrNull(new byte[] { 1, 2 }));
}
