using System;
using System.Collections.Generic;
using System.Text;
using System.Management.Automation;
using System.Collections;
using iControlSnapIn.TypeData;

namespace iControlSnapIn.CmdLet.LTM.Pool
{
    [Cmdlet(VerbsCommon.Get, iControlNouns.LTMNodeAddress)]
    public class GetLTMNodeAddress : iControlPSCmdlet
    {

        #region Parameters

        private string _name = null;
        [Parameter(Position = 0, HelpMessage = "The name of the LTM Pool")]
        [ValidateNotNullOrEmpty]
        public string Node
        {
            get { return _name; }
            set { _name = value.Replace("*", ".*"); }
        }

        #endregion

        protected override void ProcessRecord()
        {
            if (isInitialized())
            {
                try
                {
                    string [] node_list = GetiControl().LocalLBNodeAddress.get_list();
                    iControl.LocalLBObjectStatus[] object_statuses = GetiControl().LocalLBNodeAddress.get_object_status(node_list);

					for (int i = 0; i < node_list.Length; i++)
                    {
                        bool bMatch = true;
                        if (null != _name)
                        {
							bMatch = matchString(node_list[i], _name);
                        }

                        if (bMatch)
                        {
                            LTMStatusItem ltmsi = new LTMStatusItem();
                            ltmsi.Name = node_list[i];
                            ltmsi.Availability = object_statuses[i].availability_status;
                            ltmsi.Enabled = object_statuses[i].enabled_status;
                            ltmsi.Status = object_statuses[i].status_description;
                            WriteObject(ltmsi);
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
