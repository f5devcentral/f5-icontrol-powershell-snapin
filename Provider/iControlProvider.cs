using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Management.Automation;
using System.Management.Automation.Provider;
using iControlSnapIn.TypeData;

namespace iControlSnapIn.Provider
{
    [CmdletProvider("iControlProvider", ProviderCapabilities.Credentials)]
    public class iControlProvider : NavigationCmdletProvider, IContentCmdletProvider
    {
        protected const string pathSeparator = @"\";

        #region CmdletProvider Overrides

        protected override ProviderInfo Start(ProviderInfo providerInfo)
        {
            return providerInfo;
        } // Start
        protected override void Stop()
        {
            base.Stop();
        }

        #endregion

        #region DriveCmdletProvider Overrides

        protected override PSDriveInfo NewDrive(PSDriveInfo drive)
        {
            iControlPSDriveInfo _driveInfo = new iControlPSDriveInfo(drive);
            int port = 443;

            RuntimeDefinedParameterDictionary dic = DynamicParameters as RuntimeDefinedParameterDictionary;
            if ((null != dic) && dic.ContainsKey("Port"))
            {
                RuntimeDefinedParameter rdp = dic["Port"];
                port = Convert.ToInt32(rdp.Value);
            }
            else if ( (443 == port) && drive.Root.Equals("localhost") )
            {
                port = 8080;
            }

            _driveInfo.Interfaces.initialize(drive.Root, port,
                drive.Credential.UserName.Replace("\\", ""), 
                drive.Credential.GetNetworkCredential().Password);

            string version = _driveInfo.Interfaces.SystemSystemInfo.get_version();

            return _driveInfo;
        }

        protected override object NewDriveDynamicParameters()
        {
            ParameterAttribute portAttrib = new ParameterAttribute();
            
            portAttrib.Mandatory = false;
            portAttrib.ParameterSetName = null;

            RuntimeDefinedParameter portParam = new RuntimeDefinedParameter();
            //portParam.IsSet = false;
            portParam.Name = "Port";
            portParam.IsSet = false;
            portParam.Value = 443;
            portParam.ParameterType = typeof(long);
            portParam.Attributes.Add(portAttrib);
            RuntimeDefinedParameterDictionary dic = new RuntimeDefinedParameterDictionary();
            dic.Add("Port", portParam);

            return dic;
        }

        protected override PSDriveInfo RemoveDrive(PSDriveInfo drive)
        {
            iControlPSDriveInfo _driveInfo = drive as iControlPSDriveInfo;

            if (null == _driveInfo)
            {
                return null;
            }

            //TODO

            return _driveInfo;
        }

        protected override Collection<PSDriveInfo> InitializeDefaultDrives()
        {
            Collection<PSDriveInfo> drives = new Collection<PSDriveInfo>();
            bool bAutoDrive = false;

            if (bAutoDrive)
            {
                System.Security.SecureString pwd = new System.Security.SecureString();
                pwd.AppendChar('a');
                pwd.AppendChar('d');
                pwd.AppendChar('m');
                pwd.AppendChar('i');
                pwd.AppendChar('n');
                PSCredential psc = new PSCredential("admin", pwd);
                PSDriveInfo psdi = new PSDriveInfo("localhost", this.ProviderInfo, "localhost", "iControl localhost proxy", psc);

                drives.Add(psdi);
            }
            return drives;
        }// InitializeDefaultDrives

        #endregion

        #region ItemCmdletProvider Overrides

        protected override bool IsValidPath(string path)
        {
            string pathNoDrive = RemoveDrive(path);
            iControlPSDriveInfo di = this.PSDriveInfo as iControlPSDriveInfo;
            
            return true;
        }

        protected override bool ItemExists(string path)
        {
            bool bExists = false;
            WriteVerbose("ItemExists: " + path);

            PathInfo pi = new PathInfo(path);

            if (pi.IsDrive)
            {
                bExists = true;
            }
            else if (pi.IsCategory)
            {
                bExists = true;
                switch (pi.Category)
                {
                    case OBJECT_CATEGORY.GTM:
                        break;
                    case OBJECT_CATEGORY.LTM:
                        break;
                    case OBJECT_CATEGORY.MANAGEMENT:
                        break;
                    case OBJECT_CATEGORY.NETWORK:
                        break;
                    case OBJECT_CATEGORY.SYSTEM:
                        break;
                    default:
                        bExists = false;
                        break;
                }
            }
            else if (pi.IsType)
            {
                bExists = true;
                switch (pi.Type)
                {
                    case OBJECT_TYPE.GTM_WIDEIP:
                        break;
                    case OBJECT_TYPE.LTM_NODE:
                        break;
                    case OBJECT_TYPE.LTM_POOL:
                        break;
                    case OBJECT_TYPE.LTM_RULE:
                        break;
                    case OBJECT_TYPE.LTM_VIRTUAL_SERVER:
                        break;
                    case OBJECT_TYPE.MANAGEMENT_USERS:
                        break;
                    case OBJECT_TYPE.MANAGEMENT_DATABASE:
                        break;
                    case OBJECT_TYPE.NETWORK_INTERFACE:
                        break;
                    case OBJECT_TYPE.NETWORK_VLAN:
                        break;
                    case OBJECT_TYPE.SYSTEM_SERVICES:
                        break;
                    case OBJECT_TYPE.SYSTEM_SYSTEMINFO:
                        break;
                    default:
                        bExists = false;
                        break;
                }
            }
            else if (pi.IsItem)
            {
                switch (pi.Type)
                {
                    case OBJECT_TYPE.GTM_WIDEIP:
                        break;

                    case OBJECT_TYPE.LTM_NODE:
                        bExists = LTMNodeExists(pi.Item);
                        break;
                    case OBJECT_TYPE.LTM_POOL:
                        bExists = LTMPoolExists(pi.Item);
                        break;
                    case OBJECT_TYPE.LTM_RULE:
                        bExists = LTMRuleExists(pi.Item);
                        break;
                    case OBJECT_TYPE.LTM_VIRTUAL_SERVER:
                        bExists = LTMVirtualServerExists(pi.Item);
                        break;

                    case OBJECT_TYPE.MANAGEMENT_DATABASE:
                        bExists = ManagementDatabaseExists(pi.Item);
                        break;
                    case OBJECT_TYPE.MANAGEMENT_USERS:
                        bExists = ManagementUserExists(pi.Item);
                        break;

                    case OBJECT_TYPE.NETWORK_INTERFACE:
                        bExists = NetworkIntefaceExists(pi.Item);
                        break;
                    case OBJECT_TYPE.NETWORK_VLAN:
                        bExists = NetworkVLANExists(pi.Item);
                        break;

                    case OBJECT_TYPE.SYSTEM_SERVICES:
                        bExists = SystemServiceExists(pi.Item);
                        break;
                    case OBJECT_TYPE.SYSTEM_SYSTEMINFO:
                        if (pi.Item.Equals("CPU") ||
                            pi.Item.Equals("Disk") ||
                            pi.Item.Equals("Fan") ||
                            pi.Item.Equals("Memory") ||
                            pi.Item.Equals("Power"))
                        {
                            bExists = true;
                        }
                        break;
                    default:
                        bExists = false;
                        break;
                }
            }
            else if (pi.IsSubItem)
            {
                switch (pi.Type)
                {
                    case OBJECT_TYPE.LTM_POOL:
                        bExists = PoolMemberExists(pi.Item, pi.SubItem);
                        break;
                    case OBJECT_TYPE.LTM_RULE:
                        bExists = false;
                        break;
                    case OBJECT_TYPE.LTM_VIRTUAL_SERVER:
                        bExists = false;
                        break;
                    default:
                        bExists = false;
                        break;
                }
            }


            return bExists;
        }

