using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Security;
using System.Text;
using UnityEngine;
#if UNITY_7000_0_OR_NEWER
using Unity.Scripting.LifecycleManagement;
#endif

namespace LitMotion.Editor
{
#if UNITY_7000_0_OR_NEWER
    [NoAutoStaticsCleanup] // Reusable scratch buffer; it is cleared before every use.
#endif
    internal static class StackTraceHelper
    {
        static readonly StringBuilder sb = new();

        public static string AddHyperLink(this StackTrace stackTrace)
        {
            if (stackTrace == null) return "";

            sb.Clear();
            for (int i = 0; i < stackTrace.FrameCount; i++)
            {
                var sf = stackTrace.GetFrame(i);

                if (sf.GetILOffset() != -1)
                {
                    string fileName = null;
                    try
                    {
                        fileName = sf.GetFileName();
                    }
                    catch (NotSupportedException) { }
                    catch (SecurityException) { }

                    if (fileName != null)
                    {
                        sb.Append(' ');
                        sb.AppendFormat(CultureInfo.InvariantCulture, "(at {0})", AppendHyperLink(fileName, sf.GetFileLineNumber().ToString()));
                        sb.AppendLine();
                    }
                }
            }
            return sb.ToString();
        }

        static string AppendHyperLink(string path, string line)
        {
            var fi = new FileInfo(path);
            if (fi.Directory == null)
            {
                return fi.Name;
            }
            else
            {
                var fname = fi.FullName.Replace(Path.DirectorySeparatorChar, '/').Replace(Application.dataPath, "");
                var withAssetsPath = "Assets/" + fname;
                return "<a href=\"" + withAssetsPath + "\" line=\"" + line + "\">" + withAssetsPath + ":" + line + "</a>";
            }
        }
    }

}