using System;
using System.Collections.Generic;
using System.Text;
using System.Management.Automation;
using System.Collections;
using iControlSnapIn.Provider;

namespace iControlSnapIn.CmdLet.System.ConfigSync
{
    [Cmdlet(VerbsCommon.Remove, iControlNouns.File)]
    public class RemoveFile: iControlPSCmdlet
    {

        #region Parameters

        private string _name = null;
        [Parameter(Position = 0, Mandatory=true, HelpMessage = "The name of the remote file to remove")]
        [ValidateNotNullOrEmpty]
        public string RemoteFile
        {
            get { return _name; }
            set { _name = value; }
        }

        #endregion

        protected override void ProcessRecord()
        {
            if (isInitialized())
            {
                try
                {
                    GetiControl().SystemConfigSync.delete_file(_name);
                    WriteObject(true);
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
