# Invoking pabcnetcclear.exe (net10 build)

## Binary location

```
c:\develop-abc\pascalabcnet\bin\pabcnetcclear.exe
```

This is the net10.0 command-line Pascal compiler. `pabcnetc.exe` in the same folder is
the same binary (different name, same build).

## Basic invocation (from a real terminal)

```
pabcnetcclear.exe yourfile.pas
```

Output `.exe` is placed next to the source `.pas` file by default.

## The conhost.exe requirement

The compiler calls `Console.BufferWidth` at startup. Without a real Windows console
handle — which is the case when launched from Claude Code tools, scripts, or any
non-interactive host — it throws:

```
System.IO.IOException: The handle is invalid
```

and exits before compiling anything. There is no flag to suppress this.

**Always wrap the call in `conhost.exe`** when running from Claude Code or any
non-terminal environment:

```powershell
# Simple (output goes to terminal)
conhost.exe -- c:\develop-abc\pascalabcnet\bin\pabcnetcclear.exe yourfile.pas

# Capture output to files (needed inside Claude Code Bash/PowerShell tools)
$p = Start-Process -FilePath "conhost.exe" `
  -ArgumentList "--", "c:\develop-abc\pascalabcnet\bin\pabcnetcclear.exe", "yourfile.pas" `
  -Wait -PassThru -NoNewWindow `
  -RedirectStandardOutput "out.txt" `
  -RedirectStandardError "err.txt"
$p.ExitCode   # 0 = success
```

Read `out.txt` / `err.txt` after the process exits for compiler output and errors.

## Common flags

| Flag | Effect |
|------|--------|
| `/Define:NAME` | Define a conditional compilation symbol (e.g. `{$IFDEF NAME}`) |
| `/rebuild` | Force recompilation of `.pcu` pre-compiled unit cache |
| `-o <path>` | Set output file path |

Example with a define:

```powershell
$p = Start-Process -FilePath "conhost.exe" `
  -ArgumentList "--", "c:\develop-abc\pascalabcnet\bin\pabcnetcclear.exe", "/Define:PABC", "Tests.pas" `
  -Wait -PassThru -NoNewWindow `
  -RedirectStandardOutput "out.txt" -RedirectStandardError "err.txt"
```

## Working directory

Run from the directory that contains your `.pas` file, or use absolute paths. The
compiler resolves `uses ... in 'relative\path.pas'` relative to the source file's
directory, not the current working directory.

## Referencing external DLLs

Use `{$reference 'absolute\path\to\library.dll'}` at the top of the main entry file
(not inside a unit). Relative paths in `{$reference}` are resolved from the compiler
binary directory, not the source file — use absolute paths to be safe.

## Standard library search path

Pre-compiled units (`.pcu`) are in `c:\develop-abc\pascalabcnet\bin\Lib\`. The compiler
finds them automatically. If a `.pcu` is stale, delete it or use `/rebuild`.
