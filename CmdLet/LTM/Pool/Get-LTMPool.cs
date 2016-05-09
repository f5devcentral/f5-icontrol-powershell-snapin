using System;
using System.Collections.Generic;
using System.Text;
using System.Management.Automation;
using System.Collections;
using iControlSnapIn.TypeData;

namespace iControlSnapIn.CmdLet.LTM.Pool
{
    [Cmdlet(VerbsCommon.Get, iControlNouns.LTMPool)]
    public class GetLTMPool : iControlPSCmdlet
    {

        #region Parameters

        private string _name = null;
        [Parameter(Position = 0, HelpMessage = "The name of the LTM Pool")]
        [ValidateNotNullOrEmpty]
        public string Pool
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
                    string[] pool_list = GetiControl().LocalLBPool.get_list();
                    iControl.LocalLBObjectStatus[] object_statuses = GetiControl().LocalLBPool.get_object_status(pool_list);
                    iControl.CommonIPPortDefinition[][] member_list = GetiControl().LocalLBPool.get_member(pool_list);

                    for (int i = 0; i < pool_list.Length; i++)
                    {
                        bool bMatch = true;
                        if (null != _name)
                        {
							bMatch = matchString(pool_list[i], _name);
                        }

                        if (bMatch)
                        {
                            LTMPoolItem ltmpi = new LTMPoolItem();
                            ltmpi.Name = pool_list[i];
                            ltmpi.MemberCount = member_list[i].Length;
                            ltmpi.Availability = object_statuses[i].availability_status;
                            ltmpi.Enabled = object_statuses[i].enabled_status;
                            ltmpi.Status = object_statuses[i].status_description;
                            WriteObject(ltmpi);
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
