using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;

namespace GMS2GiMiSi
{

    class Version
    {
        public enum Level
        {
            Major,
            Minor,
            Revision,
            Build
        }

        public int Major { get; set; }

        public int Minor { get; set; }

        public int Revision { get; set; }

        public int Build { get; set; }

        /// <summary>
        /// 版本号
        /// </summary>
        public Version()
        { }

        /// <summary>
        /// 版本号
        /// </summary>
        /// <param name="major">major值</param>
        /// <param name="minor">minor值</param>
        /// <param name="revision">revision值</param>
        /// <param name="build">build值</param>
        public Version(int major, int minor, int revision, int build)
        {
            this.Major = major;
            this.Minor = minor;
            this.Revision = revision;
            this.Build = build;
        }
        
        /// <summary>
        /// 版本号
        /// </summary>
        /// <param name="strVer">版本号字符串</param>
        public Version(string strVer)
        {
            Regex regex = new Regex(@"(\d+)(\.(\d+)){0,3}");
            string[] list = { "0","0","0","0"};
            //int cnt = 0;
            try
            {
                if (regex.IsMatch(strVer))
                {
                    strVer = regex.Match(strVer).ToString();
                    strVer.Split('.').CopyTo(list,0);
                    /*foreach (string str in strVer.Split('.'))
                    {
                        list[cnt++] = str;
                    }*/
                    Major = Convert.ToInt32(list[0]);
                    Minor = Convert.ToInt32(list[1]);
                    Revision = Convert.ToInt32(list[2]);
                    Build = Convert.ToInt32(list[3]);
                }
                else
                {
                    throw new VersionFormatInvalid();
                }
            }
            catch(VersionFormatInvalid)
            {
                MessageBox.Show("版本号格式异常");
            }
        }

        /// <summary>
        /// 版本号转换为字符串
        /// </summary>
        /// <returns>版本号字符串</returns>
        public override string ToString()
        {
            return $"{Major}.{Minor}.{Revision}.{Build}";
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
            return ver1.Major == ver2.Major && ver1.Minor == ver2.Minor && ver1.Revision == ver2.Revision && ver1.Build == ver2.Build;
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

        public override bool Equals(object obj)
        {
            if(obj == null || obj.GetType() != typeof(Version))
            {
                return false;
            }
            else
            {
                return ToString() == (obj as Version)?.ToString();
            }
        }
        public override int GetHashCode() => base.GetHashCode();

        class VersionFormatInvalid : Exception
        {

        }

    }

    
}
