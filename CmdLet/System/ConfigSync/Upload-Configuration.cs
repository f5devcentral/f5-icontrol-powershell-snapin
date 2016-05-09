using System;
using System.Collections.Generic;
using System.Text;
using System.Management.Automation;
using System.Collections;
using System.IO;
using iControlSnapIn.Provider;

namespace iControlSnapIn.CmdLet.System.ConfigSync
{
    [Cmdlet(iControlVerbs.Upload, iControlNouns.Configuration)]
    public class UploadConfiguration : iControlPSCmdlet
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
                    iControl.SystemConfigSyncFileTransferContext ctx = new iControl.SystemConfigSyncFileTransferContext();
                    bool bContinue = true;
                    ctx.chain_type = iControl.CommonFileChainType.FILE_FIRST;
                    long chunk_size = _chunk_size;
                    long total_bytes = 0;

                    FileStream fs = new FileStream(_file, FileMode.Open);
                    BinaryReader r = new BinaryReader(fs);

                    while (bContinue)
                    {
                        ctx.file_data = r.ReadBytes(Convert.ToInt32(chunk_size));
                        if (ctx.file_data.Length != chunk_size)
                        {
                            // At the end.  Check to see if it is the first request also.
                            if (0 == total_bytes)
                            {
                                ctx.chain_type = iControl.CommonFileChainType.FILE_FIRST_AND_LAST;
                            }
                            else
                            {
                                ctx.chain_type = iControl.CommonFileChainType.FILE_LAST;
                            }
                            bContinue = false;
                        }
                        total_bytes += ctx.file_data.Length;

                        // Upload bytes.
                        GetiControl().SystemConfigSync.upload_configuration(_name, ctx);

                        // Move to middle 
                        ctx.chain_type = iControl.CommonFileChainType.FILE_MIDDLE;

                        WriteVerbose("Uploaded " + total_bytes + " bytes");
                    }

                    r.Close();

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
