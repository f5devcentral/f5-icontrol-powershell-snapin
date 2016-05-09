using System;
using System.Collections.Generic;
using System.Text;
using System.Management.Automation;
using System.Collections;
using iControlSnapIn.TypeData;

namespace iControlSnapIn.CmdLet.System.SystemInfo
{
    [Cmdlet(VerbsCommon.Get, iControlNouns.SystemFanUsage)]
    public class GetSystemFanUsage : iControlPSCmdlet
    {

        #region Parameters
        /*
        [Parameter(Position = 0,
            Mandatory = false,
            ValueFromPipelineByPropertyName = true,
            HelpMessage = "Help Text")]
        [ValidateNotNullOrEmpty]
        public string Name
        {
            
        }
 */
        #endregion

        protected override void ProcessRecord()
        {
            if (isInitialized())
            {
                try
                {
                    iControl.SystemPlatformFans fan_metrics = GetiControl().SystemSystemInfo.get_fan_metrics();
                    for (int i = 0; i < fan_metrics.fans.Length; i++)
                    {
                        SystemFanItem sfi = new SystemFanItem();
                        for (int j = 0; j < fan_metrics.fans[i].Length; j++)
                        {
                            switch (fan_metrics.fans[i][j].metric_type)
                            {
                                case iControl.SystemFanMetricType.FAN_INDEX:
                                    sfi.Index = fan_metrics.fans[i][j].value;
                                    break;
                                case iControl.SystemFanMetricType.FAN_SPEED:
                                    sfi.Speed = fan_metrics.fans[i][j].value;
                                    break;
                                case iControl.SystemFanMetricType.FAN_STATE:
                                    sfi.State = fan_metrics.fans[i][j].value;
                                    break;
                            }
                        }
                        WriteObject(sfi);
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
