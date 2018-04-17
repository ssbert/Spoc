using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using NPOI.HPSF;
using NPOI.HSSF.UserModel;
using NPOI.HSSF.Util;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using SPOC.Common.Dto;
using SPOC.Common.Helper;
using SPOC.User;
using SPOC.User.Dto.Common;
using SPOC.User.Dto.StudentInfo;
using SPOC.User.Dto.Teacher;


namespace SPOC.Common.File
{
    /// <summary>
    /// Excel导入导出
    /// </summary>
    public class ExcelImportExport
    {
        public ExcelImportExport()
        {

        }

        public ExcelImportExport(string fileUrl)
        {
            FileUrl = fileUrl;
        }
        /// <summary>
        /// 文件路径
        /// </summary>
        public string FileUrl { get; set; }
        /// <summary>
        /// 导入Excel,返回List对象集合
        /// </summary>
        /// <typeparam name="T">对象</typeparam>
        /// <param name="mapToEntity">对象映射（对象属性，对象中文名称）</param>
        /// <param name="errMsg">如果有异常，返回异常信息</param>
        /// <returns></returns>
        public List<T> ExcelImport<T>(List<ImportFieldModel> importFieldModelList, out string errMsg)
             where T : class
        {
            errMsg = string.Empty;
            HSSFWorkbook hssfworkbook = null;
            List<T> objList = new List<T>();
            try
            {
                using (FileStream file = new FileStream(FileUrl, FileMode.Open, FileAccess.Read))
                {
                    hssfworkbook = new HSSFWorkbook(file);
                }
                //   hssfworkbook = new HSSFWorkbook(file);

                NPOI.SS.UserModel.ISheet sheet = hssfworkbook.GetSheetAt(0);
                if (sheet.PhysicalNumberOfRows <= 1) return null;

                NPOI.SS.UserModel.IRow headerRow = sheet.GetRow(1);//第二行为标题行
                int cellCount = headerRow.LastCellNum;//LastCellNum = PhysicalNumberOfCells
                int rowCount = sheet.LastRowNum;//LastRowNum = PhysicalNumberOfRows - 1
                //模板为空 无法导入
                if (rowCount < 2)
                {
                    errMsg = "请添加数据再进行导入!";
                    return null;
                }

                T objEntity = Activator.CreateInstance<T>();
                Type objType = objEntity.GetType();
                string tishi = "";
                for (int i = 2; i <= rowCount; i++)
                {
                    NPOI.SS.UserModel.IRow row = sheet.GetRow(i);
                    if (row == null || string.IsNullOrEmpty(GetCellValue(row.GetCell(row.FirstCellNum)))) continue;
                    objEntity = Activator.CreateInstance<T>();
                    if (row != null)
                    {
                        for (int j = row.FirstCellNum; j < cellCount; j++)
                        {

                            if (row.GetCell(j) != null)
                            {
                                string columnName = headerRow.GetCell(j).StringCellValue;//模板标题名称
                                string columnValue = GetCellValue(row.GetCell(j));//当前模板标题对应的值
                                var fieldProperty = importFieldModelList.Where(a => a.FieldName == columnName).FirstOrDefault();
                                //string columnValue = TypeTransaction(GetCellValue(row.GetCell(j)), objType.GetProperty(fieldProperty.FieldCode).PropertyType);//当前模板标题对应的值
                                if (fieldProperty == null || string.IsNullOrEmpty(fieldProperty.FieldCode))
                                {
                                    errMsg = "模板不应该包含[" + columnName + "],请您下载正确的模板！";
                                    return null;
                                }
                                if (fieldProperty.MustFlag && string.IsNullOrEmpty(columnValue))
                                {
                                    errMsg = string.Format("第{0}行{1}列[{2}]为必填项，请您填写相关内容！", i + 1, j + 1, columnName);
                                    return null;
                                }
                                if (fieldProperty.MaxLength > 0 && columnValue != null && columnValue.Length > fieldProperty.MaxLength)
                                {
                                    errMsg = string.Format("第{0}行{1}列[{2}]的内容长度必须小于{3}!", i + 1, j + 1, columnName, fieldProperty.MaxLength);
                                    return null;
                                }
                                if (fieldProperty.FieldEnumValue != null && fieldProperty.FieldEnumValue.Count > 0)
                                {
                                    if (columnValue == null) columnValue = "";
                                    if (!fieldProperty.FieldEnumValue.Contains(columnValue))
                                    {
                                        errMsg = string.Format("第{0}行{1}列[{2}]的内容必须为[{3}]之一!", i + 1, j + 1, columnName, StringUtil.ArrayToDbString(fieldProperty.FieldEnumValue.ToArray(), ","));
                                        return null;
                                    }
                                }
                                objType.GetProperty(fieldProperty.FieldCode).SetValue(objEntity, TypeTransaction(columnValue, objType.GetProperty(fieldProperty.FieldCode).PropertyType), null);
                            }
                        }
                    }
                    //objType.GetProperty("id").SetValue(objEntity, Guid.NewGuid().ToString(), null);
                    objList.Add((T)objEntity);
                }
            }
            catch (Exception e)
            {
                errMsg = e.Message;
            }

            return objList;
        }
        /// <summary>
        /// 将指定的集合转换成DataTable。
        /// </summary>
        /// <param name="list">将指定的集合。</param>
        /// <returns>返回转换后的DataTable。</returns>
        public static DataTable ListToDataTable(IList list, out List<string> columnsNames)
        {
            DataTable table = new DataTable();
            columnsNames = new List<string>();
            if (list.Count > 0)
            {
                PropertyInfo[] propertys = list[0].GetType().GetProperties();
                foreach (PropertyInfo pi in propertys)
                {
                    Type pt = pi.PropertyType;
                    if ((pt.IsGenericType) && (pt.GetGenericTypeDefinition() == typeof(Nullable<>)))
                    {
                        pt = pt.GetGenericArguments()[0];
                    }
                    table.Columns.Add(new DataColumn(pi.Name, pt));
                    columnsNames.Add(pi.Name);
                }

                for (int i = 0; i < list.Count; i++)
                {
                    ArrayList tempList = new ArrayList();
                    foreach (PropertyInfo pi in propertys)
                    {
                        object obj = pi.GetValue(list[i], null);
                        tempList.Add(obj);
                    }
                    object[] array = tempList.ToArray();
                    table.LoadDataRow(array, true);
                }
            }
            return table;
        }

