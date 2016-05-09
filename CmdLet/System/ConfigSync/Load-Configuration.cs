using System;
using System.Collections.Generic;
using System.Text;
using System.Management.Automation;
using System.Collections;
using iControlSnapIn.Provider;

namespace iControlSnapIn.CmdLet.System.ConfigSync
{
    [Cmdlet(iControlVerbs.Load, iControlNouns.Configuration)]
    public class LoadConfiguration : iControlPSCmdlet
    {

        #region Parameters

        private string _name = null;
        [Parameter(Position = 0, Mandatory=true, HelpMessage = "The name of the saved configuration")]
        [ValidateNotNullOrEmpty]
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        private string _mode = null;
        [Parameter(Position = 1, Mandatory = true, HelpMessage = "The load mode <High, or Base>")]
        [ValidateNotNullOrEmpty]
        public string Mode
        {
            get { return _mode; }
            set { _mode = value; }
        }

        #endregion

        protected override void ProcessRecord()
        {
            if (isInitialized())
            {
                try
                {
                    bool bValid = true;
                    iControl.SystemConfigSyncLoadMode load_mode = iControl.SystemConfigSyncLoadMode.LOAD_BASE_LEVEL_CONFIG;
                    switch (_mode.ToUpper())
                    {
                        case "HIGH":
                            load_mode = iControl.SystemConfigSyncLoadMode.LOAD_HIGH_LEVEL_CONFIG;
                            break;
                        case "BASE":
                            load_mode = iControl.SystemConfigSyncLoadMode.LOAD_BASE_LEVEL_CONFIG;
                            break;
                        default:
                            bValid = false;
                            break;
                    }

                    if (!bValid)
                    {
                        handleError("Invalid Mode '" + _mode + "' specified.  Use High, or Base", _mode);
                    }
                    else
                    {
                        GetiControl().SystemConfigSync.load_configuration(_name, load_mode);
                        WriteObject(true);
                    }
                }
                catch (Exception ex)
                {
                    handleException(ex);
                }
            }
            else
            {
                handleNotInitialized();
            }
        }
    }
}
