using System;
using System.Collections.Generic;
using System.Text;
using System.Management.Automation;
using System.Collections;
using System.IO;
using iControlSnapIn.Provider;

namespace iControlSnapIn.CmdLet.System.ConfigSync
{
    [Cmdlet(iControlVerbs.Upload, iControlNouns.File)]
    public class UploadFile : iControlPSCmdlet
    {

        #region Parameters

        private string _remote_file = null;
        [Parameter(Position = 0, Mandatory = true, HelpMessage = "The local file to upload")]
        [ValidateNotNullOrEmpty]
        public string RemoteFile
        {
            get { return _remote_file; }
            set { _remote_file = value; }
        }

        private string _local_file = null;
        [Parameter(Position = 1, Mandatory = true, HelpMessage = "The name of the remote file to upload the local file to")]
        [ValidateNotNullOrEmpty]
        public string LocalFile
        {
            get { return _local_file; }
            set { _local_file = value; }
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

                    FileStream fs = new FileStream(_local_file, FileMode.Open);
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
                        GetiControl().SystemConfigSync.upload_file(_remote_file, ctx);

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
