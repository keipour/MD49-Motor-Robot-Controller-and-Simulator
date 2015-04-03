# region Includes

using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;

# endregion

namespace RobX.Library.Communication.COM
{
    /// <summary>
    /// This class maintains information about each COM port.
    /// </summary>
    public class ComPort
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
        public static List<ComPort> GetComPorts()
        {
            var comPortInfoList = new List<ComPort>();

            // Process WMI connection options
            var options = new ConnectionOptions
            {
                Impersonation = ImpersonationLevel.Impersonate,
                Authentication = AuthenticationLevel.Default,
                EnablePrivileges = true
            };

            // Define scope for WMI management operations
            var connectionScope = new ManagementScope
            {
                Path = new ManagementPath(@"\\" + Environment.MachineName + @"\root\CIMV2"),
                Options = options
            };

            connectionScope.Connect();

            // Define management query that returns instances
            var objectQuery = new ObjectQuery("SELECT * FROM Win32_PnPEntity WHERE ConfigManagerErrorCode = 0");
            var comPortSearcher = new ManagementObjectSearcher(connectionScope, objectQuery);

            // Finds Win32_PnPEntities that are COM devices (connected to COM ports and have "COM" in their names)
            using (comPortSearcher)
            {
                comPortInfoList.AddRange(from ManagementObject obj in comPortSearcher.Get()
                    where obj != null
                    select obj["Caption"]
                    into captionObj
                    where captionObj != null
                    select captionObj.ToString()
                    into caption
                    where caption.Contains("(COM")
                    select new ComPort
                    {
                        Name = caption.Substring(caption.LastIndexOf("(COM", StringComparison.Ordinal)).Replace("(", string.Empty).Replace(")", string.Empty), Description = caption
                    });
            }
            return comPortInfoList;
        }

        # endregion
    }
}