using System;
using System.Collections.Generic;
using System.Text;

namespace iControlSnapIn.CmdLet.Deployment
{
    public class DeploymentPSCmdLet : iControlPSCmdlet
    {
        private String _status = "";

        public DeploymentPSCmdLet()
        {
        }

        protected void reportStatus(String status)
        {
            reportStatus(status, false);
        }
        protected void reportStatus(String status, bool bFlush)
        {
            _status = _status + status;
            if (bFlush)
            {
                WriteVerbose(_status);
                _status = "";
            }
        }

        protected bool deleteHealthMonitor(String name)
        {
            bool bSuccess = true;
            reportStatus("Removing Health Monitor " + name + "...");
            try
            {
                GetiControl().LocalLBMonitor.delete_template(new string[] { name });
                reportStatus("SUCCESS", true);
            }
            catch (Exception ex)
            {
                reportStatus("FAILURE", true);
                reportStatus(ex.Message, true);
            }
            return bSuccess;
        }
        protected bool deletePool(String name)
        {
            bool bSuccess = true;
            reportStatus("Removing Pool " + name + "...");
            try
            {
                GetiControl().LocalLBPool.delete_pool(new string[] { name });
                reportStatus("SUCCESS", true);
            }
            catch (Exception ex)
            {
                reportStatus("FAILURE", true);
                reportStatus(ex.Message, true);
            }
            return bSuccess;
        }
        protected bool deleteHttpProfile(String name)
        {
            bool bSuccess = true;
            reportStatus("Removing HTTP Profile " + name + "...");
            try
            {
                GetiControl().LocalLBProfileHttp.delete_profile(new string[] { name });
                reportStatus("SUCCESS", true);
            }
            catch (Exception ex)
            {
                reportStatus("FAILURE", true);
                reportStatus(ex.Message, true);
            }
            return bSuccess;
        }
        protected bool deleteTCPProfile(String name)
        {
            bool bSuccess = true;
            reportStatus("Removing TCP Profile " + name + "...");
            try
            {
                GetiControl().LocalLBProfileTCP.delete_profile(new string[] { name });
                reportStatus("SUCCESS", true);
            }
            catch (Exception ex)
            {
                reportStatus("FAILURE", true);
                reportStatus(ex.Message, true);
            }
            return bSuccess;
        }
        protected bool deletePersistenceProfile(String name)
        {
            bool bSuccess = true;
            reportStatus("Removing Cookie Profile " + name + "...");
            try
            {
                GetiControl().LocalLBProfilePersistence.delete_profile(new string[] { name });
                reportStatus("SUCCESS", true);
            }
            catch (Exception ex)
            {
                reportStatus("FAILURE", true);
                reportStatus(ex.Message, true);
            }
            return bSuccess;
        }
        protected bool deleteVirtualServer(String name)
        {
            bool bSuccess = true;
            reportStatus("Removing Virtual Server" + name + "...");
            try
            {
                GetiControl().LocalLBVirtualServer.delete_virtual_server(new string[] { name });
                reportStatus("SUCCESS", true);
            }
            catch (Exception ex)
            {
                reportStatus("FAILURE", true);
                reportStatus(ex.Message, true);
            }
            return bSuccess;
        }

        protected bool itemExistsInList(String item, String[] list)
        {
            bool bFound = false;
            for (int i = 0; i < list.Length; i++)
            {
                if (list[i].Equals(item))
                {
                    bFound = true;
                    break;
                }
            }
            return bFound;
        }
        protected bool healthMonitorExists(String name)
        {
            bool bFound = false;
            reportStatus("Checking for Health Monitor " + name + "...", true); 
            iControl.LocalLBMonitorMonitorTemplate[] templates = GetiControl().LocalLBMonitor.get_template_list();
            for (int i = 0; i < templates.Length; i++)
            {
                if (templates[i].template_name.Equals(name))
                {
                    bFound = true;
                    break;
                }
            }
            reportStatus(bFound ? "EXISTS" : "DOES NOT EXIST", true);

            return bFound;
        }
        protected bool poolExists(String name)
        {
            bool bFound = false;
            reportStatus("Checking for Pool " + name + "...");
            String[] pool_list = GetiControl().LocalLBPool.get_list();
            bFound = itemExistsInList(name, pool_list);
            reportStatus(bFound ? "EXISTS" : "DOES NOT EXIST", true);
            return bFound;
        }
        protected bool httpProfileExists(String name)
        {
            bool bFound = false;
            reportStatus("Checking for HTTP Profile " + name + "...");
            String[] profile_list = GetiControl().LocalLBProfileHttp.get_list();
            bFound = itemExistsInList(name, profile_list);
            reportStatus(bFound ? "EXISTS" : "DOES NOT EXIST", true);
            return bFound;
        }
        protected bool tcpProfileExists(String name)
        {
            bool bFound = false;
            reportStatus("Checking for TCP Profile " + name + "...");
            String[] profile_list = GetiControl().LocalLBProfileTCP.get_list();
            bFound = itemExistsInList(name, profile_list);
            reportStatus(bFound ? "EXISTS" : "DOES NOT EXIST", true);
            return bFound;
        }
        protected bool persistenceProfileExists(String name)
        {
            bool bFound = false;
            reportStatus("Checking for Cookie Persistence Profile " + name + "...");
            String[] profile_list = GetiControl().LocalLBProfilePersistence.get_list();
            bFound = itemExistsInList(name, profile_list);
            reportStatus(bFound ? "EXISTS" : "DOES NOT EXIST", true);
            return bFound;
        }
        protected bool virtualServerExists(String name)
        {
            bool bFound = false;
            reportStatus("Checking for Cookie Persistence Profile " + name + "...");
            String[] vs_list = GetiControl().LocalLBVirtualServer.get_list();
            bFound = itemExistsInList(name, vs_list);
            reportStatus(bFound ? "EXISTS" : "DOES NOT EXIST", true);
            return bFound;
        }

    }
}
