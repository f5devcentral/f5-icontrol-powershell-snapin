using System;
using System.Collections.Generic;
using System.Text;
using System.Management.Automation;
using System.Collections;

namespace iControlSnapIn.CmdLet.Global
{
    [Cmdlet(VerbsCommon.Get, iControlNouns.iControl)]
    public class GetiControl : iControlPSCmdlet
    {

        #region Parameters

        #endregion

        protected override void ProcessRecord()
        {
            if (isInitialized())
            {
                WriteObject(GetiControl());
            }
            else
            {
                handleNotInitialized();
            }
        }
    }
}
