using System;
using System.Collections.Generic;
using System.Text;
using System.Management.Automation;
using System.Collections;
using iControlSnapIn.TypeData;

namespace iControlSnapIn.CmdLet.LTM.VirtualServer
{
    [Cmdlet(VerbsLifecycle.Enable, iControlNouns.LTMVirtualServer)]
    public class EnableLTMVirtualServer : iControlPSCmdlet
    {

        #region Parameters

        private string _name = null;
        [Parameter(Position=0, HelpMessage="The name of the LTM Virtual Server")]
        [ValidateNotNullOrEmpty]
        public string VirtualServer
        {
            get { return _name; }
            set { _name = value.Replace("*", ".*"); }
        }

		private LTMStatusItem _vs_item;
		[Parameter(Position = 1, HelpMessage = "The LTMStatusItem describing the virtual server", ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
		[Alias("item")]
		[ValidateNotNullOrEmpty]
		public LTMStatusItem VirtualServerItem
		{
			get { return _vs_item; }
			set { _vs_item = value; }
		}

		#endregion

		protected bool ValidateInput()
		{
			bool bValid = false;

			if ((null == VirtualServer) && (null == VirtualServerItem))
			{
				handleError("You must specify either the VirtualServer or VirtualServerItem parameter", "Input Validation Error");
			}
			else
			{
				if (null != VirtualServerItem)
				{
					VirtualServer = VirtualServerItem.Name;
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
						String[] vs_list = GetiControl().LocalLBVirtualServer.get_list();
						iControl.CommonEnabledState[] enabled_states = GetiControl().LocalLBVirtualServer.get_enabled_state(vs_list);

						// Build a list of indexes
						ArrayList index_list = new ArrayList();

						for (int i = 0; i < vs_list.Length; i++)
						{
							if (matchString(vs_list[i], _name))
							{
								index_list.Add(i);
							}
						}

						if (index_list.Count > 0)
						{
							String[] target_vs_list = new String[index_list.Count];
							iControl.CommonEnabledState[] target_enabled_states = new iControl.CommonEnabledState[index_list.Count];
							for (int j = 0; j < index_list.Count; j++)
							{
								int index = Convert.ToInt32(index_list[j]);
								target_vs_list[j] = vs_list[index];
								target_enabled_states[j] = iControl.CommonEnabledState.STATE_ENABLED;
							}

							GetiControl().LocalLBVirtualServer.set_enabled_state(target_vs_list, target_enabled_states);

							LTMStatusItem si = new LTMStatusItem();
							si.Name = VirtualServer;
							si.Availability = iControl.LocalLBAvailabilityStatus.AVAILABILITY_STATUS_NONE;
							si.Enabled = iControl.LocalLBEnabledStatus.ENABLED_STATUS_ENABLED;
							si.Status = "";

							WriteObject(si);
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
