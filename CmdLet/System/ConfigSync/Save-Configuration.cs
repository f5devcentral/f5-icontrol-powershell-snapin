using System;
using System.Collections.Generic;
using System.Text;
using System.Management.Automation;
using System.Collections;
using iControlSnapIn.Provider;

namespace iControlSnapIn.CmdLet.System.ConfigSync
{
    [Cmdlet(iControlVerbs.Save, iControlNouns.Configuration)]
    public class SaveConfiguration : iControlPSCmdlet
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
        [Parameter(Position = 1, Mandatory=true, HelpMessage = "The save mode <Full, High, or Base>")]
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
                    iControl.SystemConfigSyncSaveMode save_mode = iControl.SystemConfigSyncSaveMode.SAVE_FULL;
                    switch (_mode.ToUpper())
                    {
                        case "FULL":
                            save_mode = iControl.SystemConfigSyncSaveMode.SAVE_FULL;
                            break;
                        case "HIGH":
                            save_mode = iControl.SystemConfigSyncSaveMode.SAVE_HIGH_LEVEL_CONFIG;
                            break;
                        case "BASE":
                            save_mode = iControl.SystemConfigSyncSaveMode.SAVE_BASE_LEVEL_CONFIG;
                            break;
                        default:
                            bValid = false;
                            break;
                    }

                    if (!bValid)
                    {
                        handleError("Invalid Mode '" + _mode + "' specified.  Use Full, High, or Base", _mode);
                    }
                    else
                    {
                        GetiControl().SystemConfigSync.save_configuration(_name, save_mode);
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
