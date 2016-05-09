using System;
using System.Collections.Generic;
using System.Text;

namespace iControlSnapIn.TypeData
{
    public class iControlPSItem
    {
        public string Name;
        public bool HasChildren;
        public string Value;
    }

    public class GTMStatusItem
    {
        public string Name;
        public iControl.CommonEnabledStatus Enabled;
        public iControl.CommonAvailabilityStatus Availability;
        public string Status;
    }

    public class GTMWideIPItem : GTMStatusItem
    {
        public iControl.GlobalLBLBMethod LBMethod;
        public string Application;
    }

    public class LTMStatusItem
    {
        public string Name;
        public iControl.LocalLBAvailabilityStatus Availability;
        public iControl.LocalLBEnabledStatus Enabled;
        public string Status;
    }

    public class LTMPoolItem : LTMStatusItem
    {
        public int MemberCount;
    }

    public class LTMPoolMemberItem : LTMStatusItem
    {
		public string Pool;
        public string Address;
        public long Port;
    }

    public class LTMRAMCacheEntry
    {
        public string ProfileName;
        public string Hostname;
        public string Uri;
        public string VaryType;
        public long VaryCount;
        public long Hits;
        public long Received;
        public long LastSent;
        public long Expiration;
        public long Size;

    }

    public class LTMRuleDefinitionItem
    {
        public string Name;
        public string Definition;
    }

    public class ManagementDatabaseItem
    {
        public string Name;
        public string Value;
    }

    public class ManagementUserItem
    {
        public string Name;
        public string FullName;
        public string HomeDirectory;
        public long UserId;
        public string LoginShell;
        public iControl.ManagementUserManagementUserRole UserRole;
    }

    public class NetworkInterfaceItem
    {
        public string Name;
        public string MacAddress;
        public iControl.NetworkingInterfacesMediaType MediaType;
        public iControl.NetworkingMediaStatus MediaStatus;
        public iControl.CommonEnabledState Enabled;
    }

    public class NetworkVLANItem
    {
        public string Name;
        public long Id;
        public iControl.CommonEnabledState FailsafeState;
        public string MacMasqueradeAddress;
        public string MacAddress;
    }

    public class SystemServiceItem
    {
        public iControl.SystemServicesServiceType Name;
        public iControl.SystemServicesServiceStatusType Status;
    }

    public class SystemCPUItem
    {
        public long Index;
        public long TemperatureF;
        public long FanSpeed;
        public long Id;
        public UInt64 UsageUser;
        public UInt64 UsageNiced;
        public UInt64 UsageSystem;
        public UInt64 UsageIdle;
        public UInt64 UsageIRQ;
        public UInt64 UsageSoftIRQ;
        public UInt64 UsageIOWait;
    }

    public class SystemDiskItem
    {
        public string PartitionName;
        public UInt64 BlockSize;
        public UInt64 TotalBlocks;
        public UInt64 FreeBlocks;
        public UInt64 TotalNodes;
        public UInt64 FreeNodes;
    }

    public class SystemFanItem
    {
        public long Index;
        public long State;
        public long Speed;
    }

    public class SystemMemoryItem
    {
        public string Name;
        public UInt64 CurrentAllocated;
        public UInt64 MaximumAllocated;
        public UInt64 Size;
    }

    public class SystemPowerSupplyItem
    {
        public long Index;
        public long State;
    }

    public class SystemTimeZone
    {
        public long GmtOffset;
        public String TimeZone;
        public bool DaylightSavings;
    }

    public class SystemInformation
    {
        public String SystemName;
        public String Hostname;
        public String OSRelease;
        public String OSMachine;
        public String OSVersion;
        public String Platform;
        public String ProductCategory;
        public String ChassisSerial;
        public String SwitchBoardSerial;
        public String SwitchBoardPartRevision;
        public String HostBoardSerial;
        public String HostBoardPartRevision;
        public String AnnunciatorBoardSerial;
        public String AnnunciatorBoardPartRevision;
    }

    public class ProductInformation
    {
        public String ProductCode;
        public String ProductVersion;
        public String PackageVersion;
        public String PackageEdition;
        public String[] ProductFeatures;
    }

    public class ConfigFileEntry
    {
        public String Name;
        public String TimeStamp;
    }

	public class Statistic
	{
		public String Type;
		public UInt64 Value;
	}

	public class ObjectStatistics
	{
		public String Name;
		public Statistic[] Statistics;
	}

	public class ServerStateItem
	{
		public string Name;
		public ServerState State;
	}

	public enum ServerState
	{
		SERVER_STATE_ENABLED = 0,
		SERVER_STATE_DISABLED = 1,
		SERVER_STATE_OFFLINE = 2,
		SERVER_STATE_UNKNOWN = 3,
	};
}