        protected override void GetItem(string path)
        {
            PathInfo pi = new PathInfo(path);

            if (pi.IsDrive)
            {
                // nada
            }
            else
            {
                switch (pi.Category)
                {
                    case OBJECT_CATEGORY.GTM:
                        handleGTM(pi, false, false);
                        break;
                    case OBJECT_CATEGORY.LTM:
                        handleLTM(pi, false, false);
                        break;
                    case OBJECT_CATEGORY.MANAGEMENT:
                        handleManagement(pi, false, false);
                        break;
                    case OBJECT_CATEGORY.NETWORK:
                        handleNetworking(pi, false, false);
                        break;
                    case OBJECT_CATEGORY.SYSTEM:
                        handleSystem(pi, false, false);
                        break;
                }
            }

            //else  if (pi.IsCategory)
            //{
            //    // nada
            //}
            //else if (pi.IsType)
            //{
            //    // nada
            //}
            //else if (pi.IsItem)
            //{
            //    switch (pi.Type)
            //    {
            //        case OBJECT_TYPE.LTM_POOL:
            //            handleLTMPools(pi, false, false);
            //            break;
            //        case OBJECT_TYPE.LTM_RULE:
            //            handleLTMRules(pi, false, false);
            //            break;
            //        case OBJECT_TYPE.LTM_VIRTUAL_SERVER:
            //            handleLTMVirtuals(pi, false, false);
            //            break;
            //        case OBJECT_TYPE.MANAGEMENT_USERS:
            //            handleManagementUsers(pi, false, false);
            //        default:
            //            break;
            //    }
            //}
            //else if (pi.IsSubItem)
            //{
            //    switch (pi.Type)
            //    {
            //        case OBJECT_TYPE.LTM_POOL:
            //            handleLTMPools(pi, false, false);
            //            break;
            //        case OBJECT_TYPE.LTM_RULE:
            //            break;
            //        case OBJECT_TYPE.LTM_VIRTUAL_SERVER:
            //            break;
            //        default:
            //            break;
            //    }
            //}

        }

        #endregion

        #region ContainerCmdletProvider Overrides

        protected override void GetChildItems(string path, bool recurse)
        {
            PathInfo pi = new PathInfo(path);
            processPath(pi, false, recurse);
        }

        protected override void GetChildNames(string path, ReturnContainers returnContainers)
        {
            PathInfo pi = new PathInfo(path);
            processPath(pi, true, false);
        }

        protected override bool HasChildItems(string path)
        {
            if (PathIsDrive(path))
            {
                return true;
            }
            string pathNoDrive = RemoveDrive(path);

            return true;
        }

        #endregion

        #region NavigationCmdletProvider Overrides

        protected override bool IsItemContainer(string path)
        {
            PathInfo pi = new PathInfo(path);
            bool bIsContainer = false;
            if (pi.IsDrive)
            {
                bIsContainer = true;
            }
            else if ( pi.IsCategory )
            {
                bIsContainer = true;
            }
            else if (pi.IsType)
            {
                bIsContainer = true;
            }
            else if (pi.IsItem)
            {
                bIsContainer = false;
                switch (pi.Type)
                {
                    case OBJECT_TYPE.GTM_WIDEIP:
                        break;
                    case OBJECT_TYPE.LTM_NODE:
                        break;
                    case OBJECT_TYPE.LTM_POOL:
                        bIsContainer = true;
                        break;
                    case OBJECT_TYPE.LTM_RULE:
                        break;
                    case OBJECT_TYPE.LTM_VIRTUAL_SERVER:
                        break;
                    case OBJECT_TYPE.MANAGEMENT_DATABASE:
                        break;
                    case OBJECT_TYPE.MANAGEMENT_USERS:
                        break;
                    case OBJECT_TYPE.NETWORK_INTERFACE:
                        break;
                    case OBJECT_TYPE.NETWORK_VLAN:
                        break;
                    case OBJECT_TYPE.SYSTEM_SERVICES:
                        break;
                    case OBJECT_TYPE.SYSTEM_SYSTEMINFO:
                        bIsContainer = true;
                        break;
                    default:
                        bIsContainer = false;
                        break;
                }
            }
            else if (pi.IsSubItem)
            {
                bIsContainer = false;
                switch (pi.Type)
                {
                    case OBJECT_TYPE.LTM_POOL:
                        bIsContainer = false;
                        break;
                    case OBJECT_TYPE.LTM_RULE:
                        bIsContainer = false;
                        break;
                    case OBJECT_TYPE.LTM_VIRTUAL_SERVER:
                        bIsContainer = false;
                        break;
                    default:
                        bIsContainer = false;
                        break;
                }
            }
            return bIsContainer;
        }

        #endregion

        #region Component Handlers

        private void WriteDirectoryInfo(PathInfo pi, string dirName)
        {
            //System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(pi.Path + "\\" + dirName);
            //System.DateTime nowUTC = DateTime.Now;
            //System.DateTime now = nowUTC.ToLocalTime();
            //di.CreationTime = now;
            //di.CreationTimeUtc = nowUTC;
            //di.LastAccessTime = now;
            //di.LastAccessTimeUtc = nowUTC;
            //di.LastWriteTime = now;
            //di.LastWriteTimeUtc = nowUTC;
            //di.Attributes = System.IO.FileAttributes.Directory;

            //WriteItemObject(di, pi.Path, true);
            WriteItemObject(dirName, pi.Path, true);
        }

        private void WriteFileInfo(PathInfo pi, string fileName)
        {
            WriteItemObject(fileName, pi.Path, false);
        }

        private void processPath(string path, bool bJustNames, bool recurse)
        {
            PathInfo pi = new PathInfo(path);
            processPath(pi, bJustNames, recurse);
        }

        private void processPath(PathInfo pi, bool bJustName, bool recurse)
        {
            switch (pi.Category)
            {
                case OBJECT_CATEGORY.GTM:
                    handleGTM(pi, bJustName, recurse);
                    break;
                case OBJECT_CATEGORY.LTM:
                    handleLTM(pi, bJustName, recurse);
                    break;
                case OBJECT_CATEGORY.MANAGEMENT:
                    handleManagement(pi, bJustName, recurse);
                    break;
                case OBJECT_CATEGORY.NETWORK:
                    handleNetworking(pi, bJustName, recurse);
                    break;
                case OBJECT_CATEGORY.SYSTEM:
                    handleSystem(pi, bJustName, recurse);
                    break;
                default:
                    if (recurse)
                    {
                        PathInfo pi2 = new PathInfo(pi.Path + "\\GTM");
                        handleGTM(pi2, bJustName, recurse);
                        pi2.ParsePath(pi.Path + "\\LTM");
                        handleLTM(pi2, bJustName, recurse);
                        pi2.ParsePath(pi.Path + "\\Management");
                        handleManagement(pi2, bJustName, recurse);
                        pi2.ParsePath(pi.Path + "\\Networking");
                        handleNetworking(pi2, bJustName, recurse);
                        pi2.ParsePath(pi.Path + "\\System");
                        handleSystem(pi2, bJustName, recurse);
                    }
                    else
                    {
                        WriteDirectoryInfo(pi, "GTM");
                        WriteDirectoryInfo(pi, "LTM");
                        WriteDirectoryInfo(pi, "Management");
                        WriteDirectoryInfo(pi, "Networking");
                        WriteDirectoryInfo(pi, "System");
                    }
                    break;
            }
        }

