using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Management.Automation;
using System.Collections;
using iControlSnapIn.TypeData;

namespace iControlSnapIn.CmdLet.Management.DBVariable
{
    [Cmdlet(VerbsCommon.Get, iControlNouns.DBVariable)]
    public class GetDBVariable : iControlPSCmdlet
    {

        #region Parameters

        private string _name = null;
        [Parameter(Position=0, HelpMessage="The name of the LTM Pool")]
        [ValidateNotNullOrEmpty]
        public string Name
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
                    iControl.ManagementDBVariableVariableNameValue [] var_list = GetiControl().ManagementDBVariable.get_list();

                    for (int i = 0; i < var_list.Length; i++)
                    {
                        bool bMatch = true;
                        if (null != _name)
                        {
                            bMatch = (true == matchString(var_list[i].name, _name, RegexOptions.IgnoreCase));
                        }

                        if (bMatch)
                        {
                            ManagementDatabaseItem mdbi = new ManagementDatabaseItem();
                            mdbi.Name = var_list[i].name;
                            mdbi.Value = var_list[i].value;
                            WriteObject(mdbi);
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
