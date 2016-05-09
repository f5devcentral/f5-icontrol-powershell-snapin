using System;
using System.Collections.Generic;
using System.Text;
using System.Management.Automation;
using System.Collections;
using iControlSnapIn.Provider;

namespace iControlSnapIn.CmdLet.Deployment
{
    [Cmdlet(VerbsCommon.Remove, iControlNouns.SharePoint2007Deployment, SupportsShouldProcess=true)]
    public class RemoveSharePoint2007Deployment : DeploymentPSCmdLet
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
                    Remove();
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

        private bool Remove()
        {
            bool bSuccess = true;

            deleteVirtualServer(getVSName());
            deletePersistenceProfile(getCookieProfile());
            deleteTCPProfile(getTCPProfile());
            deleteHttpProfile(getHTTPProfile());
            deletePool(getPoolName());
            deleteHealthMonitor(getHealthMonitor());

            return bSuccess;
        }
    }
}
