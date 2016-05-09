using System;
using System.Collections.Generic;
using System.Text;
using System.Management.Automation;
using System.Collections;
using iControlSnapIn.TypeData;

namespace iControlSnapIn.CmdLet.LTM.VirtualServer
{
    [Cmdlet(VerbsCommon.Get, iControlNouns.LTMVritualServerRule)]
    public class GetLTMVirtualServerRule : iControlPSCmdlet
    {

        #region Parameters

        private string _name = null;
        [Parameter(Position = 0, HelpMessage = "The name of the LTM Virtual Server")]
        [ValidateNotNullOrEmpty]
        public string VirtualServer
        {
            get { return _name; }
            set { _name = value.Replace("*", ".*"); }
        }

		private LTMStatusItem _vs_item;
		[Parameter(Position = 1, HelpMessage = "The LTMStatusItem describing the virtual server", ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
		[ValidateNotNullOrEmpty]
		public LTMStatusItem VirtualServerItem
		{
			get { return _vs_item; }
			set { _vs_item = value; }
		}

		private LTMRuleDefinitionItem _rule_item;
		[Parameter(Position = 2, HelpMessage = "The LTMRuleDefinitionItem describing the iRule", ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
		[ValidateNotNullOrEmpty]
		public LTMRuleDefinitionItem RuleItem
		{
			get { return _rule_item; }
			set { _rule_item = value; }
		}

		private string _rule = null;
		[Parameter(Position = 3, HelpMessage = "The name of the iRule")]
		[ValidateNotNullOrEmpty]
		public string Rule
		{
			get { return _rule; }
			set { _rule = value.Replace("*", ".*"); }
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
			if ((null == Rule) && (null == RuleItem))
			{
				//handleError("You must specify either the VirtualServer or VirtualServerItem parameter", "Input Validation Error");
			}
			else
			{
				if (null != RuleItem)
				{
					Rule = RuleItem.Name;
				}
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
							if (true == matchString(vs_list[i], _name))
							{
								index_list.Add(i);
							}
						}

						if (index_list.Count > 0)
						{
							String[] target_vs_list = new String[index_list.Count];
							for (int j = 0; j < index_list.Count; j++)
							{
								int index = Convert.ToInt32(index_list[j]);
								target_vs_list[j] = vs_list[index];
							}

							iControl.LocalLBVirtualServerVirtualServerRule[][] rule_lists = GetiControl().LocalLBVirtualServer.get_rule(target_vs_list);

							String [] target_rules;

							for (int i = 0; i < rule_lists.Length; i++)
							{
								target_rules = new String[rule_lists[i].Length];
								for (int j = 0; j < rule_lists[i].Length; j++)
								{
									target_rules[j] = rule_lists[i][j].rule_name;
								}

								iControl.LocalLBRuleRuleDefinition [] rule_defs = GetiControl().LocalLBRule.query_rule(target_rules);

								for (int j = 0; j < rule_defs.Length; j++)
								{
									bool bFoundMatch = true;
									if (null != Rule)
									{
										bFoundMatch = false;
										if (true == matchString(rule_defs[j].rule_name, Rule))
										{
											bFoundMatch = true;
										}
									}
									if ( bFoundMatch )
									{
										LTMRuleDefinitionItem item = new LTMRuleDefinitionItem();
										item.Name = rule_defs[j].rule_name;
										item.Definition = rule_defs[j].rule_definition;
										WriteObject(item);
									}
								}
							}
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
