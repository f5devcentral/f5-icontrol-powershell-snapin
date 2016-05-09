using System;
using System.Collections.Generic;
using System.Text;
using System.Management.Automation;
using System.Collections;
using iControlSnapIn.Provider;

namespace iControlSnapIn.CmdLet.Deployment
{
    [Cmdlet(VerbsCommon.New, iControlNouns.SharePoint2007Deployment, SupportsShouldProcess=true)]
    public class NewSharePoint2007Deployment : DeploymentPSCmdLet
    {

        #region Parameters

        private string _app_name = null;
        [Parameter(Position = 0, Mandatory = true, HelpMessage = "A User Friendly name for the application")]
        [ValidateNotNullOrEmpty]
        public string ApplicationName
        {
            get { return _app_name; }
            set { _app_name = value; }
        }

        private string _virtual_server = null;
        [Parameter(Position = 1, Mandatory=true, HelpMessage = "The IP:Port of the target virtual server")]
        [ValidateNotNullOrEmpty]
        public string VirtualServer
        {
            get { return _virtual_server; }
            set { _virtual_server = value; }
        }

        private string [] _pool_members = null;
        [Parameter(Position = 2, Mandatory = true, HelpMessage = "The list of pool members (IP:Port)")]
        [ValidateNotNullOrEmpty]
        public string [] PoolMembers
        {
            get { return _pool_members; }
            set { _pool_members = value; }
        }

        private bool _compression = true;
        [Parameter(Position = 3, Mandatory = true, HelpMessage = "Boolean to specify whether compression is enabled.")]
        [ValidateNotNullOrEmpty]
        public bool Compression
        {
            get { return _compression; }
            set { _compression = value; }
        }

        #endregion

        private String getHealthMonitor() { return _app_name + "_SPS_http_monitor"; }
        private String getPoolName() { return _app_name + "_SPS_servers"; }
        private String getHTTPProfile() { return _app_name + "_SPS_http_opt"; }
        private String getCookieProfile() { return _app_name + "_SPS_cookie_opt"; }
        private String getTCPProfile() { return _app_name + "_SPS_tcp_opt"; }
        private String getVSName() { return _app_name + "_SPS_http_vs"; }
        private String getDefaultSNATName() { return "DefaultSNAT"; }

