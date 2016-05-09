param([string]$DestinationDir = "..\dist\PowerShell\12.1.0\bin");

Remove-Item -Recurse -Force $DestinationDir;
mkdir $DestinationDir;
Copy-Item -Recurse -Force FormatData $DestinationDir\FormatData;
Copy-Item -Recurse -Force Help $DestinationDir\Help;
Copy-Item -Recurse -Force TypeData $DestinationDir\TypeData;

Copy-Item -Force f5.ico $DestinationDir;
Copy-Item -Force Reference\iControl.dll $DestinationDir;
Copy-Item -Force obj\Release\iControlSnapIn.dll $DestinationDir;
Copy-Item -Force iControlSnapin.psc1 $DestinationDir
Copy-Item -Force Readme.txt $DestinationDir;
Copy-Item -Force setupSnapin.ps1 $DestinationDir;
