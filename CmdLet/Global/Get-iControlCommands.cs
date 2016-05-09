using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Management.Automation;
using System.Collections;
using System.Reflection;

namespace iControlSnapIn.CmdLet.Global
{
    [Cmdlet(VerbsCommon.Get, iControlNouns.iControlCommands)]
    public class GetiControlCommands : iControlPSCmdlet
    {

        #region Parameters

        private string _name = null;
        [Parameter(Position = 0, HelpMessage = "The hostname of the managed device")]
        [ValidateNotNullOrEmpty]
        public string Name
        {
            get { return _name; }
            set { _name = value.Replace("*", ".*"); }
        }

        #endregion

        private bool isDerivedFrom(Type objectType, Type targetType)
        {
            bool bIsDerivedFrom = false;
            if (null != objectType)
            {
                if (objectType.Equals(targetType))
                {
                    bIsDerivedFrom = true;
                }
                else
                {
                    bIsDerivedFrom = isDerivedFrom(objectType.BaseType, targetType);
                }
            }
            return bIsDerivedFrom;
        }

        protected override void ProcessRecord()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            if (null != assembly)
            {
                ArrayList al = new ArrayList();

                Type[] types = assembly.GetTypes();
                foreach (Type type in types)
                {
                    if ( isDerivedFrom(type, typeof(PSCmdlet)) )
                    {
                        object[] attrs = type.GetCustomAttributes(true);
                        if (null != attrs)
                        {
                            foreach (object o in attrs )
                            {
                                if ( o.GetType().Equals(typeof(CmdletAttribute)) )
                                {
                                    CmdletAttribute a = (CmdletAttribute)o;
                                    al.Add(a.VerbName + "-" + a.NounName);

                                }
                            }
                        }
                    }
                }

                al.Sort();

                for (int i = 0; i < al.Count; i++)
                {
                    String cmdName = al[i].ToString();
                    bool bMatch = true;
                    if (null != _name)
                    {
						bMatch = matchString(cmdName, _name, RegexOptions.IgnoreCase);
                    }
                    if (bMatch)
                    {
                        WriteObject(cmdName);
                    }
                }
            }
        }
    }
}
