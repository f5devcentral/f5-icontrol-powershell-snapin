using System;
using System.Collections.Generic;
using System.Text;
using System.Management.Automation;
using System.Collections;
using System.IO;
using iControlSnapIn.Provider;

namespace iControlSnapIn.CmdLet.System.ConfigSync
{
    [Cmdlet(iControlVerbs.Download, iControlNouns.Configuration)]
    public class DownloadConfiguration : iControlPSCmdlet
    {

        #region Parameters

        private string _name = null;
        [Parameter(Position = 0, Mandatory=true, HelpMessage = "The name of the configuration to download")]
        [ValidateNotNullOrEmpty]
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        private string _file = null;
        [Parameter(Position = 1, Mandatory = true, HelpMessage = "The local file to download the configuration into")]
        [ValidateNotNullOrEmpty]
        public string LocalFile
        {
            get { return _file; }
            set { _file = value; }
        }

        private long _chunk_size = 64 * 1024;
        [Parameter(Position = 2, HelpMessage = "The chunk size for each partial request")]
        [ValidateNotNullOrEmpty]
        public long ChunkSize
        {
            get { return _chunk_size; }
            set { _chunk_size = value; }
        }

        #endregion

        protected override void ProcessRecord()
        {
            if (isInitialized())
            {
                try
                {
                    iControl.SystemConfigSyncFileTransferContext ctx;
                    long chunk_size = _chunk_size;
                    long file_offset = 0;
                    bool bContinue = true;
                    FileMode fm = FileMode.CreateNew;
                    if (File.Exists(_file))
                    {
                        fm = FileMode.Truncate;
                    }

                    FileStream fs = new FileStream(_file, fm);
                    BinaryWriter w = new BinaryWriter(fs);

                    while (bContinue)
                    {
                        ctx = GetiControl().SystemConfigSync.download_configuration(_name, chunk_size, ref file_offset);

                        // Append data to file
                        w.Write(ctx.file_data, 0, ctx.file_data.Length);

                        WriteVerbose("Bytes Transferred: " + file_offset);

                        if ((iControl.CommonFileChainType.FILE_LAST == ctx.chain_type) ||
                            (iControl.CommonFileChainType.FILE_FIRST_AND_LAST == ctx.chain_type))
                        {
                            bContinue = false;
                        }
                    }

                    w.Close();

                    WriteObject(true);
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
    }
}
