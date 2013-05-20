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
            StreamReader reader = null;
            configDefaults(config);
            string Line = "";
            try
            {
                reader = new StreamReader(configpath);
                Line = reader.ReadToEnd();
                string[] tokens = Line.Split(',');
                int spot = 0;
                do
                {
                    if (tokens[spot].Equals("Path"))
                        config.Path = tokens[spot + 1];
                    if (tokens[spot].Equals("GreenSpace"))
                        config.GreenSpace = ulong.Parse(tokens[spot + 1]);
                    if (tokens[spot].Equals("YellowSpace"))
                        config.YellowSpace = ulong.Parse(tokens[spot + 1]);
                    if (tokens[spot].Equals("RedSpace"))
                        config.RedSpace = ulong.Parse(tokens[spot + 1]);
                    if (tokens[spot].Equals("GreenBattery"))
                        config.GreenBattery = int.Parse(tokens[spot + 1]);
                    if (tokens[spot].Equals("YellowBattery"))
                        config.YellowBattery = int.Parse(tokens[spot + 1]);
                    if (tokens[spot].Equals("RedBattery"))
                        config.RedBattery = int.Parse(tokens[spot + 1]);
                    spot++;
                }
                while (spot<tokens.Length);
                config.GreenSpace *= 1024 * 1024 * 1024;
                config.YellowSpace *= 1024 * 1024 * 1024;
                config.RedSpace *= 1024 * 1024 * 1024;
            }
            catch (Exception)
            {
                configDefaults(config);//if it craps out set to defaults.
            }
            finally
            {
                if (reader != null)
                    reader.Close();
            }
            return config;
        }

        private static void configDefaults(ConfigData config)
        {
            config.Path = @"C:\Data";
            config.GreenSpace = 1024L * 1024L * 1024L * 100L;
            config.YellowSpace = 1024L * 1024L * 1024L * 50L;
            config.RedSpace = 1024L * 1024L * 1024L * 20L;
            config.GreenBattery = 95;
            config.YellowBattery = 85;
            config.RedBattery = 50;
        }
       
    }

    public class ConfigData
    {

        public string Path
        {
           get;
           set;
        }

        public ulong GreenSpace
        {
            get;
            set;
        }

        public ulong YellowSpace
        {
            get;
            set;
        }

        public ulong RedSpace
        {
            get;
            set;
        }

        public int GreenBattery
        {
            get;
            set;
        }

        public int YellowBattery
        {
            get;
            set;
        }

        public int RedBattery
        {
            get;
            set;
        }

    }
}
