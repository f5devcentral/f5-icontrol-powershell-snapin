using System;
using System.Collections.Generic;
using System.Text;
using System.Management.Automation;
using System.Collections;
using iControlSnapIn.TypeData;

namespace iControlSnapIn.CmdLet.LTM.RamCache

{
    [Cmdlet(VerbsCommon.Get, iControlNouns.LTMRAMCacheEntries)]
    public class GetLTMRAMCacheEntries : iControlPSCmdlet
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

        private long _max_responses = 1000;
        [Parameter(Position = 3, HelpMessage = "The maximum number of responses in the result set (defaults to 1000)")]
        [ValidateNotNullOrEmpty]
        public long MaxResponses
        {
            get { return _max_responses; }
            set { _max_responses = value; }
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
                    keys[0].maximum_responses = _max_responses;
                    iControl.LocalLBRAMCacheInformationRAMCacheEntry [][] cache_entries = 
                        GetiControl().LocalLBRAMCacheInformation.get_ramcache_entry(keys);
                    for (int i = 0; i < cache_entries[0].Length; i++)
                    {
                        LTMRAMCacheEntry rce = new LTMRAMCacheEntry();
                        rce.ProfileName = cache_entries[0][i].profile_name;
                        rce.Hostname = cache_entries[0][i].host_name;
                        rce.Uri = cache_entries[0][i].uri;
                        switch (cache_entries[0][i].vary_type)
                        {
                            case iControl.LocalLBRAMCacheInformationRAMCacheVaryType.RAM_CACHE_VARY_ACCEPT_ENCODING:
                                rce.VaryType = "ACCEPT_ENCODING";
                                break;
                            case iControl.LocalLBRAMCacheInformationRAMCacheVaryType.RAM_CACHE_VARY_BOTH:
                                rce.VaryType = "BOTH";
                                break;
                            case iControl.LocalLBRAMCacheInformationRAMCacheVaryType.RAM_CACHE_VARY_NONE:
                                rce.VaryType = "NONE";
                                break;
                            case iControl.LocalLBRAMCacheInformationRAMCacheVaryType.RAM_CACHE_VARY_USERAGENT:
                                rce.VaryType = "USERAGENT";
                                break;

                        }
                        rce.VaryCount = cache_entries[0][i].vary_count;
                        rce.Hits = cache_entries[0][i].hits;
                        rce.Received = cache_entries[0][i].received;
                        rce.LastSent = cache_entries[0][i].last_sent;
                        rce.Expiration = cache_entries[0][i].expiration;
                        rce.Size = cache_entries[0][i].size;

                        WriteObject(rce);
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
