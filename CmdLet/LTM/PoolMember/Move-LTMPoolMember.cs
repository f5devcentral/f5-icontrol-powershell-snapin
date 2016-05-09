using System;
using System.Collections.Generic;
using System.Text;
using System.Management.Automation;
using System.Collections;
using iControlSnapIn.TypeData;

namespace iControlSnapIn.CmdLet.LTM.PoolMember
{
    [Cmdlet(VerbsCommon.Move, iControlNouns.LTMPoolMember, SupportsShouldProcess = true)]
    public class MoveLTMPoolMember : iControlPSCmdlet
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

		private string _target = null;
        [Parameter(Position = 3, Mandatory = true, HelpMessage = "The Target pool to move the member to")]
        [ValidateNotNullOrEmpty]
        public string Target
        {
            get { return _target; }
            set { _target = value; }
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
						String[] sSplit = _member.Split(new char[] { ':' });
						if (2 == sSplit.Length)
						{
							String[] pool_list = new String[] { _pool };
							iControl.CommonIPPortDefinition[][] member_defs = new iControl.CommonIPPortDefinition[1][];
							member_defs[0] = new iControl.CommonIPPortDefinition[1];
							member_defs[0][0] = new iControl.CommonIPPortDefinition();
							member_defs[0][0].address = sSplit[0];
							member_defs[0][0].port = Convert.ToInt32(sSplit[1]);
							String[] target_pool_list = new String[] { _target };

							// Remove from first pool
							GetiControl().LocalLBPool.remove_member(pool_list, member_defs);

							// Add to target pool
							GetiControl().LocalLBPool.add_member(target_pool_list, member_defs);

							LTMPoolMemberItem pmi = new LTMPoolMemberItem();
							pmi.Pool = Target;
							pmi.Address = sSplit[0];
							pmi.Port = Convert.ToInt32(sSplit[1]);
							pmi.Name = sSplit[0] + ":" + Convert.ToInt32(sSplit[1]);
							pmi.Availability = iControl.LocalLBAvailabilityStatus.AVAILABILITY_STATUS_NONE;
							pmi.Enabled = iControl.LocalLBEnabledStatus.ENABLED_STATUS_ENABLED;
							pmi.Status = "";

							WriteObject(pmi);
						}
						else
						{
							handleError("Invalid member ip:port format", "bad state");
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
}
