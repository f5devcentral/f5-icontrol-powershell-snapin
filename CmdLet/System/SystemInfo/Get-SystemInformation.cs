using System;
using System.Collections.Generic;
using System.Text;
using System.Management.Automation;
using System.Collections;
using iControlSnapIn.TypeData;

namespace iControlSnapIn.CmdLet.System.SystemInfo
{
    [Cmdlet(VerbsCommon.Get, iControlNouns.SystemInformation)]
    public class GetSystemInformation : iControlPSCmdlet
    {

        #region Parameters

        #endregion

        protected override void ProcessRecord()
        {
            if (isInitialized())
            {
                iControl.SystemSystemInformation sysInfo = GetiControl().SystemSystemInfo.get_system_information();
                SystemInformation si = new SystemInformation();
                si.AnnunciatorBoardPartRevision = sysInfo.annunciator_board_part_revision;
                si.AnnunciatorBoardSerial = sysInfo.annunciator_board_serial;
                si.ChassisSerial = sysInfo.chassis_serial;
                si.HostBoardPartRevision = sysInfo.host_board_part_revision;
                si.HostBoardSerial = sysInfo.host_board_serial;
                si.Hostname = sysInfo.host_name;
                si.OSMachine = sysInfo.os_machine;
                si.OSRelease = sysInfo.os_release;
                si.OSVersion = sysInfo.os_version;
                si.Platform = sysInfo.platform;
                si.ProductCategory = sysInfo.product_category;
                si.SwitchBoardPartRevision = sysInfo.switch_board_part_revision;
                si.SwitchBoardSerial = sysInfo.switch_board_serial;
                si.SystemName = sysInfo.system_name;

                WriteObject(si);
            }
            else
            {
                handleNotInitialized();
            }
        }
    }
}
