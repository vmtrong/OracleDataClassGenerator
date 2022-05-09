
using Oracle.ManagedDataAccess.Client;
using System;
using System.Data;
using System.Text;

namespace OracleDataClassGenerator.Engine
{
    public class Generator
    {
        public static DataTable GetTableSchema(string tableName)
        {
            DataTable schemaTable;
            OracleConnection con = new OracleConnection(AppUtil.ConnectionString);
            con.Open();

            string cmdstr = "SELECT * FROM " + tableName + " where 1=0";
            OracleCommand cmd = new OracleCommand(cmdstr, con);

            //get the reader
            OracleDataReader reader = cmd.ExecuteReader();

            //get the schema table
            schemaTable = reader.GetSchemaTable();

            reader.Close();

            // Close the connection
            con.Close();


            return schemaTable;
        }
        public static DataTable GetAll_Tab_Columns(string tableName, string userName)
        {
            DataTable schemaTable = new DataTable();
            OracleConnection con = new OracleConnection(AppUtil.ConnectionString);
            con.Open();

            string cmdstr = "SELECT  column_name, data_type, data_length FROM all_tab_columns where owner = :owner and table_name = :tableName";
            OracleCommand cmd = new OracleCommand(cmdstr, con);
            cmd.Parameters.Add("owner", OracleDbType.Varchar2).Value = userName.ToUpper();
            cmd.Parameters.Add("tableName", OracleDbType.Varchar2).Value = tableName;
            //get the reader
            OracleDataAdapter oda = new OracleDataAdapter(cmd);
            oda.Fill(schemaTable);


            // Close the connection
            con.Close();


            return schemaTable;
        }

        public static DataTable GetTableData()
        {
            DataTable dt = new DataTable();
            OracleConnection con = new OracleConnection(AppUtil.ConnectionString);
            con.Open();

            string cmdstr = "SELECT * FROM " + AppUtil.TableName;
            OracleCommand cmd = new OracleCommand(cmdstr, con);

            OracleDataAdapter oda = new OracleDataAdapter(cmd);
            oda.Fill(dt);

            // Close the connection
            con.Close();

            return dt;
        }
        public static string ToSchama()
        {
            string info = "";
            DataTable dt = new DataTable();
            OracleConnection con = new OracleConnection(AppUtil.ConnectionString);
            con.Open();

            string cmdstr = "select dbms_metadata.get_ddl('TABLE','"+ AppUtil.TableName + "') from dual";
            OracleCommand cmd = new OracleCommand(cmdstr, con);

            OracleDataAdapter oda = new OracleDataAdapter(cmd);
            oda.Fill(dt);
            if (dt.Rows.Count > 0)
                info = dt.Rows[0][0].ToString();
            // Close the connection
            con.Close();

            return info;
        }
        public static string ToPackage(string PackageName)
        {
            string info = "";
            DataTable dt = new DataTable();
            OracleConnection con = new OracleConnection(AppUtil.ConnectionString);
            con.Open();

            string cmdstr = "select dbms_metadata.get_ddl('PACKAGE','" + PackageName + "') from dual";
            OracleCommand cmd = new OracleCommand(cmdstr, con);

            OracleDataAdapter oda = new OracleDataAdapter(cmd);
            oda.Fill(dt);
            if (dt.Rows.Count > 0)
                info = dt.Rows[0][0].ToString();
            // Close the connection
            con.Close();

            return info;
        }

