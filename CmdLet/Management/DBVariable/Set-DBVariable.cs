using System;
using System.Collections.Generic;
using System.Text;
using System.Management.Automation;
using System.Collections;
using iControlSnapIn.TypeData;

namespace iControlSnapIn.CmdLet.Management.DBVariable
{
    [Cmdlet(VerbsCommon.Set, iControlNouns.DBVariable)]
    public class SetDBVariable : iControlPSCmdlet
    {

        #region Parameters

        private string _name = null;
        [Parameter(Position=0, Mandatory=true, HelpMessage="The database variable name")]
        [ValidateNotNullOrEmpty]
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        private string _value = null;
        [Parameter(Position=1, Mandatory=true, HelpMessage="The database variable value")]
        [ValidateNotNullOrEmpty]
        public string Value
        {
            get { return _value; }
            set { _value = value; }
        }

        #endregion

        protected override void ProcessRecord()
        {
            if (isInitialized())
            {
                try
                {
                    iControl.ManagementDBVariableVariableNameValue[] variables = new iControl.ManagementDBVariableVariableNameValue[1];
                    variables[0] = new iControl.ManagementDBVariableVariableNameValue();
                    variables[0].name = _name;
                    variables[0].value = _value;
                    if (variableExists(_name))
                    {
                        // modify
                        GetiControl().ManagementDBVariable.modify(variables);
                    }
                    else
                    {
                        // create
                        GetiControl().ManagementDBVariable.create(variables);
                    }
                    ManagementDatabaseItem mdbi = new ManagementDatabaseItem();
                    mdbi.Name = _name;
                    mdbi.Value = _value;
                    WriteObject(mdbi);
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

        private bool variableExists(String name)
        {
            bool bExists = false;
            try
            {
                iControl.ManagementDBVariableVariableNameValue[] var_list =
                    GetiControl().ManagementDBVariable.query(new string[] { name });
                if (1 == var_list.Length)
                {
                    bExists = true;
                }
            }
            catch (Exception)
            {
            }
            return bExists;
        }
    }
}
