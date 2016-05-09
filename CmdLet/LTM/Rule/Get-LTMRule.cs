using System;
using System.Collections.Generic;
using System.Text;
using System.Management.Automation;
using System.Collections;
using iControlSnapIn.TypeData;

namespace iControlSnapIn.CmdLet.LTM.Rule
{
    [Cmdlet(VerbsCommon.Get, iControlNouns.LTMRule, SupportsShouldProcess = true)]
    public class GetLTMRule : iControlPSCmdlet
    {

        #region Parameters

        private string _name = null;

        [Parameter(Position = 0, HelpMessage = "The name of the LTM iRule")]
        [ValidateNotNullOrEmpty]
        public string Rule
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
                    iControl.LocalLBRuleRuleDefinition[] rule_defs =
                        GetiControl().LocalLBRule.query_all_rules();

                    foreach (iControl.LocalLBRuleRuleDefinition rule_def in rule_defs)
                    {
                        bool bMatch = true;
                        if (null != _name)
                        {
                            bMatch = (true == matchString(rule_def.rule_name, _name));
                        }
                        if (bMatch)
                        {
                            LTMRuleDefinitionItem item = new LTMRuleDefinitionItem();
                            item.Name = rule_def.rule_name;
                            item.Definition = rule_def.rule_definition;
                            WriteObject(item);
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
