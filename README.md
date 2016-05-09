# f5-icontrol-powershell-snapin
PowerShell Snapin for F5's iControl SOAP Library

Thanks for taking a look at the iControl PowerShell SnapIn.

## Installation
For the first time use, you will have to run the included setupSnapIn.ps1 in the c:\program files\F5 Networks directory from within PowerShell.

```powershell
PS: c:\program files\f5 networks> .\setupSnapIn.ps1
```

## Running
Once you've installed the SnapIn into PowerShells SnapIn directory, you can instantiate the SnapIn as follows

```powershell
PS: > Add-PSSnapIn -iControlSnapIn
```

Now, the SnapIn is loaded into the runtime.  You can now make iControl calls.  To get a list of all the iControl CmdLets, you can use the following Cmdlet.

```powershell
PS: > Get-F5.iControlCommands
```

This will list all available iControl Cmdlets included in the iControlSnapIn.  Before making any of the other calls, you will have to initialize the iControl connection to the BIG-IP with the Initialize-iControl Cmdlet.

```powershell
PS: > Initialize-F5.iControl -Hostname <bigip_address> -Credentials (Get-Credential)
```

This will prompt with a dialog box for the connection credentials.  If you prefer to pass them in in clear text, you can use the following optional parameters

```powershell
PS: > Initialize-F5.iControl -Hostname <bigip_address> -Username <username> -Password <password>
```

Now, just give them all a shot.  Remember you can always use autocomplete to find out the parameters for each cmdlet.
