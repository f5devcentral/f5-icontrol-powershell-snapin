using System;
using System.Collections.Generic;
using System.Text;
using System.Management.Automation;
using System.Collections;
using iControlSnapIn.TypeData;

namespace iControlSnapIn.CmdLet.System.SystemInfo
{
    [Cmdlet(VerbsCommon.Get, iControlNouns.SystemDiskUsage)]
    public class GetSystemDiskUsage : iControlPSCmdlet
    {

        #region Parameters

        #endregion

        protected override void ProcessRecord()
        {
            if (isInitialized())
            {
                try
                {
                    iControl.SystemDiskUsageInformation disk_usage = GetiControl().SystemSystemInfo.get_disk_usage_information();
                    for (int i = 0; i < disk_usage.usages.Length; i++)
                    {
                        SystemDiskItem sdi = new SystemDiskItem();
                        sdi.BlockSize = Utility.build64(disk_usage.usages[i].block_size);
                        sdi.FreeBlocks = Utility.build64(disk_usage.usages[i].free_blocks);
                        sdi.FreeNodes = Utility.build64(disk_usage.usages[i].free_nodes);
                        sdi.PartitionName = disk_usage.usages[i].partition_name;
                        sdi.TotalBlocks = Utility.build64(disk_usage.usages[i].total_blocks);
                        sdi.TotalNodes = Utility.build64(disk_usage.usages[i].total_nodes);

                        WriteObject(sdi);
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
