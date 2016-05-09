using System;
using System.Collections.Generic;
using System.Text;
using System.Management.Automation;
using System.Collections;
using iControlSnapIn.TypeData;

namespace iControlSnapIn.CmdLet.LTM.PoolMember
{
    [Cmdlet(VerbsCommon.Get, iControlNouns.LTMPoolMember, SupportsShouldProcess = true)]
    public class GetLTMPoolMember : iControlPSCmdlet
    {

        #region Parameters

        private string _pool = null;
        [Parameter(Position=0, Mandatory=true, HelpMessage="The name of the LTM Pool")]
        [ValidateNotNullOrEmpty]
        public string Pool
        {
            get { return _pool; }
            set { _pool = value; }
        }

        private string _member = null;
        [Parameter(Position=1, HelpMessage="The IP:port of the LTM Pool Member")]
        [ValidateNotNullOrEmpty]
        public string Member
        {
            get { return _member; }
            set { _member = value; }
        }

        #endregion

        protected override void ProcessRecord()
        {
            if (isInitialized())
            {
                try
                {
                    string[] pool_list = GetiControl().LocalLBPool.get_list();
                    iControl.CommonIPPortDefinition[][] member_list = GetiControl().LocalLBPool.get_member(pool_list);
                    iControl.LocalLBPoolMemberMemberObjectStatus [][] object_statuses = GetiControl().LocalLBPoolMember.get_object_status(pool_list);

					for (int i = 0; i < pool_list.Length; i++)
                    {
                        bool bMatchPool = true;
                        if (null != _pool)
                        {
							bMatchPool = matchString(pool_list[i], _pool);
                        }

                        if (bMatchPool)
                        {
                            for(int j=0; j<object_statuses[i].Length; j++)
                            {
                                String member = object_statuses[i][j].member.address + ":" + object_statuses[i][j].member.port.ToString();
                                bool bMatchMember = true;
                                if (null != _member)
                                {
									bMatchMember = matchString(member, _member);
                                }

                                if ( bMatchMember )
                                {
                                    LTMPoolMemberItem mi = new LTMPoolMemberItem();
									mi.Pool = pool_list[i];
                                    mi.Name = object_statuses[i][j].member.address + ":" + object_statuses[i][j].member.port.ToString();
                                    mi.Address = object_statuses[i][j].member.address;
                                    mi.Port = object_statuses[i][j].member.port;
                                    mi.Availability = object_statuses[i][j].object_status.availability_status;
                                    mi.Enabled = object_statuses[i][j].object_status.enabled_status;
                                    mi.Status = object_statuses[i][j].object_status.status_description;

                                    WriteObject(mi);
                                }
                            }
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
