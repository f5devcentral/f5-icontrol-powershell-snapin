using System;
using System.Collections.Generic;
using System.Text;
using System.Management.Automation;
using iControlSnapIn.TypeData;

namespace iControlSnapIn.CmdLet.LTM.VirtualServer
{
	[Cmdlet(VerbsCommon.Get, iControlNouns.LTMVirtualServerStatistics, SupportsShouldProcess = true)]
	public class GetLTMVirtualServerStatistics : iControlPSCmdlet
	{
        #region Parameters

        private string _name = null;
        [Parameter(Position = 0,
            HelpMessage = "The name of the LTM Virtual Server")]
        [ValidateNotNullOrEmpty]
        public string VirtualServer
        {
            get { return _name; }
            set { _name = value; }
        }

		private LTMStatusItem _vs_item;
		[Parameter(Position = 1, HelpMessage = "The LTMStatusItem describing the virtual server", ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
		[Alias("item")]
		[ValidateNotNullOrEmpty]
		public LTMStatusItem VirtualServerItem
		{
			get { return _vs_item; }
			set { _vs_item = value; }
		}

		#endregion

		protected bool ValidateInput()
		{
			bool bValid = false;

			if ((null == VirtualServer) && (null == VirtualServerItem))
			{
				handleError("You must specify either the VirtualServer or VirtualServerItem parameter", "Input Validation Error");
			}
			else
			{
				if (null != VirtualServerItem)
				{
					VirtualServer = VirtualServerItem.Name;
				}
				bValid = true;
			}

			return bValid;
		}

		protected override void ProcessRecord()
		{
			if (ValidateInput())
			{
				if (isInitialized())
				{
					try
					{
						iControl.LocalLBVirtualServerVirtualServerStatistics lbvsStats = GetiControl().LocalLBVirtualServer.get_statistics(new string[] { _name });

						ObjectStatistics os = new ObjectStatistics();
						os.Name = VirtualServer;
						os.Statistics = new Statistic[lbvsStats.statistics[0].statistics.Length];
						for (int i = 0; i < os.Statistics.Length; i++)
						{
							os.Statistics[i] = new Statistic();
							os.Statistics[i].Type = lbvsStats.statistics[0].statistics[i].type.ToString().Replace("STATISTIC_", "");
							os.Statistics[i].Value = Utility.build64(lbvsStats.statistics[0].statistics[i].value);
						}
						WriteObject(os);
						//foreach (iControl.CommonStatistic stat in lbvsStats.statistics[0].statistics)
						//{
						//    Statistic s = new Statistic();
						//    s.Type = stat.type.ToString().Replace("STATISTIC_", "");
						//    s.Value = Utility.build64(stat.value);
						//    WriteObject(s);
						//}
					}
					catch (Exception ex)
					{
						ErrorRecord er = new ErrorRecord(ex, "2", ErrorCategory.OpenError, "error");
						WriteError(er);
					}
				}
				else
				{
					handleNotInitialized();
				}
			}
		}

	}
}
