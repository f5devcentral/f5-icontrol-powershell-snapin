using System;
using System.Collections.Generic;
using System.Text;
using System.Management.Automation;
using System.ComponentModel;

namespace iControlSnapIn
{
    [RunInstaller(true)]
    public class iControlSnapIn : PSSnapIn
    {
        public override string Name
        {
            get { return "iControlSnapIn"; }
        }
        public override string Vendor
        {
            get { return "F5 Networks, Inc."; }
        }
        public override string VendorResource
        {
            get { return "iControlSnapIn,F5 Networks, Inc."; }
        }
        public override string Description
        {
            get { return "iControl Snap-in for F5 Device Management"; }
        }
        public override string DescriptionResource
        {
            get { return "iControlSnapIn,iControl Snap-in for F5 Device Management"; }
        }
    }
}