        private void processPathEx(PathInfo pi, bool bJustNames, bool recurse)
        {
            if (pi.IsDrive)
            {
                WriteDirectoryInfo(pi, "GTM");
                WriteDirectoryInfo(pi, "LTM");
                WriteDirectoryInfo(pi, "Management");
                WriteDirectoryInfo(pi, "Networking");
                WriteDirectoryInfo(pi, "System");
            }
            else if (pi.IsCategory)
            {
                switch (pi.Category)
                {
                    case OBJECT_CATEGORY.GTM:
                        WriteDirectoryInfo(pi, "WideIPs");
                        break;
                    case OBJECT_CATEGORY.LTM:
                        WriteDirectoryInfo(pi, "Nodes");
                        WriteDirectoryInfo(pi, "Pools");
                        WriteDirectoryInfo(pi, "Rules");
                        WriteDirectoryInfo(pi, "VirtualServers");
                        break;
                    case OBJECT_CATEGORY.MANAGEMENT:
                        WriteDirectoryInfo(pi, "Database");
                        WriteDirectoryInfo(pi, "Users");
                        break;
                    case OBJECT_CATEGORY.NETWORK:
                        WriteDirectoryInfo(pi, "Interfaces");
                        WriteDirectoryInfo(pi, "VLANs");
                        break;
                    case OBJECT_CATEGORY.SYSTEM:
                        WriteDirectoryInfo(pi, "Services");
                        WriteDirectoryInfo(pi, "SystemInfo");
                        break;
                }
            }
            else if (pi.IsType)
            {
                switch (pi.Category)
                {
                    case OBJECT_CATEGORY.GTM:
                        switch (pi.Type)
                        {
                            case OBJECT_TYPE.GTM_WIDEIP:
                                break;
                        }
                        break;

                    case OBJECT_CATEGORY.LTM:
                        switch (pi.Type)
                        {
                            case OBJECT_TYPE.LTM_NODE:
                                handleLTMNodes(pi, bJustNames, recurse);
                                break;
                            case OBJECT_TYPE.LTM_POOL:
                                handleLTMPools(pi, bJustNames, recurse);
                                break;
                            case OBJECT_TYPE.LTM_RULE:
                                handleLTMRules(pi, bJustNames, recurse);
                                break;
                            case OBJECT_TYPE.LTM_VIRTUAL_SERVER:
                                handleLTMVirtuals(pi, bJustNames, recurse);
                                break;
                        }
                        break;

                    case OBJECT_CATEGORY.MANAGEMENT:
                        switch (pi.Type)
                        {
                            case OBJECT_TYPE.MANAGEMENT_DATABASE:
                                handleManagementDatabase(pi, bJustNames, recurse);
                                break;
                            case OBJECT_TYPE.MANAGEMENT_USERS:
                                handleManagementUsers(pi, bJustNames, recurse);
                                break;
                        }
                        break;

                    case OBJECT_CATEGORY.NETWORK:
                        switch (pi.Type)
                        {
                            case OBJECT_TYPE.NETWORK_INTERFACE:
                                handleNetworkInterface(pi, bJustNames, recurse);
                                break;
                            case OBJECT_TYPE.NETWORK_VLAN:
                                handleNetworkVLANs(pi, bJustNames, recurse);
                                break;
                        }
                        break;

                    case OBJECT_CATEGORY.SYSTEM:
                        switch (pi.Type)
                        {
                            case OBJECT_TYPE.SYSTEM_SYSTEMINFO:
                                WriteDirectoryInfo(pi, "CPU");
                                WriteDirectoryInfo(pi, "Disk");
                                WriteDirectoryInfo(pi, "Fan");
                                WriteDirectoryInfo(pi, "Memory");
                                WriteDirectoryInfo(pi, "Power");
                                break;
                            case OBJECT_TYPE.SYSTEM_SERVICES:
                                break;
                        }
                        break;
                }
            }
            else if (pi.IsItem)
            {
                switch (pi.Category)
                {
                    case OBJECT_CATEGORY.GTM:
                        switch (pi.Type)
                        {
                            case OBJECT_TYPE.GTM_WIDEIP:
                                break;
                        }
                        break;

                    case OBJECT_CATEGORY.LTM:
                        switch (pi.Type)
                        {
                            case OBJECT_TYPE.LTM_NODE:
                                handleLTMNodes(pi, bJustNames, recurse);
                                break;
                            case OBJECT_TYPE.LTM_POOL:
                                handleLTMPools(pi, bJustNames, recurse);
                                break;
                            case OBJECT_TYPE.LTM_RULE:
                                handleLTMRules(pi, bJustNames, recurse);
                                break;
                            case OBJECT_TYPE.LTM_VIRTUAL_SERVER:
                                handleLTMVirtuals(pi, bJustNames, recurse);
                                break;
                        }
                        break;

                    case OBJECT_CATEGORY.MANAGEMENT:
                        switch (pi.Type)
                        {
                            case OBJECT_TYPE.MANAGEMENT_DATABASE:
                                handleManagementDatabase(pi, bJustNames, recurse);
                                break;
                            case OBJECT_TYPE.MANAGEMENT_USERS:
                                handleManagementUsers(pi, bJustNames, recurse);
                                break;
                        }
                        break;

                    case OBJECT_CATEGORY.NETWORK:
                        switch (pi.Type)
                        {
                            case OBJECT_TYPE.NETWORK_INTERFACE:
                                handleNetworkInterface(pi, bJustNames, recurse);
                                break;
                            case OBJECT_TYPE.NETWORK_VLAN:
                                handleNetworkVLANs(pi, bJustNames, recurse);
                                break;
                        }
                        break;

                    case OBJECT_CATEGORY.SYSTEM:
                        switch (pi.Type)
                        {
                            case OBJECT_TYPE.SYSTEM_SYSTEMINFO:
                                WriteDirectoryInfo(pi, "CPU");
                                WriteDirectoryInfo(pi, "Disk");
                                WriteDirectoryInfo(pi, "Fan");
                                WriteDirectoryInfo(pi, "Memory");
                                WriteDirectoryInfo(pi, "Power");
                                break;
                            case OBJECT_TYPE.SYSTEM_SERVICES:
                                break;
                        }
                        break;
                }

            }
            else if (pi.IsSubItem)
            {
                switch (pi.Type)
                {
                    case OBJECT_TYPE.LTM_POOL:
                        handleLTMPools(pi, bJustNames, recurse);
                        break;
                    case OBJECT_TYPE.LTM_RULE:
                        break;
                    case OBJECT_TYPE.LTM_VIRTUAL_SERVER:
                        break;
                }
            }
        }

