using System;
using System.Collections.Generic;
using System.Text;
using System.Management.Automation;
using System.Collections;
using iControlSnapIn.TypeData;

namespace iControlSnapIn.CmdLet.System.SystemInfo
{
    [Cmdlet(VerbsCommon.Get, iControlNouns.SystemPowerUsage)]
    public class GetSystemPowerUsage : iControlPSCmdlet
    {

        #region Parameters

        #endregion

        protected override void ProcessRecord()
        {
            if (isInitialized())
            {
                try
                {
                    iControl.SystemPlatformPowerSupplies power_metrics = GetiControl().SystemSystemInfo.get_power_supply_metrics();

                    for (int i = 0; i < power_metrics.power_supplies.Length; i++)
                    {
                        SystemPowerSupplyItem spsi = new SystemPowerSupplyItem();
                        for (int j = 0; j < power_metrics.power_supplies[i].Length; j++)
                        {
                            switch (power_metrics.power_supplies[i][j].metric_type)
                            {
                                case iControl.SystemPSMetricType.PS_INDEX:
                                    spsi.Index = power_metrics.power_supplies[i][j].value;
                                    break;
                                case iControl.SystemPSMetricType.PS_STATE:
                                    spsi.State = power_metrics.power_supplies[i][j].value;
                                    break;
                            }
                        }

                        WriteObject(spsi);
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
