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
param($g_bigip=$null, $g_uid=$null, $g_pwd=$null)
set-psdebug -strict

#-------------------------------------------------------------------------
# function usage
#-------------------------------------------------------------------------
function usage()
{
	Write-Host "Usage: SystemInfo.ps1 host uid pwd";
	exit;
}

#-------------------------------------------------------------------------
# function Get-SystemInfo
#-------------------------------------------------------------------------
function Get-SystemInfo()
{
	$SystemInfo = (Get-F5.iControl).SystemSystemInfo.get_system_information();
	$system_name = $SystemInfo.system_name;
	$host_name   = $SystemInfo.host_name;
	$os_release  = $SystemInfo.os_release;
	$os_machine  = $SystemInfo.os_machine;
	$os_version  = $SystemInfo.os_version;
	$platform = $SystemInfo.platform;
	$product_category = $SystemInfo.product_category;
	$chassis_serial = $SystemInfo.chassis_serial;
	$switch_board_serial = $SystemInfo.switch_board_serial;
	$switch_board_part_revision = $SystemInfo.switch_board_part_revision;
	$host_board_serial = $SystemInfo.host_board_serial;
	$host_board_part_revision = $SystemInfo.host_board_part_revision;
	$annunciator_board_serial = $SystemInfo.annunciator_board_serial;
	$annunciator_board_part_revision = $SystemInfo.annunciator_board_part_revision;

	Write-Host @"
#-------------------------------------
# System Information
#-------------------------------------
  System Name            : ${system_name}
  Host Name              : ${host_name}
  OS Release             : ${os_release}
  OS Machine             : ${os_machine}
  OS Version             : ${os_version}
  Platform               : ${platform}
  Product Category       : ${product_category}
  Chassis Serial #       : ${chassis_serial}
  Switch Board Serial #  : ${switch_board_serial}
  Switch Board Revision #: ${switch_board_part_revision}
  Host Board Serial #    : ${host_board_serial}
  Host Board Revision #  : ${host_board_part_revision}
  Annunciator Serial #   : ${annunciator_board_serial}
  Annunciator Revision # : ${annunciator_board_part_revision}
"@
}

function Get-SystemId()
{
	$system_id = (Get-F5.iControl).SystemSystemInfo.get_system_id();
	
	Write-Host @"
#-------------------------------------
# System Id
#-------------------------------------
  System Id: $system_id
"@
}

function Get-Time()
{
	$SystemTime = (Get-F5.iControl).SystemSystemInfo.get_time();
	$year = $SystemTime.year;
	$month = $SystemTime.month;
	$date = $SystemTime.day;
	$hour = $SystemTime.hour;
	$minute = $SystemTime.minute;
	$second = $SystemTime.second;
	$MonthArray = "Jan", "Feb", "Mar", "Apr", "May", "Jun",
		"Jul", "Aug", "Sept", "Oct", "Nov", "Dec";
	$month_str = $MonthArray[$month-1];

	$TimeZone = (Get-F5.iControl).SystemSystemInfo.get_time_zone();
	$gmt_offset = $TimeZone.gmt_offset;
	$time_zone = $TimeZone.time_zone;
	$is_daylight_saving_time = $TimeZone.is_daylight_saving_time;

	Write-Host @"
#-------------------------------------
# System Time
#-------------------------------------
  UTC Time         : ${month_str} ${date} ${hour}:${minute}:${second} $year
  Time Zone        : ${time_zone} (GMT${gmt_offset})
  Daylight Savings : ${is_daylight_saving_time}
"@
}

function Get-ProductInfo()
{
	$ProductInfo = (Get-F5.iControl).SystemSystemInfo.get_product_information();
	$product_code = $ProductInfo.product_code;
	$product_version = $ProductInfo.product_version;
	$package_version = $ProductInfo.package_version;
	$package_edition = $ProductInfo.package_edition;
	$product_features = $ProductInfo.product_features;
	
	Write-Host @"
#-------------------------------------
# Product Information
#-------------------------------------
  Product Code     : ${product_code}
  Product Version  : ${product_version}
  Package Version  : ${package_version}
  Package Edition  : ${package_edition}
  Product Features : 
"@
	foreach ($feature in $product_features)
	{
		Write-Host "                     $feature";
	}
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

$success = Initialize-F5.iControl -HostName $g_bigip -Username $g_uid -Password $g_pwd;

Get-SystemInfo;
Get-SystemId;
Get-Time;
Get-ProductInfo;