        public HSSFWorkbook GenerateData(string headerText, string sheetName, string[] columnName, string[] columnTitle, UserInfoSeacheExport seachValue, List<BatchDeleteRequestInputByUser> inputList, StudentInfoInputDto input, IUserInfoService userInfoService, IStudentInfoService studentGuidService)
        {
            HSSFWorkbook hssfworkbook = new HSSFWorkbook();
            ISheet sheet = hssfworkbook.CreateSheet(sheetName);
            #region 设置文件属性信息
            //创建一个文档摘要信息实体。
            DocumentSummaryInformation dsi = PropertySetFactory.CreateDocumentSummaryInformation();
            dsi.Company = "Team"; //
            hssfworkbook.DocumentSummaryInformation = dsi;
            //创建一个摘要信息实体。
            SummaryInformation si = PropertySetFactory.CreateSummaryInformation();
            si.Subject = "本文档由系统生成";
            si.Author = "系统";
            si.Title = headerText;
            si.Subject = headerText;
            si.CreateDateTime = DateTime.Now;
            hssfworkbook.SummaryInformation = si;
            #endregion
            ICellStyle dateStyle = hssfworkbook.CreateCellStyle();
            IDataFormat format = hssfworkbook.CreateDataFormat();
            dateStyle.DataFormat = format.GetFormat("yyyy-mm-dd");
            #region 取得列宽

            int[] colWidth = new int[columnName.Length];
            for (int i = 0; i < columnName.Length; i++)
            {
                colWidth[i] = Encoding.GetEncoding(936).GetBytes(columnTitle[i]).Length;
            }
            List<string> columnsName = new List<string>();

            #endregion
            int rowIndex = 0;
            int sheetIndex = 0;

            int total = 0;
            input.PageSize = 10000;
            input.CurrentPage = 0;
            var userList = studentGuidService.GetStudentInfoDtoList(inputList, input, ref total).Select(a => a.GetUserInfoImport()).ToList();
            int repeateCount = total / 10000 + 1;
            for (int ii = 0; ii < repeateCount; ii++)
            { 
                if (ii != 0)
                {
                    input.CurrentPage = ii;
                    studentGuidService.GetStudentInfoDtoList(inputList, input, ref total);
                }
                DataTable table = ListToDataTable(userList, out columnsName);
                foreach (DataRow row in table.Rows)
                {
                    #region 新建表，填充表头，填充列头，样式

                    if (rowIndex == 65535 || rowIndex == 0)
                    {
                        if (rowIndex != 0)
                        {
                            sheet = hssfworkbook.CreateSheet(sheetName + ((int)sheetIndex / 65535).ToString(CultureInfo.InvariantCulture));
                        }

                        #region 表头及样式

                        {
                            IRow headerRow = sheet.CreateRow(0);
                            headerRow.HeightInPoints = 25;
                            headerRow.CreateCell(0).SetCellValue(headerText);
                            ICellStyle headStyle = hssfworkbook.CreateCellStyle();
                            headStyle.Alignment = HorizontalAlignment.Center;
                            IFont font = hssfworkbook.CreateFont();
                            font.FontHeightInPoints = 20;
                            font.Boldweight = 700;
                            headStyle.SetFont(font);
                            headerRow.GetCell(0).CellStyle = headStyle;
                            sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(0, 0, 0, columnName.Length - 1));
                        }

                        #endregion

                        #region 列头及样式

                        {
                            IRow headerRow;
                            headerRow = sheet.CreateRow(1);
                            ICellStyle headStyle = hssfworkbook.CreateCellStyle();
                            headStyle.Alignment = HorizontalAlignment.Center;
                            IFont font = hssfworkbook.CreateFont();
                            font.FontHeightInPoints = 10;
                            font.Boldweight = 700;
                            headStyle.SetFont(font);

                            for (int i = 0; i < columnName.Length; i++)
                            {
                                headerRow.CreateCell(i).SetCellValue(columnTitle[i]);
                                headerRow.GetCell(i).CellStyle = headStyle;
                                //设置列宽 
                                if ((colWidth[i] + 1) * 256 > 30000)
                                {
                                    sheet.SetColumnWidth(i, 10000);
                                }
                                else
                                {
                                    sheet.SetColumnWidth(i, (colWidth[i] + 1) * 256);
                                }
                            }
                        }

                        #endregion

                        rowIndex = 2;
                    }

                    #endregion

                    #region 填充数据

                    IRow dataRow = sheet.CreateRow(rowIndex);
                    for (int i = 0; i < columnName.Length; i++)
                    {
                        ICell newCell = dataRow.CreateCell(i);
                        if (!columnsName.Contains(columnName[i]))
                        {
                            continue;
                        }

                        string drValue = row[columnName[i]].ToString();

                        if (!table.Columns.Contains(columnName[i]))
                        {
                            continue;
                        }
                        switch (table.Columns[columnName[i]].DataType.ToString())
                        {
                            case "System.String": //字符串类型   
                                if (drValue.ToUpper() == "TRUE")
                                    newCell.SetCellValue("是");
                                else if (drValue.ToUpper() == "FALSE")
                                    newCell.SetCellValue("否");
                                else if (columnName[i] == "user_eductional") { newCell.SetCellValue(string.IsNullOrEmpty(drValue) ? "" : drValue + "年制"); }
                                else if (columnName[i] == "level")
                                {
                                    switch (drValue)
                                    {
                                        case "1": { newCell.SetCellValue("专科"); } break;
                                        case "2": { newCell.SetCellValue("专升本"); } break;
                                        case "3": { newCell.SetCellValue("本科"); } break;
                                        default: newCell.SetCellValue(drValue); break;
                                    }
                                }
                                else
                                {
                                    newCell.SetCellValue(drValue);
                                }
                                break;
                            case "System.DateTime": //日期类型    
                                DateTime dateV;
                                DateTime.TryParse(drValue, out dateV);
                                newCell.SetCellValue(dateV);

                                newCell.CellStyle = dateStyle; //格式化显示    
                                break;
                            case "System.Boolean": //布尔型    
                                bool boolV = false;
                                bool.TryParse(drValue, out boolV);
                                if (boolV)
                                    newCell.SetCellValue("是");
                                else
                                    newCell.SetCellValue("否");
                                break;
                            case "System.Int16": //整型    
                            case "System.Int32":
                            case "System.Int64":
                            case "System.Byte":
                                int intV = 0;
                                int.TryParse(drValue, out intV);
                                newCell.SetCellValue(intV);
                                break;
                            case "System.Decimal": //浮点型    
                            case "System.Double":
                                double doubV = 0;
                                double.TryParse(drValue, out doubV);
                                newCell.SetCellValue(doubV);
                                break;
                            case "System.DBNull": //空值处理    
                                newCell.SetCellValue("");
                                break;
                            default:
                                newCell.SetCellValue("");
                                break;
                        }

                    }

                    #endregion

                    rowIndex++;
                    sheetIndex++;
                }

            }
            return hssfworkbook;
        }

