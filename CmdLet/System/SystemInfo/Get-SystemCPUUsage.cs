using System;
using System.Collections.Generic;
using System.Text;
using System.Management.Automation;
using System.Collections;
using iControlSnapIn.TypeData;

namespace iControlSnapIn.CmdLet.System.SystemInfo
{
    [Cmdlet(VerbsCommon.Get, iControlNouns.SystemCPUUsage)]
    public class GetSystemCPUUsage : iControlPSCmdlet
    {

        #region Parameters

        #endregion

        protected override void ProcessRecord()
        {
            if (isInitialized())
            {
                try
                {
                    iControl.SystemPlatformCPUs cpu_metrics = GetiControl().SystemSystemInfo.get_cpu_metrics();
                    iControl.SystemCPUUsageInformation usage_info = GetiControl().SystemSystemInfo.get_cpu_usage_information();

                    for (int i = 0; i < usage_info.usages.Length; i++)
                    {
                        SystemCPUItem sci = new SystemCPUItem();
                        for (int j = 0; j < cpu_metrics.cpus[i].Length; j++)
                        {
                            switch (cpu_metrics.cpus[i][j].metric_type)
                            {
                                case iControl.SystemCPUMetricType.CPU_INDEX:
                                    sci.Index = cpu_metrics.cpus[i][j].value;
                                    break;
                                case iControl.SystemCPUMetricType.CPU_FAN_SPEED:
                                    sci.FanSpeed = cpu_metrics.cpus[i][j].value;
                                    break;
                                case iControl.SystemCPUMetricType.CPU_TEMPERATURE:
                                    sci.TemperatureF = cpu_metrics.cpus[i][j].value;
                                    break;
                            }
                        }
                        sci.Id = usage_info.usages[i].cpu_id;
                        sci.UsageIdle = Utility.build64(usage_info.usages[i].idle);
                        sci.UsageIOWait = Utility.build64(usage_info.usages[i].iowait);
                        sci.UsageIRQ = Utility.build64(usage_info.usages[i].irq);
                        sci.UsageNiced = Utility.build64(usage_info.usages[i].niced);
                        sci.UsageSoftIRQ = Utility.build64(usage_info.usages[i].softirq);
                        sci.UsageSystem = Utility.build64(usage_info.usages[i].system);
                        sci.UsageUser = Utility.build64(usage_info.usages[i].user);

                        WriteObject(sci);
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