        private void handleGTM(PathInfo pi, bool bJustNames, bool recurse)
        {
            if (OBJECT_CATEGORY.GTM == pi.Category)
            {
                switch (pi.Type)
                {
                    case OBJECT_TYPE.GTM_WIDEIP:
                        handleGTMWideIPs(pi, bJustNames, recurse);
                        break;
                    default:
                        if (recurse)
                        {
                            processPath(pi.Path + "\\WideIPs", bJustNames, recurse);
                        }
                        else
                        {
                            WriteDirectoryInfo(pi, "WideIPs");
                        }
                        break;
                }
            }
        }

        private void handleLTM(PathInfo pi, bool bJustNames, bool recurse)
        {
            if (OBJECT_CATEGORY.LTM == pi.Category)
            {
                switch (pi.Type)
                {
                    case OBJECT_TYPE.LTM_NODE:
                        handleLTMNodes(pi, bJustNames, recurse);
                        break;
                    case OBJECT_TYPE.LTM_POOL:
                        handleLTMPools(pi, bJustNames, recurse);
                        break;
                    case OBJECT_TYPE.LTM_RULE:
                        handleLTMRules(pi, bJustNames, recurse);
                        break;
                    case OBJECT_TYPE.LTM_VIRTUAL_SERVER:
                        handleLTMVirtuals(pi, bJustNames, recurse);
                        break;
                    default:
                        if (recurse)
                        {
                            processPath(pi.Path + "\\Nodes", bJustNames, recurse);
                            processPath(pi.Path + "\\Pools", bJustNames, recurse);
                            processPath(pi.Path + "\\Rules", bJustNames, recurse);
                            processPath(pi.Path + "\\VirtualServers", bJustNames, recurse);
                        }
                        else
                        {
                            WriteDirectoryInfo(pi, "Nodes");
                            WriteDirectoryInfo(pi, "Pools");
                            WriteDirectoryInfo(pi, "Rules");
                            WriteDirectoryInfo(pi, "VirtualServers");
                        }
                        break;
                }
            }
        }

        private void handleManagement(PathInfo pi, bool bJustNames, bool recurse)
        {
            if (OBJECT_CATEGORY.MANAGEMENT == pi.Category)
            {
                switch (pi.Type)
                {
                    case OBJECT_TYPE.MANAGEMENT_DATABASE:
                        handleManagementDatabase(pi, bJustNames, recurse);
                        break;
                    case OBJECT_TYPE.MANAGEMENT_USERS:
                        handleManagementUsers(pi, bJustNames, recurse);
                        break;
                    default:
                        if (recurse)
                        {
                            processPath(pi.Path + "\\Database", bJustNames ,recurse);
                            processPath(pi.Path + "\\Users", bJustNames, recurse);
                        }
                        else
                        {
                            WriteDirectoryInfo(pi, "Database");
                            WriteDirectoryInfo(pi, "Users");
                        }
                        break;
                }
            }
        }

        private void handleNetworking(PathInfo pi, bool bJustNames, bool recurse)
        {
            if (OBJECT_CATEGORY.NETWORK == pi.Category)
            {
                switch (pi.Type)
                {
                    case OBJECT_TYPE.NETWORK_INTERFACE:
                        handleNetworkInterface(pi, bJustNames, recurse);
                        break;
                    case OBJECT_TYPE.NETWORK_VLAN:
                        handleNetworkVLANs(pi, bJustNames, recurse);
                        break;
                    default:
                        if (recurse)
                        {
                            processPath(pi.Path + "\\Interfaces", bJustNames, recurse);
                            processPath(pi.Path + "\\VLANs", bJustNames, recurse);
                        }
                        else
                        {
                            WriteDirectoryInfo(pi, "Interfaces");
                            WriteDirectoryInfo(pi, "VLANs");
                        }
                        break;
                }
            }
        }

        private void handleSystem(PathInfo pi, bool bJustNames, bool recurse)
        {
            if (OBJECT_CATEGORY.SYSTEM == pi.Category)
            {
                switch (pi.Type)
                {
                    case OBJECT_TYPE.SYSTEM_SERVICES:
                        handleSystemServices(pi, bJustNames, recurse);
                        break;
                    case OBJECT_TYPE.SYSTEM_SYSTEMINFO:
                        handleSystemSystemInfo(pi, bJustNames, recurse);
                        break;
                    default:
                        if (recurse)
                        {
                            processPath(pi.Path + "\\Services", bJustNames, recurse);
                            processPath(pi.Path + "\\SystemInfo", bJustNames, recurse);
                        }
                        else
                        {
                            WriteDirectoryInfo(pi, "Services");
                            WriteDirectoryInfo(pi, "SystemInfo");
                        }
                        break;
                }
            }
        }

        private void handleGTMWideIPs(PathInfo pi, bool bJustNames, bool recurse)
        {
            iControlPSDriveInfo di = this.PSDriveInfo as iControlPSDriveInfo;
            if (pi.IsType || pi.IsItem)
            {
                string [] wideip_list = di.Interfaces.GlobalLBWideIP.get_list();
                if (bJustNames)
                {
                    foreach (string wideip in wideip_list)
                    {
                        WriteItemObject(wideip, pi.Path, false);
                    }
                }
                else
                {
                    iControl.GlobalLBLBMethod [] lb_methods = di.Interfaces.GlobalLBWideIP.get_lb_method(wideip_list);
                    iControl.CommonObjectStatus [] object_statuses = di.Interfaces.GlobalLBWideIP.get_object_status(wideip_list);
                    string [] app_list = di.Interfaces.GlobalLBWideIP.get_application(wideip_list);
                    for (int i = 0; i < wideip_list.Length; i++)
                    {
                        bool bProcess = true;
                        if (pi.IsItem)
                        {
                            bProcess = false;
                            if (pi.Item.Equals(wideip_list[i]))
                            {
                                bProcess = true;
                            }
                        }

                        if ( bProcess )
                        {
                            GTMWideIPItem gwi = new GTMWideIPItem();
                            gwi.Name = wideip_list[i];
                            gwi.Application = app_list[i];
                            gwi.Availability = object_statuses[i].availability_status;
                            gwi.Enabled = object_statuses[i].enabled_status;
                            gwi.Status = object_statuses[i].status_description;

                            WriteItemObject(gwi, pi.Path, false);
                        }
                    }
                }
            }

        }

        private void handleLTMNodes(PathInfo pi, bool bJustNames, bool recurse)
        {
            iControlPSDriveInfo di = this.PSDriveInfo as iControlPSDriveInfo;
            if (pi.IsType || pi.IsItem)
            {
                string[] node_list = di.Interfaces.LocalLBNodeAddress.get_list();
                if (bJustNames)
                {
                    foreach (string vs in node_list)
                    {
                        WriteItemObject(vs, pi.Path, false);
                    }
                }
                else
                {
                    iControl.LocalLBObjectStatus []  object_statuses = di.Interfaces.LocalLBNodeAddress.get_object_status(node_list);
                    for (int i = 0; i < object_statuses.Length; i++)
                    {
                        bool bProcess = true;
                        if (pi.IsItem)
                        {
                            bProcess = false;
                            if (pi.Item.Equals(node_list[i]))
                            {
                                bProcess = true;
                            }
                        }
                        if (bProcess)
                        {
                            LTMStatusItem si = new LTMStatusItem();
                            si.Name = node_list[i];
                            si.Availability = object_statuses[i].availability_status;
                            si.Enabled = object_statuses[i].enabled_status;
                            si.Status = object_statuses[i].status_description;

                            WriteItemObject(si, pi.Path, false);
                        }
                    }
                }
            }
        }

