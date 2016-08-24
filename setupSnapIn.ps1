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
    if ( Test-Path "$env:windir\Microsoft.Net\Framework${platform}\v4.0.30319" )
    {
      Write-Host '.NET v4 present'
      $installUtil = "$env:windir\Microsoft.Net\Framework${platform}\v4.0.30319\installUtil.exe";
    }
    ElseIf ( Test-Path "$env:windir\Microsoft.Net\Framework${platform}\v3.5" )
    {
      Write-Host '.NET v3.5 present'
      $installUtil = "$env:windir\Microsoft.Net\Framework${platform}\v3.5\installUtil.exe";
    }
    ElseIf ( Test-Path "$env:windir\Microsoft.Net\Framework${platform}\v3.0" )
    {
      Write-Host '.NET v3.0 present'
      $installUtil = "$env:windir\Microsoft.Net\Framework${platform}\v3.0\installUtil.exe";
    }
    ElseIf ( Test-Path "$env:windir\Microsoft.Net\Framework${platform}\v2.0.507272" )
    {
      Write-Host '.NET v2 present'
      $installUtil = "$env:windir\Microsoft.Net\Framework${platform}\v2.0.50727\installUtil.exe";
    }
    Else
    {
      Write-Host 'Could not find any version of .NET >=2 and <=4'
      Exit 1
    }
    
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
