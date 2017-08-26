using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;

namespace GMS2TranslationFileInstaller
{
    class Version
    {
        private int major;
        public int Major { get => major; set => major = value; }

        private int minor;
        public int Minor { get => minor; set => minor = value; }

        private int revision;
        public int Revision { get => revision; set => revision = value; }

        private int build;
        public int Build { get => build; set => build = value; }

        public Version()
        { }
        public Version(int major,int minor,int revision,int build)
        {
            this.major = major;
            this.minor = minor;
            this.revision = revision;
            this.build = build;
        }
        public Version(string strVer)
        {
            Regex regex = new Regex(@"(\d+)\.(\d+)\.(\d+)\.(\d+)");
            string[] list = new string[4]; 
            if (regex.IsMatch(strVer))
            {
                strVer = regex.Match(strVer).ToString();
                list = strVer.Split('.');
                Major = Convert.ToInt32(list[0]);
                Minor = Convert.ToInt32(list[1]);
                Revision = Convert.ToInt32(list[2]);
                Build = Convert.ToInt32(list[3]);
            }
        }

        public override string ToString()
        {
            return String.Format("{0}.{1}.{2}.{3}",Major,Minor,Revision,Build);
        }
        
        public static bool operator<(Version ver1,Version ver2)
        {
            if(ver1.Major<ver2.Major)
            {
                return true;
            }
            else if(ver1.Major == ver2.Major)
            {
                if(ver1.Minor < ver2.Minor)
                {
                    return true;
                }
                else if(ver1.Minor == ver2.Minor)
                {
                    if(ver1.Revision < ver2.Revision)
                    {
                        return true;
                    }
                    else if(ver1.Revision == ver2.Revision)
                    {
                        if(ver1.Build < ver2.Build)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }
        public static bool operator>(Version ver1,Version ver2)
        {
            if (ver1.Major > ver2.Major)
            {
                return true;
            }
            else if (ver1.Major == ver2.Major)
            {
                if (ver1.Minor > ver2.Minor)
                {
                    return true;
                }
                else if (ver1.Minor == ver2.Minor)
                {
                    if (ver1.Revision > ver2.Revision)
                    {
                        return true;
                    }
                    else if (ver1.Revision == ver2.Revision)
                    {
                        if (ver1.Build > ver2.Build)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;

        }
        public static bool operator==(Version ver1,Version ver2)
        {
            return ver1.Equals(ver2);
        }
        public static bool operator!=(Version ver1,Version ver2)
        {
            return !(ver1 == ver2);
        }
        public static bool operator<=(Version ver1,Version ver2)
        {
            return !(ver1 > ver2);
        }
        public static bool operator>=(Version ver1,Version ver2)
        {
            return !(ver1 < ver2);
        }

        


    }
}
