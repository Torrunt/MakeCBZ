cd "C:\Program Files (x86)\Microsoft\ILMerge"

mkdir "%~dp0MakeCBZ\bin\MakeCBZ\"

ILMerge.exe /target:winexe /targetplatform:"v4,C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.5" /out:"%~dp0MakeCBZ\bin\MakeCBZ\MakeCBZ.exe" "%~dp0MakeCBZ\bin\Release\MakeCBZ.exe" "%~dp0MakeCBZ\Ionic.Zip.dll" "%~dp0MakeCBZ\SharpCompress.dll"

del "%~dp0MakeCBZ\bin\MakeCBZ\MakeCBZ.pdb"
