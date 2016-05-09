using System;
using System.Collections.Generic;
using System.Text;
using System.Management.Automation;

namespace iControlSnapIn.Provider
{
    internal class iControlPSDriveInfo : PSDriveInfo
    {
        private iControl.Interfaces _interfaces = null;

        public iControl.Interfaces Interfaces
        {
            get { return _interfaces; }
        }

        public bool initalize(string hostname, string username, string password)
        {
            return _interfaces.initialize(hostname, username, password);
        }

        public iControlPSDriveInfo(PSDriveInfo driveInfo)
            : base(driveInfo)
        {
            _interfaces = new iControl.Interfaces();
        }

    }
}
