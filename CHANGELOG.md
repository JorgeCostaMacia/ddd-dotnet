# Changelog

What a consumer of these packages has to act on, newest first. Versions that only touched the docs, the build or the internals are not listed.

## 11.0.0

The five packages continue those of shared-net under new ids and namespaces: `JorgeCostaMacia.Aggregate`, `.DomainEvent`, `.Entity`, `.Exception` and `.ValueObject` are `JorgeCostaMacia.DDD.Aggregate`, `.DDD.DomainEvent` and so on. Swap each `PackageReference` for its `JorgeCostaMacia.DDD.<X>` counterpart and rename the namespaces in your `using`s (`JorgeCostaMacia.ValueObject.Domain` becomes `JorgeCostaMacia.DDD.ValueObject.Domain`); the types, their members and their behaviour are those of 10.1.5. `JorgeCostaMacia.GuidMySqlConverter` and `JorgeCostaMacia.ValueObject.EfConverter` do not continue: MySqlConnector's `GuidFormat=Binary16` stores the GUIDs, and EF Core's `ComplexProperty` over `.Value` maps the value objects.
