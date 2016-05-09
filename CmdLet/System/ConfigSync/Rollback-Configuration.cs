using System;
using System.Collections.Generic;
using System.Text;
using System.Management.Automation;
using System.Collections;
using iControlSnapIn.Provider;

namespace iControlSnapIn.CmdLet.System.ConfigSync
{
    [Cmdlet(iControlVerbs.Rollback, iControlNouns.Configuration)]
    public class RollbackConfiguration: iControlPSCmdlet
    {

        #region Parameters

        #endregion

        protected override void ProcessRecord()
        {
            if (isInitialized())
            {
                try
                {
                    GetiControl().SystemConfigSync.rollback_configuration();
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
