using System;
using System.Collections.Generic;
using System.Text;
using System.Management.Automation;
using System.Collections;
using iControlSnapIn.TypeData;

namespace iControlSnapIn.CmdLet.System.SystemInfo
{
    [Cmdlet(VerbsCommon.Get, iControlNouns.SystemTimeZone)]
    public class GetSystemTimeZone : iControlPSCmdlet
    {

        #region Parameters

        #endregion

        protected override void ProcessRecord()
        {
            if (isInitialized())
            {
                try
                {
                    iControl.CommonTimeZone tz = GetiControl().SystemSystemInfo.get_time_zone();
                    SystemTimeZone stz = new SystemTimeZone();
                    stz.GmtOffset = tz.gmt_offset;
                    stz.TimeZone = tz.time_zone;
                    stz.DaylightSavings = tz.is_daylight_saving_time;
                    WriteObject(stz);
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
