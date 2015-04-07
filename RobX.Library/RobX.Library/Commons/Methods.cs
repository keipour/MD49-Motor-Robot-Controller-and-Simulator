# region Includes

using System;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Net;
using System.Xml;

# endregion

namespace RobX.Library.Commons
{
    /// <summary>
    /// This class contains public static methods used in the project.
    /// </summary>
    public static class Methods
    {
        # region Number Conversion Methods

        /// <summary>
        /// Reads an unsigned 8-bit integer as a signed 8-bit integer.
        /// </summary>
        /// <param name="number">An 8-bit unsigned integer.</param>
        /// <returns>Returns signed integer</returns>
        public static sbyte ConvertUnsignedByteToSigned(int number)
        {
            number = number % 256;
            if (number <= 127)
                return (sbyte)number;
            
            return (sbyte)(number - 256);
        }

        /// <summary>
        /// Reads a signed 8-bit integer as an unsigned 8-bit integer.
        /// </summary>
        /// <param name="number">An 8-bit signed integer.</param>
        /// <returns>Returns unsigned integer (in range 0-255).</returns>
        public static byte ConvertSignedByteToUnsigned(int number)
        {
            return (byte)(((number % 256) + 256) % 256);
        }

        # endregion

        # region String Validation Methods

        /// <summary>
        /// This method checks if the input string is a valid TCP port number.
        /// </summary>
        /// <returns>Returns true if the input string is a valid TCP port number; otherwise returns false.</returns>
        public static bool IsValidPort(string str)
        {
            ushort port;
            return ushort.TryParse(str, out port) && port >= 2;
        }

        /// <summary>
        /// This method checks if the input string is a valid IP address.
        /// </summary>
        /// <returns>Returns true if the input string is a valid IP address; otherwise returns false.</returns>
        public static bool IsValidIpAddress(string str)
        {
            IPAddress ip;
            return IPAddress.TryParse(str, out ip);
        }

        # endregion

        # region String and File Manipulation Methods

        /// <summary>
        /// Reads a text file and replaces all special text keys with their equivalent values in the (key, value) pair.
        /// </summary>
        /// <param name="fileName">Path and name of the text file that should be read.</param>
        /// <param name="startTag">Starting tag to specify the special key.</param>
        /// <param name="endTag">Ending tag to specify the special key.</param>
        /// <param name="replaceParameters">The collection of (key, value) pairs.</param>
        /// <returns>The text of the file with replaced (key, value) pairs.</returns>
        public static string ReadFormattedFile(string fileName, string startTag, string endTag,
            NameValueCollection replaceParameters = null)
        {
            try
            {
                var text = File.ReadAllText(fileName);

                if (replaceParameters == null) return text;

                for (var i = 0; i < replaceParameters.Count; ++i)
                    text = text.Replace(startTag + replaceParameters.GetKey(i) + endTag, replaceParameters[i]);

                return text;

            }
            catch (Exception)
            {
                return String.Empty;
            }
        }

        # endregion

        # region Settings Loader

        /// <summary>
        /// Reads settings from application configuration.
        /// </summary>
        /// <param name="applicationPath">Path to the executable file of the application.</param>
        /// <param name="sectionName">XML path to the settings section of the application configuration. For example: 
        /// applicationSettings/MyProject.Properties.Settings. Check the app.cpnfig to get an idea about the path.</param>
        /// <returns>Name-value pairs of the settings read by the function.</returns>
        public static NameValueCollection GetApplicationSettings(string applicationPath, string sectionName)
        {
            try
            {
                // Read application settings from application exe
                var config = ConfigurationManager.OpenExeConfiguration(applicationPath);

                // Read application settings section from application configuration
                var myParamsSection = config.GetSection(sectionName);

                // Convert application configuration to XML
                var myParamsSectionRawXml = myParamsSection.SectionInformation.GetRawXml();
                var sectionXmlDoc = new XmlDocument();
                sectionXmlDoc.Load(new StringReader(myParamsSectionRawXml));

                // Read name-value pairs from XML
                var resultNameValueCol = new NameValueCollection();
                if (sectionXmlDoc.DocumentElement == null) return resultNameValueCol;

                var xmlNodeList = sectionXmlDoc.DocumentElement.SelectNodes("//setting");

                if (xmlNodeList == null) return resultNameValueCol;

                foreach (XmlNode node in xmlNodeList)
                {
                    var nameNode = node.SelectSingleNode("@name");
                    if (nameNode == null) continue;

                    resultNameValueCol.Add(nameNode.Value, node.InnerText);
                }
                return resultNameValueCol;
            }
            catch (Exception)
            {
                return null;
            }
        }

        # endregion
    }
}
