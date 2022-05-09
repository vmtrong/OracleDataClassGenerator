using System;

namespace OracleDataClassGenerator.Engine
{
    public class Definition
    {
        public struct VarContent
        {
            public string Name;

            public string Type;
        }

        private static string classInfoDefine = string.Empty;

        private static string classAccessDefine = string.Empty;

        private static string classCachingDefine = string.Empty;

        private static string nameSpaceDefine = string.Empty;

        private static string connectionStringDefine = string.Empty;

        public static string ClassInfoDefine
        {
            get
            {
                return Definition.classInfoDefine;
            }
            set
            {
                Definition.classInfoDefine = value;
            }
        }

        public static string ClassAccessDefine
        {
            get
            {
                return Definition.classAccessDefine;
            }
            set
            {
                Definition.classAccessDefine = value;
            }
        }

        public static string ClassCachingDefine
        {
            get
            {
                return Definition.classCachingDefine;
            }
            set
            {
                Definition.classCachingDefine = value;
            }
        }

        public static string NameSpaceDefine
        {
            get
            {
                return Definition.nameSpaceDefine;
            }
            set
            {
                Definition.nameSpaceDefine = value;
            }
        }

        public static string ConnectionStringDefine
        {
            get
            {
                return Definition.connectionStringDefine;
            }
            set
            {
                Definition.connectionStringDefine = value;
            }
        }
    }
}
