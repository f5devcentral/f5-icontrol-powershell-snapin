using System;
using System.Collections.Generic;
using System.Text;
using System.Management.Automation;
using System.Collections;
using iControlSnapIn.TypeData;

namespace iControlSnapIn.CmdLet.LTM.PoolMember
{
    [Cmdlet(VerbsCommon.Set, iControlNouns.LTMPoolMemberState, SupportsShouldProcess = true)]
    public class SetLTMPoolMemberState : iControlPSCmdlet
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

		private ServerState _server_state = ServerState.SERVER_STATE_UNKNOWN;
		private string _state = null;
		[Parameter(Position = 3, HelpMessage = "The State of the LTM Pool Member [Enabled|Disabled|Offline]")]
		[ValidateNotNullOrEmpty]
		public string State
		{
			get { return _state; }
			set { _state = value; }
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

			if (null != State)
			{
				switch(State.ToLower())
				{
					case "enabled":
						_server_state = ServerState.SERVER_STATE_ENABLED;
						break;
					case "disabled":
						_server_state = ServerState.SERVER_STATE_DISABLED;
						break;
					case "offline":
						_server_state = ServerState.SERVER_STATE_OFFLINE;
						break;
					default:
						handleError("You must specify a State value of \"Enabled\", \"Disabled\", or \"Offline\"", "Input Validation Error");
						bValid = false;
						break;
				}

			}
			else
			{
				handleError("You must specify a State value of \"Enabled\", \"Disabled\", or \"Offline\"", "Input Validation Error");
				bValid = false;
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


								iControl.LocalLBPoolMemberMemberMonitorState[][] monitor_states = new iControl.LocalLBPoolMemberMemberMonitorState[1][];
								monitor_states[0] = new iControl.LocalLBPoolMemberMemberMonitorState[1];
								monitor_states[0][0] = new iControl.LocalLBPoolMemberMemberMonitorState();
								monitor_states[0][0].member = new iControl.CommonIPPortDefinition();
								monitor_states[0][0].member.address = sSplit[0];
								monitor_states[0][0].member.port = Convert.ToInt32(sSplit[1]);

								iControl.LocalLBPoolMemberMemberSessionState[][] session_states = new iControl.LocalLBPoolMemberMemberSessionState[1][];
								session_states[0] = new iControl.LocalLBPoolMemberMemberSessionState[1];
								session_states[0][0] = new iControl.LocalLBPoolMemberMemberSessionState();
								session_states[0][0].member = new iControl.CommonIPPortDefinition();
								session_states[0][0].member.address = sSplit[0];
								session_states[0][0].member.port = Convert.ToInt32(sSplit[1]);

								switch (_server_state)
								{
									case ServerState.SERVER_STATE_DISABLED:
										monitor_states[0][0].monitor_state = iControl.CommonEnabledState.STATE_ENABLED;
										session_states[0][0].session_state = iControl.CommonEnabledState.STATE_DISABLED;
										break;
									case ServerState.SERVER_STATE_ENABLED:
										monitor_states[0][0].monitor_state = iControl.CommonEnabledState.STATE_ENABLED;
										session_states[0][0].session_state = iControl.CommonEnabledState.STATE_ENABLED;
										break;
									case ServerState.SERVER_STATE_OFFLINE:
										monitor_states[0][0].monitor_state = iControl.CommonEnabledState.STATE_DISABLED;
										session_states[0][0].session_state = iControl.CommonEnabledState.STATE_DISABLED;
										break;
								}

								GetiControl().LocalLBPoolMember.set_monitor_state(pool_list, monitor_states);
								GetiControl().LocalLBPoolMember.set_session_enabled_state(pool_list, session_states);

								LTMPoolMemberItem pmi = new LTMPoolMemberItem();
								pmi.Pool = Pool;
								pmi.Address = sSplit[0];
								pmi.Port = Convert.ToInt32(sSplit[1]);
								pmi.Name = sSplit[0] + ":" + Convert.ToInt32(sSplit[1]);
								pmi.Availability = iControl.LocalLBAvailabilityStatus.AVAILABILITY_STATUS_NONE;
								pmi.Enabled = (_server_state == ServerState.SERVER_STATE_ENABLED) ?	iControl.LocalLBEnabledStatus.ENABLED_STATUS_ENABLED : iControl.LocalLBEnabledStatus.ENABLED_STATUS_DISABLED;
								pmi.Status = "";

								WriteObject(pmi);
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
