using System;
using System.Collections.Generic;
using System.Text;
using System.Management.Automation;
using System.Collections;
using iControlSnapIn.Provider;

namespace iControlSnapIn.CmdLet.LTM.RamCache
{
    [Cmdlet(VerbsCommon.Remove, iControlNouns.LTMRAMCacheEntries)]
    public class RemoveLTMRAMCacheEntries : iControlPSCmdlet
    {

        #region Parameters

        private string _profile_name = null;
        [Parameter(Position = 0, Mandatory=true, HelpMessage = "The profile name to be used for the match key")]
        [ValidateNotNullOrEmpty]
        public string ProfileName
        {
            get { return _profile_name; }
            set { _profile_name = value; }
        }

        private string _host_name = null;
        [Parameter(Position = 1, HelpMessage = "The hostname to be used for the match key")]
        [ValidateNotNullOrEmpty]
        public string Hostname
        {
            get { return _host_name; }
            set { _host_name = value; }
        }

        private string _uri = null;
        [Parameter(Position = 2, HelpMessage = "The uri to be used for the match key")]
        [ValidateNotNullOrEmpty]
        public string Uri
        {
            get { return _uri; }
            set { _uri = value; }
        }

        #endregion

        protected override void ProcessRecord()
        {
            if (isInitialized())
            {
                try
                {
                    iControl.LocalLBRAMCacheInformationRAMCacheKey[] keys = new iControl.LocalLBRAMCacheInformationRAMCacheKey[1];
                    keys[0] = new iControl.LocalLBRAMCacheInformationRAMCacheKey();
                    keys[0].host_name = (null != _host_name) ? _host_name : "";
                    keys[0].profile_name = (null != _profile_name) ? _profile_name : "";
                    keys[0].uri = (null != _uri) ? _uri : "";
                    keys[0].maximum_responses = 0;

                    GetiControl().LocalLBRAMCacheInformation.evict_ramcache_entry(keys);
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
