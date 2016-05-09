using System;
using System.Collections.Generic;
using System.Text;
using System.Management.Automation;
using System.Collections;
using iControlSnapIn.Provider;

namespace iControlSnapIn.CmdLet.System.SystemInfo
{
    [Cmdlet(VerbsCommon.Get, iControlNouns.SystemTime)]
    public class GetSystemTime : iControlPSCmdlet
    {

        #region Parameters

        #endregion

        protected override void ProcessRecord()
        {
            if (isInitialized())
            {
                try
                {
                    iControl.CommonTimeStamp ts = GetiControl().SystemSystemInfo.get_time();
                    DateTime dt = new DateTime(
                        (int)ts.year, (int)ts.month, (int)ts.day, (int)ts.hour, (int)ts.minute, (int)ts.second, DateTimeKind.Utc);
                    WriteObject(dt);
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
