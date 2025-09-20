name: Build PDFCompressor

on:
  workflow_dispatch:   # lets you trigger it manually

jobs:
  build:
    runs-on: windows-2022   # stay on a stable Windows runner

    steps:
      # 1. Checkout your repository code
      - uses: actions/checkout@v3

      # 2. Install .NET Framework 4.8.1 developer pack (to get csc.exe)
      - name: Install .NET Framework targeting pack
        run: choco install netfx-4.8.1-devpack -y

      # 3. Compile with the C# compiler
      - name: Compile with csc
        shell: pwsh
        run: |
          mkdir out
          & "C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe" `
            /t:winexe `
            /out:out\PDFCompressor.exe `
            PDFCompressor.cs

      # 4. Upload the generated EXE as an artifact
      - name: Upload artifact
        uses: actions/upload-artifact@v4
        with:
          name: PDFCompressor
          path: out/PDFCompressor.exe