        private void handleLTMPools(PathInfo pi, bool bJustNames, bool recurse)
        {
            iControlPSDriveInfo di = this.PSDriveInfo as iControlPSDriveInfo;
            if (pi.IsType || pi.IsItem)
            {
                // No items given so return top level list
                string[] pool_list = di.Interfaces.LocalLBPool.get_list();
                iControl.LocalLBObjectStatus[] object_statuses = di.Interfaces.LocalLBPool.get_object_status(pool_list);
                for (int i = 0; i < pool_list.Length; i++)
                {
                    String pool = pool_list[i];
                    iControl.CommonIPPortDefinition[][] member_list = di.Interfaces.LocalLBPool.get_member(new string[] { pool });

                    if (bJustNames)
                    {
                        WriteItemObject(pool, pathSeparator, member_list[0].Length > 0);
                    }
                    else
                    {
                        LTMPoolItem ltmpi = new LTMPoolItem();
                        ltmpi.Name = pool;
                        ltmpi.MemberCount = member_list[0].Length;
                        ltmpi.Availability = object_statuses[i].availability_status;
                        ltmpi.Enabled = object_statuses[i].enabled_status;
                        ltmpi.Status = object_statuses[i].status_description;
                        WriteItemObject(ltmpi, pi.Path, member_list[0].Length > 0);
                    }
                }
            }
            else if (pi.IsItem)
            {
                // pool name supplied, return a list of members
                String pool_name = pi.Item;
                iControl.CommonIPPortDefinition[][] member_list = di.Interfaces.LocalLBPool.get_member(new string[] { pool_name });
                iControl.LocalLBPoolMemberMemberObjectStatus[][] object_statuses = di.Interfaces.LocalLBPoolMember.get_object_status(new string[] { pool_name });
                //                foreach (iControl.CommonIPPortDefinition member in member_list[0])
                for (int i = 0; i < object_statuses[0].Length; i++)
                {
                    iControl.LocalLBPoolMemberMemberObjectStatus object_status = object_statuses[0][i];
                    iControl.CommonIPPortDefinition member = object_status.member;

                    bool bProcess = true;
                    if (pi.IsSubItem)
                    {
                        bProcess = false;
                        if (pi.SubItem.Equals(member.address + ":" + member.port.ToString()))
                        {
                            bProcess = true;
                        }
                    }

                    if (bProcess)
                    {
                        if (bJustNames)
                        {
                            WriteItemObject(member.address + ":" + member.port.ToString(), pi.Path, false);
                        }
                        else
                        {
                            LTMPoolMemberItem mi = new LTMPoolMemberItem();
                            mi.Name = member.address + ":" + member.port.ToString();
                            mi.Address = member.address;
                            mi.Port = member.port;
                            mi.Availability = object_status.object_status.availability_status;
                            mi.Enabled = object_status.object_status.enabled_status;
                            mi.Status = object_status.object_status.status_description;

                            WriteItemObject(mi, pi.Path, false);
                        }
                    }
                }
            }
        }

        private void handleLTMRules(PathInfo pi, bool bJustNames, bool recurse)
        {
            iControlPSDriveInfo di = this.PSDriveInfo as iControlPSDriveInfo;
            if (pi.IsType || pi.IsItem)
            {
                // list all the rules...
                iControl.LocalLBRuleRuleDefinition[] rule_list = di.Interfaces.LocalLBRule.query_all_rules();
                foreach (iControl.LocalLBRuleRuleDefinition rule in rule_list)
                {
                    bool bProcess = true;
                    if (pi.IsItem)
                    {
                        bProcess = false;
                        if (pi.Item.Equals(rule.rule_name))
                        {
                            bProcess = true;
                        }
                    }
                    if (bProcess)
                    {
                        if (bJustNames)
                        {
                            WriteItemObject(rule.rule_name, pi.Path, false);
                        }
                        else
                        {
                            LTMRuleDefinitionItem rdi = new LTMRuleDefinitionItem();
                            rdi.Name = rule.rule_name;
                            rdi.Definition = rule.rule_definition;
                            WriteItemObject(rdi, pi.Path, false);
                        }
                    }
                }
            }
        }

        private void handleLTMVirtuals(PathInfo pi, bool bJustNames, bool recurse)
        {
            iControlPSDriveInfo di = this.PSDriveInfo as iControlPSDriveInfo;
            if (pi.IsType || pi.IsItem)
            {
                // list all the virtuals
                string [] vs_list = di.Interfaces.LocalLBVirtualServer.get_list();
                if ( bJustNames )
                {
                    foreach (string vs in vs_list)
                    {
                        WriteItemObject(vs, pi.Path, false);
                    }
                }
                else
                {
                    iControl.LocalLBObjectStatus [] object_statuses = di.Interfaces.LocalLBVirtualServer.get_object_status(vs_list);
                    for(int i=0; i<object_statuses.Length; i++)
                    {
                        bool bProcess = true;
                        if (pi.IsItem)
                        {
                            bProcess = false;
                            if ( pi.Item.Equals(vs_list[i])) 
                            {
                                bProcess = true;
                            }
                        }
                        if (bProcess)
                        {
                            LTMStatusItem si = new LTMStatusItem();
                            si.Name = vs_list[i];
                            si.Availability = object_statuses[i].availability_status;
                            si.Enabled = object_statuses[i].enabled_status;
                            si.Status = object_statuses[i].status_description;

                            WriteItemObject(si, pi.Path, false);
                        }
                    }
                }
            }
        }

        private void handleManagementDatabase(PathInfo pi, bool bJustNames, bool recurse)
        {
            iControlPSDriveInfo di = this.PSDriveInfo as iControlPSDriveInfo;
            if (pi.IsType | pi.IsItem)
            {
                iControl.ManagementDBVariableVariableNameValue [] name_vals = di.Interfaces.ManagementDBVariable.get_list();
                foreach (iControl.ManagementDBVariableVariableNameValue name_val in name_vals)
                {
                    if (bJustNames)
                    {
                        WriteItemObject(name_val.name, pi.Path, false);
                    }
                    else
                    {
                        bool bProcess = true;
                        if (pi.IsItem)
                        {
                            bProcess = false;
                            if (pi.Item.Equals(name_val.name))
                            {
                                bProcess = true;
                            }
                        }
                        if (bProcess)
                        {
                            ManagementDatabaseItem mdi = new ManagementDatabaseItem();
                            mdi.Name = name_val.name;
                            mdi.Value = name_val.value;
                            WriteItemObject(mdi, pi.Path, false);
                        }
                    }
                }
            }
        }

