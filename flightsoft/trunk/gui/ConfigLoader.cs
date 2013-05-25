using System;
using System.IO;

namespace gui
{
    class ConfigLoader
    {
        public const long Default_GreenSpace = 1024L * 1024L * 1024L * 100L;
        public const long Default_YellowSpace = 1024L * 1024L * 1024L * 50L;
        public const long Default_RedSpace = 1024L * 1024L * 1024L * 20L;

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
                if(config.GreenSpace<Default_GreenSpace)
                    config.GreenSpace *= 1024 * 1024 * 1024;
                if(config.YellowSpace<Default_YellowSpace)
                    config.YellowSpace *= 1024 * 1024 * 1024;   
                if(config.RedSpace<Default_RedSpace)
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
            config.GreenSpace = Default_GreenSpace;
            config.YellowSpace = Default_YellowSpace;
            config.RedSpace = Default_RedSpace;
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
