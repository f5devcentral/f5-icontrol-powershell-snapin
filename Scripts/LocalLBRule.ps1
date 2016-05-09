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
param($g_bigip=$null, $g_uid=$null, $g_pwd=$null, $g_rule=$null)

#-------------------------------------------------------------------------
# function usage
#-------------------------------------------------------------------------
function usage()
{
	Write-Host "Usage: LocalLBRule.ps1 host uid pwd [rule_name]";
	exit;
}

#-------------------------------------------------------------------------
# function Get-RuleInfo
#-------------------------------------------------------------------------
function Get-RuleInfo()
{
	param([string[]] $rule_list)
	Write-Host "Retreiving rule info...";

	$rule_definition_list = (Get-F5.iControl).LocalLBRule.query_rule($rule_list);
	
	foreach ($RuleDefinition in $rule_definition_list)
	{
		$rule_name = $RuleDefinition.rule_name;
		$rule_definition = $RuleDefinition.rule_definition;

		Write-Host "#----------------------------------------";
		Write-Host "# RULE: $rule_name";
		Write-Host "#----------------------------------------";
		Write-Output $rule_definition;
	}
}

#-------------------------------------------------------------------------
# function Get-AllRuleInfo
#-------------------------------------------------------------------------
function Get-AllRuleInfo()
{
	$rule_list = (Get-F5.iControl).LocalLBRule.get_list();
	Get-RuleInfo $rule_list;
}

#-------------------------------------------------------------------------
# Exception Handler
#-------------------------------------------------------------------------
trap [System.Exception]
{
	Write-Error $_.Exception.Message;
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

if ( $g_rule -ne $null )
{
	Get-RuleInfo $g_rule;
}
else
{
	Get-AllRuleInfo;
}

