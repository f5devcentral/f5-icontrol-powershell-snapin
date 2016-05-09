using System;
using System.Collections.Generic;
using System.Text;
using System.Management.Automation;
using System.Collections;
using iControlSnapIn.TypeData;

namespace iControlSnapIn.CmdLet.LTM.PoolMember
{
    [Cmdlet(VerbsCommon.Get, iControlNouns.LTMPoolMemberState, SupportsShouldProcess = true)]
    public class GetLTMPoolMemberState : iControlPSCmdlet
    {

        #region Parameters

        private string _pool = null;
        [Parameter(Position = 0, HelpMessage = "The name of the LTM Pool")]
        [ValidateNotNullOrEmpty]
        public string Pool
        {
            get { return _pool; }
            set { _pool = value; }
        }

        private string _member = null;
        [Parameter(Position = 1, HelpMessage = "The IP:port of the LTM Pool Member")]
        [ValidateNotNullOrEmpty]
        public string Member
        {
            get { return _member; }
            set { _member = value; }
        }

		private LTMPoolMemberItem _pool_member_item;
		[Parameter(Position = 2, HelpMessage = "The LTMPoolMemberItem describing the pool member", ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
		[Alias("item")]
		[ValidateNotNullOrEmpty]
		public LTMPoolMemberItem PoolMember
		{
			get { return _pool_member_item; }
			set { _pool_member_item = value; }
		}

		#endregion

		private bool ValidateInput()
		{
			bool bValid = false;
			if ((null != _pool) && (null != _member))
			{
				bValid = true;
			}
			else if (null != _pool_member_item)
			{
				if ((null != _pool_member_item.Address) &&
					 (null != _pool_member_item.Pool))
				{
					if ((0 != _pool_member_item.Pool.Length) ||
						 (0 != _pool_member_item.Address.Length))
					{
						Pool = _pool_member_item.Pool;
						Member = _pool_member_item.Address + ":" + _pool_member_item.Port.ToString();
						bValid = true;
					}
				}
			}
			if (!bValid)
			{
				handleError("You must specify either the Pool and Member, or the PoolMember parameter ", "Input Validation Error");
			}
			return bValid;
		}

		protected override void ProcessRecord()
        {
			if (ValidateInput())
			{
				if (isInitialized())
				{
					try
					{
						if (null == _pool)
						{
							handleError("You must supply a pool name", "bad state");
						}
						else if (null == _member)
						{
							handleError("You must supply a member ip:port definition", "bad state");
						}
						else
						{
							String[] sSplit = _member.Split(new char[] { ':' });
							if (2 == sSplit.Length)
							{
								String[] pool_list = new String[] { _pool };
								iControl.LocalLBPoolMemberMemberObjectStatus[][] object_status_lists = GetiControl().LocalLBPoolMember.get_object_status(pool_list);
								iControl.LocalLBPoolMemberMemberMonitorStatus[][] monitor_status_lists = GetiControl().LocalLBPoolMember.get_monitor_status(pool_list);

								iControl.LocalLBPoolMemberMemberSessionState [][] session_state_lists = GetiControl().LocalLBPoolMember.get_session_enabled_state(pool_list);

								for (int i = 0; i < monitor_status_lists[0].Length; i++)
								{
									if (monitor_status_lists[0][i].member.address.Equals(sSplit[0]) &&
										monitor_status_lists[0][i].member.port == Convert.ToInt32(sSplit[1]))
									{
										iControl.LocalLBMonitorStatus status = monitor_status_lists[0][i].monitor_status;
										iControl.CommonEnabledState state = session_state_lists[0][i].session_state;
										// matched the requested node.
										ServerStateItem ssi = new ServerStateItem();
										ssi.Name = Member;
										if ((status == iControl.LocalLBMonitorStatus.MONITOR_STATUS_UP) &&
											(state == iControl.CommonEnabledState.STATE_ENABLED))
										{
											ssi.State = ServerState.SERVER_STATE_ENABLED;
										}
										else if ((status == iControl.LocalLBMonitorStatus.MONITOR_STATUS_UP) &&
											(state == iControl.CommonEnabledState.STATE_DISABLED))
										{
											ssi.State = ServerState.SERVER_STATE_DISABLED;
										}
										else if ((status != iControl.LocalLBMonitorStatus.MONITOR_STATUS_UP) &&
											(state == iControl.CommonEnabledState.STATE_DISABLED))
										{
											ssi.State = ServerState.SERVER_STATE_OFFLINE;
										}
										else
										{
											ssi.State = ServerState.SERVER_STATE_OFFLINE;
										}

										WriteObject(ssi);
									}
								}

							}
							else
							{
								handleError("Invalid member ip:port format", "bad state");
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
					handleError("You must first successfully call Initialize-iControl", "bad state");
				}
			}
        }
    }
}
