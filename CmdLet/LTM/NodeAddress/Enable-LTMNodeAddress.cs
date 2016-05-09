using System;
using System.Collections.Generic;
using System.Text;
using System.Management.Automation;
using System.Collections;
using iControlSnapIn.TypeData;

namespace iControlSnapIn.CmdLet.LTM.VirtualServer
{
    [Cmdlet(VerbsLifecycle.Enable, iControlNouns.LTMNodeAddress)]
    public class EnableLTMNodeAddress : iControlPSCmdlet
    {

        #region Parameters

        private string _name = null;
        [Parameter(Position=0, HelpMessage="The address of the node to enable")]
        [ValidateNotNullOrEmpty]
        public string Node
        {
            get { return _name; }
            set { _name = value.Replace("*", ".*"); }
        }

		private LTMStatusItem _node_item;
		[Parameter(Position = 1, HelpMessage = "The LTMStatusItem describing the node address", ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
		[Alias("item")]
		[ValidateNotNullOrEmpty]
		public LTMStatusItem NodeItem
		{
			get { return _node_item; }
			set { _node_item = value; }
		}

		#endregion

		protected bool ValidateInput()
		{
			bool bValid = false;

			if ((null == Node) && (null == NodeItem))
			{
				handleError("You must specify either the Node or NodeItem parameter", "Input Validation Error");

			}
			else
			{
				if (null != NodeItem)
				{
					Node = NodeItem.Name;
				}
				bValid = true;
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
						String[] node_list = GetiControl().LocalLBNodeAddress.get_list();

						// Build a list of indexes
						ArrayList index_list = new ArrayList();

						for (int i = 0; i < node_list.Length; i++)
						{
							if (matchString(node_list[i], _name))
							{
								index_list.Add(i);
							}
						}

						if (index_list.Count > 0)
						{
							String[] target_node_list = new String[index_list.Count];

							iControl.CommonEnabledState[] target_enabled_states = new iControl.CommonEnabledState[index_list.Count];
							for (int j = 0; j < index_list.Count; j++)
							{
								int index = Convert.ToInt32(index_list[j]);
								target_node_list[j] = node_list[index];
								target_enabled_states[j] = iControl.CommonEnabledState.STATE_ENABLED;
							}

							GetiControl().LocalLBNodeAddress.set_session_enabled_state(target_node_list, target_enabled_states);

							LTMStatusItem ltmsi = new LTMStatusItem();
							ltmsi.Name = Node;
							ltmsi.Availability = iControl.LocalLBAvailabilityStatus.AVAILABILITY_STATUS_NONE;
							ltmsi.Enabled = iControl.LocalLBEnabledStatus.ENABLED_STATUS_ENABLED;
							ltmsi.Status = "";
							WriteObject(ltmsi);
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
}
