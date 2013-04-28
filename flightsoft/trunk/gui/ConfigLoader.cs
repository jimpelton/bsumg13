using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace gui
{
    class ConfigLoader
    {

        public ConfigLoader()
        {

        }

        public static ConfigData LoadConfig(string configpath)
        {
            ConfigData config = new ConfigData();
            StreamReader reader = new StreamReader(configpath);
            string Line = "";
            try
            {
                do
                {
                    Line = reader.ReadLine();
                    string[] tokens = Line.Split(',');
                    if(tokens.Length>1&&tokens[0].Equals("Path"))
                        config.Path = tokens[1];
                }
                while (reader.Peek() != -1);
            }

            catch
            {
                
            }

            finally
            {
                reader.Close();
            }
             return config;
        }
       
    }

    public class ConfigData
    {
        public string Path
        {
           get;
           set;
        }
    }
}
