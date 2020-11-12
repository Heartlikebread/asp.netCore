using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Epplus_Excel_Example.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using System.IO;
using OfficeOpenXml;
using System.Data;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;

namespace Epplus_Excel_Example.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private IHostEnvironment _env; //抓專案位置
        public HomeController(ILogger<HomeController> logger, IHostEnvironment env)
        {
            _logger = logger;
            _env = env;
        }

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult SingleFile(IFormFile file)
        {
            var dir = _env.ContentRootPath;
            StreamReader reader = new StreamReader(file.OpenReadStream());
            String content = reader.ReadToEnd();
            string name = file.FileName;

            int iIndex = name.LastIndexOf(".");

            //取得副檔名
            var extension= name.Substring(iIndex);
            if (extension != ".xlsx")
            {
                throw new Exception("檔案必須是.xlsx！");
            }

            string filename = dir + "/wwwroot/暫存區/" + name;

            //檢查目錄 沒有就建立一筆
            Censor(dir + "/wwwroot/暫存區/");

            //檢查是否有重複檔案刪除
            //if (System.IO.File.Exists(filename))
            //{
            //    System.IO.File.Delete(filename);
            //}
            #region 取得檔案存暫存區
            //SaveTemporary(file, filename);
            #endregion

            #region 讀取Excel轉成table 檢查頁籤 給程式使用
            LoadToTable(filename);
            #endregion

            #region 對Excel進行讀取與寫入
            ReadWrite(filename);
            #endregion

            return RedirectToAction("Index");
        }
        /// <summary>
        /// 檢查體路徑是否存在若不存在建立目錄
        /// </summary>
        private void Censor(string path)
        {
            if (Directory.Exists(path))
            {
                //資料夾存在
            }
            else
            {
                //新增資料夾
                Directory.CreateDirectory(path);
            }

        }
        /// <summary>
        /// 取得檔案存暫存區
        /// </summary>
        /// <param name="path"></param>
        private void SaveTemporary(IFormFile file,string path)
        {
            using (FileStream fs = System.IO.File.Create(path))
            {
                #region 單純取得檔案
                file.CopyTo(fs);
                fs.Flush();
                #endregion
            }
        }
        /// <summary>
        /// 讀取Excel表進行Table轉換
        /// </summary>
        /// <param name="path"></param>
        private void LoadToTable(string path) {
            //開檔
            using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                //確認是否有輸入最大讀取行數
                int numReadRows = 1000;
                string[] columns;
                int endRowNumber;
                DataTable sourceTable = new DataTable();

                #region xlsx檔案格式
                //載入Excel檔案
                ExcelPackage ep = new ExcelPackage(fs);
                ExcelWorksheet sheet = ep.Workbook.Worksheets[0];
                if (sheet == null)
                {

                    throw new Exception("載入的 Excel 檔案中沒有Sheet！");
                }

                //取得資料行數
                numReadRows = sheet.Dimension.End.Row;

                //根據Header的長度決定宣告大小，總共欄位大小
                columns = new string[sheet.Dimension.Columns];

                //先由Excel取得欄位名稱
                for (int colNum = sheet.Dimension.Start.Column; colNum < sheet.Dimension.End.Column + 1; colNum++)
                {
                    if (sheet.Cells[1, colNum].Text.TrimEnd().Length>0) // 如果有欄位標題為空不讀進來
                    {
                        columns[colNum - 1] = sheet.Cells[1, colNum].Text.Replace("\n", "").Trim();
                    }
                }

                //新增欄位名稱
                for (int i = 0; i < columns.Length; i++)
                {
                    if (columns[i].TrimEnd().Length== 0)
                    {
                        continue;
                    }
                    sourceTable.Columns.Add(columns[i], typeof(string));
                }
                sourceTable.AcceptChanges();

                //取得最大讀取行數
                if (numReadRows + 1 <= sheet.Dimension.End.Row)
                {
                    endRowNumber = numReadRows + 1;
                }
                else
                {
                    endRowNumber = sheet.Dimension.End.Row;
                }

                //將檔案內容轉換為DataTable型態
                for (int rowNum = 2; rowNum <= endRowNumber; rowNum++)
                {
                    DataRow dtrow = sourceTable.NewRow();

                    //將目前此筆資料寫入DataRow
                    for (int i = 0; i < columns.Length; i++)
                    {
                        string colName = columns[i];
                        if (colName.Trim().Length>0)
                        {
                            dtrow[colName] = sheet.Cells[rowNum, i + 1].Text;
                        }
                    }

                    sourceTable.Rows.Add(dtrow);
                }
                #endregion           
            }
        }
        /// <summary>
        /// 讀取某個頁籤 寫入資料
        /// </summary>
        /// <param name="path">檔案位置</param>
        private void ReadWrite(string path)
        {
            #region 模擬寫入table
            var _MastTable = new DataTable();
            //加入欄位名稱
            _MastTable.Columns.Add("欄位一");
            _MastTable.Columns.Add("欄位二");
            _MastTable.Columns.Add("欄位三");
            _MastTable.Columns.Add("欄位四");
            _MastTable.AcceptChanges();

            //塞入資料
            var rand = new Random();
            for (int j = 0; j < 10; j++)
            {
                var newRow = _MastTable.NewRow();
                newRow["欄位一"] = string.Format("{0}", j+1 * rand.Next(0, 100));
                newRow["欄位二"] = string.Format("{0}", j+1 * rand.Next(0, 100));
                newRow["欄位三"] = string.Format("{0}", j+1 * rand.Next(0, 100));
                newRow["欄位四"] = string.Format("{0}", j+1 * rand.Next(0, 100));
                _MastTable.Rows.Add(newRow);
            }
            
            #endregion

            //開檔
            using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Read, FileShare.ReadWrite))
            {
                //载入Excel文件
                using (ExcelPackage ep = new ExcelPackage(fs))
                {
                    ExcelWorksheet sheet = ep.Workbook.Worksheets["資料表"];
                    if (sheet == null)
                    {
                        throw new Exception("載入的 Excel 檔案中沒有Sheet！");                        
                    }
                    //塞入table
                    sheet.Cells.LoadFromDataTable(_MastTable, true);
                    #region 手動塞入寫法
                    ////塞標頭欄位    
                    //for (int j = 0; j < _MastTable.Columns.Count; j++)
                    //{
                    //    //只印出此次補號的前幾筆                         
                    //    sheet.Cells[1, j + 1].Value = _MastTable.Columns[j];
                    //}

                    ////塞資料                    
                    //for (int i = 0; i < _MastTable.Rows.Count; i++)
                    //{
                    //    for (int j = 0; j < _MastTable.Columns.Count; j++)
                    //    {
                    //        sheet.Cells[i + 1, j + 1].Value = _MastTable.Rows[i][j].ToString();
                    //    }
                    //}
                    #endregion
                    //建立文件
                    using (FileStream createStream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
                    {
                        ep.SaveAs(createStream);//存檔
                    }
                }
            }
        }
        /// <summary>
        /// 建置Execl
        /// </summary>
        /// <param name="path"></param>
        public string CreatExcel()
        {
            #region 模擬寫入table
            var _MastTable = new DataTable();
            //加入欄位名稱
            _MastTable.Columns.Add("欄位一");
            _MastTable.Columns.Add("欄位二");
            _MastTable.Columns.Add("欄位三");
            _MastTable.Columns.Add("欄位四");
            _MastTable.AcceptChanges();

            //塞入資料
            var rand = new Random();
            for (int j = 0; j < 10; j++)
            {
                var newRow = _MastTable.NewRow();
                newRow["欄位一"] = string.Format("{0}", j+1 * rand.Next(0, 100));
                newRow["欄位二"] = string.Format("{0}", j+1 * rand.Next(0, 100));
                newRow["欄位三"] = string.Format("{0}", j+1 * rand.Next(0, 100));
                newRow["欄位四"] = string.Format("{0}", j+1 * rand.Next(0, 100));
                _MastTable.Rows.Add(newRow);
            }

            #endregion
            string sid = DateTime.Now.ToString("yyyyMMddHHmmss");
            var path = _env.ContentRootPath+"/wwwroot/暫存區/" + sid + ".xlsx";
           
            System.IO.FileInfo filePath = new System.IO.FileInfo(path);

            if (!System.IO.File.Exists(filePath.ToString()))
            {

                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                //建立一筆新的Excel文件
                ExcelPackage ep = new ExcelPackage();
                //產生頁籤
                ExcelWorksheet sheet = ep.Workbook.Worksheets.Add("資料表");
                //塞入table
                sheet.Cells.LoadFromDataTable(_MastTable, true);
                #region 手動塞入寫法
                ////塞標頭欄位    
                //for (int j = 0; j < _MastTable.Columns.Count; j++)
                //{
                //    //只印出此次補號的前幾筆                         
                //    sheet.Cells[1, j + 1].Value = _MastTable.Columns[j];
                //}

                ////塞資料                    
                //for (int i = 0; i < _MastTable.Rows.Count; i++)
                //{
                //    for (int j = 0; j < _MastTable.Columns.Count; j++)
                //    {
                //        sheet.Cells[i + 1, j + 1].Value = _MastTable.Rows[i][j].ToString();
                //    }
                //}
                #endregion
                //建立文件
                ep.SaveAs(new FileInfo(path));
            }
            return "OK";
        }



        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            //return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            return View();
        }
    }
}
