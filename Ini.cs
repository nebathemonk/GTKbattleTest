using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Collections.Generic;

namespace Ini
{
    /// <summary>
    /// Create a New INI file to store or load data
    /// </summary>
    public class IniFile
    {
        public string path;

        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section,
                                                             string key, string val, string filePath);
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section,
                                                          string key, string def, StringBuilder retVal,
                                                          int size, string filePath);
        //[DllImport("kernel32")]
        //private static extern List<string> GetPrivateProfileSectionNames(List<string> section,
        //                                                     int size, string filePath);

        /// <summary>
        /// INIFile Constructor.
        /// </summary>
        /// <PARAM name="INIPath"></PARAM>
        public IniFile(string INIPath)
        {
            path = INIPath;
			//Console.WriteLine (path);
        }
        /// <summary>
        /// Write Data to the INI File
        /// </summary>
        /// <PARAM name="Section"></PARAM>
        /// Section name
        /// <PARAM name="Key"></PARAM>
        /// Key Name
        /// <PARAM name="Value"></PARAM>
        /// Value Name
        public void IniWriteValue(string Section, string Key, string Value)
        {
            WritePrivateProfileString(Section, Key, Value, this.path);

			//open file
			//check for key string
			//if it is already there, overwrite its value
			//if it is not, amend it to the file
        }

        /// <summary>
        /// Read Data Value From the Ini File
        /// </summary>
        /// <PARAM name="Section"></PARAM>
        /// <PARAM name="Key"></PARAM>
        /// <PARAM name="Path"></PARAM>
        /// <returns></returns>
        public string IniReadValue(string Section, string Key)
        {
            /*StringBuilder temp = new StringBuilder(255);
            int i = GetPrivateProfileString(Section, Key, "", temp,
                                            255, this.path);
			*/
			string tempLine;
			string value = null;

			using(System.IO.StreamReader file = new System.IO.StreamReader(this.path))
			{
				while((tempLine = file.ReadLine()) != null){
					//tempLine = file.ReadLine();
					string[] things = tempLine.Split (new Char[]{'='},StringSplitOptions.None);
					if(things[0]==Key){value=things[1];}
					//should set value to whatever is past = when the key matches
				}
			}

            return value;
        }

    }
}

/*
 Using the class
Steps to use the Ini class:

In your project namespace definition add 

using INI;

Create a INIFile like this

INIFile ini = new INIFile("C:\\test.ini");

Use IniWriteValue to write a new value to a specific key in a section or use IniReadValue to read a value FROM a key in a specific Section.
That's all. It's very easy in C# to include API functions in your class(es).
*/