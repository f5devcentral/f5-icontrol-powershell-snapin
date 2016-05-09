using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Management.Automation;
using System.Collections;

namespace iControlSnapIn.CmdLet
{
    public class iControlPSCmdlet : PSCmdlet
    {

        #region Parameters
        #endregion

        protected void handleError(String msg, String target)
        {
            Exception ex = new Exception(msg);
            ErrorRecord er = new ErrorRecord(ex, "2", ErrorCategory.PermissionDenied, target);
            WriteError(er);
        }

        protected void handleException(Exception ex)
        {
            ErrorRecord er = new ErrorRecord(ex, "2", ErrorCategory.OpenError, "error");
            WriteError(er);
        }

        protected void handleNotInitialized()
        {
            Exception ex = new Exception("You must first successfully call Initialize-iControl");
            ErrorRecord er = new ErrorRecord(ex, "2", ErrorCategory.PermissionDenied, "bad state");
            WriteError(er);
        }

        protected override void ProcessRecord()
        {
        }

        protected iControl.Interfaces GetiControl()
        {
            return Globals._interfaces;
        }

        protected bool isInitialized()
        {
            return (null != GetiControl());
        }

		protected bool matchString(String source, String pattern)
		{
			return matchString(source, pattern, false);
		}

		protected bool matchString(String source, String pattern, RegexOptions regExOpts)
		{
			return matchString(source, pattern, regExOpts.Equals(RegexOptions.IgnoreCase));
		}

		public static bool matchString(string source, string pattern, bool caseSensitive)
		{
			bool match = false;

			// Make input parameters lower-case if case is not an issue
			if (!caseSensitive)
			{
				source = source.ToLower();
				pattern = pattern.ToLower();
			}

			// Escape regexp special character in pattern
			pattern = pattern.Replace(".", @"\.");

			// Replace valid wildcards with regexp equivalents
			pattern = pattern.Replace('?', '.').Replace("*", ".*");

			// Add boundaries to pattern
			pattern = @"\A" + pattern + @"\z";

			// Search for a match
			try
			{
				match = Regex.IsMatch(source, pattern);
			}
			catch /* (ArgumentException ex) */
			{
				// Syntax error in the regular expression
			}

			// Return result
			return match;
		}
    }
}
