using System;
using System.Collections.Generic;
using System.Text;
using System.Management.Automation;
using System.Collections;
using iControlSnapIn.TypeData;

namespace iControlSnapIn.CmdLet.LTM.VirtualServer
{
    [Cmdlet(VerbsCommon.Get, iControlNouns.LTMVirtualServer, SupportsShouldProcess = true)]
    public class GetLTMVirtualServer : iControlPSCmdlet
    {

        #region Parameters

        private string _name = null;
        [Parameter(Position = 0,
            HelpMessage = "The name of the LTM Virtual Server")]
        [ValidateNotNullOrEmpty]
        public string VirtualServer
        {
            get { return _name; }
            set { _name = value; }
        }

        #endregion

        protected override void ProcessRecord()
        {
            if (isInitialized())
            {
                try
                {
                    String[] vs_list = GetiControl().LocalLBVirtualServer.get_list();
                    iControl.LocalLBObjectStatus[] object_statuses = GetiControl().LocalLBVirtualServer.get_object_status(vs_list);

                    for (int i = 0; i < vs_list.Length; i++)
                    {
                        bool bMatch = true;
                        if (null != _name)
                        {
                            bMatch = (matchString(vs_list[i], _name));
                        }

                        if (bMatch)
                        {
                            LTMStatusItem si = new LTMStatusItem();
                            si.Name = vs_list[i];
                            si.Availability = object_statuses[i].availability_status;
                            si.Enabled = object_statuses[i].enabled_status;
                            si.Status = object_statuses[i].status_description;

                            WriteObject(si);
                        }
                    }
                }
                catch (Exception ex)
                {
                    ErrorRecord er = new ErrorRecord(ex, "2", ErrorCategory.OpenError, "error");
                    WriteError(er);
                }
            }
            else
            {
                handleNotInitialized();
            }
        }
    }
}
