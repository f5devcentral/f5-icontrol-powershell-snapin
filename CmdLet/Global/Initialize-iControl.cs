using System;
using System.Collections.Generic;
using System.Text;
using System.Management.Automation;
using System.Collections;
using System.Windows.Forms;
using System.Net;

namespace iControlSnapIn.CmdLet.Global
{
    [Cmdlet(iControlVerbs.Initialize, iControlNouns.iControl)]
    public class InitializeiControl : iControlPSCmdlet
    {
//        private iControl.Interfaces _interfaces = new iControl.Interfaces();

        #region Parameters

        private string _hostname = null;
        [Parameter(Position=0, Mandatory=true, HelpMessage="The hostname of the managed device")]
        [ValidateNotNullOrEmpty]
        public string HostName
        {
            get { return _hostname; }
            set { _hostname = value; }
        }

        private PSCredential _pscreds = null;
        [Parameter(Position = 1, HelpMessage = "Your credentials for the managed device (PSCredential)")]
        [ValidateNotNullOrEmpty]
        public PSCredential PSCredentials
        {
            get { return _pscreds; }
            set { _pscreds = value; }
        }

        private NetworkCredential _creds = null;
        [Parameter(Position = 2, HelpMessage = "Your credentials for the managed device (NetworkCredential)")]
        [ValidateNotNullOrEmpty]
        public NetworkCredential Credentials
        {
            get { return _creds; }
            set { _creds = value; }
        }

        private String _username = null;
        [Parameter(Position = 3, HelpMessage = "Your username for the managed device")]
        [ValidateNotNullOrEmpty]
        public String Username
        {
            get { return _username; }
            set { _username = value; }
        }

        private String _password = null;
        [Parameter(Position = 4, HelpMessage = "Your password for the managed device")]
        [ValidateNotNullOrEmpty]
        public String Password
        {
            get { return _password; }
            set { _password = value; }
        }

		private String _auth_client_ip = null;
		[Parameter(Position = 5, HelpMessage = "The client ip of your token based authentication device.")]
		[ValidateNotNullOrEmpty]
		public String AuthClientIP
		{
			get { return _auth_client_ip; }
			set { _auth_client_ip = value; }
		}

		private String _auth_token = null;
		[Parameter(Position = 6, HelpMessage = "The authentication token")]
		[ValidateNotNullOrEmpty]
		public String AuthToken
		{
			get { return _auth_token; }
			set { _auth_token= value; }
		}

		private string _proxy_address = null;
		[Parameter(Position = 7, HelpMessage = "The hostname of an optional proxy server")]
		[ValidateNotNullOrEmpty]
		public string ProxyAddress
		{
			get { return _proxy_address; }
			set { _proxy_address = value; }
		}

		private long _proxy_port = 0;
		[Parameter(Position = 8, HelpMessage = "The port of an optional proxy server")]
		[ValidateNotNullOrEmpty]
		public long ProxyPort
		{
			get { return _proxy_port; }
			set { _proxy_port = value; }
		}

		private string _proxy_user = null;
		[Parameter(Position = 9, HelpMessage = "The username for an optional proxy server")]
		[ValidateNotNullOrEmpty]
		public string ProxyUser
		{
			get { return _proxy_user; }
			set { _proxy_user = value; }
		}

		private string _proxy_pass = null;
		[Parameter(Position = 10, HelpMessage = "The password for an optional proxy server")]
		[ValidateNotNullOrEmpty]
		public string ProxyPass
		{
			get { return _proxy_pass; }
			set { _proxy_pass = value; }
		}



        #endregion

        private bool validCredentials()
        {
            bool bValid = true;
            if ( string.IsNullOrEmpty(_username) || (string.IsNullOrEmpty(_password)) )
            {
                // Username and password were not supplied
                if ( null != _creds )
                {
                    _username = _creds.UserName.Replace("\\", "");
                    _password = _creds.Password;
                }
                else if ( null != _pscreds )
                {
                    _username = _pscreds.UserName.Replace("\\", "");
                    _password = _pscreds.GetNetworkCredential().Password;
                }
            }

			bool bUsingToken = (!string.IsNullOrEmpty(_auth_client_ip) && !string.IsNullOrEmpty(_auth_token));

            if ( !bUsingToken && (string.IsNullOrEmpty(_username) || (string.IsNullOrEmpty(_password))) )
            {
                handleError("Please supply credentials...", "Credentials");
                bValid = false;
            }
            return bValid;
        }

        protected override void ProcessRecord()
        {
            if (validCredentials())
            {
                if (!isInitialized())
                {
                    Globals._interfaces = new iControl.Interfaces();
                }

				bool initialized = false;


				if (!string.IsNullOrEmpty(_auth_client_ip) && !string.IsNullOrEmpty(_auth_token))
				{
					// "EM" based Token Authentication
					String[] sSplit = _hostname.Split(new char[] { ':' });
					initialized =
						(2 == sSplit.Length) ?
						GetiControl().initializeWithToken(sSplit[0], Convert.ToUInt32(sSplit[1]), _auth_client_ip, _auth_token) :
						GetiControl().initializeWithToken(_hostname, _auth_client_ip, _auth_token);
					
				}
				else
				{
					// Basic Authentication
					if ( string.IsNullOrEmpty(_username) || (string.IsNullOrEmpty(_password)) )
					{
						iControl.Dialogs.ConnectionDialog connDlg = new iControl.Dialogs.ConnectionDialog();
						DialogResult dr = connDlg.ShowDialog();
						if (DialogResult.OK == dr)
						{
							_username = connDlg.Credentials.UserName;
							_password = connDlg.Credentials.Password;
							WebProxy proxy = connDlg.ConnectionInfo.getWebProxy();
							if (null != proxy)
							{
								_proxy_address = proxy.Address.Host;
								_proxy_port = proxy.Address.Port;
								if (null != proxy.Credentials)
								{
									_proxy_user = ((NetworkCredential)proxy.Credentials).UserName;
									_proxy_pass = ((NetworkCredential)proxy.Credentials).Password;
								}
							}
						}
					}

					if (!string.IsNullOrEmpty(_username) && !(string.IsNullOrEmpty(_password)))
					{
						String[] sSplit = _hostname.Split(new char[] { ':' });
						if ((null != _proxy_address) && (0 != _proxy_port))
						{
							initialized =
								(2 == sSplit.Length) ?
								GetiControl().initialize(sSplit[0], Convert.ToUInt32(sSplit[1]), _username, _password, _proxy_address, _proxy_port, _proxy_user, _proxy_pass) :
								GetiControl().initialize(_hostname, 443, _username, _password, _proxy_address, _proxy_port, _proxy_user, _proxy_pass);
						}
						else
						{
							initialized =
								(2 == sSplit.Length) ?
								GetiControl().initialize(sSplit[0], Convert.ToUInt32(sSplit[1]), _username, _password) :
								GetiControl().initialize(_hostname, _username, _password);
						}	                        
					}
				}

				if (initialized)
				{
					try
					{
						string ver = GetiControl().SystemSystemInfo.get_version();
						WriteObject(true, false);
					}
					catch (Exception ex)
					{
						handleException(ex);
					}
				}
				else
				{
					handleError("Could not initialize connection with supplied information", _hostname);
				}

            }
        }
    }
}
