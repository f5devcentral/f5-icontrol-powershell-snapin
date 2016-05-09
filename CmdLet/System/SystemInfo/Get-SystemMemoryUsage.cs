using System;
using System.Collections.Generic;
using System.Text;
using System.Management.Automation;
using System.Collections;
using iControlSnapIn.TypeData;

namespace iControlSnapIn.CmdLet.System.SystemInfo
{
    [Cmdlet(VerbsCommon.Get, iControlNouns.SystemMemoryUsage)]
    public class GetSystemMemoryUsage : iControlPSCmdlet
    {

        #region Parameters

        #endregion

        protected override void ProcessRecord()
        {
            if (isInitialized())
            {
                try
                {
                    iControl.SystemMemoryUsageInformation memory_info = GetiControl().SystemSystemInfo.get_memory_usage_information();

                    SystemMemoryItem smi = new SystemMemoryItem();
                    smi.Name = "Total";
                    smi.MaximumAllocated = 0;
                    smi.CurrentAllocated = 0;
                    smi.Size = Utility.build64(memory_info.total_memory);
                    WriteObject(smi);

                    smi.Name = "Used";
                    smi.Size = Utility.build64(memory_info.used_memory);
                    WriteObject(smi);

                    //                smi.Usages = new SystemMemorySubItem[memory_info.usages.Length];
                    for (int i = 0; i < memory_info.usages.Length; i++)
                    {
                        smi.Name = memory_info.usages[i].subsystem_name;
                        smi.CurrentAllocated = Utility.build64(memory_info.usages[i].current_allocated);
                        smi.MaximumAllocated = Utility.build64(memory_info.usages[i].maximum_allocated);
                        smi.Size = Utility.build64(memory_info.usages[i].size);

                        WriteObject(smi);
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
