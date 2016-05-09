param([switch]$force)

function Is-SnapinRegistered()
{
  $registered = $false;
  if ( $null -ne (Get-PSSnapIn -Registered | where { $_.Name -eq "iControlSnapIn" } ) )
  {
    $registered = $true;
  }
  $registered;
}

function Install-Snapin()
{
  param(
    [string]$assembly = $null
  );
  
  foreach ($platform in ("", "64") )
  {
    Write-Host "Registering $assembly on platform '$platform'";
    $installUtil = "$env:windir\Microsoft.Net\Framework${platform}\v2.0.50727\installUtil.exe";
    if ( [System.IO.File]::Exists($installUtil) )
    {
      Set-Alias installUtil $installUtil;
      installUtil $assembly /LogToConsole=false /LogFile=;
    }
  }
}

function Remove-Snapin()
{
  param(
    [string]$assembly = $null
  );
  
  foreach ($platform in ("", "64") )
  {
    Write-Host "Unregistering $assembly on platform '$platform'";
    $installUtil = "$env:windir\Microsoft.Net\Framework${platform}\v2.0.50727\installUtil.exe";
    if ( [System.IO.File]::Exists($installUtil) )
    {
      Set-Alias installUtil $installUtil;
      installUtil /u $assembly /LogToConsole=false /LogFile=;
    }
  }
}

if ( ($force) -or !(Is-SnapinRegistered) )
{
  Install-Snapin -assembly iControlSnapin.dll;
}
else
{
  Write-Host "iControlSnapIn already registered..."
}
