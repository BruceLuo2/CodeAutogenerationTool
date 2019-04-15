using EasyFramework.Attr;
using EasyFramework.Core.Enum;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CodeAutogenerationTool.Helper
{
    /// <summary>
    /// MS Sql代码自动生成
    /// </summary>
    class MSSqlAutoGenerationHelper
    {
        public static string path = "";//生成代码路径
        public static string nameSpace = "";//命名空间

        /// <summary>
        /// 生成MVC中的MC
        /// </summary>
        /// <param name="modelProperties">类字段信息</param>
        /// <param name="modelAttribute">表信息</param>
        /// <returns>返回true表示生成成功, 否则失败</returns>
        public static bool GenerateMC(List<ModelProperty> modelProperties, ModelAttribute modelAttribute)
        {
            return GenerateModel(modelProperties, modelAttribute) && GenerateController(modelAttribute);
        }

        /// <summary>
        /// 生成Model
        /// </summary>
        /// <param name="modelProperties">类字段信息</param>
        /// <param name="modelAttribute">表信息</param>
        /// <returns>返回true表示生成成功, 否则失败</returns>
        public static bool GenerateModel(List<ModelProperty> modelProperties, ModelAttribute modelAttribute)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("using System;\n");
            sb.Append("using EasyFramework.Attr;\n");
            sb.Append("using EasyFramework.Models;\n");
            sb.Append(string.Format("namespace {0}.Models\n", nameSpace));
            sb.Append("{\n");
            sb.Append(string.Format("    [Model(\"{0}\", \"{1}\")]\n", modelAttribute.TableName, modelAttribute.PrimaryKey));
            sb.Append(string.Format("    public class {0} : BaseModel\n", modelAttribute.TableName));
            sb.Append("    {\n");
            foreach (ModelProperty modelProperty in modelProperties)
            {
                //字段类型
                string type = "";
                //字段长度信息
                string lengthInfo = "";
                //字段默认值
                string _default = string.IsNullOrEmpty(modelProperty.Default) ? "" : string.Format(", Default = \"{0}\"", modelProperty.Default);
                //说明
                string explain = string.IsNullOrEmpty(modelProperty.Explain) ? "" : string.Format(", Explain = \"{0}\"", modelProperty.Explain);
                //能否为空
                string IsNull = string.Format(", IsNull = {0}", modelProperty.IsNull.ToString().ToLower());
                switch (MSDBType.GetTypeBySqlTypeName(modelProperty.ColType).Name)
                {
                    case "String":
                        type = "string";
                        lengthInfo = string.Format(", MinLength = {0}, MaxLength = {1}", modelProperty.MinLength, modelProperty.MaxLength);
                        break;
                    case "Int32":
                        type = "int";
                        break;
                    case "Boolean":
                        type = "bool";
                        break;
                    case "DateTime":
                        type = "DateTime";
                        break;
                    case "Double":
                        type = "double";
                        break;
                }
                sb.Append(string.Format("        [ModelProperty(\"{0}\", \"{1}\"{2}{3}{4}{5})]\n", modelProperty.ColName, modelProperty.ColType, lengthInfo, _default, explain, IsNull));
                sb.Append(string.Format("        public {0} {1} {2}\n\n", type, modelProperty.ColName, "{ get; set; }"));
            }
            sb.Append("    }\n");
            sb.Append("}");
            try
            {
                FileStream fs = new FileStream(string.Format("{0}/Models/{1}.cs", path, modelAttribute.TableName), FileMode.Create);
                byte[] bytes = Encoding.UTF8.GetBytes(sb.ToString());
                fs.Write(bytes, 0, bytes.Length);
                fs.Close();
                GenerateLog4Net();
            }
            catch (Exception e)
            {
                string sss = e.Message;
                return false;
            }
            return true;
        }

        /// <summary>
        /// 生成MVC中的C
        /// </summary>
        /// <param name="modelAttribute">表信息</param>
        /// <returns>true表示生成成功 否则 失败</returns>
        public static bool GenerateController(ModelAttribute modelAttribute)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("using System;\n");
            sb.Append("using System.Web;\n");
            sb.Append("using System.Web.Mvc;\n");
            sb.Append("using System.Collections.Generic;\n");
            sb.Append("using EasyFramework.Core;\n");
            sb.Append(string.Format("namespace {0}.Controllers\n", nameSpace));
            sb.Append("{\n");
            sb.Append(string.Format("    public class {0}Controller : BaseController\n", modelAttribute.TableName));
            sb.Append("    {\n");
            sb.Append("        public ActionResult Index()\n");
            sb.Append("        {\n");
            sb.Append("            return View();\n");
            sb.Append("        }\n");
            sb.Append("    }\n");
            sb.Append("}\n");
            try
            {
                FileStream fs = new FileStream(string.Format("{0}/Controllers/{1}Controller.cs", path, modelAttribute.TableName), FileMode.Create);
                byte[] bytes = Encoding.UTF8.GetBytes(sb.ToString());
                fs.Write(bytes, 0, bytes.Length);
                fs.Close();
                GenerateLog4Net();
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 生成三层中的DAL
        /// </summary>
        /// <param name="modelProperties">类字段信息</param>
        /// <param name="modelAttribute">表信息</param>
        /// <returns>返回true表示生成成功, 否则失败</returns>
        public static bool GenerateDAL(List<ModelProperty> modelProperties, ModelAttribute modelAttribute)
        {
            return false;
        }

        #region 公用配置创建

        /// <summary>
        /// 生成log4net配置文件
        /// </summary>
        private static void GenerateLog4Net()
        {
            if (!Directory.Exists(path + @"\bin"))
            {
                Directory.CreateDirectory(path + @"\bin");
            }
            if (!File.Exists(path + @"\bin\Log4Net.config"))
            {
                bool c = File.Exists("Log4Net.config");
                File.Copy("Log4Net.config", path + "/bin/Log4Net.config");
            }
        }

        #endregion
    }
}