        public static string ToContainerClass(DataTable _table)
        {
            string info = "using System;\r\nnamespace " + Definition.NameSpaceDefine;
            info += "\r\n{\r\n";
            info += "\tpublic class " + Definition.ClassInfoDefine + " : StandardInfo";
            info += "\r\n\t{";

            for (int i = 0; i < _table.Rows.Count; i++)
            {
                string varType = _table.Rows[i]["DATATYPE"].ToString().Replace("System.", "").Trim();
                string varPublic = _table.Rows[i]["COLUMNNAME"].ToString();
                if (!varPublic.Contains("COLUMN"))
                    info += "\r\n\t\t public " + varType + " " + varPublic + " { get; set; }";
            }
            info += "\r\n\t}";
            return info + "\r\n}";
        }
        public static string ToAccessClass(DataTable _table)
        {
            string firtColumn = _table.Rows[0]["COLUMNNAME"].ToString();
            if (firtColumn != "ID")
                firtColumn = _table.Rows[0]["COLUMNNAME"].ToString();
            StringBuilder info = new StringBuilder();
            info.AppendLine("using System;");
            info.AppendLine("using System.Data;");
            info.AppendLine("using System.Data.OracleClient;");
            info.AppendLine("namespace " + Definition.NameSpaceDefine);
            info.AppendLine("{");
            info.AppendLine("\tpublic class " + Definition.ClassAccessDefine);
            info.AppendLine("\t{");
            //Get paged
            //GetAll
            info.AppendLine("\t\tpublic DataTable GetPaged(string keyWord, int currentPage, int pageSize, out int itemCount)");
            info.AppendLine("\t\t{");
            info.AppendLine("\t\t\tDataTable retVal = null;");
            info.AppendLine("\t\t\tvar conn = DataConnector.GetOracleConnection();");
            info.AppendLine("\t\t\tvar cmd = new OracleCommand(\"PCK_" + AppUtil.TableName + ".PRC_GET_PAGED\", conn);");
            info.AppendLine("\t\t\tcmd.CommandType = CommandType.StoredProcedure;");
            info.AppendLine("\t\t\tcmd.Parameters.Add(new OracleParameter(\"P_KEYWORD\", keyWord));");
            info.AppendLine("\t\t\tcmd.Parameters.Add(new OracleParameter(\"P_CURRENTPAGE\", currentPage));");
            info.AppendLine("\t\t\tcmd.Parameters.Add(new OracleParameter(\"P_PAGESIZE\", pageSize));\n");
            info.AppendLine("\t\t\tvar outputParam = new OracleParameter(\"P_ROWCOUNT\", OracleType.Number);");
            info.AppendLine("\t\t\toutputParam.Direction = ParameterDirection.Output;");
            info.AppendLine("\t\t\tcmd.Parameters.Add(outputParam);\n");
            info.AppendLine("\t\t\tvar outputParam2 = new OracleParameter(\"P_OUT\", OracleType.Cursor);");
            info.AppendLine("\t\t\toutputParam2.Direction = ParameterDirection.Output;");
            info.AppendLine("\t\t\tcmd.Parameters.Add(outputParam2);\n");

            info.AppendLine("\t\t\ttry");
            info.AppendLine("\t\t\t{");
            info.AppendLine("\t\t\t\tretVal = new DataTable();");
            info.AppendLine("\t\t\t\tvar da = new OracleDataAdapter(cmd);");
            info.AppendLine("\t\t\t\tda.Fill(retVal);");
            info.AppendLine("\t\t\t\titemCount = Convert.ToInt32(outputParam.Value);");

            info.AppendLine("\t\t\t}");
            info.AppendLine("\t\t\tcatch (Exception ex)");
            info.AppendLine("\t\t\t{");
            info.AppendLine("\t\t\t\tthrow ex;");
            info.AppendLine("\t\t\t}");
            info.AppendLine("\t\t\tfinally");
            info.AppendLine("\t\t\t{");
            info.AppendLine("\t\t\t\tconn.Close();");
            info.AppendLine("\t\t\t\t");
            info.AppendLine("\t\t\t}");
            info.AppendLine("\t\t\treturn retVal;");
            info.AppendLine("\t\t}");


            //insert
            info.AppendLine("\t\tpublic static decimal Insert (" + Definition.ClassInfoDefine + " entity)");
            info.AppendLine("\t\t{");
            info.AppendLine("\t\t\tdecimal id = 0;");

            StringBuilder strFields = new StringBuilder();
            info.AppendLine("\t\t\t/*");
            info.Append("\t\t\tvar sql = @\"INSERT INTO " + AppUtil.TableName + "(");


            int countList = _table.Rows.Count;
            int limited = 8;// 1 lần chứa 8 dòng
            if (countList > limited)
            {
                var numberOfSplits = (double)countList / limited;//Số lần tách

                numberOfSplits = Math.Truncate(numberOfSplits);//bỏ qua phần không nguyên của số thập phân (không giống như làm tròn)
                for (int j = 0; j < numberOfSplits; j++)
                {
                    int start = limited * j;
                    int end = limited * (j + 1) - 1;

                    //x = 500 * 0; y = 500 * (0 + 1) - 1 = 499
                    //x = 500 * 1; y = 500 * (1 + 1) - 1 = 999
                    //x = 500 * 2; y = 500 * (2 + 1) - 1 = 1499

                    strFields.AppendLine();
                    strFields.Append("\t\t\t\t");
                    for (int k = start; k <= end; k++)
                    {
                        var paramName = _table.Rows[k]["COLUMNNAME"].ToString();
                        if (!paramName.Contains("COLUMN"))
                            strFields.Append(", " + paramName);
                    }

                }

                // Xử lý phần còn dư
                var mod = (double)countList % limited; // phần dư của phép chia
                if (mod > 0)
                {
                    strFields.AppendLine();
                    strFields.Append("\t\t\t\t");
                    for (int j = countList - (int)mod; j < countList; j++)
                    {
                        var paramName = _table.Rows[j]["COLUMNNAME"].ToString();
                        if (!paramName.Contains("COLUMN"))
                            strFields.Append(", " + paramName);
                    }
                }
            }
            else
            {
                strFields.AppendLine();
                strFields.Append("\t\t\t\t");
                for (int j = 0; j < countList; j++)
                {
                    var paramName = _table.Rows[j]["COLUMNNAME"].ToString();
                    if (!paramName.Contains("COLUMN"))
                        strFields.Append(", " + paramName);
                }

            }
            int startIndex = strFields.ToString().IndexOf(",") + 2;
            info.AppendLine(strFields.ToString().Substring(startIndex) + ") ");
            info.AppendLine("\t\t\t\tVALUES (" + strFields.ToString().Substring(startIndex).Replace(", ", ", :P_") + ") ");
            if (firtColumn == "ID")
                info.AppendLine("\t\t\t\tRETURNING ID INTO :P_ID\";");
            else
                info.AppendLine("\t\t\t\t\";");

            info.AppendLine("\t\t\tvar conn = DataConnector.GetOracleConnection();");
            info.AppendLine("\t\t\tvar cmd = new OracleCommand(sql, conn);");
            info.AppendLine("\t\t\t*/");

            info.AppendLine("\t\t\tvar conn = DataConnector.GetOracleConnection();");
            info.AppendLine("\t\t\tvar cmd = new OracleCommand(\"PKG_" + AppUtil.TableName + ".PRC_INSERT\", conn);");
            info.AppendLine("\t\t\tcmd.CommandType = CommandType.StoredProcedure;");
            for (int i = 0; i < _table.Rows.Count; i++)
            {
                string paramName = _table.Rows[i]["COLUMNNAME"].ToString();
                if (!paramName.Equals("ID") && !paramName.Contains("COLUMN"))
                {
                    string paramType = _table.Rows[i]["DATATYPE"].ToString().Replace("System.", "").Trim();
                    if (paramType == "DateTime")
                    {
                        /*
                        info.AppendLine("\t\t\tif (entity." + paramName + " == DateTime.MinValue)");
                        info.AppendLine("\t\t\t\tcmd.Parameters.Add(new OracleParameter(\"P_" + paramName + "\", DBNull.Value));");
                        info.AppendLine("\t\t\telse");
                        info.AppendLine("\t\t\t\tcmd.Parameters.Add(new OracleParameter(\"P_" + paramName + "\", entity." + paramName + "));");
                        */
                        info.AppendLine("\t\t\tcmd.Parameters.Add(new OracleParameter(\"P_" + paramName + "\", AppUtil.DateTimeToDBObject(entity." + paramName + ")));");
                    }
                    if (paramType == "String")
                    {
                        info.AppendLine("\t\t\tcmd.Parameters.Add(new OracleParameter(\"P_" + paramName + "\", AppUtil.ToString(entity." + paramName + ")));");
                    }
                    else
                        info.AppendLine("\t\t\tcmd.Parameters.Add(new OracleParameter(\"P_" + paramName + "\", entity." + paramName + "));");
                }
            }
            info.AppendLine();
            if (firtColumn == "ID")
            {
                info.AppendLine("\t\t\tvar outputParam = new OracleParameter(\"P_ID\", OracleType.Number);");
                info.AppendLine("\t\t\toutputParam.Direction = ParameterDirection.Output;");
                info.AppendLine("\t\t\tcmd.Parameters.Add(outputParam);");
            }
            info.AppendLine();

            info.AppendLine("\t\t\ttry");
            info.AppendLine("\t\t\t{");
            info.AppendLine("\t\t\t\tif (conn.State != ConnectionState.Open)");
            info.AppendLine("\t\t\t\t\tconn.Open();");
            info.AppendLine("\t\t\t\tcmd.ExecuteNonQuery();");
            if (firtColumn == "ID")
                info.AppendLine("\t\t\t\tid = Convert.ToDecimal(outputParam.Value);");
            else
                info.AppendLine("\t\t\t\tid = 1;");

            info.AppendLine("\t\t\t}");
            info.AppendLine("\t\t\tcatch (Exception ex)");
            info.AppendLine("\t\t\t{");
            info.AppendLine("\t\t\t\tthrow ex;");
            info.AppendLine("\t\t\t}");
            info.AppendLine("\t\t\tfinally");
            info.AppendLine("\t\t\t{");
            info.AppendLine("\t\t\t\tconn.Close();");
            info.AppendLine("\t\t\t\t");
            info.AppendLine("\t\t\t}");
            info.AppendLine("\t\t\treturn id;");
            info.AppendLine("\t\t}");

            //update
            info.AppendLine("\t\tpublic static bool Update (" + Definition.ClassInfoDefine + " entity)");
            info.AppendLine("\t\t{");
            info.AppendLine("\t\t\tbool retVal = false;");

            info.AppendLine("\t\t\t/*");
            info.Append("\t\t\tvar sql = @\"UPDATE " + AppUtil.TableName + " SET ");
            strFields = new StringBuilder();
            for (int i = 0; i < _table.Rows.Count; i++)
            {
                string paramName = _table.Rows[i]["COLUMNNAME"].ToString();
                if (!paramName.Equals("ID") && !paramName.Contains("COLUMN") && !paramName.Equals(firtColumn))
                    strFields.AppendLine("\t\t\t\t\t," + paramName + " = :P_" + paramName + "");
            }
            int sIndex = strFields.ToString().IndexOf(",") + 1;
            info.Append(strFields.ToString().Substring(sIndex));
            info.AppendLine("\t\t\t\t\tWHERE " + firtColumn + " = entity." + firtColumn + "\";");

            info.AppendLine("\t\t\tvar conn = DataConnector.GetOracleConnection();");
            info.AppendLine("\t\t\tvar cmd = new OracleCommand(sql, conn);");
            info.AppendLine("\t\t\t*/");

            info.AppendLine("\t\t\tvar conn = DataConnector.GetOracleConnection();");
            info.AppendLine("\t\t\tvar cmd = new OracleCommand(\"PKG_" + AppUtil.TableName + ".PRC_UPDATE\", conn);");
            info.AppendLine("\t\t\tcmd.CommandType = CommandType.StoredProcedure;");
            for (int i = 0; i < _table.Rows.Count; i++)
            {
                string paramName = _table.Rows[i]["COLUMNNAME"].ToString();
                if (!paramName.Contains("COLUMN"))
                {
                    string paramType = _table.Rows[i]["DATATYPE"].ToString().Replace("System.", "").Trim();
                    if (paramType == "DateTime")
                    {
                        /*
                        info.AppendLine("\t\t\tif (entity." + paramName + " == DateTime.MinValue)");
                        info.AppendLine("\t\t\t\tcmd.Parameters.Add(new OracleParameter(\"P_" + paramName + "\", DBNull.Value));");
                        info.AppendLine("\t\t\telse");
                        info.AppendLine("\t\t\t\tcmd.Parameters.Add(new OracleParameter(\"P_" + paramName + "\", entity." + paramName + "));");
                        */
                        info.AppendLine("\t\t\tcmd.Parameters.Add(new OracleParameter(\"P_" + paramName + "\", AppUtil.DateTimeToDBObject(entity." + paramName + ")));");
                    }
                    if (paramType == "String")
                    {
                        info.AppendLine("\t\t\tcmd.Parameters.Add(new OracleParameter(\"P_" + paramName + "\", AppUtil.ToString(entity." + paramName + ")));");
                    }
                    else
                        info.AppendLine("\t\t\tcmd.Parameters.Add(new OracleParameter(\"P_" + paramName + "\", entity." + paramName + "));");
                }
            }
            info.AppendLine();

            info.AppendLine("\t\t\ttry");
            info.AppendLine("\t\t\t{");
            info.AppendLine("\t\t\t\tif (conn.State != ConnectionState.Open)");
            info.AppendLine("\t\t\t\t\tconn.Open();");
            info.AppendLine("\t\t\t\tcmd.ExecuteNonQuery();");
            info.AppendLine("\t\t\t\tretVal = true;");
            info.AppendLine("\t\t\t}");
            info.AppendLine("\t\t\tcatch (Exception ex)");
            info.AppendLine("\t\t\t{");
            info.AppendLine("\t\t\t\tthrow ex;");
            info.AppendLine("\t\t\t}");
            info.AppendLine("\t\t\tfinally");
            info.AppendLine("\t\t\t{");
            info.AppendLine("\t\t\t\tconn.Close();");
            info.AppendLine("\t\t\t\t");

            info.AppendLine("\t\t\t}");
            info.AppendLine("\t\t\treturn retVal;");
            info.AppendLine("\t\t}");


            //GetAll
            info.AppendLine("\t\tpublic static DataTable GetAll()");
            info.AppendLine("\t\t{");
            info.AppendLine("\t\t\tDataTable retVal = null;");
            info.AppendLine("\t\t\tvar conn = DataConnector.GetOracleConnection();");
            info.AppendLine("\t\t\tvar cmd = new OracleCommand(\"SELECT * FROM " + AppUtil.TableName + "\", conn);");
            info.AppendLine("\t\t\ttry");
            info.AppendLine("\t\t\t{");
            info.AppendLine("\t\t\t\tretVal = new DataTable();");
            info.AppendLine("\t\t\t\tvar da = new OracleDataAdapter(cmd);");
            info.AppendLine("\t\t\t\tda.Fill(retVal);");
            info.AppendLine("\t\t\t}");
            info.AppendLine("\t\t\tcatch (Exception ex)");
            info.AppendLine("\t\t\t{");
            info.AppendLine("\t\t\t\tthrow ex;");
            info.AppendLine("\t\t\t}");
            info.AppendLine("\t\t\tfinally");
            info.AppendLine("\t\t\t{");
            info.AppendLine("\t\t\t\tconn.Close();");
            info.AppendLine("\t\t\t\t");
            info.AppendLine("\t\t\t}");
            info.AppendLine("\t\t\treturn retVal;");
            info.AppendLine("\t\t}");


            //Delete
            if (firtColumn == "ID")
                info.AppendLine("\t\tpublic static bool Delete (decimal id)");
            else
                info.AppendLine("\t\tpublic static bool Delete (string " + firtColumn.ToLower() + ")");
            info.AppendLine("\t\t{");

            info.AppendLine("\t\t\tbool retVal = false;");
            info.AppendLine("\t\t\tvar conn = DataConnector.GetOracleConnection();");
            info.AppendLine("\t\t\tvar cmd = new OracleCommand(\"PKG_" + AppUtil.TableName + ".PRC_DELETE\", conn);");
            info.AppendLine("\t\t\tcmd.CommandType = CommandType.StoredProcedure;");
            info.AppendLine("\t\t\tcmd.Parameters.Add(new OracleParameter(\"P_" + firtColumn + "\", " + firtColumn.ToLower() + "));");
            info.AppendLine("\t\t\ttry");
            info.AppendLine("\t\t\t{");
            info.AppendLine("\t\t\t\tif (conn.State != ConnectionState.Open)");
            info.AppendLine("\t\t\t\t\tconn.Open();");
            info.AppendLine("\t\t\t\tcmd.ExecuteNonQuery();");
            info.AppendLine("\t\t\t\tretVal = true;");
            info.AppendLine("\t\t\t}");
            info.AppendLine("\t\t\tcatch (Exception ex)");
            info.AppendLine("\t\t\t{");
            info.AppendLine("\t\t\t\tthrow ex;");
            info.AppendLine("\t\t\t}");
            info.AppendLine("\t\t\tfinally");
            info.AppendLine("\t\t\t{");
            info.AppendLine("\t\t\t\tconn.Close();");
            info.AppendLine("\t\t\t\t");

            info.AppendLine("\t\t\t}");
            info.AppendLine("\t\t\treturn retVal;");
            info.AppendLine("\t\t}");



            //GetInfo
            info.AppendLine();
            if (firtColumn == "ID")
                info.AppendLine("\t\tpublic static " + Definition.ClassInfoDefine + " GetInfo(decimal id)");
            else
                info.AppendLine("\t\tpublic static " + Definition.ClassInfoDefine + " GetInfo(string " + firtColumn.ToLower() + ")");
            info.AppendLine("\t\t{");
            info.AppendLine("\t\t\t" + Definition.ClassInfoDefine + " retVal = null;");
            info.AppendLine("\t\t\tvar conn = DataConnector.GetOracleConnection();");
            info.AppendLine("\t\t\tvar cmd = new OracleCommand(\"SELECT * FROM " + AppUtil.TableName + " WHERE " + firtColumn + " = :P_" + firtColumn + "\", conn);");
            info.AppendLine("\t\t\tcmd.Parameters.Add(new OracleParameter(\"P_" + firtColumn + "\", " + firtColumn.ToLower() + "));");
            info.AppendLine("\t\t\tOracleDataReader dr = null;");
            info.AppendLine("\t\t\ttry");
            info.AppendLine("\t\t\t{");
            info.AppendLine("\t\t\t\tif (conn.State != ConnectionState.Open)");
            info.AppendLine("\t\t\t\t\tconn.Open();");
            info.AppendLine("\t\t\t\tdr = cmd.ExecuteReader();");
            info.AppendLine("\t\t\t\tif (dr.Read())");
            info.AppendLine("\t\t\t\t{");
            info.AppendLine("\t\t\t\t\tretVal = new " + Definition.ClassInfoDefine + "();");
            info.AppendLine("\t\t\t\t\tretVal.FillDataProperty(dr);");
            /*
            for (int i = 0; i < _table.Rows.Count; i++)
            {
                string paramName = _table.Rows[i]["COLUMNNAME"].ToString();
                string paramType = _table.Rows[i]["DATATYPE"].ToString().Replace("System.", "").Trim();

                if (!paramName.Contains("COLUMN"))
                    info.AppendLine("\t\t\t\t\t//retVal." + paramName + " = " + ConvertUtility.ToVarConvert(paramType) + "(dr[\"" + paramName + "\"]);");
            }
            */
            info.AppendLine("\t\t\t\t}");
            info.AppendLine("\t\t\t}");
            info.AppendLine("\t\t\tcatch (Exception ex)");
            info.AppendLine("\t\t\t{");
            info.AppendLine("\t\t\t\tthrow ex;");
            info.AppendLine("\t\t\t}");
            info.AppendLine("\t\t\tfinally");
            info.AppendLine("\t\t\t{");
            info.AppendLine("\t\t\t\tif (dr != null) dr.Close();");
            info.AppendLine("\t\t\t\tconn.Close();");
            info.AppendLine("\t\t\t\t");
            info.AppendLine("\t\t\t}");
            info.AppendLine("\t\t\treturn retVal;");
            info.AppendLine("\t\t}");
            info.AppendLine("\t}");
            info.AppendLine("}");

            return info.ToString();
        }
        public static string ToPackage(DataTable _table)
        {
            StringBuilder info = new StringBuilder();

            string firtColumn = _table.Rows[0]["COLUMN_NAME"].ToString();
            string firtColumnType = _table.Rows[0]["DATA_TYPE"].ToString().Replace("TIMESTAMP(6)", "DATE");
            bool hasColumnNameIsID = false;
            for (int i = 0; i < _table.Rows.Count; i++)
            {
                if (_table.Rows[i]["COLUMN_NAME"].ToString().Equals("ID"))
                {
                    hasColumnNameIsID = true;
                    break;
                }
            }
            //--------------------------------------------------
            //HEADER
            //--------------------------------------------------
            info.AppendLine("CREATE OR REPLACE PACKAGE PCK_" + AppUtil.TableName + " IS");
            //      GET_PAGED
            info.AppendLine("\tPROCEDURE PRC_GET_PAGED (");
            info.AppendLine("\t\tp_keyword VARCHAR2");
            //info.AppendLine("\t\t,p_orderby VARCHAR2");
            //info.AppendLine("\t\t,p_orderdirection VARCHAR2");
            info.AppendLine("\t\t,p_currentpage NUMBER");
            info.AppendLine("\t\t,p_pagesize NUMBER");
            info.AppendLine("\t\t,p_rowcount OUT NUMBER");
            info.AppendLine("\t\t,p_out	OUT SYS_REFCURSOR");
            info.AppendLine("\t);\n");


            //      DELETE
            info.AppendLine("\tPROCEDURE PRC_DELETE (");
            if (hasColumnNameIsID)
                info.AppendLine("\t\tp_ID " + firtColumnType);
            else
                info.AppendLine("\t\tp_" + firtColumn.ToLower() + "	" + firtColumnType);
            info.AppendLine("\t);\n");


            //      INSERT
            info.AppendLine("\tPROCEDURE PRC_INSERT (");
            for (int i = 0; i < _table.Rows.Count; i++)
            {
                string paramName = _table.Rows[i]["COLUMN_NAME"].ToString();
                string paramType = _table.Rows[i]["DATA_TYPE"].ToString().Replace("TIMESTAMP(6)", "DATE");
                string firstParamName = _table.Rows[0]["COLUMN_NAME"].ToString();

                if (firstParamName == "ID")
                    firstParamName = _table.Rows[1]["COLUMN_NAME"].ToString();

                if (!paramName.Contains("COLUMN") && paramName != "ID")
                {
                    if (paramName == firstParamName)
                        info.AppendLine("\t\tp_" + paramName.ToLower() + "	" + paramType);
                    else
                        info.AppendLine("\t\t,p_" + paramName.ToLower() + "	" + paramType);
                }
            }
            if (hasColumnNameIsID)
                info.AppendLine("\t\t,p_ID	OUT NUMBER");
            info.AppendLine("\t);\n");



            //      UPDATE
            info.AppendLine("\tPROCEDURE PRC_UPDATE (");
            for (int i = 0; i < _table.Rows.Count; i++)
            {
                string paramName = _table.Rows[i]["COLUMN_NAME"].ToString();
                string paramType = _table.Rows[i]["DATA_TYPE"].ToString().Replace("TIMESTAMP(6)", "DATE");
                string firstParamName = _table.Rows[0]["COLUMN_NAME"].ToString();

                if (!paramName.Contains("COLUMN"))
                {
                    if (paramName == firstParamName)
                        info.AppendLine("\t\tp_" + paramName.ToLower() + "	" + paramType);
                    else
                        info.AppendLine("\t\t,p_" + paramName.ToLower() + "	" + paramType);
                }
            }
            info.AppendLine("\t);\n");

            //--------------------------------------------------
            info.AppendLine("END PCK_" + AppUtil.TableName + ";\n");
            //--------------------------------------------------

            //--------------------------------------------------
            //BODY
            //--------------------------------------------------
            info.AppendLine("CREATE OR REPLACE PACKAGE BODY PCK_" + AppUtil.TableName + " IS");
            //      GET_PAGED
            info.AppendLine("\tPROCEDURE PRC_GET_PAGED (");
            info.AppendLine("\t\tp_keyword VARCHAR2");
            //info.AppendLine("\t\t,p_orderby VARCHAR2");
            //info.AppendLine("\t\t,p_orderdirection VARCHAR2");
            info.AppendLine("\t\t,p_currentpage NUMBER");
            info.AppendLine("\t\t,p_pagesize NUMBER");
            info.AppendLine("\t\t,p_rowcount OUT NUMBER");
            info.AppendLine("\t\t,p_out	OUT SYS_REFCURSOR");
            info.AppendLine("\t) AS");

            info.AppendLine("\tv_startindex NUMBER;");
            info.AppendLine("\tv_endindex NUMBER;");
            //info.AppendLine("\tv_orderby VARCHAR2(30);");
            //info.AppendLine("\tv_orderdirection VARCHAR2(4);");
            info.AppendLine("\tv_condition VARCHAR(32760);");
            info.AppendLine("\tv_sqltext VARCHAR(32760);");

            info.AppendLine("\tBEGIN");

            info.AppendLine("\t\tv_startindex := NVL(p_currentpage,0) * NVL(p_pagesize,0) + 1;");
            info.AppendLine("\t\tv_endindex := v_startindex + NVL(p_pagesize,0) - 1;\n");

            //info.AppendLine("\t\tIF p_orderby IS NULL OR TRIM(p_orderby) IS NULL THEN");
            //info.AppendLine("\t\t\tv_orderby := '" + firtColumn + "';  --Mac dinh theo nghiep vu");
            //info.AppendLine("\t\tELSE");
            //info.AppendLine("\t\t\tv_orderby := p_orderby;");
            //info.AppendLine("\t\tEND IF;\n");

            //info.AppendLine("\t\tIF p_orderdirection <> 'ASC' AND p_orderdirection <> 'DESC' THEN");
            //info.AppendLine("\t\t\tv_orderdirection := 'ASC';");
            //info.AppendLine("\t\tEND IF;\n");

            info.AppendLine("\t\tv_condition := ' WHERE 0 = 0 ';\n");

            info.AppendLine("\t\tIF p_keyword IS NOT NULL AND  TRIM(p_keyword) IS NOT NULL THEN");
            if (firtColumn == "ID")
                info.AppendLine("\t\t\tv_condition := v_condition || ' AND a." + _table.Rows[1]["COLUMN_NAME"].ToString() + " LIKE ''%' || p_keyword || '%''';");
            else
                info.AppendLine("\t\t\tv_condition := v_condition || ' AND a." + firtColumn + " LIKE ''%' || p_keyword || '%''';");
            info.AppendLine("\t\tEND IF;\n");

            info.AppendLine("\t\tv_sqltext:= 'SELECT * FROM (");
            //info.AppendLine("\t\t\tSELECT ROW_NUMBER() OVER (ORDER BY ' || V_ORDERBY || ' ' || V_ORDERDIRECTION || ') STT ");
            info.AppendLine("\t\t\tSELECT ROW_NUMBER() OVER (ORDER BY " + firtColumn + " ASC) STT ");
            info.AppendLine("\t\t\t,a.*");
            info.AppendLine("\t\t\tFROM " + AppUtil.TableName + " a ' || v_condition || ')");
            info.AppendLine("\t\t\tWHERE  STT BETWEEN ' || v_startindex || ' AND ' || v_endindex ;\n");
            info.AppendLine("\t\tOPEN p_out FOR v_sqltext;\n");
            info.AppendLine("\t\tv_sqltext:= 'SELECT COUNT(1) FROM " + AppUtil.TableName + " a ' || v_condition;\n");
            info.AppendLine("\t\tEXECUTE IMMEDIATE v_sqltext INTO p_rowcount;\n");
            //info.AppendLine("\t\t--NOTE: USE FOR");
            //info.AppendLine("\t\t/*");
            //info.AppendLine("\t\tprivate void dgvList_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)");
            //info.AppendLine("\t\t{");
            //info.AppendLine("\t\t\torderBy = dgvList.SortedColumn.Name;");
            //info.AppendLine("\t\t\tswitch (dgvList.SortOrder.ToString())");
            //info.AppendLine("\t\t\t{");
            //info.AppendLine("\t\t\t\tcase \"Ascending\":");
            //info.AppendLine("\t\t\t\t\torderDirection = \"ASC\";");
            //info.AppendLine("\t\t\t\t\tbreak;");
            //info.AppendLine("\t\t\t\tcase \"Descending\":");
            //info.AppendLine("\t\t\t\t\torderDirection = \"DESC\";");
            //info.AppendLine("\t\t\t\t\tbreak;");
            //info.AppendLine("\t\t\t}");
            //info.AppendLine("\t\t\tLoadData();");
            //info.AppendLine("\t\t}");
            //info.AppendLine("\t\t*/");
            info.AppendLine("\tEND;\n");

            //      DELETE
            info.AppendLine("\tPROCEDURE PRC_DELETE (");
            if (hasColumnNameIsID)
                info.AppendLine("\t\tp_ID	" + firtColumnType);
            else
                info.AppendLine("\t\tp_" + firtColumn.ToLower() + "	" + firtColumnType);
            info.AppendLine("\t) AS");
            info.AppendLine("\tBEGIN");
            if (hasColumnNameIsID)
                info.AppendLine("\t\tDELETE FROM " + AppUtil.TableName + " WHERE ID = p_ID;");
            else
                info.AppendLine("\t\tDELETE FROM " + AppUtil.TableName + " WHERE " + firtColumn + " = p_" + firtColumn.ToLower() + ";");
            info.AppendLine("\tEND;\n");

            //      INSERT
            info.AppendLine("\tPROCEDURE PRC_INSERT (");
            for (int i = 0; i < _table.Rows.Count; i++)
            {
                string paramName = _table.Rows[i]["COLUMN_NAME"].ToString();
                string paramType = _table.Rows[i]["DATA_TYPE"].ToString().Replace("TIMESTAMP(6)", "DATE");
                string firstParamName = _table.Rows[0]["COLUMN_NAME"].ToString();

                if (firstParamName == "ID")
                    firstParamName = _table.Rows[1]["COLUMN_NAME"].ToString();

                if (!paramName.Contains("COLUMN") && paramName != "ID")
                {
                    if (paramName == firstParamName)
                        info.AppendLine("\t\tp_" + paramName.ToLower() + "	" + paramType);
                    else
                        info.AppendLine("\t\t,p_" + paramName.ToLower() + "	" + paramType);
                }
            }
            if (hasColumnNameIsID)
                info.AppendLine("\t\t,p_ID	OUT NUMBER");

            info.AppendLine("\t) AS");
            info.AppendLine("\tBEGIN");
            info.AppendLine("\t\tINSERT INTO " + AppUtil.TableName + " (");
            for (int i = 0; i < _table.Rows.Count; i++)
            {
                string paramName = _table.Rows[i]["COLUMN_NAME"].ToString();
                string paramType = _table.Rows[i]["DATA_TYPE"].ToString().Replace("TIMESTAMP(6)", "DATE");

                if (!paramName.Contains("COLUMN"))
                {
                    if (paramName == firtColumn)
                        info.AppendLine("\t\t\t" + paramName.ToLower());
                    else
                        info.AppendLine("\t\t\t," + paramName.ToLower());
                }
            }
            info.AppendLine("\t\t) VALUES (");
            for (int i = 0; i < _table.Rows.Count; i++)
            {
                string paramName = _table.Rows[i]["COLUMN_NAME"].ToString();
                string paramType = _table.Rows[i]["DATA_TYPE"].ToString().Replace("TIMESTAMP(6)", "DATE");

                if (!paramName.Contains("COLUMN"))
                {
                    if (paramName == "ID")
                        info.AppendLine("\t\t\t" + AppUtil.TableName + "_SEQ.NEXTVAL");
                    else if (paramName == firtColumn)
                        info.AppendLine("\t\t\tp_" + paramName.ToLower());
                    else
                        info.AppendLine("\t\t\t,p_" + paramName.ToLower());
                }
            }

            if (hasColumnNameIsID)
            {
                info.AppendLine("\t\t) ");
                info.AppendLine("\t\tRETURNING ID INTO p_ID;");
            }
            else
                info.AppendLine("\t\t); ");

            info.AppendLine("\tEND;\n");


            //      UPDATE
            info.AppendLine("\tPROCEDURE PRC_UPDATE (");
            for (int i = 0; i < _table.Rows.Count; i++)
            {
                string paramName = _table.Rows[i]["COLUMN_NAME"].ToString();
                string paramType = _table.Rows[i]["DATA_TYPE"].ToString().Replace("TIMESTAMP(6)", "DATE");

                if (!paramName.Contains("COLUMN"))
                {
                    if (paramName == firtColumn)
                        info.AppendLine("\t\tP_" + paramName.ToLower() + "	" + paramType);
                    else
                        info.AppendLine("\t\t,P_" + paramName.ToLower() + "	" + paramType);
                }
            }
            info.AppendLine("\t) AS\n");
            info.AppendLine("\tBEGIN");
            info.AppendLine("\t\tUPDATE " + AppUtil.TableName + " SET");
            for (int i = 0; i < _table.Rows.Count; i++)
            {
                string paramName = _table.Rows[i]["COLUMN_NAME"].ToString();
                string paramType = _table.Rows[i]["DATA_TYPE"].ToString().Replace("TIMESTAMP(6)", "DATE");
                string firstParamName = _table.Rows[0]["COLUMN_NAME"].ToString();
                if (firstParamName == "ID")
                    firstParamName = _table.Rows[1]["COLUMN_NAME"].ToString();

                if (!paramName.Contains("COLUMN") && paramName != "ID")
                {
                    if (paramName == firstParamName)
                        info.AppendLine("\t\t" + paramName.ToLower() + " = p_" + paramName.ToLower());
                    else
                        info.AppendLine("\t\t," + paramName.ToLower() + " = p_" + paramName.ToLower());
                }
            }
            if (hasColumnNameIsID)
                info.AppendLine("\t\tWHERE ID = p_ID;");
            else
                info.AppendLine("\t\tWHERE " + firtColumn + " = p_" + firtColumn.ToLower() + ";");
            info.AppendLine("\tEND;\n");

            //--------------------------------------------------
            info.AppendLine("END PCK_" + AppUtil.TableName + ";\n");
            //--------------------------------------------------

            return info.ToString();
        }
    }
}
