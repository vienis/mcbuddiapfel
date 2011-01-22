using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;

namespace CSDBCReader
{
    public class DBCXmlFile : DBCFile
    {
        DBCDataColumn[] columns;

        /// <summary>
        /// Creates a new DBCXmlFile instance, which reads a DBC file using meta information in an XML document.
        /// </summary>
        /// <param name="xmlFileName">The filename of the XML document to load.</param>
        public DBCXmlFile(string xmlFileName)
        {
            //-- Open, read and process the XML document contents
            Stream xmlStream = File.Open(xmlFileName, FileMode.Open);
            readXml(xmlStream);
            xmlStream.Close();

            //-- Execute the base custom ctor
            string dbcFile = deriveDBCFileNameFromXml(xmlFileName);
            base.LoadDBCFile(dbcFile, columns);
        }

        /// <summary>
        /// Derives the .dbc filename from the XML file name.
        /// </summary>
        /// <param name="xmlFileName">The filename of the loaded XML document</param>
        /// <returns>Returns the DBC file name</returns>
        string deriveDBCFileNameFromXml(string xmlFileName)
        {
            return System.IO.Path.GetFileNameWithoutExtension(xmlFileName) + ".dbc";
        }

        /// <summary>
        /// Returns the data table containing the DBC data
        /// </summary>
        /// <returns>A DBCDataTable containing the data.</returns>
        public override DBCDataTable GetDataTable()
        {
            return base.GetDataTable();
        }

        /// <summary>
        /// Parses an XML formatted type
        /// </summary>
        /// <param name="value">Type name</param>
        /// <returns>A Type</returns>
        Type parseXMLType(string value)
        {
            Type ret = typeof(object);
            switch (value.ToLower())
            {
                case "string":
                    ret = typeof(string);
                    break;
                case "string*":
                    ret = typeof(DBCBitMask);
                    break;
                case "integer":
                    ret = typeof(int);
                    break;
                case "integer*":
                    ret = typeof(int);
                    break;
                case "float":
                    ret = typeof(float);
                    break;
                case "float*":
                    ret = typeof(float);
                    break;
                case "bitmask":
                    ret = typeof(DBCBitMask);
                    break;
            }

            return ret;
        }

        /// <summary>
        /// Check if the given column name exists
        /// </summary>
        /// <param name="name">Column name to check on.</param>
        /// <returns>True if the column name already exists.</returns>
        bool doesColumnNameExist(string name)
        {
            foreach (DBCDataColumn column in columns)
            {
                if (!column.IsNull)
                {
                    if (column.Name.Equals(name))
                        return true;
                }
            }

            //-- Column name not found
            return false;
        }

        /// <summary>
        /// Reads the formatted XML document, supplying information about the DBC file to read.
        /// </summary>
        /// <param name="xmlStream">A stream containing the XML document.</param>
        void readXml(Stream xmlStream)
        {
            //-- Create a new XmlTextReader 
            XmlTextReader xml = new XmlTextReader(xmlStream);

            //-- Move to the content
            xml.MoveToContent();

            xml.ReadToDescendant("field_count");
            int fieldCount = int.Parse(xml.ReadElementContentAsString());
            columns = new DBCDataColumn[fieldCount];

            //-- Move to the 'fields' node
            bool found = false;
            while (xml.Read() && !found)
            {
                if (xml.LocalName == "fields")
                    break;
            }

            int index = 0;
            while (xml.Read())
            {
                if (xml.NodeType == XmlNodeType.Element)
                {
                    // Check if this node is a 'field' element
                    if (xml.LocalName != "field")
                        break;

                    // Try retrieving the column's name
                    string fieldName = null;
                    try { fieldName = xml.GetAttribute("name"); }
                    catch
                    {
                        throw new DBCException("Couldn't parse field attribute 'name' (" + xml.LineNumber + ", " + xml.LinePosition + ").");
                    }

                    // Try parsing the column's type
                    Type fieldType = null;
                    try {  fieldType = parseXMLType(xml.GetAttribute("type")); }
                    catch
                    {
                        throw new DBCException("Couldn't parse field attribute 'type' (" + xml.LineNumber + ", " + xml.LinePosition + ").");
                    }

                    // Check if this column name has already been taken
                    if (doesColumnNameExist(fieldName))
                        throw new DBCException("Ambigious field name '" + fieldName + "' (" + xml.LineNumber + ", " + xml.LinePosition + ").");

                    // Set this column with the associating information
                    columns[index] = new DBCDataColumn(fieldName, fieldType, fieldType == typeof(string));

                    // Increase the column pointer
                    index++;
                }       
            }
        }
    }
}