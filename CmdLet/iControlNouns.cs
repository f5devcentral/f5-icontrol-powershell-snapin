using System;
using System.Collections.Generic;
using System.Text;

namespace iControlSnapIn.CmdLet
{
    internal static class iControlNouns
    {
		// F5 Specific Namespace Previx
		public const string NOUN_PREFIX = "F5.";

        // Deployment
        public const string SharePoint2007Deployment = NOUN_PREFIX + "SharePoint2007Deployment";

        // Global
		public const string iControl = NOUN_PREFIX + "iControl";
		public const string iControlCommands = NOUN_PREFIX + "iControlCommands";

        // Node Address
		public const string LTMNodeAddress = NOUN_PREFIX + "LTMNodeAddress";

        // Pool
		public const string LTMPool = NOUN_PREFIX + "LTMPool";

        // PoolMember
		public const string LTMPoolMember = NOUN_PREFIX + "LTMPoolMember";
		public const string LTMPoolMemberState = NOUN_PREFIX + "LTMPoolMemberState";

        // Rule
		public const string LTMRule = NOUN_PREFIX + "LTMRule";

        // Management
		public const string DBVariable = NOUN_PREFIX + "DBVariable";

        // RAMCache
		public const string LTMRAMCacheEntries = NOUN_PREFIX + "LTMRAMCacheEntries";

        // Virtual Server
		public const string LTMVirtualServer = NOUN_PREFIX + "LTMVirtualServer";
		public const string LTMVirtualServerStatistics = NOUN_PREFIX + "LTMVirtualServerStatistics";
		public const string LTMVritualServerRule = NOUN_PREFIX + "LTMVirtualServerRule";

        // System
		public const string SystemInformation = NOUN_PREFIX + "SystemInformation";
		public const string ProductInformation = NOUN_PREFIX + "ProductInformation";
		public const string SystemCPUUsage = NOUN_PREFIX + "SystemCPUUsage";
		public const string SystemDiskUsage = NOUN_PREFIX + "SystemDiskUsage";
		public const string SystemFanUsage = NOUN_PREFIX + "SystemFanUsage";
		public const string SystemMemoryUsage = NOUN_PREFIX + "SystemMemoryUsage";
		public const string SystemPowerUsage = NOUN_PREFIX + "SystemPowerUsage";

		public const string SystemId = NOUN_PREFIX + "SystemId";
		public const string SystemTime = NOUN_PREFIX + "SystemTime";
		public const string SystemTimeZone = NOUN_PREFIX + "SystemTimeZone";
		public const string SystemUptime = NOUN_PREFIX + "SystemUptime";

		public const string ConfigurationList = NOUN_PREFIX + "ConfigurationList";
		public const string Configuration = NOUN_PREFIX + "Configuration";
		public const string File = NOUN_PREFIX + "File";
    }
}