        private void handleManagementUsers(PathInfo pi, bool bJustNames, bool recurse)
        {
            iControlPSDriveInfo di = this.PSDriveInfo as iControlPSDriveInfo;
            if (pi.IsType | pi.IsItem)
            {
                iControl.ManagementUserManagementUserID [] user_list = di.Interfaces.ManagementUserManagement.get_list();
                string [] user_names = new string[user_list.Length];
                for(int i=0; i<user_list.Length; i++)
                {
                    user_names[i] = user_list[i].name;
                }
                string [] home_dirs = di.Interfaces.ManagementUserManagement.get_home_directory(user_names);
                long [] user_ids = di.Interfaces.ManagementUserManagement.get_user_id(user_names);
                iControl.ManagementUserManagementUserRole [] user_roles = di.Interfaces.ManagementUserManagement.get_role(user_names);
                string [] login_shells = di.Interfaces.ManagementUserManagement.get_login_shell(user_names);
                for(int i=0; i<user_list.Length; i++)
                {
                    iControl.ManagementUserManagementUserID user_info = user_list[i];

                    if (bJustNames)
                    {
                        WriteItemObject(user_info.name, pi.Path, false);
                    }
                    else
                    {
                        bool bProcess = true;
                        if (pi.IsItem)
                        {
                            bProcess = false;
                            if (pi.Item.Equals(user_info.name))
                            {
                                bProcess = true;
                            }
                        }
                        if (bProcess)
                        {
                            ManagementUserItem mui = new ManagementUserItem();
                            mui.Name = user_info.name;
                            mui.FullName = user_info.full_name;
                            mui.HomeDirectory = home_dirs[i];
                            mui.LoginShell = login_shells[i];
                            mui.UserId = user_ids[i];
                            mui.UserRole = user_roles[i];

                            WriteItemObject(mui, pi.Path, false);
                        }
                    }
                }
            }
        }

        private void handleNetworkInterface(PathInfo pi, bool bJustNames, bool recurse)
        {
            iControlPSDriveInfo di = this.PSDriveInfo as iControlPSDriveInfo;
            if (pi.IsType | pi.IsItem)
            {
                string [] iface_list = di.Interfaces.NetworkingInterfaces.get_list();
                string[] mac_list = di.Interfaces.NetworkingInterfaces.get_mac_address(iface_list);
                iControl.NetworkingInterfacesMediaType [] media_list = di.Interfaces.NetworkingInterfaces.get_media(iface_list);
                iControl.NetworkingMediaStatus [] status_list = di.Interfaces.NetworkingInterfaces.get_media_status(iface_list);
                iControl.CommonEnabledState[] enabled_states = di.Interfaces.NetworkingInterfaces.get_enabled_state(iface_list);

                for (int i = 0; i < iface_list.Length; i++)
                {
                    if (bJustNames)
                    {
                        WriteItemObject(iface_list[i], pi.Path, false);
                    }
                    else
                    {
                        bool bProcess = true;
                        if (pi.IsItem)
                        {
                            bProcess = false;
                            if (pi.Item.Equals(iface_list[i]))
                            {
                                bProcess = true;
                            }
                        }
                        if (bProcess)
                        {
                            NetworkInterfaceItem nii = new NetworkInterfaceItem();
                            nii.Name = iface_list[i];
                            nii.MacAddress = mac_list[i];
                            nii.MediaStatus = status_list[i];
                            nii.MediaType = media_list[i];
                            nii.Enabled = enabled_states[i];

                            WriteItemObject(nii, pi.Path, false);
                        }
                    }
                }
            }
        }

        private void handleNetworkVLANs(PathInfo pi, bool bJustNames, bool recurse)
        {
            iControlPSDriveInfo di = this.PSDriveInfo as iControlPSDriveInfo;
            if (pi.IsType | pi.IsItem)
            {
                string [] vlan_list = di.Interfaces.NetworkingVLAN.get_list();
                long [] vlan_id_list = di.Interfaces.NetworkingVLAN.get_vlan_id(vlan_list);
                iControl.CommonEnabledState [] failsafe_state_lists = di.Interfaces.NetworkingVLAN.get_failsafe_state(vlan_list);
                string [] true_mac_list = di.Interfaces.NetworkingVLAN.get_true_mac_address(vlan_list);
                string[] mac_masquerade_list = di.Interfaces.NetworkingVLAN.get_mac_masquerade_address(vlan_list); 

                for (int i = 0; i < vlan_list.Length; i++)
                {
                    if (bJustNames)
                    {
                        WriteItemObject(vlan_list[i], pi.Path, false);
                    }
                    else
                    {
                        bool bProcess = true;
                        if (pi.IsItem)
                        {
                            bProcess = false;
                            if (pi.Item.Equals(vlan_list[i]))
                            {
                                bProcess = true;
                            }
                        }
                        if (bProcess)
                        {
                            NetworkVLANItem nvi = new NetworkVLANItem();
                            nvi.Name = vlan_list[i];
                            nvi.Id = vlan_id_list[i];
                            nvi.FailsafeState = failsafe_state_lists[i];
                            nvi.MacAddress = true_mac_list[i];
                            nvi.MacMasqueradeAddress = mac_masquerade_list[i];

                            WriteItemObject(nvi, pi.Path, false);
                        }
                    }
                }
            }
        }

        private void handleSystemServices(PathInfo pi, bool bJustNames, bool recurse)
        {
            iControlPSDriveInfo di = this.PSDriveInfo as iControlPSDriveInfo;
            if (pi.IsType | pi.IsItem)
            {
                iControl.SystemServicesServiceStatus[] status_list = di.Interfaces.SystemServices.get_all_service_statuses();
                for (int i = 0; i < status_list.Length; i++)
                {
                    if (bJustNames)
                    {
                        WriteItemObject(status_list[i].service, pi.Path, false);
                    }
                    else
                    {
                        bool bProcess = true;
                        if (pi.IsItem)
                        {
                            bProcess = false;
                            if (pi.Item.Equals(status_list[i].service))
                            {
                                bProcess = true;
                            }
                        }
                        if (bProcess)
                        {
                            SystemServiceItem ssi = new SystemServiceItem();
                            ssi.Name = status_list[i].service;
                            ssi.Status = status_list[i].status;

                            WriteItemObject(ssi, pi.Path, false);
                        }
                    }
                }
            }
        }

        private void handleSystemSystemInfo(PathInfo pi, bool bJustNames, bool recurse)
        {
            // root\\Category\\Type\\Item
            // bigip\\System\\SystemInfo
            switch (pi.Item)
            {
                case "CPU":
                    handleSystemSystemInfoCPU(pi, bJustNames, recurse);
                    break;
                case "Disk":
                    handleSystemSystemInfoDisk(pi, bJustNames, recurse);
                    break;
                case "Fan":
                    handleSystemSystemInfoFan(pi, bJustNames, recurse);
                    break;
                case "Memory":
                    handleSystemSystemInfoMemory(pi, bJustNames, recurse);
                    break;
                case "Power":
                    handleSystemSystemInfoPower(pi, bJustNames, recurse);
                    break;
                default:
                    if (recurse)
                    {
                        processPath(pi.Path + "\\CPU", bJustNames, recurse);
                        processPath(pi.Path + "\\Disk", bJustNames, recurse);
                        processPath(pi.Path + "\\Fan", bJustNames, recurse);
                        processPath(pi.Path + "\\Memory", bJustNames, recurse);
                        processPath(pi.Path + "\\Power", bJustNames, recurse);
                    }
                    else
                    {
                        WriteDirectoryInfo(pi, "CPU");
                        WriteDirectoryInfo(pi, "Disk");
                        WriteDirectoryInfo(pi, "Fan");
                        WriteDirectoryInfo(pi, "Memory");
                        WriteDirectoryInfo(pi, "Power");
                    }
                    break;
            }
        }

