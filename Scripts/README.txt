#----------------------------------------------------------------------------
# The contents of this file are subject to the "END USER LICENSE AGREEMENT FOR F5
# Software Development Kit for iControl"; you may not use this file except in
# compliance with the License. The License is included in the iControl
# Software Development Kit.
#
# Software distributed under the License is distributed on an "AS IS"
# basis, WITHOUT WARRANTY OF ANY KIND, either express or implied. See
# the License for the specific language governing rights and limitations
# under the License.
#
# The Original Code is iControl Code and related documentation
# distributed by F5.
#
# The Initial Developer of the Original Code is F5 Networks,
# Inc. Seattle, WA, USA. Portions created by F5 are Copyright (C) 1996-2007 F5 Networks,
# Inc. All Rights Reserved.  iControl (TM) is a registered trademark of F5 Networks, Inc.
#
# Alternatively, the contents of this file may be used under the terms
# of the GNU General Public License (the "GPL"), in which case the
# provisions of GPL are applicable instead of those above.  If you wish
# to allow use of your version of this file only under the terms of the
# GPL and not to allow others to use your version of this file under the
# License, indicate your decision by deleting the provisions above and
# replace them with the notice and other provisions required by the GPL.
# If you do not delete the provisions above, a recipient may use your
# version of this file under either the License or the GPL.
#----------------------------------------------------------------------------

iControl PowerShell Scripts README

Enclosed in this distribution are sample iControl scripts for use with the
Microsoft Windows PowerShell command processor.

==========================
 Prerequisites
==========================
1. The Microsoft Windows PowerShell.  This can be downloaded from the following link:

http://www.microsoft.com/windowsserver2003/technologies/management/powershell/download.mspx

2. The iControl PowerShell Scripts are dependent on the F5 iControl PowerShell CmdLet.
This can be downloaded from the Labs section on DevCentral.f5.com.

==========================
 Installation
==========================
1. Extract the scripts to a local directory.

==========================
 Using the scripts
==========================
1. Ensure that the iControl SnapIn is installed in the current console with the following Cmdlet.

PS C:\> Add-PSSnapin iControlSnapIn

2. "Dot source" the scripts to run them.

PS C:\scripts> . .\SystemInfo.ps1 bigip user pass
