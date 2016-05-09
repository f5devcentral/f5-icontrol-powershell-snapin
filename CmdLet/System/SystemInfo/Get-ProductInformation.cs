using System;
using System.Collections.Generic;
using System.Text;
using System.Management.Automation;
using System.Collections;
using iControlSnapIn.TypeData;

namespace iControlSnapIn.CmdLet.System.SystemInfo
{
    [Cmdlet(VerbsCommon.Get, iControlNouns.ProductInformation)]
    public class GetProductInformation : iControlPSCmdlet
    {

        #region Parameters
        /*
        [Parameter(Position = 0,
            Mandatory = false,
            ValueFromPipelineByPropertyName = true,
            HelpMessage = "Help Text")]
        [ValidateNotNullOrEmpty]
        public string Name
        {
            
        }
 */
        #endregion

        protected override void ProcessRecord()
        {
            if (isInitialized())
            {
                iControl.SystemProductInformation prodInfo = 
                    GetiControl().SystemSystemInfo.get_product_information();
                ProductInformation pi = new ProductInformation();
                pi.PackageEdition = prodInfo.package_edition;
                pi.PackageVersion = prodInfo.package_version;
                pi.ProductCode = prodInfo.product_code;
                pi.ProductFeatures = prodInfo.product_features;
                pi.ProductVersion = prodInfo.product_version;

                WriteObject(pi);
            }
            else
            {
                handleNotInitialized();
            }
        }
    }
}
