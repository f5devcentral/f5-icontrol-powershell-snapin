using System;
using System.Collections.Generic;
using System.Text;
using System.Management.Automation;
using System.Collections;
using iControlSnapIn.TypeData;

namespace iControlSnapIn.CmdLet.System.ConfigSync
{
    [Cmdlet(VerbsCommon.Get, iControlNouns.ConfigurationList)]
    public class GetConfigurationList : iControlPSCmdlet
    {

        #region Parameters

        #endregion

        protected override void ProcessRecord()
        {
            if (isInitialized())
            {
                try
                {
                    iControl.SystemConfigSyncConfigFileEntry [] config_list = 
                        GetiControl().SystemConfigSync.get_configuration_list();
                    foreach (iControl.SystemConfigSyncConfigFileEntry entry in config_list)
                    {
                        ConfigFileEntry cfe = new ConfigFileEntry();
                        cfe.Name = entry.file_name;
                        cfe.TimeStamp = entry.file_datetime;

                        WriteObject(cfe);
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
