# region Includes

using System;
using System.Collections.Generic;
using System.Management;

# endregion

namespace RobX.Communication.COM
{
    /// <summary>
    /// This class maintains information about each COM port.
    /// </summary>
    public class COMPort
    {
        # region Public Fields

        /// <summary>
        /// Name of the COM port. Usually is similar to "COM...".
        /// </summary>
        public string Name;

        /// <summary>
        /// Description (Full name) associated with the COM port. 
        /// </summary>
        public string Description;

        # endregion

        # region Public Static Functions

        /// <summary>
        /// Gets all the active COM ports of the host. 
        /// </summary>
        /// <returns>The list of all active COM ports of the host.</returns>
        public static List<COMPort> GetCOMPorts()
        {
            List<COMPort> comPortInfoList = new List<COMPort>();

            // Process WMI connection options
            ConnectionOptions options = new ConnectionOptions();
            options.Impersonation = ImpersonationLevel.Impersonate;
            options.Authentication = AuthenticationLevel.Default;
            options.EnablePrivileges = true;

            // Define scope for WMI management operations
            ManagementScope connectionScope = new ManagementScope();
            connectionScope.Path = new ManagementPath(@"\\" + Environment.MachineName + @"\root\CIMV2");
            connectionScope.Options = options;
            connectionScope.Connect();

            // Define management query that returns instances
            ObjectQuery objectQuery = new ObjectQuery("SELECT * FROM Win32_PnPEntity WHERE ConfigManagerErrorCode = 0");
            ManagementObjectSearcher comPortSearcher = new ManagementObjectSearcher(connectionScope, objectQuery);

            // Finds Win32_PnPEntities that are COM devices (connected to COM ports and have "COM" in their names)
            using (comPortSearcher)
            {
                string caption = null;
                foreach (ManagementObject obj in comPortSearcher.Get())
                {
                    if (obj == null) continue;
                    object captionObj = obj["Caption"];
                    if (captionObj == null) continue;

                    caption = captionObj.ToString();
                    if (caption.Contains("(COM"))
                    {
                        COMPort comPortInfo = new COMPort();
                        comPortInfo.Name = caption.Substring(caption.LastIndexOf("(COM")).Replace(
                            "(", string.Empty).Replace(")", string.Empty);
                        comPortInfo.Description = caption;
                        comPortInfoList.Add(comPortInfo);
                    }
                }
            }
            return comPortInfoList;
        }

        # endregion
    }
}