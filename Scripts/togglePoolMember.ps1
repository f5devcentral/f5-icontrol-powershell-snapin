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
param($g_bigip=$null, $g_uid=$null, $g_pwd=$null, $g_pool=$null, $g_member=$null)
set-psdebug -strict

#-------------------------------------------------------------------------
# function usage
#-------------------------------------------------------------------------
function usage()
{
	Write-Host "Usage: Toggle-PoolMember.ps1 host uid pwd [pool_name [addr:port]]";
	exit;
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
# function Get-PoolList
#-------------------------------------------------------------------------
function Get-PoolList()
{
	Write-Host "Available Pools";
	foreach ($pool in (Get-F5.iControl).LocalLBPool.get_list())
	{
		Write-Host "  ${pool}";
	}
}

#-------------------------------------------------------------------------
# function Get-PoolMembers
#-------------------------------------------------------------------------
function Get-PoolMembers([string] $pool_name)
{
	$member_session_states_lists = (Get-F5.iControl).LocalLBPoolMember.get_session_enabled_state($pool_name);
	Write-Host "Available Members for pool $pool_name:";
	$member_session_states = $member_session_states_lists[0];
	foreach ($member_session_state in $member_session_states)
	{
		$member = $member_session_state.member;
		$session_state = $member_session_state.session_state;
		$address = $member.address;
		$port = $member.port;
		Write-Host "  ${address}:${port} - ${session_state}";
	}
}

#-------------------------------------------------------------------------
# function Toggle-PoolMember
#-------------------------------------------------------------------------
function Toggle-PoolMember([string] $pool_name, [string] $member)
{
	# find the current state of the member
	$state = Get-MemberState $pool_name $member;
	
	# toggle the state
	if ( $state -eq "STATE_ENABLED" )
	{
		$state = "STATE_DISABLED";
	}
	else
	{
		$state = "STATE_ENABLED";
	}
	
	# set member to toggled state
	Set-MemberState $pool_name $member $state
}

#-------------------------------------------------------------------------
# function Set-MemberState
#-------------------------------------------------------------------------
function Set-MemberState([string] $pool_name, [string] $member_def, [string] $state)
{
	$pool_list = @($pool_name);

	$member = New-Object -typeName iControl.CommonIPPortDefinition;
	$member.address,$member.port = $member_def.Split(':');
	$session_state = New-Object -typeName iControl.LocalLBPoolMemberMemberSessionState;
	$session_state.member = $member;
	$session_state.session_state = $state;

	[iControl.LocalLBPoolMemberMemberSessionState[]]$session_states =
		@($session_state);

	[iControl.LocalLBPoolMemberMemberSessionState[][]]$session_states_list = 
		(,$session_states);

	(Get-F5.iControl).LocalLBPoolMember.set_session_enabled_state(
		$pool_list,
		$session_states_list
	);
	Write-Host "Pool '$pool_name' member '$member_def' state set to $state";
}

#-------------------------------------------------------------------------
# function Get-MemberState
#-------------------------------------------------------------------------
function Get-MemberState([string] $pool_name, [string] $member_def)
{
	$the_state = "NOT_FOUND";

	$member_address,$member_port = $member_def.Split(':');
	
	$member_session_states_lists = (Get-F5.iControl).LocalLBPoolMember.get_session_enabled_state($pool_name);
	$member_session_states = $member_session_states_lists[0];
	foreach ($member_session_state in $member_session_states)
	{
		$session_state = $member_session_state.session_state;
		$member = $member_session_state.member;
		$address = $member.address;
		$port = $member.port;
		
		if ( ($member_address -eq $address) -and ($member_port -eq $port) )
		{
			$the_state = $session_state;
			break;
		}
	}
	return $the_state;
}

#-------------------------------------------------------------------------
# Exception Handler
#-------------------------------------------------------------------------
trap [System.Exception]
{
	Write-Error "Caught Exception"
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

$success = Initialize-F5.iControl -Hostname $g_bigip -Username $g_uid -Password $g_pwd

if ( $g_pool -eq $null )
{
	Get-PoolList;
}
elseif ( $g_member -eq $null )
{
	Get-PoolMembers $g_pool;
}
else
{
	Toggle-PoolMember $g_pool $g_member;
}
