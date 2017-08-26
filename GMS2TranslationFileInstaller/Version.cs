using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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



    }
}
