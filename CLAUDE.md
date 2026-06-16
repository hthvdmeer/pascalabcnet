# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

PascalABCNET is a compiler and IDE for Pascal-like languages targeting the .NET Framework. The project consists of a multi-language compiler infrastructure, a Windows-based IDE (VisualPascalABCNET), and support for additional languages like Python and Kotlin via a pluggable language integration system.

**Key Technologies:**
- .NET Framework 4.0+ (C#)
- MSBuild / dotnet CLI
- GPPG (parser generator) for language parsing
- Custom syntax tree and semantic tree representations
- Code generation to IL/.NET assemblies

## Build System

### Main Solution Files

- **PascalABCNET.sln** - Primary solution for Windows development (Visual Studio 2019+)
- **PascalABCNETLinux.sln** - Linux/Mono build (targets .NET 4.0 via Mono)
- **pabcnetc.sln** - Console compiler standalone solution

### Build Commands

```bash
# Debug build (fastest for development)
dotnet build --no-incremental PascalABCNET.sln

# Release build (full optimization)
dotnet build -c Release --no-incremental -v d PascalABCNET.sln

# Complete release build with unit compilation and tests
_RebuildReleaseAndRunTests.bat (Windows, requires admin)
_RebuildReleaseAndRunTests.sh (Linux/macOS, requires mono-complete)
```

### One-Time Setup

On Windows, register required assemblies before first build:
```bash
_RegisterHelix.bat  # Requires admin privileges
```

This installs HelixToolkit.dll and HelixToolkit.Wpf.dll into the GAC.

### Output

Compiled binaries are placed in `bin/` directory:
- `pabcnetc.exe` - Command-line compiler
- `VisualPascalABCNET.exe` - Windows IDE
- `VisualPascalABCNETLinux.exe` - Linux IDE (via Mono)
- `Compiler.dll` - Core compiler library
- `*.pcu` files - Pre-compiled unit cache in `bin/Lib/`

## Testing

### Running Tests

```bash
# Run all test suites (6 test groups)
cd bin
TestRunner.exe 1  # Test group 1
TestRunner.exe 2  # Test group 2
# ... continue through 6

# For SPython tests
cd TestSuiteAdditionalLanguages/SPythonTests
../../bin/TestRunner.exe 1  # Run SPython test group 1
```

### Test Organization

- **TestSuite/** - Pascal language test files (`.pas` files with expected outputs)
- **TestSuiteAdditionalLanguages/SPythonTests/** - SPython language tests
- **bin/TestRunner.exe** - Compiled test runner (built from TestSuite/TestRunner.pas)

Each test group processes a batch of `.pas` test files, compiles them, and verifies output against expected results.

### Compilation Workflow

The test runner compiles test files using the newly built compiler, so testing validates both the compiler itself and the standard library (PABCRtl).

## Architecture

### Compilation Pipeline

The compiler follows a multi-stage pipeline:

1. **Parsing** → Syntax Tree
   - Language-specific parsers in `Parsers/` (using GPPG-generated parsers)
   - Syntax tree representation in `SyntaxTree/`

2. **Semantic Analysis** → Semantic Tree
   - Syntax-to-semantic tree conversion in `SyntaxTreeConverters/`
   - Symbol resolution and type checking in `SemanticTree/`
   - Visitor pattern implementation in `SyntaxVisitors/`

3. **Code Generation** → .NET IL
   - `NETGenerator/` - Generates IL from semantic tree
   - `TreeConverter/` - Intermediate tree transformations
   - Optimization passes in `Optimizer/`

4. **Assembly Creation**
   - Roslyn integration via `CompilerTools/`
   - Output types: console app, DLL, or Windows executable

### Core Projects & Dependencies

**Compiler Core:**
- `Compiler/` - Main compiler orchestration (ICompiler interface)
- `Errors/` - Unified error handling and reporting
- `CompilerTools/` - Compiler utilities and Roslyn integration

**Language Abstraction:**
- `ParserTools/` - Base classes for parser implementation
- `LanguageIntegrator/` - BaseLanguage/BaseParser abstractions for multi-language support
- `Localization/` - String resource management

**Tree Representations:**
- `SyntaxTree/` - Immutable syntax tree node definitions
- `SemanticTree/` - Semantic tree after analysis
- `SyntaxTreeConverters/` - AST → Semantic tree conversion
- `SyntaxVisitors/` - Visitor pattern traversal helpers
- `TreeConverter/` - Additional tree transformations

**Utilities:**
- `CoreUtils/` - Core utility functions
- `StringConstants/` - Compiler constant definitions
- `Configuration/` - Global assembly versioning

### Multi-Language Support

The system supports pluggable languages:
- **PascalABCLanguageInfo/** - Main Pascal language metadata
- **AdditionalLanguages/** - Python (SPython), Kotlin, and others
  - Each language implements: Parser, LanguageInfo (metadata), Language (integration class)
  - Naming convention: `*LanguageInfo.dll` with `*Language` class

Additional languages follow the integration guide in `documentation/DeveloperGuides/Integrating new language.md`.

### IDE Architecture

**VisualPascalABCNET** (Windows):
- Main form: `VisualPascalABCNET/Form1.cs`
- Text editor: `ICSharpCode.TextEditor/` (rich text editor)
- Forms designer support via `FormsDesignerBinding/`
- Plugin system in `VisualPlugins/`

**VisualPascalABCNETLinux** (Mono-based):
- Parallel architecture to Windows version
- Text editor: `ICSharpCode.TextEditorLinux/`

**Plugin System:**
- Base: `PluginsSupport/`, `PluginsSupportLinux/`
- Built-in plugins: CompilerController, InternalErrorReport, CodeCompletion, SyntaxTreeVisualisator, etc.

### Standard Library (PABCRtl)

Located in `ReleaseGenerators/PABCRtl/`:
- Runtime support library for compiled programs
- Assembly signing via `KeyPair.snk`
- Installed to GAC and `bin/Lib/PABCRtl.dll` during build

## CI/CD

GitHub Actions workflows in `.github/workflows/`:
- `buildandruntests.yml` - Windows CI (runs on windows-latest)
- `buildandruntestslinux.yml` - Linux CI (runs on ubuntu-latest with Mono)

Both workflows:
1. Install Helix/NUnit dependencies
2. Build in Release mode
3. Compile Pascal standard modules
4. Run full test suite (6 test groups)

Triggered on: push, pull_request, workflow_dispatch, and release creation

## Key Files & Directories

- **Documentation** in `documentation/` folder:
  - `Compiler/Compiler_api.md` - Public compiler API
  - `Compiler/Compilation algorithm/` - Compilation algorithm in pseudocode
  - `DeveloperGuides/Integrating new language.md` - Guide for adding languages
- **Release tooling** in `ReleaseGenerators/` - Build scripts, installer generation, module compilation
- **CodeExamples/** - Sample programs in various supported languages
- **Libraries/** - Third-party dependencies (WinForms docking, text editing controls)

## Common Development Tasks

### Adding a Language

1. Create folder in `AdditionalLanguages/YourLanguage/`
2. Implement parser (GPPG-based), LanguageInfo, and Language classes
3. Follow naming: `*LanguageInfo.dll` assembly with `*Language` class
4. Target .NET Framework 4.0, output to `bin/`
5. Reference: `documentation/DeveloperGuides/Integrating new language.md`

### Working with Compiler Code

The Compiler.csproj has no Program.cs entry point—it's a library. Entry points:
- Console compiler: `pabcnetc/ConsoleCompiler.cs` (main method in PABCNETC.csproj)
- IDE: `VisualPascalABCNET/Program.cs` (WinForms startup)

### Debugging Compilation

During compilation, if in Debug build:
- Log output written to `bin/log.txt` (enables detailed tracing)
- Syntax tree nodes printed with full type info for inspection
- Use `SyntaxTreeVisualisator` plugin in IDE for tree visualization

### Module Compilation

Standard modules are pre-compiled to `.pcu` (Pascal Compiled Unit) files:
```bash
cd ReleaseGenerators
../bin/pabcnetc RebuildStandartModules.pas /rebuild
```

The `/rebuild` flag forces recompilation. `.pcu` files are cached in `bin/Lib/` to speed up compilation.

## Notes for Claude Code

- **Solution complexity**: PascalABCNET.sln contains 50+ projects (core compiler, IDE, plugins, utilities, tests). Changes often cascade across tree representation layers.
- **Build time**: Full Release build with module recompilation takes ~30 seconds. Use Debug builds for rapid iteration.
- **Testing workflow**: Always run the test suite after compiler changes since the standard library depends on compiler features.
- **Language support**: The system is designed for multi-language compilation. Changes to core compiler infrastructure may affect all integrated languages (Pascal, SPython, Kotlin, etc.).