        private void handleSystemSystemInfoCPU(PathInfo pi, bool bJustNames, bool recurse)
        {
            iControlPSDriveInfo di = this.PSDriveInfo as iControlPSDriveInfo;
            if (String.IsNullOrEmpty(pi.SubItem))
            {
                iControl.SystemPlatformCPUs cpu_metrics = di.Interfaces.SystemSystemInfo.get_cpu_metrics();
                iControl.SystemCPUUsageInformation usage_info = di.Interfaces.SystemSystemInfo.get_cpu_usage_information();

                for (int i = 0; i < usage_info.usages.Length; i++)
                {
                    if (bJustNames)
                    {
                        WriteItemObject(usage_info.usages[i].cpu_id, pi.Path, false);
                    }
                    else
                    {
                        SystemCPUItem sci = new SystemCPUItem();
                        for (int j = 0; j < cpu_metrics.cpus[i].Length; j++)
                        {
                            switch (cpu_metrics.cpus[i][j].metric_type)
                            {
                                case iControl.SystemCPUMetricType.CPU_INDEX:
                                    sci.Index = cpu_metrics.cpus[i][j].value;
                                    break;
                                case iControl.SystemCPUMetricType.CPU_FAN_SPEED:
                                    sci.FanSpeed = cpu_metrics.cpus[i][j].value;
                                    break;
                                case iControl.SystemCPUMetricType.CPU_TEMPERATURE:
                                    sci.TemperatureF = cpu_metrics.cpus[i][j].value;
                                    break;
                            }
                        }
                        sci.Id = usage_info.usages[i].cpu_id;
                        sci.UsageIdle = Utility.build64(usage_info.usages[i].idle);
                        sci.UsageIOWait = Utility.build64(usage_info.usages[i].iowait);
                        sci.UsageIRQ = Utility.build64(usage_info.usages[i].irq);
                        sci.UsageNiced = Utility.build64(usage_info.usages[i].niced);
                        sci.UsageSoftIRQ = Utility.build64(usage_info.usages[i].softirq);
                        sci.UsageSystem = Utility.build64(usage_info.usages[i].system);
                        sci.UsageUser = Utility.build64(usage_info.usages[i].user);

                        WriteItemObject(sci, pi.Path, false);
                    }
                }
            }
        }

        private void handleSystemSystemInfoDisk(PathInfo pi, bool bJustNames, bool recurse)
        {
            iControlPSDriveInfo di = this.PSDriveInfo as iControlPSDriveInfo;
            if (String.IsNullOrEmpty(pi.SubItem))
            {
                iControl.SystemDiskUsageInformation disk_usage = di.Interfaces.SystemSystemInfo.get_disk_usage_information();
                for (int i = 0; i < disk_usage.usages.Length; i++)
                {
                    if (bJustNames)
                    {
                        WriteItemObject(disk_usage.usages[i].partition_name, pi.Path, false);
                    }
                    else
                    {
                        SystemDiskItem sdi = new SystemDiskItem();
                        sdi.BlockSize = Utility.build64(disk_usage.usages[i].block_size);
                        sdi.FreeBlocks = Utility.build64(disk_usage.usages[i].free_blocks);
                        sdi.FreeNodes = Utility.build64(disk_usage.usages[i].free_nodes);
                        sdi.PartitionName = disk_usage.usages[i].partition_name;
                        sdi.TotalBlocks = Utility.build64(disk_usage.usages[i].total_blocks);
                        sdi.TotalNodes = Utility.build64(disk_usage.usages[i].total_nodes);

                        WriteItemObject(sdi, pi.Path, false);
                    }
                }
            }
        }

        private void handleSystemSystemInfoFan(PathInfo pi, bool bJustNames, bool recurse)
        {
            iControlPSDriveInfo di = this.PSDriveInfo as iControlPSDriveInfo;
            if (String.IsNullOrEmpty(pi.SubItem))
            {
                iControl.SystemPlatformFans fan_metrics = di.Interfaces.SystemSystemInfo.get_fan_metrics();
                for (int i = 0; i < fan_metrics.fans.Length; i++)
                {
                    SystemFanItem sfi = new SystemFanItem();
                    for (int j=0; j<fan_metrics.fans[i].Length; j++)
                    {
                        switch(fan_metrics.fans[i][j].metric_type)
                        {
                            case iControl.SystemFanMetricType.FAN_INDEX:
                                sfi.Index = fan_metrics.fans[i][j].value;
                                break;
                            case iControl.SystemFanMetricType.FAN_SPEED:
                                sfi.Speed = fan_metrics.fans[i][j].value;
                                break;
                            case iControl.SystemFanMetricType.FAN_STATE:
                                sfi.State = fan_metrics.fans[i][j].value;
                                break;
                        }
                    }
                    if (bJustNames)
                    {
                        WriteItemObject(sfi.Index, pi.Path, false);
                    }
                    else
                    {
                        WriteItemObject(sfi, pi.Path, false);
                    }
                }
            }

        }

        private void handleSystemSystemInfoMemory(PathInfo pi, bool bJustNames, bool recurse)
        {
            iControlPSDriveInfo di = this.PSDriveInfo as iControlPSDriveInfo;
            if (String.IsNullOrEmpty(pi.SubItem))
            {
                iControl.SystemMemoryUsageInformation memory_info = di.Interfaces.SystemSystemInfo.get_memory_usage_information();

                SystemMemoryItem smi = new SystemMemoryItem();
                smi.Name = "Total";
                smi.MaximumAllocated = 0;
                smi.CurrentAllocated = 0;
                smi.Size = Utility.build64(memory_info.total_memory);
                WriteItemObject(smi, pi.Path, false);

                smi.Name = "Used";
                smi.Size = Utility.build64(memory_info.used_memory);
                WriteItemObject(smi, pi.Path, false);
                
                //                smi.Usages = new SystemMemorySubItem[memory_info.usages.Length];
                for (int i = 0; i < memory_info.usages.Length; i++)
                {
                    smi.Name = memory_info.usages[i].subsystem_name;
                    smi.CurrentAllocated = Utility.build64(memory_info.usages[i].current_allocated);
                    smi.MaximumAllocated = Utility.build64(memory_info.usages[i].maximum_allocated);
                    smi.Size = Utility.build64(memory_info.usages[i].size);

                    WriteItemObject(smi, pi.Path, false);
                }
            }
        }

        private void handleSystemSystemInfoPower(PathInfo pi, bool bJustNames, bool recurse)
        {
            iControlPSDriveInfo di = this.PSDriveInfo as iControlPSDriveInfo;
            if (String.IsNullOrEmpty(pi.SubItem))
            {
                iControl.SystemPlatformPowerSupplies power_metrics = di.Interfaces.SystemSystemInfo.get_power_supply_metrics();

                for (int i = 0; i < power_metrics.power_supplies.Length; i++)
                {
                    SystemPowerSupplyItem spsi = new SystemPowerSupplyItem();
                    for (int j = 0; j < power_metrics.power_supplies[i].Length; j++)
                    {
                        switch (power_metrics.power_supplies[i][j].metric_type)
                        {
                            case iControl.SystemPSMetricType.PS_INDEX:
                                spsi.Index = power_metrics.power_supplies[i][j].value;
                                break;
                            case iControl.SystemPSMetricType.PS_STATE:
                                spsi.State = power_metrics.power_supplies[i][j].value;
                                break;
                        }
                    }

                    WriteItemObject(spsi, pi.Path, false);
                }
            }

        }

        #endregion

        #region Existance Handlers

