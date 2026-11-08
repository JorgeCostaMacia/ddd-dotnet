# ddd-dotnet — working in this repo

DDD building blocks for .NET — value objects with their FluentValidation validators, aggregates, entities, domain events and the domain exception hierarchy — each scoped to a single concern and shipped independently on NuGet under `JorgeCostaMacia.DDD.*`. Moved out of **shared-net** in 11.0.0, which renamed the packages and their namespaces from `JorgeCostaMacia.<X>` to `JorgeCostaMacia.DDD.<X>`: the ids now match the namespaces, and the prefix names the repo, as `JorgeCostaMacia.Quartz.*` and `JorgeCostaMacia.Serilog` do. The old ids stay on nuget.org, deprecated in favour of these.

## Layout

- `src/JorgeCostaMacia.DDD.<Package>/` — one package per folder. `test/JorgeCostaMacia.DDD.<Package>.Tests/` — its tests, mirroring the package's folders (`Domain/`); the convention tests over a whole assembly (the factory surface, `ToString`, the validation exception shapes) sit at the suite's root. `DomainEvent` and `Entity` are marker interfaces with nothing to test. `assets/` — icons + social preview.
- **3-tier `Directory.Build.props`**: **root** (repo identity — Authors / Company / Copyright / Repository — + the single lockstep `VersionPrefix`; TFM `net10.0`; ImplicitUsings, Nullable, AnalysisLevel, EnforceCodeStyleInBuild, TreatWarningsAsErrors, the NuGet audit; `IsPackable` false by default) → **`src/`** (`IsPackable` true, package-output: icon / readme / license, SourceLink, symbols, `GenerateDocumentationFile`, pack of LICENSE/COPYRIGHT/icon/README) → **`test/`** (test settings). Each `src` csproj declares **only** `Description` / `PackageTags` and its references; everything else is inherited from the props (don't restate it).
- **Namespaces** are `JorgeCostaMacia.DDD.<Package>.Domain`: the assembly name gives the root, the folder the rest, and the analyzers keep them matched. Inside them, `Exception` is also the namespace `JorgeCostaMacia.DDD.Exception`, so the BCL type is written `System.Exception`.

## Targets & stack

- TFM: **`net10.0`**, single-target. net6/7/8/9 were dropped as they reached EOL — net9 in May 2026, net8 in November 2026 — and the last multi-targeting release is **6.0.6**, under the old `JorgeCostaMacia.<X>` ids, which stays on nuget.org for anyone pinned to a down-level runtime. Being single-target is what removes the per-TFM `#if NET9_0_OR_GREATER` branches: there is now **one** version of every package. **Do not reintroduce a per-TFM conditional** — if a dependency needs one, the answer is to move the whole repo, not to split it.
- FluentValidation: the validators of `ValueObject` and the failure list `Exception`'s `ValidationException` carries. The only third-party dependency of the packages.
- Tests: **xUnit v4 (the `xunit.v3` package, 4.x) on Microsoft.Testing.Platform v2 (MTP)** — test projects are `OutputType=Exe`, `dotnet test` runs MTP because the root **`global.json`** opts in, and `xunit.runner.visualstudio` is referenced for IDE discovery, as `testing-dotnet` sets up. Running them needs the **.NET 10 SDK**.
- Source is **UTF-8 without BOM** (`.editorconfig` `charset = utf-8`). camelCase locals, PascalCase types, I-prefixed interfaces. Copyright year stays **2023** (deliberate — don't bump).
- **Explicit types everywhere — spell the type out.** Never `var`, never target-typed `new()`, never collection expressions `[]`: write `new Foo(...)`, `new byte[] { ... }`, `new List<T> { ... }`, `Array.Empty<T>()`. The `.editorconfig` sets all three to explicit, but only `var` is analyzer-enforced — the analyzer never reports the implicit forms. Develop (`dotnet-build-nuget` from github-workflows) guards target-typed `new(` and implicit `new[]` with a **grep step** that fails the build, and collection expressions `[…]` with a second one (`code-style-dotnet`'s pattern: attributes, indexers, array types and list patterns never match it). Do not introduce any of them.

## The value-object factory surface

Every value object carries **four** public static factories, and a contract test
(`ValueObjectCreationSurfaceTests`) scans the assembly by reflection and fails if one is missing:

| verb | input | returns | semantics |
|---|---|---|---|
| `From(T)` | a value | **non-null** | converts through `Convert`, **unvalidated** |
| `Create(T)` | a value | **non-null** | `From` + `Validate()` — nothing invalid escapes |
| `FromOrNull(T?)` | a value or nothing | nullable | absence propagates, still unvalidated |
| `CreateOrNull(T?)` | a value or nothing | nullable | absence short-circuits; a supplied value is validated |

Two rules keep the pair honest:

- **The `OrNull` pair is about absence, not failure.** It answers "the field did not come, what do I
  build?" — never "this might be invalid, don't throw at me". That is why there is no `TryFrom`:
  `From` only converts and cannot fail, so there is nothing to try. Nor a `FromOrDefault`: an
  out-of-range value is a caller mistake, and coercing it would hide the mistake *and* give two
  callers different behaviour for the same rule.
- **`CreateOrNull` branches on what `FromOrNull` returned**, not on whether the input was null, and
  validates with `vo?.Validate()`. Branching on the input would hard-code "non-null in, non-null
  out" and would break the day a `From` maps a sentinel to absence.

The nine bases that derive from another base (`EmailValueObject : StringValueObject`,
`PageSizeValueObject : IntValueObject`, `DateTimeUtcValueObject : DateTimeValueObject`, …) declare all
four with **`static new`**, because a static factory returning the derived type cannot be inherited.
The compiler proves each one is needed: `CS0109` ("`new` not required") would appear otherwise, and
the build carries zero warnings.

## Inter-package dependencies

Packages reference each other via **`ProjectReference`** (ValueObject → Exception, Aggregate → DomainEvent). `dotnet pack` turns each `ProjectReference` into a NuGet `<dependency>` at the sibling's version, so the dependency graph still ships in the nuspec — but you build against local source and **release everything together** (no phased, tier-by-tier publishing). Don't reintroduce `PackageReference` between these packages.

Nothing here depends on another `JorgeCostaMacia.*` repo. Other repos depend on this one, through the published packages: bus-dotnet's transports on `DDD.DomainEvent` (their `Event` implements `IDomainEvent`), http-dotnet on `DDD.Exception` (it maps the hierarchy to ProblemDetails). A breaking change here is a major for them too.

## Dependencies — Central Package Management

Third-party package versions are centralized in **`Directory.Packages.props`** (repo root, `ManagePackageVersionsCentrally=true`): add or bump them **there** as `<PackageVersion>`, and reference packages in csproj **without** a `Version`. (Inter-package deps are `ProjectReference`, not packages — see above — so CPM doesn't manage them.)

**The strict gates are on**: `TreatWarningsAsErrors` and the NuGet vulnerability audit on every restore (direct and transitive, every severity), so any warning — an advisory included — fails the build. The fix is a version bump or a transitive override in CPM, never `NoWarn`. The root `nuget.config` clears every inherited source and keeps nuget.org alone: this library consumes nothing else, and an inherited second source would raise `NU1507`. It is **deny-read** for agents — hand changes to it to the user.

## CI / publishing

The three workflows call the reusable ones of [github-workflows](https://github.com/JorgeCostaMacia/github-workflows) (`@v1`): `develop.yml` runs `dotnet-build-nuget` on develop and PRs (the explicit-types and collection-expression greps, build, the `dotnet format` check, test, pack — no publish); `main.yml` runs the same build on a push to `main` and then pushes the `nuget` artifact to nuget.org with Trusted Publishing (`NuGet/login`, OIDC) — that job stays here because the nuget.org policy names this repo's `main.yml`; `release.yml` creates the GitHub Release of each `v*` tag. **The (central) `VersionPrefix` is the publish gate**: `--skip-duplicate` leaves an existing version alone, so only a release that moved it publishes.

## Branching & releases — GitFlow

Use the **`gitflow` skill** for any branch/release work — never improvise.

- Feature/bugfix → `feature/`|`bugfix/<name>-<ts>` from develop → finish `--no-ff` into develop.
- Release → `release/<version>` from develop → bump the **single** `VersionPrefix` in the **root** `Directory.Build.props` (the one holding the solution identity — all packages version in lockstep) → Release Finish (merge develop+main, annotated tag `v<version>`, atomic push). One bump versions everything; the `ProjectReference` cross-deps follow automatically.
- Use git's **default merge message** (`--no-ff --no-edit`, never `-m`).
- Branch prefixes only: `feature` / `bugfix` / `release` / `hotfix`.

## Git etiquette

- Commit under **your own identity** — don't hardcode anyone's name/email.
- Keep history clean — **no** `Co-Authored-By` / AI-assistant trailers in commits or messages.
- Merges use git's **default** message (see *Branching & releases* above).

## Relevant skills

Skills that apply to this repo — let them trigger, or invoke explicitly. The first group is from `jorgecostamacia-agent-skills` (each `-dotnet` skill brings the generic one it realizes); the rest from `dotnet-agent-skills` (the `dotnet/skills` marketplace).

A skill listed here but missing on this machine is one command away: `claude plugin marketplace add JorgeCostaMacia/skills` (or `dotnet/skills`) once, then `claude plugin install <skill>@<marketplace>`.

- **`ddd-dotnet`** (with **`ddd`**) — **the spec this library implements**: the base types, the four-factory surface (`From`, `FromOrNull`, `Create`, `CreateOrNull`) and why every one is declared, `Convert` as the one place a value is altered, no coercion and no defaults, the aggregate's `From`/`Create`, the exception family with its UUIDv7 codes, events on the root. A change here is a change in what that skill describes: keep the two in step.
- **`validation-dotnet`** — the validators: one per validated thing, per-call `Create()` chains with `Include`/`SetValidator`, no `Cascade`, `RaiseValidationException` raising the typed exception, and what each base validator checks — the list there must match this repo's validators.
- **`testing-dotnet`** (with **`testing`**) — xUnit v3 on Microsoft.Testing.Platform, the test project layout, convention tests over a kind of type (the factory surface, the distinct exception codes).
- **`code-style-dotnet`** — how the C# is written: explicit types, the 120-column width with declarations and constructions vertical, comments wrapped too, naming, the gates.
- **`build-dotnet`** — the three-tier `Directory.Build.props`, the single `VersionPrefix`, CPM, the three-day package age, the NuGet audit as a build gate, the library's packing metadata, and the `nuget.config` base.
- **`clean-architecture`** / **`solid`** — the inward dependency rule behind the `Domain` namespaces, and design review of the public surface.
- **`gitflow`** — all branch/release work.
- **`dotnet`** — C# language server + general .NET development.
- **`dotnet-msbuild`** — `Directory.Build.props`, project-file quality, CPM.
- **`dotnet-nuget`** — dependency management.
- **`dotnet-test`** / **`dotnet-test-migration`** — tests; the xUnit v4 / MTP v2 setup.
- **`dotnet-upgrade`** — target-framework migrations.

Not relevant to this repo (skip): `efcore-dotnet` / `efcore-postgres-dotnet` (the mapping of these types is the consuming service's — value objects through `ComplexProperty` over `.Value`), `logging-dotnet`, `messaging` / `bus-dotnet` / `bus-kafka-dotnet`, `clean-architecture-dotnet` (no modules or composition contexts here), `github-actions` / `github-actions-dotnet` (the company's CI; this repo's runs on github-workflows), `dotnet-aspnetcore` (that's http-dotnet), `dotnet-data`, `dotnet-ai`, `dotnet-maui`, `dotnet-blazor`, `dotnet-template-engine`, `dotnet11`, `dotnet-diag`, `dotnet-advanced`.

## Build & test

```
dotnet format ddd-dotnet.slnx                  # apply .editorconfig (using order, whitespace) — run before committing
dotnet build  ddd-dotnet.slnx -c Release
dotnet test   ddd-dotnet.slnx -c Release       # MTP v2 via global.json (needs the .NET 10 SDK); --logger is VSTest-only (MTP0001)
dotnet pack   ddd-dotnet.slnx -c Release        # packs all packable; tests are IsPackable=false
```

The tests need nothing external.

Run **`dotnet format` before committing** — it applies the `.editorconfig` (using ordering, whitespace, line endings), the CLI equivalent of Visual Studio's *Code Cleanup*, and it is the normalizer for a `.cs` a shell tool wrote with LF. Develop checks it after the build with `dotnet format --verify-no-changes --no-restore`: the solution is clean, so the check only ever fails on what the push itself brought. `.gitattributes` checks `*.cs` out as CRLF (`eol=crlf`), so the Linux runner sees what the `.editorconfig` requires.
