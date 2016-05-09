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
param($g_bigip=$null, $g_uid=$null, $g_pwd=$null, $g_pool=$null)
set-psdebug -strict


#-------------------------------------------------------------------------
# function usage
#-------------------------------------------------------------------------
function usage()
{
	Write-Host "Usage: LocalLBPool.ps1 host uid pwd [pool_name]";
	exit;
}

#-------------------------------------------------------------------------
# function Get-PoolInfo
#-------------------------------------------------------------------------
function Get-PoolInfo()
{
	param([string[]] $pool_list)
	Write-Host "Retreiving pool info...";

	$pool_status_list = (Get-F5.iControl).LocalLBPool.get_object_status($pool_list);
	$pool_member_status_lists = (Get-F5.iControl).LocalLBPoolMember.get_object_status($pool_list);

	for($i=0; $i -lt $pool_list.Length; $i++)
	{
		$pool_name = $pool_list[$i];
		$object_status = $pool_status_list[$i];
		$availability_status = $object_status.availability_status;
		$enabled_status = $object_status.enabled_status;
		$status_description = $object_status.status_description;
		
		Write-Host @"
POOL $pool_name
|   Enabled      : ${enabled_status}
|   Availability : ${availability_status}
|   Description  : ${status_description}
"@
		
		$pool_member_status_list = $pool_member_status_lists[$i];
		for($j=0; $j -lt $pool_member_status_list.Length; $j++)
		{
			$MemberObjectStatus = $pool_member_status_list[$j];
			$member = $MemberObjectStatus.member;
			$address = $member.address;
			$port = $member.port;
			$object_status = $MemberObjectStatus.object_status;
			$availability_status = $object_status.availability_status;
			$enabled_status = $object_status.enabled_status;
			$status_description = $object_status.status_description;
			
			Write-Host @"
+---MEMBER ${address}:${port}
	    |   Enabled      : ${enabled_status}
	    |   Availability : ${availability_status}
	    |   Description  : ${status_description}
"@
		}
	}
}

#-------------------------------------------------------------------------
# function Get-AllPoolInfo
#-------------------------------------------------------------------------
function Get-AllPoolInfo()
{
	$pool_list = (Get-F5.iControl).LocalLBPool.get_list();
	Get-PoolInfo $pool_list;
}

#-------------------------------------------------------------------------
# Exception Handler
#-------------------------------------------------------------------------
trap [System.Exception]
{
	Write-Host "Exception Caught"
#	Write-Error $_.Exception.Message;
	exit;
}

#-------------------------------------------------------------------------
# Main Application Logic
#-------------------------------------------------------------------------
if ( ($g_bigip -eq $null) -or ($g_uid -eq $null) -or ($g_pwd -eq $null) )
{
	usage;
}

$success = Initialize-F5.iControl -HostName $g_bigip -Username $g_uid -Password $g_pwd

if ( $g_pool -ne $null )
{
	Get-PoolInfo $g_pool;
}
else
{
	Get-AllPoolInfo;
}