        private bool ExistsInList(string name, string[] item_list)
        {
            bool bExists = false;
            foreach (string s in item_list)
            {
                if (s.Equals(name))
                {
                    bExists = true;
                    break;
                }
            }
            return bExists;
        }

        private bool LTMNodeExists(string name)
        {
            bool bExists = false;
            iControlPSDriveInfo di = this.PSDriveInfo as iControlPSDriveInfo;
            string[] item_list = di.Interfaces.LocalLBNodeAddress.get_list();
            bExists = ExistsInList(name, item_list);
            return bExists;
        }

        private bool LTMPoolExists(string name)
        {
            bool bExists = false;
            iControlPSDriveInfo di = this.PSDriveInfo as iControlPSDriveInfo;
            string[] item_list = di.Interfaces.LocalLBPool.get_list();
            bExists = ExistsInList(name, item_list);
            return bExists;
        }

        private bool PoolMemberExists(string pool_name, string member)
        {
            bool bExists = false;
            iControlPSDriveInfo di = this.PSDriveInfo as iControlPSDriveInfo;
            string[] pool_list = di.Interfaces.LocalLBPool.get_list();
            foreach (string pool in pool_list)
            {
                if (pool.Equals(pool_name))
                {
                    iControl.CommonIPPortDefinition[][] member_list = di.Interfaces.LocalLBPool.get_member(new string[] { pool_name });
                    foreach (iControl.CommonIPPortDefinition member_def in member_list[0])
                    {
                        if (member.Equals(member_def.address + ":" + member_def.port.ToString()))
                        {
                            bExists = true;
                            break;
                        }
                    }
                    break;
                }
            }
            return bExists;
        }

        private bool LTMRuleExists(string name)
        {
            bool bExists = false;
            iControlPSDriveInfo di = this.PSDriveInfo as iControlPSDriveInfo;
            string[] item_list = di.Interfaces.LocalLBRule.get_list();
            bExists = ExistsInList(name, item_list);
            return bExists;
        }

        private bool LTMVirtualServerExists(string name)
        {
            bool bExists = false;
            iControlPSDriveInfo di = this.PSDriveInfo as iControlPSDriveInfo;
            string[] item_list = di.Interfaces.LocalLBVirtualServer.get_list();
            bExists = ExistsInList(name, item_list);
            return bExists;
        }

        private bool ManagementDatabaseExists(string name)
        {
            bool bExists = false;
            iControlPSDriveInfo di = this.PSDriveInfo as iControlPSDriveInfo;
            iControl.ManagementDBVariableVariableNameValue[] name_vals = di.Interfaces.ManagementDBVariable.get_list();
            foreach (iControl.ManagementDBVariableVariableNameValue name_val in name_vals)
            {
                if (name_val.name.Equals(name))
                {
                    bExists = true;
                    break;
                }
            }
            return bExists;
        }

        private bool ManagementUserExists(string name)
        {
            bool bExists = false;
            iControlPSDriveInfo di = this.PSDriveInfo as iControlPSDriveInfo;

            iControl.ManagementUserManagementUserID[] user_list = di.Interfaces.ManagementUserManagement.get_list();
            foreach (iControl.ManagementUserManagementUserID user_info in user_list)
            {
                if (user_info.name.Equals(name))
                {
                    bExists = true;
                    break;
                }
            }
            return bExists;
        }

        private bool NetworkIntefaceExists(string name)
        {
            bool bExists = false;
            iControlPSDriveInfo di = this.PSDriveInfo as iControlPSDriveInfo;
            string[] item_list = di.Interfaces.NetworkingInterfaces.get_list();
            bExists = ExistsInList(name, item_list);
            return bExists;
        }

        private bool NetworkVLANExists(string name)
        {
            bool bExists = false;
            iControlPSDriveInfo di = this.PSDriveInfo as iControlPSDriveInfo;
            string[] item_list = di.Interfaces.NetworkingVLAN.get_list();
            bExists = ExistsInList(name, item_list);
            return bExists;
        }

        private bool SystemServiceExists(string name)
        {
            bool bExists = false;
            iControlPSDriveInfo di = this.PSDriveInfo as iControlPSDriveInfo;
            iControl.SystemServicesServiceType [] service_list = di.Interfaces.SystemServices.get_list();
            foreach (iControl.SystemServicesServiceType service in service_list)
            {
                if (service.ToString().Equals(name))
                {
                    bExists = true;
                    break;
                }
            }
            return bExists;
        }

        #endregion

        private string RemoveDrive(string path)
        {
            string pdn = path.Replace(this.PSDriveInfo.Root, "");
            if (pdn.StartsWith(pathSeparator))
            {
                pdn = pdn.Remove(0, 1);
            }
            return pdn.Replace("\\", "/");
        }

        private bool PathIsDrive(string path)
        {
            if (String.IsNullOrEmpty(path.Replace(this.PSDriveInfo.Root, "")) ||
                String.IsNullOrEmpty(path.Replace(this.PSDriveInfo.Root + pathSeparator, "")))
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        #region IContentCmdletProvider Members

        public void ClearContent(string path)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public object ClearContentDynamicParameters(string path)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public IContentReader GetContentReader(string path)
        {
            StringContentReader scr = null;
            PathInfo pi = new PathInfo(path);
            iControlPSDriveInfo di = this.PSDriveInfo as iControlPSDriveInfo;

            switch (pi.Category)
            {
                case OBJECT_CATEGORY.GTM:
                    switch (pi.Type)
                    {
                        case OBJECT_TYPE.GTM_WIDEIP:
                            break;
                    }
                    break;
                case OBJECT_CATEGORY.LTM:
                    switch (pi.Type)
                    {
                        case OBJECT_TYPE.LTM_NODE:
                            break;
                        case OBJECT_TYPE.LTM_POOL:
                            break;
                        case OBJECT_TYPE.LTM_RULE:
                            iControl.LocalLBRuleRuleDefinition[] rule_defs =
                                di.Interfaces.LocalLBRule.query_rule(new string[] { pi.Item });
                            scr = new StringContentReader(rule_defs[0].rule_definition);
                            break;
                        case OBJECT_TYPE.LTM_VIRTUAL_SERVER:
                            break;

                    }
                    break;
                case OBJECT_CATEGORY.MANAGEMENT:
                    switch (pi.Type)
                    {
                        case OBJECT_TYPE.MANAGEMENT_DATABASE:
                            break;
                        case OBJECT_TYPE.MANAGEMENT_USERS:
                            break;
                    }
                    break;
                case OBJECT_CATEGORY.NETWORK:
                    switch (pi.Type)
                    {
                        case OBJECT_TYPE.NETWORK_INTERFACE:
                            break;
                        case OBJECT_TYPE.NETWORK_VLAN:
                            break;
                    }
                    break;
                case OBJECT_CATEGORY.SYSTEM:
                    switch (pi.Type)
                    {
                        case OBJECT_TYPE.SYSTEM_SERVICES:
                            break;
                        case OBJECT_TYPE.SYSTEM_SYSTEMINFO:
                            break;
                    }
                    break;
            }
            
            return scr;
        }

        public object GetContentReaderDynamicParameters(string path)
        {
            //throw new Exception("The method or operation is not implemented.");
            return null;
        }

        public IContentWriter GetContentWriter(string path)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public object GetContentWriterDynamicParameters(string path)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion
    }
}