        public HSSFWorkbook GenerateTeacherInfoData(string headerText, string sheetName, string[] columnName, string[] columnTitle, UserInfoSeacheExport seachValue, List<BatchDeleteRequestInputByUser> inputList, TeacherInfoInputDto input, IUserInfoService _userInfoService, ITeacherInfoService _studentGuidService)
        {
            HSSFWorkbook hssfworkbook = new HSSFWorkbook();
            ISheet sheet = hssfworkbook.CreateSheet(sheetName);
            #region 设置文件属性信息
            //创建一个文档摘要信息实体。
            DocumentSummaryInformation dsi = PropertySetFactory.CreateDocumentSummaryInformation();
            dsi.Company = "Team"; //
            hssfworkbook.DocumentSummaryInformation = dsi;
            //创建一个摘要信息实体。
            SummaryInformation si = PropertySetFactory.CreateSummaryInformation();
            si.Subject = "本文档由系统生成";
            si.Author = "系统";
            si.Title = headerText;
            si.Subject = headerText;
            si.CreateDateTime = DateTime.Now;
            hssfworkbook.SummaryInformation = si;
            #endregion
            ICellStyle dateStyle = hssfworkbook.CreateCellStyle();
            IDataFormat format = hssfworkbook.CreateDataFormat();
            dateStyle.DataFormat = format.GetFormat("yyyy-mm-dd");
            #region 取得列宽

            int[] colWidth = new int[columnName.Length];
            for (int i = 0; i < columnName.Length; i++)
            {
                colWidth[i] = Encoding.GetEncoding(936).GetBytes(columnTitle[i]).Length;
            }
            List<string> columnsName = new List<string>();

            #endregion
            int rowIndex = 0;
            int sheetIndex = 0;

            bool isGetCount = false;

            int total = 0;
            input.PageSize = 10000;
            input.CurrentPage = 0;
            var userList = _studentGuidService.GetTeacherInfoDtoList(inputList, input, ref total).Select(a => a.GetTacherInfoImport()).ToList();
            int repeateCount = total / 10000 + 1;
            for (int ii = 0; ii < repeateCount; ii++)
            {
                var itemUserList = new List<tacher_info_import>();
                if (ii == 0)
                {
                    // repeateCount = userList.Count / 10000 + 1;
                    //   isGetCount = true;
                    itemUserList = userList;
                }
                else
                {
                    input.CurrentPage = ii;
                    itemUserList = _studentGuidService.GetTeacherInfoDtoList(inputList, input, ref total).Select(a => a.GetTacherInfoImport()).ToList();
                }
                DataTable table = ListToDataTable(userList, out columnsName);
                foreach (DataRow row in table.Rows)
                {
                    #region 新建表，填充表头，填充列头，样式

                    if (rowIndex == 65535 || rowIndex == 0)
                    {
                        if (rowIndex != 0)
                        {
                            sheet = hssfworkbook.CreateSheet(sheetName + ((int)sheetIndex / 65535).ToString(CultureInfo.InvariantCulture));
                        }

                        #region 表头及样式

                        {
                            IRow headerRow = sheet.CreateRow(0);
                            headerRow.HeightInPoints = 25;
                            headerRow.CreateCell(0).SetCellValue(headerText);
                            ICellStyle headStyle = hssfworkbook.CreateCellStyle();
                            headStyle.Alignment = HorizontalAlignment.Center;
                            IFont font = hssfworkbook.CreateFont();
                            font.FontHeightInPoints = 20;
                            font.Boldweight = 700;
                            headStyle.SetFont(font);
                            headerRow.GetCell(0).CellStyle = headStyle;
                            sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(0, 0, 0, columnName.Length - 1));
                        }

                        #endregion

                        #region 列头及样式

                        {
                            IRow headerRow;
                            headerRow = sheet.CreateRow(1);
                            ICellStyle headStyle = hssfworkbook.CreateCellStyle();
                            headStyle.Alignment = HorizontalAlignment.Center;
                            IFont font = hssfworkbook.CreateFont();
                            font.FontHeightInPoints = 10;
                            font.Boldweight = 700;
                            headStyle.SetFont(font);

                            for (int i = 0; i < columnName.Length; i++)
                            {
                                headerRow.CreateCell(i).SetCellValue(columnTitle[i]);
                                headerRow.GetCell(i).CellStyle = headStyle;
                                //设置列宽 
                                if ((colWidth[i] + 1) * 256 > 30000)
                                {
                                    sheet.SetColumnWidth(i, 10000);
                                }
                                else
                                {
                                    sheet.SetColumnWidth(i, (colWidth[i] + 1) * 256);
                                }
                            }
                        }

                        #endregion

                        rowIndex = 2;
                    }

                    #endregion

                    #region 填充数据

                    IRow dataRow = sheet.CreateRow(rowIndex);
                    for (int i = 0; i < columnName.Length; i++)
                    {
                        ICell newCell = dataRow.CreateCell(i);
                        if (!columnsName.Contains(columnName[i]))
                        {
                            continue;
                        }

                        string drValue = row[columnName[i]].ToString();

                        if (!table.Columns.Contains(columnName[i]))
                        {
                            continue;
                        }
                        switch (table.Columns[columnName[i]].DataType.ToString())
                        {
                            case "System.String": //字符串类型   
                                if (drValue.ToUpper() == "TRUE")
                                    newCell.SetCellValue("是");
                                else if (drValue.ToUpper() == "FALSE")
                                    newCell.SetCellValue("否");
                                else if (columnName[i] == "user_eductional") { newCell.SetCellValue(string.IsNullOrEmpty(drValue) ? "" : drValue + "年制"); }
                                else if (columnName[i] == "level")
                                {
                                    switch (drValue)
                                    {
                                        case "1": { newCell.SetCellValue("专科"); } break;
                                        case "2": { newCell.SetCellValue("专升本"); } break;
                                        case "3": { newCell.SetCellValue("本科"); } break;
                                        default: newCell.SetCellValue(drValue); break;
                                    }
                                }
                                else
                                {
                                    newCell.SetCellValue(drValue);
                                }
                                break;
                            case "System.DateTime": //日期类型    
                                DateTime dateV;
                                DateTime.TryParse(drValue, out dateV);
                                newCell.SetCellValue(dateV);

                                newCell.CellStyle = dateStyle; //格式化显示    
                                break;
                            case "System.Boolean": //布尔型    
                                bool boolV = false;
                                bool.TryParse(drValue, out boolV);
                                if (boolV)
                                    newCell.SetCellValue("是");
                                else
                                    newCell.SetCellValue("否");
                                break;
                            case "System.Int16": //整型    
                            case "System.Int32":
                            case "System.Int64":
                            case "System.Byte":
                                int intV = 0;
                                int.TryParse(drValue, out intV);
                                newCell.SetCellValue(intV);
                                break;
                            case "System.Decimal": //浮点型    
                            case "System.Double":
                                double doubV = 0;
                                double.TryParse(drValue, out doubV);
                                newCell.SetCellValue(doubV);
                                break;
                            case "System.DBNull": //空值处理    
                                newCell.SetCellValue("");
                                break;
                            default:
                                newCell.SetCellValue("");
                                break;
                        }

                    }

                    #endregion

                    rowIndex++;
                    sheetIndex++;
                }

            }
            return hssfworkbook;
        }

        /// <summary>
        /// 导入Excel,返回List对象集合
        /// </summary>
        /// <typeparam name="T">对象</typeparam>
        /// <param name="mapToEntity">对象映射（对象属性，对象中文名称）</param>
        /// <param name="errMsg">如果有异常，返回异常信息</param>
        /// <returns></returns>
        public List<T> ExcelImport<T>(Stream steam, List<ImportFieldModel> importFieldModelList, out string errMsg)
             where T : class
        {
            errMsg = string.Empty;
            HSSFWorkbook hssfworkbook = null;
            List<T> objList = new List<T>();
            try
            {
                hssfworkbook = new HSSFWorkbook(steam);
                NPOI.SS.UserModel.ISheet sheet = hssfworkbook.GetSheetAt(0);
                if (sheet.PhysicalNumberOfRows <= 1) return null;

                NPOI.SS.UserModel.IRow headerRow = sheet.GetRow(1);//第二行为标题行
                int cellCount = headerRow.LastCellNum;//LastCellNum = PhysicalNumberOfCells
                int rowCount = sheet.LastRowNum;//LastRowNum = PhysicalNumberOfRows - 1
                //模板为空 无法导入
                if (rowCount < 2)
                {
                    errMsg = "请添加数据再进行导入!";
                    return null;
                }

                T objEntity = Activator.CreateInstance<T>();
                Type objType = objEntity.GetType();
                for (int i = 2; i <= rowCount; i++)
                {
                    NPOI.SS.UserModel.IRow row = sheet.GetRow(i);
                    if (row == null || string.IsNullOrEmpty(GetCellValue(row.GetCell(row.FirstCellNum)))) continue;
                    objEntity = Activator.CreateInstance<T>();
                    if (row != null)
                    {
                        for (int j = headerRow.FirstCellNum; j < cellCount; j++)
                        {


                            string columnName = headerRow.GetCell(j).StringCellValue;//模板标题名称
                            string columnValue = GetCellValue(row.GetCell(j)).Trim();//当前模板标题对应的值
                            var fieldProperty = importFieldModelList.Where(a => a.FieldName == columnName).FirstOrDefault();
                            //string columnValue = TypeTransaction(GetCellValue(row.GetCell(j)), objType.GetProperty(fieldProperty.FieldCode).PropertyType);//当前模板标题对应的值
                            if (fieldProperty == null || string.IsNullOrEmpty(fieldProperty.FieldCode))
                            {
                                errMsg = "模板不应该包含[" + columnName + "],请您下载正确的模板！";
                                return null;
                            }
                            if (((fieldProperty.remark != null && fieldProperty.remark.Contains("必填")) || fieldProperty.MustFlag) && string.IsNullOrEmpty(columnValue.Trim()))
                            {
                                errMsg += string.Format("第{0}行{1}列[{2}]为必填项，请您填写相关内容！ ", i + 1, j + 1, columnName);
                                //  return null;
                            }
                            if (fieldProperty.MaxLength > 0 && columnValue != null && columnValue.Length > fieldProperty.MaxLength)
                            {
                                errMsg += string.Format("第{0}行{1}列[{2}]的内容长度必须小于{3}! ", i + 1, j + 1, columnName, fieldProperty.MaxLength);
                                // return null;
                            }
                            //if (fieldProperty.FieldEnumValue != null && fieldProperty.FieldEnumValue.Count > 0 && columnValue != null) {
                            //    if (!fieldProperty.FieldEnumValue.Any(a => a == columnValue.Trim())) {
                            //        errMsg += string.Format("第{0}行{1}列[{2}]为格式不正确，请按照提示中的选项进行填写！ ", i + 1, j + 1, columnName);
                            //    }
                            //}

                            if (fieldProperty.FieldEnumValue != null && fieldProperty.FieldEnumValue.Count > 0 && !string.IsNullOrEmpty(columnValue))
                            {
                                if (!fieldProperty.FieldEnumValue.Any(a => a == columnValue.Trim()))
                                {
                                    errMsg += string.Format("第{0}行{1}列[{2}]为格式不正确，请按照提示中的选项进行填写！ ", i + 1, j + 1, columnName);
                                }
                            }
                            if (fieldProperty.FieldEnumValue != null && fieldProperty.FieldEnumValue.Count > 0 && !string.IsNullOrEmpty(columnValue))
                            {
                                if (columnValue == null) columnValue = "";
                                if (!fieldProperty.FieldEnumValue.Contains(columnValue))
                                {
                                    errMsg += string.Format("第{0}行{1}列[{2}]的内容必须为[{3}]之一! ", i + 1, j + 1, columnName, StringUtil.ArrayToDbString(fieldProperty.FieldEnumValue.ToArray(), ","));
                                    //  return null;
                                }
                            }
                            // if (!string.IsNullOrEmpty(errMsg)) { return null; }
                            if (string.IsNullOrEmpty(errMsg))
                            {
                                objType.GetProperty(fieldProperty.FieldCode).SetValue(objEntity, TypeTransaction(columnValue, objType.GetProperty(fieldProperty.FieldCode).PropertyType), null);
                            }


                        }
                    }

                    if (!string.IsNullOrEmpty(errMsg)) { errMsg += "<br/>"; }
                    else
                    {
                        objList.Add((T)objEntity);
                    }
                }
                if (!string.IsNullOrEmpty(errMsg)) { return null; }
            }
            catch (Exception e)
            {
                errMsg = e.Message;
            }

            return objList;
        }
        /// <summary>
        /// 导出模板信息流
        /// </summary>
        /// <param name="importFieldModelList">模板属性配置</param>
        /// <param name="fieldName">需要导出的模板字段名</param>
        /// <returns></returns>
        public static Stream ExportTemplate(List<ImportFieldModel> importFieldModelList, List<string> fieldName)
        {
            //创建Excel文件的对象
            NPOI.HSSF.UserModel.HSSFWorkbook book = new NPOI.HSSF.UserModel.HSSFWorkbook();
            //添加一个sheet
            NPOI.SS.UserModel.ISheet sheet1 = book.CreateSheet("Sheet1");
            //给sheet1添加第一行的头部标题备注
            NPOI.SS.UserModel.IRow row1 = sheet1.CreateRow(0);
            //给sheet1添加第二行的头部标题
            NPOI.SS.UserModel.IRow row2 = sheet1.CreateRow(1);
            ICellStyle cstyle = book.CreateCellStyle();
            cstyle.DataFormat = HSSFDataFormat.GetBuiltinFormat("@");


            //必填，红色，居中。
            IFont font = book.CreateFont();
            font.Boldweight = short.MaxValue;
            font.Color = HSSFColor.Red.Index;
            ICellStyle style = book.CreateCellStyle();
            style.Alignment = HorizontalAlignment.Center;//居中显示
            style.VerticalAlignment = VerticalAlignment.Top;//垂直居中
            style.WrapText = true;
            style.SetFont(font);

            IFont font2 = book.CreateFont();
            font2.Boldweight = short.MaxValue;
            ICellStyle style1 = book.CreateCellStyle();
            style1.Alignment = HorizontalAlignment.Center;//居中显示
            style1.SetFont(font2);
            var tempList = importFieldModelList.OrderBy(a => a.OrderNum).ToList();

            int i = 0;
            foreach (var item in tempList)
            {
                sheet1.SetDefaultColumnStyle(i, cstyle);
                if (fieldName == null || fieldName.Contains(item.FieldName))
                {
                    ICell cell = row1.CreateCell(i);
                    cell.CellStyle = style;
                    ICell cell2 = row2.CreateCell(i);
                    cell2.SetCellValue(item.FieldName);

                    cell2.CellStyle = style;
                    if (item.MustFlag || item.isDateTimeType || (item.remark != null && item.remark.Contains("必填")))
                    {
                        string tempV = "";

                        if (item.MustFlag || (item.remark != null && item.remark.Contains("必填")))
                        {
                            //  font.Color = HSSFColor.Red.Index;
                            //  style.SetFont(font);
                            tempV = item.remark == null ? "必填" : (item.remark.Contains("必填") ? item.remark : "必填<br/>" + item.remark);
                        }
                        if (item.isDateTimeType)
                        {
                            if (tempV == "")
                            {
                                cell2.CellStyle = style1;
                            }
                            tempV = tempV + (tempV == "" ? "<br/>" : "") + "格式:1911-01-01";
                        }

                        cell.SetCellValue(tempV);


                        if (tempV.Contains("<br/>"))
                        {
                            cell.SetCellValue(tempV.Replace("<br/>", "\r\n"));

                            // var addNum = tempV.Contains("助教") ? 20: 0;
                            var tempH = Convert.ToInt16(255 * (tempV.Split(new string[] { "<br/>" }, StringSplitOptions.RemoveEmptyEntries).Length + 1));
                            if (row1.Height < tempH)
                                row1.Height = tempH;
                        }
                    }
                    else
                    {
                        cell.SetCellValue(item.remark == null ? "" : item.remark.Replace("<br/>", "\r\n"));
                        cell2.CellStyle = style1;
                    }


                    if (item.FieldLength > 0)
                        sheet1.SetColumnWidth(i, 2 * (item.FieldLength + 4) * 256);
                    else
                        sheet1.SetColumnWidth(i, 2 * (item.FieldName.Length + 4) * 256);
                    i++;
                }

            }
            // 写入到客户端 
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            book.Write(ms);
            ms.Seek(0, SeekOrigin.Begin);
            return ms;

        }


        /// <summary>
        /// 根据Excel列类型获取列的值
        /// </summary>
        /// <param name="cell">Excel列</param>
        /// <returns></returns>
        public static string GetCellValue(NPOI.SS.UserModel.ICell cell)
        {
            if (cell == null)
                return string.Empty;
            switch (cell.CellType)
            {
                case CellType.Blank:
                    return string.Empty;
                case CellType.Boolean:
                    return cell.BooleanCellValue.ToString();
                case CellType.Error:
                    return cell.ErrorCellValue.ToString();
                case CellType.Numeric:
                case CellType.Unknown:
                default:
                    return cell.ToString();
                case CellType.String:
                    return cell.StringCellValue;
                case CellType.Formula:
                    try
                    {
                        HSSFFormulaEvaluator e = new HSSFFormulaEvaluator(cell.Sheet.Workbook);
                        e.EvaluateInCell(cell);
                        return cell.ToString();
                    }
                    catch
                    {
                        return cell.NumericCellValue.ToString();
                    }
            }
        }

        public static Stream ExportTemplate(Dictionary<string, string> remarkMap, Dictionary<string, string> entityMap)
        {
            //创建Excel文件的对象
            NPOI.HSSF.UserModel.HSSFWorkbook book = new NPOI.HSSF.UserModel.HSSFWorkbook();
            //添加一个sheet
            NPOI.SS.UserModel.ISheet sheet1 = book.CreateSheet("Sheet1");
            //#region 表头及样式
            //{
            //    NPOI.SS.UserModel.IRow headerRow = sheet1.CreateRow(0);
            //    headerRow.HeightInPoints = 25;
            //    headerRow.CreateCell(0).SetCellValue("111");

            //    ICellStyle headStyle = book.CreateCellStyle();
            //    headStyle.Alignment = HorizontalAlignment.Center;
            //    HSSFFont font1 = (HSSFFont)book.CreateFont();
            //    font1.FontHeightInPoints = 20;
            //    font1.Boldweight = 700;
            //    headStyle.SetFont(font1);
            //    headerRow.GetCell(0).CellStyle = headStyle;
            //    //CellRangeAddress a = new CellRangeAddress(0, 0, 0, remarkMap.Count - 1);
            //    //sheet1.AddMergedRegion(a);

            //}
            //#endregion

            //给sheet1添加第一行的备注
            NPOI.SS.UserModel.IRow row1 = sheet1.CreateRow(0);
            int i = 0;
            ICellStyle style = book.CreateCellStyle();
            style.Alignment = HorizontalAlignment.Center;//居中显示
            style.FillBackgroundColor = 10;
            style.IsHidden = false;//单元格是否隐藏
            style.IsLocked = false;//单元格是否锁定
            style.VerticalAlignment = VerticalAlignment.Center;//垂直居中
            //设置单元格字体
            HSSFFont font = (HSSFFont)book.CreateFont();
            font.Color = 10;
            font.FontHeight = 10 * 20;//设置字体大小
            font.FontName = "Arial";//设置字体为黑体
            font.IsItalic = false;//是否是斜体        
            style.SetFont(font);
            row1.RowStyle = style;



            ICellStyle style2 = book.CreateCellStyle();
            style2.Alignment = HorizontalAlignment.Center;//居中显示
            style2.FillBackgroundColor = 10;
            style2.IsHidden = false;//单元格是否隐藏
            style2.IsLocked = false;//单元格是否锁定
            style2.VerticalAlignment = VerticalAlignment.Center;//垂直居中
            //设置单元格字体
            HSSFFont font2 = (HSSFFont)book.CreateFont();
            font2.FontHeight = 10 * 20;//设置字体大小
            font2.FontName = "Arial";//设置字体为黑体
            font2.IsItalic = false;//是否是斜体     
            font2.Boldweight = (short)NPOI.SS.UserModel.FontBoldWeight.Bold;
            style2.SetFont(font2);

            foreach (var m in remarkMap)
            {
                ICell cell = row1.CreateCell(i);
                cell.CellStyle = style;
                cell.SetCellValue(m.Value);
                sheet1.SetColumnWidth(i, Encoding.Default.GetBytes(m.Value).Length * 256 * 2);
                i++;
            }

            //给sheet1添加第二行的头部标题
            NPOI.SS.UserModel.IRow row2 = sheet1.CreateRow(1);
            i = 0;
            foreach (var m in entityMap)
            {
                ICell cell = row2.CreateCell(i);
                cell.CellStyle = style2;
                cell.SetCellValue(m.Value);
                sheet1.SetColumnWidth(i, Encoding.Default.GetBytes(m.Value).Length * 256 * 2);
                i++;
            }
            // 写入到客户端 
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            book.Write(ms);
            ms.Seek(0, SeekOrigin.Begin);
            return ms;

        }

        /// <summary>
        /// 动态执行装箱操作
        /// </summary>
        /// <param name="value">属性的值</param>
        /// <param name="valueType">属性的类型</param>
        /// <returns>装箱后的对象</returns>
        private object TypeTransaction(string value, Type valueType)
        {
            if (valueType == typeof(string) || valueType == typeof(String))
            {
                return value;
            }
            object obj = new object();
            try
            {
                #region 如何优化

                if (string.IsNullOrEmpty(value))
                {
                    if (valueType == typeof(int))
                    {
                        obj = default(int);
                    }
                    else if (valueType == typeof(double) || valueType == typeof(Double))
                    {
                        obj = default(double);
                    }
                    else if (valueType == typeof(float))
                    {
                        obj = default(float);
                    }
                    else if (valueType == typeof(long))
                    {
                        obj = default(long);
                    }
                    else if (valueType == typeof(DateTime))
                    {
                        obj = default(DateTime);
                    }
                    else if (valueType == typeof(Boolean))
                    {
                        obj = default(Boolean);
                    }
                }

                #endregion 如何优化
                else
                {
                    if (valueType.Name == "Boolean")
                    {
                        if ("0否".Contains(value)) value = "false";
                        else value = "true";
                    }
                    obj = valueType.GetMethod("Parse", new Type[] { typeof(string) }).Invoke(null, new object[] { value });
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            return obj;

        }

        /// <summary>
        /// 导出excel
        /// </summary>
        /// <param name="headerText">表头</param>
        /// <param name="entityMap">列</param>
        /// <param name="reportData">数据</param>
        /// <returns></returns>
        public static Stream ExportExcle(string headerText, Dictionary<string, string> entityMap, DataTable reportData)
        {
            //创建Excel文件的对象
            NPOI.HSSF.UserModel.HSSFWorkbook book = new NPOI.HSSF.UserModel.HSSFWorkbook();
            //添加一个sheet
            NPOI.SS.UserModel.ISheet sheet1 = book.CreateSheet("Sheet1");
            if (!string.IsNullOrEmpty(headerText))
            #region 表头及样式
            {
                NPOI.SS.UserModel.IRow headerRow = sheet1.CreateRow(0);
                headerRow.HeightInPoints = 25;
                headerRow.CreateCell(0).SetCellValue(headerText);

                ICellStyle headStyle = book.CreateCellStyle();
                headStyle.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
                headStyle.BorderLeft = BorderStyle.Thin;
                headStyle.BorderRight = BorderStyle.Thin;
                headStyle.BorderTop = BorderStyle.Thin;
                headStyle.Alignment = HorizontalAlignment.Center;
                HSSFFont font1 = (HSSFFont)book.CreateFont();
                font1.FontHeightInPoints = 20;
                font1.Boldweight = 700;
                headStyle.SetFont(font1);
                headerRow.GetCell(0).CellStyle = headStyle;
                CellRangeAddress a = new CellRangeAddress(0, 0, 0, entityMap.Count - 1);
                sheet1.AddMergedRegion(a);

            }
            #endregion

            NPOI.SS.UserModel.IRow row1 = sheet1.CreateRow(string.IsNullOrEmpty(headerText) ? 0 : 1);
            int i = 0;
            ICellStyle style = book.CreateCellStyle();
            style.Alignment = HorizontalAlignment.Center;//居中显示
            style.FillBackgroundColor = 10;
            style.IsHidden = false;//单元格是否隐藏
            style.IsLocked = false;//单元格是否锁定
            style.VerticalAlignment = VerticalAlignment.Center;//垂直居中

            //设置单元格字体
            HSSFFont font = (HSSFFont)book.CreateFont();
            //font.Color = 10;
            font.FontHeight = 10 * 20;//设置字体大小
            font.FontName = "Arial";//设置字体为黑体
            font.IsItalic = false;//是否是斜体   
            font.FontHeightInPoints = 10;
            font.Boldweight = 700;
            style.SetFont(font);

            ICellStyle style2 = book.CreateCellStyle();
            style2.Alignment = HorizontalAlignment.Center;
            style2.VerticalAlignment = VerticalAlignment.Top;
            style2.WrapText = true;
            style2.Alignment = HorizontalAlignment.Center;
            style2.BorderBottom = BorderStyle.Thin;
            style2.BorderLeft = BorderStyle.Thin;
            style2.BorderRight = BorderStyle.Thin;
            style2.BorderTop = BorderStyle.Thin;
            IFont font2 = book.CreateFont();
            font2.FontHeight = 11 * 20;
            style2.SetFont(font2);


            row1.RowStyle = style;
            var dateStyle = book.CreateCellStyle();
            var format = book.CreateDataFormat();
            dateStyle.DataFormat = format.GetFormat("yyyy-mm-dd");
            foreach (var m in entityMap)
            {
                ICell cell = row1.CreateCell(i);
                cell.CellStyle = style2;
                cell.SetCellValue(m.Value);
                sheet1.SetColumnWidth(i, Encoding.Default.GetBytes(m.Value).Length * 256 * 2);
                i++;
            }
            i = 0;
            foreach (DataRow m in reportData.Rows)
            {

                NPOI.SS.UserModel.IRow row2 = sheet1.CreateRow(string.IsNullOrEmpty(headerText) ? 1 + i : 2 + i);
                int j = 0;
                foreach (var key in entityMap.Keys)
                {
                    ICell cell = row2.CreateCell(j);
                    DataColumn column = reportData.Columns[key];
                    var drValue = m[key].ToString();
                    switch (column.DataType.ToString())
                    {
                        case "System.String"://字符串类型
                            cell.SetCellValue(drValue);
                            break;
                        case "System.DateTime"://日期类型
                            DateTime dateV;
                            DateTime.TryParse(drValue, out dateV);
                            cell.SetCellValue(dateV);

                            cell.CellStyle = dateStyle;//格式化显示
                            break;
                        case "System.Boolean"://布尔型
                            bool boolV = false;
                            bool.TryParse(drValue, out boolV);
                            cell.SetCellValue(boolV);
                            break;
                        case "System.Int16"://整型
                        case "System.Int32":
                        case "System.Int64":
                        case "System.Byte":
                            int intV = 0;
                            int.TryParse(drValue, out intV);
                            cell.SetCellValue(intV);
                            break;
                        case "System.Decimal"://浮点型
                        case "System.Double":
                            double doubV = 0;
                            double.TryParse(drValue, out doubV);
                            cell.SetCellValue(doubV);
                            break;
                        case "System.DBNull"://空值处理
                            cell.SetCellValue("");
                            break;
                        default:
                            cell.SetCellValue("");
                            break;
                    }
                    cell.CellStyle = style2;
                    sheet1.SetColumnWidth(i, Encoding.Default.GetBytes(drValue).Length * 256 * 2);
                    j++;
                }

                i++;


            }
            // 写入到客户端 
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            book.Write(ms);
            ms.Seek(0, SeekOrigin.Begin);
            return ms;

        }

    }
    /// <summary>
    /// 重写Npoi流方法
    /// </summary>
    public class NpoiMemoryStream : MemoryStream
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public NpoiMemoryStream()
        {
            AllowClose = true;
        }

        /// <summary>
        /// 是否允许关闭
        /// </summary>
        public bool AllowClose { get; set; }
        /// <summary>
        /// 关闭流
        /// </summary>
        public override void Close()
        {
            if (AllowClose)
                base.Close();
        }
    }
}