        protected override void ProcessRecord()
        {
            if (isInitialized())
            {
                try
                {
                    if (null == _app_name)
                    {
                        handleError("You must supply an application name", "bad state");
                    }
                    else if (null == _virtual_server)
                    {
                        handleError("You must supply a virtual server", "bad state");
                    }
                    else if (null == _pool_members)
                    {
                        handleError("You must supply a list of pool members in IP:port format", "bad state");
                    }
                    else
                    {
                        if (Verify())
                        {
                            Deploy();
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

        private bool Verify()
        {
            bool bValid = false;
            reportStatus("Validating configuration...", true);

            if (healthMonitorExists(getHealthMonitor()))
            {
                bValid = false;
            }
            if (poolExists(getPoolName()))
            {
                bValid = false;
            }
            if (httpProfileExists(getHTTPProfile()))
            {
                bValid = false;
            }
            if (tcpProfileExists(getTCPProfile()))
            {
                bValid = false;
            }
            if (persistenceProfileExists(getCookieProfile()))
            {
                bValid = false;
            }
            if (virtualServerExists(getVSName()))
            {
                bValid = false;
            }

            return bValid;
        }

        private void Deploy()
        {
            bool bSuccess = true;

            if (bSuccess)
            {
                bSuccess = createHealthMonitor();
            }
            if (bSuccess)
            {
                bSuccess = createPool();
            }
            if (bSuccess)
            {
                bSuccess = attachHealthMonitor();
            }
            if (bSuccess)
            {
                bSuccess = createHttpProfile();
            }
            if (bSuccess)
            {
                bSuccess = createTCPProfile();
            }
            if (bSuccess)
            {
                bSuccess = createPersistenceProfile();
            }
            if (bSuccess)
            {
                bSuccess = createVirtualServer();
            }
            if (bSuccess)
            {
                bSuccess = createDefaultSNAT();
            }
        }

        private bool createHealthMonitor()
        {
            reportStatus("Creating Health Montitor '" + getHealthMonitor() + "'... ");
            bool bSuccess = false;
            try
            {
                iControl.LocalLBMonitorMonitorTemplate[] templates = new iControl.LocalLBMonitorMonitorTemplate[1];
                templates[0] = new iControl.LocalLBMonitorMonitorTemplate();
                templates[0].template_name = getHealthMonitor();
                templates[0].template_type = iControl.LocalLBMonitorTemplateType.TTYPE_UNSET;

                iControl.LocalLBMonitorCommonAttributes[] attrs = new iControl.LocalLBMonitorCommonAttributes[1];
                attrs[0] = new iControl.LocalLBMonitorCommonAttributes();
                attrs[0].parent_template = "http";
                attrs[0].interval = 30;
                attrs[0].timeout = 91;
                attrs[0].dest_ipport = new iControl.LocalLBMonitorIPPort();
                attrs[0].dest_ipport.address_type = iControl.LocalLBAddressType.ATYPE_UNSET;
                attrs[0].dest_ipport.ipport = new iControl.CommonIPPortDefinition();
                attrs[0].dest_ipport.ipport.address = "127.0.0.1";
                attrs[0].dest_ipport.ipport.port = 80;
                attrs[0].is_read_only = false;
                attrs[0].is_directly_usable = true;
                GetiControl().LocalLBMonitor.create_template(templates, attrs);

                reportStatus("SUCCESS", true);

                bSuccess = true;
            }
            catch (Exception ex)
            {
                reportStatus("FAILURE", true);
                reportStatus(ex.Message, true);
            }

            return bSuccess;
        }
        private bool createPool()
        {
            reportStatus("Creating Pool '" + getPoolName() + "'... ");
            bool bSuccess = false;
            try
            {
                String pool_name = getPoolName();
                String[] pool_names = new String[] { pool_name };
                iControl.LocalLBLBMethod[] lb_methods = new iControl.LocalLBLBMethod[] { iControl.LocalLBLBMethod.LB_METHOD_LEAST_CONNECTION_MEMBER };

                int num_members = _pool_members.Length;

                iControl.CommonIPPortDefinition[][] members = new iControl.CommonIPPortDefinition[1][];
                members[0] = new iControl.CommonIPPortDefinition[num_members];
                for (int i = 0; i < num_members; i++)
                {
                    members[0][i] = new iControl.CommonIPPortDefinition();
                    String [] sSplit = _pool_members[i].Split(new char [] {':'});
                    members[0][i].address = sSplit[0];
                    members[0][i].port = Convert.ToInt32(sSplit[1]);
                }

                GetiControl().LocalLBPool.create(pool_names, lb_methods, members);

                reportStatus("SUCCESS", true);
                bSuccess = true;
            }
            catch (Exception ex)
            {
                reportStatus("FAILURE", true);
                reportStatus(ex.Message, true);
            }

            return bSuccess;
        }
        private bool attachHealthMonitor()
        {
            reportStatus("Attaching Health Monitor '" + getHealthMonitor() + "'... ");
            bool bSuccess = false;
            try
            {
                String template_name = getHealthMonitor();
                String pool_name = getPoolName();

                iControl.LocalLBPoolMonitorAssociation[] associations = new iControl.LocalLBPoolMonitorAssociation[1];
                associations[0] = new iControl.LocalLBPoolMonitorAssociation();
                associations[0].pool_name = pool_name;

                iControl.LocalLBMonitorRule monitor_rule = new iControl.LocalLBMonitorRule();
                monitor_rule.type = iControl.LocalLBMonitorRuleType.MONITOR_RULE_TYPE_SINGLE;
                monitor_rule.quorum = 0;
                monitor_rule.monitor_templates = new string[] { template_name };
                associations[0].monitor_rule = monitor_rule;

                GetiControl().LocalLBPool.set_monitor_association(associations);

                reportStatus("SUCCESS", true);
                bSuccess = true;
            }
            catch (Exception ex)
            {
                reportStatus("FAILURE", true);
                reportStatus(ex.Message);
            }

            return bSuccess;
        }
        private bool detachHealthMonitor()
        {
            bool bSuccess = true;
            return bSuccess;
        }
        private bool createHttpProfile()
        {
            reportStatus("Creating HTTP Profile '" + getHTTPProfile() + "'... ");
            bool bSuccess = false;
            try
            {
                String profile_name = getHTTPProfile();
                String[] profile_names = new String[] { profile_name };
                String[] default_profile_names = new String[] { "http-wan-optimized-compression-caching" };

                // Create Profile
                GetiControl().LocalLBProfileHttp.create(profile_names);

                iControl.LocalLBProfileHttpRedirectRewriteMode[] rewrite_modes = new iControl.LocalLBProfileHttpRedirectRewriteMode[1];
                rewrite_modes[0] = new iControl.LocalLBProfileHttpRedirectRewriteMode();
                rewrite_modes[0].default_flag = false;
                rewrite_modes[0].value = iControl.LocalLBHttpRedirectRewriteMode.HTTP_REDIRECT_REWRITE_MODE_MATCHING;
                GetiControl().LocalLBProfileHttp.set_redirect_rewrite_mode(profile_names, rewrite_modes);

                // IF ! Web Accellerator
                if (_compression)
                {
                    // set default (parent) profile
                    GetiControl().LocalLBProfileHttp.set_default_profile(profile_names, default_profile_names);

                    // set compression enabled
                    iControl.LocalLBProfileHttpCompressionMode[] modes = new iControl.LocalLBProfileHttpCompressionMode[1];
                    modes[0] = new iControl.LocalLBProfileHttpCompressionMode();
                    modes[0].default_flag = false;
                    modes[0].value = iControl.LocalLBHttpCompressionMode.HTTP_COMPRESSION_MODE_ENABLE;
                    GetiControl().LocalLBProfileHttp.set_compression_mode(profile_names, modes);

                    // Add content compression include list
                    String[][] comp_include_lists = new String[1][];
                    comp_include_lists[0] = new String[5];
                    comp_include_lists[0][0] = "application/pdf";
                    comp_include_lists[0][1] = "application/vnd.ms-powerpoint";
                    comp_include_lists[0][2] = "application/vnd.ms-excel";
                    comp_include_lists[0][3] = "application/msword";
                    comp_include_lists[0][4] = "application/vnd.ms-publisher";
                    GetiControl().LocalLBProfileHttp.add_compression_content_type_include(profile_names, comp_include_lists);

                    // Keep Accept Encoding
                    iControl.LocalLBProfileEnabledState[] enabled_states = new iControl.LocalLBProfileEnabledState[1];
                    enabled_states[0] = new iControl.LocalLBProfileEnabledState();
                    enabled_states[0].default_flag = false;
                    enabled_states[0].value = iControl.CommonEnabledState.STATE_ENABLED;
                    GetiControl().LocalLBProfileHttp.set_keep_accept_encoding_header_state(profile_names, enabled_states);

                    // RAM Cache URI Includes
                    String[][] cache_include_lists = new String[1][];
                    cache_include_lists[0] = new String[1];
                    cache_include_lists[0][0] = "/_layouts/images/*";
                    GetiControl().LocalLBProfileHttp.add_ramcache_uri_include(profile_names, cache_include_lists);
                }

                reportStatus("SUCCESS", true);
                bSuccess = true;
            }
            catch (Exception ex)
            {
                reportStatus("FAILURE", true);
                reportStatus(ex.Message, true);
            }

            return bSuccess;
        }
        private bool createTCPProfile()
        {
            reportStatus("Creating TCP Profile '" + getTCPProfile() + "'... ");
            String profile_name = getTCPProfile();
            String[] default_profile_names = new String[1];
            default_profile_names[0] = "tcp-lan-optimized";

            String[] profile_names = new String[] { profile_name };
            bool bSuccess = false;
            try
            {
                // create profile
                GetiControl().LocalLBProfileTCP.create(profile_names);

                // Set the default (parent) profile
                GetiControl().LocalLBProfileTCP.set_default_profile(profile_names, default_profile_names);

                reportStatus("SUCCESS", true);
                bSuccess = true;
            }
            catch (Exception ex)
            {
                reportStatus("FAILURE", true);
                reportStatus(ex.Message, true);
            }

            return bSuccess;
        }
        private bool createPersistenceProfile()
        {
            reportStatus("Creating Cookie Persistence Profile '" + getCookieProfile() + "'... ");
            String profile_name = getCookieProfile();
            String[] profile_names = new String[] { profile_name };
            bool bSuccess = false;
            try
            {
                iControl.LocalLBPersistenceMode[] modes = new iControl.LocalLBPersistenceMode[1];
                modes[0] = new iControl.LocalLBPersistenceMode();
                modes[0] = iControl.LocalLBPersistenceMode.PERSISTENCE_MODE_COOKIE;
                GetiControl().LocalLBProfilePersistence.create(profile_names, modes);

                reportStatus("SUCCESS", true);
                bSuccess = true;
            }
            catch (Exception ex)
            {
                reportStatus("FAILURE", true);
                reportStatus(ex.Message, true);
            }

            return bSuccess;
        }
        private bool createVirtualServer()
        {
            reportStatus("Creating Virtual Server '" + getVSName() + "'... ");
            String virtual_name = getVSName();
            String[] virtual_names = new String[] { virtual_name };
            String pool_name = getPoolName();

            String [] sVirtualDef = _virtual_server.Split(new char[] { ':' });

            bool bSuccess = false;
            try
            {
                // Create virtual server
                iControl.CommonVirtualServerDefinition[] vs_defs = new iControl.CommonVirtualServerDefinition[1];
                vs_defs[0] = new iControl.CommonVirtualServerDefinition();
                vs_defs[0].address = sVirtualDef[0];
                vs_defs[0].port = Convert.ToInt32(sVirtualDef[1]);
                vs_defs[0].name = virtual_name;
                vs_defs[0].protocol = iControl.CommonProtocolType.PROTOCOL_TCP;

                String[] wildmasks = new String[] { "255.255.255.255" };

                iControl.LocalLBVirtualServerVirtualServerResource[] resources = new iControl.LocalLBVirtualServerVirtualServerResource[1];
                resources[0] = new iControl.LocalLBVirtualServerVirtualServerResource();
                resources[0].default_pool_name = pool_name;
                resources[0].type = iControl.LocalLBVirtualServerVirtualServerType.RESOURCE_TYPE_POOL;

                iControl.LocalLBVirtualServerVirtualServerProfile[][] profiles = new iControl.LocalLBVirtualServerVirtualServerProfile[1][];
                profiles[0] = new iControl.LocalLBVirtualServerVirtualServerProfile[1];
                profiles[0][0] = new iControl.LocalLBVirtualServerVirtualServerProfile();
                profiles[0][0].profile_context = iControl.LocalLBProfileContextType.PROFILE_CONTEXT_TYPE_ALL;
                profiles[0][0].profile_name = getTCPProfile();

                GetiControl().LocalLBVirtualServer.create(vs_defs, wildmasks, resources, profiles);

                // Add HTTP Profile
                profiles = new iControl.LocalLBVirtualServerVirtualServerProfile[1][];
                profiles[0] = new iControl.LocalLBVirtualServerVirtualServerProfile[1];
                profiles[0][0] = new iControl.LocalLBVirtualServerVirtualServerProfile();
                profiles[0][0].profile_context = iControl.LocalLBProfileContextType.PROFILE_CONTEXT_TYPE_ALL;
                profiles[0][0].profile_name = getHTTPProfile();

                GetiControl().LocalLBVirtualServer.add_profile(virtual_names, profiles);

                // Add Persistence Profile
                iControl.LocalLBVirtualServerVirtualServerPersistence[][] per_profiles = new iControl.LocalLBVirtualServerVirtualServerPersistence[1][];
                per_profiles[0] = new iControl.LocalLBVirtualServerVirtualServerPersistence[1];
                per_profiles[0][0] = new iControl.LocalLBVirtualServerVirtualServerPersistence();
                per_profiles[0][0].default_profile = true;
                per_profiles[0][0].profile_name = getCookieProfile(); ;

                GetiControl().LocalLBVirtualServer.add_persistence_profile(virtual_names, per_profiles);

                reportStatus("SUCCESS", true);
                bSuccess = true;
            }
            catch (Exception ex)
            {
                reportStatus("FAILURE", true);
                reportStatus(ex.Message, true);
            }

            return bSuccess;
        }
        private bool createDefaultSNAT()
        {
            reportStatus("Creating Default SNAT'" + getDefaultSNATName() + "'... ");
            bool bSuccess = false;
            try
            {
                // create default SNAT
                iControl.LocalLBSNATSNATDefinition[] snat_def_list = new iControl.LocalLBSNATSNATDefinition[1];
                snat_def_list[0] = new iControl.LocalLBSNATSNATDefinition();
                snat_def_list[0].name = getDefaultSNATName();
                snat_def_list[0].target = new iControl.LocalLBSNATTranslation();
                snat_def_list[0].target.type = iControl.LocalLBSnatType.SNAT_TYPE_AUTOMAP;
                snat_def_list[0].target.translation_object = "";

                iControl.LocalLBSNATSNATOriginalAddress[][] orig_addr_list = new iControl.LocalLBSNATSNATOriginalAddress[1][];
                orig_addr_list[0] = new iControl.LocalLBSNATSNATOriginalAddress[1];
                orig_addr_list[0][0] = new iControl.LocalLBSNATSNATOriginalAddress();
                orig_addr_list[0][0].original_address = "0.0.0.0";
                orig_addr_list[0][0].wildmask = "0.0.0.0";

                iControl.CommonVLANFilterList[] vlan_filter_list = new iControl.CommonVLANFilterList[1];
                vlan_filter_list[0] = new iControl.CommonVLANFilterList();
                vlan_filter_list[0].state = iControl.CommonEnabledState.STATE_DISABLED;
                vlan_filter_list[0].vlans = new string[0];

                GetiControl().LocalLBSNAT.create(snat_def_list, orig_addr_list, vlan_filter_list);

                reportStatus("SUCCESS", true);
                bSuccess = true;
            }
            catch (Exception ex)
            {
                reportStatus("FAILURE", true);
                reportStatus(ex.Message, true);
            }

            return bSuccess;
        }
        private bool deleteDefaultSNAT()
        {
            bool bSuccess = true;
            return bSuccess;
        }

    }
}
