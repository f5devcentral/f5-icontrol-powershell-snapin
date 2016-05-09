using System;
using System.Collections.Generic;
using System.Text;

namespace iControlSnapIn.Provider
{
    public enum OBJECT_CATEGORY
    {
        UNKNOWN,
        LTM,
        GTM,
        NETWORK,
        MANAGEMENT,
        SYSTEM,
    }

    public enum OBJECT_TYPE
    {
        UNKNOWN,
        GTM_WIDEIP,
        LTM_NODE,
        LTM_POOL,
        LTM_RULE,
        LTM_VIRTUAL_SERVER,
        MANAGEMENT_DATABASE,
        MANAGEMENT_USERS,
        NETWORK_INTERFACE,
        NETWORK_VLAN,
        SYSTEM_SERVICES,
        SYSTEM_SYSTEMINFO,
        SYSTEM_SYSTEMINFO_CPU,
        SYSTEM_SYSTEMINFO_DISK,
        SYSTEM_SYSTEMINFO_FAN,
        SYSTEM_SYSTEMINFO_MEMORY,
        SYSTEM_SYSTEMINFO_POWER,
    }

    // theboss\\LTM\\Pools\\my_pool\\10.10.10.10
    // theboss\\LTM\\Rules\\my_rule
    // theboss\\LTM\\Virtual Servers\\my_vip
    // theboss\\Networking\\Interfaces\iface
    // theboss\\Management\\Users\\user
    public class PathInfo
    {
        #region Member Variables

        private string _path = null;
        private string _root = null;
        private OBJECT_CATEGORY _category = OBJECT_CATEGORY.UNKNOWN;
        private OBJECT_TYPE _type = OBJECT_TYPE.UNKNOWN;
        private string _item = null;
        private string _subitem = null;
        private bool _isdrive = false;
        private bool _iscategory = false;
        private bool _istype = false;
        private bool _isitem = false;
        private bool _issubitem = false;

        public string Path
        {
            get { return _path; }
            set { _path = value; }
        }

        public string Root
        {
            get { return _root; }
            set { _root = value; }
        }

        public OBJECT_CATEGORY Category
        {
            get { return _category; }
            set { _category = value; }
        }

        public OBJECT_TYPE Type
        {
            get { return _type; }
            set { _type = value; }
        }

        public string Item
        {
            get { return _item; }
            set { _item = value; }
        }

        public string SubItem
        {
            get { return _subitem; }
            set { _subitem = value; }
        }

        public bool IsDrive
        {
            get { return _isdrive; }
            set { _isdrive = value; }
        }

        public bool IsCategory
        {
            get { return _iscategory; }
            set { _iscategory = value; }
        }

        public bool IsType
        {
            get { return _istype; }
            set { _istype = value; }
        }

        public bool IsItem
        {
            get { return _isitem; }
            set { _isitem = value; }
        }

        public bool IsSubItem
        {
            get { return _issubitem; }
            set { _issubitem = value; }
        }

        #endregion 

        #region Constructors

        public PathInfo()
        {
        }
        public PathInfo(string path)
        {
            ParsePath(path);
        }

        #endregion

        public void ParsePath(string path)
        {
            Path = path;

            IsDrive = false;
            IsCategory = false;
            IsType = false;
            IsItem = false;
            IsSubItem = false;

            if ( path.Length > 0 )
            {
                String[] sSplit = path.Replace("\\", "/").TrimEnd(new char [] {'/'}).Split(new char[] { '/' });
                if (1 == sSplit.Length)
                {
                    // theboss\\
                    Root = sSplit[0];
                    IsDrive = true;
                    IsCategory = false;
                    IsType = false;
                    IsItem = false;
                    IsSubItem = false;
                }
                if (sSplit.Length > 1)
                {
                    // theboss\\LTM
                    Category = StringToCategory(sSplit[1]);
 
                    IsDrive = false;
                    IsCategory = true;
                    IsType = false;
                    IsItem = false;
                    IsSubItem = false;
                }
                if (sSplit.Length > 2)
                {
                    // theboss\\LTM\\Pools
                    Type = StringToType(sSplit[2]);

                    IsDrive = false;
                    IsCategory = false;
                    IsType = true;
                    IsItem = false;
                    IsSubItem = false;
                }
                if (sSplit.Length > 3)
                {
                    // theboss\\LTM\\Pools\\pool_name
                    Item = sSplit[3];

                    IsDrive = false;
                    IsCategory = false;
                    IsType = false;
                    IsItem = true;
                    IsSubItem = false;
                }
                if (sSplit.Length > 4)
                {
                    // theboss\\LTM\\Pools\\pool_name\\member_def
                    SubItem = sSplit[3];

                    IsDrive = false;
                    IsCategory = false;
                    IsType = false;
                    IsItem = false;
                    IsSubItem = true;
                }
            }
        }

        public OBJECT_CATEGORY StringToCategory(string str)
        {
            OBJECT_CATEGORY type = OBJECT_CATEGORY.UNKNOWN;
            switch (str)
            {
                case "GTM": { type = OBJECT_CATEGORY.GTM; break; }
                case "LTM": { type = OBJECT_CATEGORY.LTM; break; }
                case "Management": { type = OBJECT_CATEGORY.MANAGEMENT; break; }
                case "Networking": { type = OBJECT_CATEGORY.NETWORK; break; }
                case "System": { type = OBJECT_CATEGORY.SYSTEM; break; }
            }
            return type;
        }

        public OBJECT_TYPE StringToType(string str)
        {
            OBJECT_TYPE type = OBJECT_TYPE.UNKNOWN;
            switch (str)
            {
                case "WideIPs": { type = OBJECT_TYPE.GTM_WIDEIP; break;  }
                case "Nodes": { type = OBJECT_TYPE.LTM_NODE; break;  }
                case "Pools": { type = OBJECT_TYPE.LTM_POOL; break; }
                case "Rules": { type = OBJECT_TYPE.LTM_RULE; break; }
                case "VirtualServers": { type = OBJECT_TYPE.LTM_VIRTUAL_SERVER; break; }
                case "Database": { type = OBJECT_TYPE.MANAGEMENT_DATABASE; break; }
                case "Users": { type = OBJECT_TYPE.MANAGEMENT_USERS; break; }
                case "Interfaces": { type = OBJECT_TYPE.NETWORK_INTERFACE; break; }
                case "VLANs": { type = OBJECT_TYPE.NETWORK_VLAN; break; }
                case "Services": { type = OBJECT_TYPE.SYSTEM_SERVICES; break; }
                case "SystemInfo": { type = OBJECT_TYPE.SYSTEM_SYSTEMINFO; break; }
                case "CPU": { type = OBJECT_TYPE.SYSTEM_SYSTEMINFO_CPU; break; }
                case "Disk": { type = OBJECT_TYPE.SYSTEM_SYSTEMINFO_DISK; break; }
                case "Fan": { type = OBJECT_TYPE.SYSTEM_SYSTEMINFO_FAN; break; }
                case "Memory": { type = OBJECT_TYPE.SYSTEM_SYSTEMINFO_MEMORY; break; }
                case "Power": { type = OBJECT_TYPE.SYSTEM_SYSTEMINFO_POWER; break; }
            }
            return type;
        }
    }
}
