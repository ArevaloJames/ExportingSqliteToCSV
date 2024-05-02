using ExportingSqliteToCsv.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using System.Diagnostics;
using System.Text;

namespace ExportingSqliteToCsv.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

       

        public IActionResult Index()
        {
            string sqliteFilePath = Path.Combine("D:\\RealSoft\\RealPOS\\realhq\\", "journal.sqlite");

            if (!System.IO.File.Exists(sqliteFilePath))
            {
                TempData["ExportError"] = "Database is not found.";
                TempData["ExportErrorPrompt"] = "Please contact MIS - Enterprise.";


                return RedirectToAction("FirstPage");
            }


            return RedirectToAction("FirstPage");
        }

        public IActionResult FirstPage()
        {
            return View();

        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        //public IActionResult Read()
        //{
        //    string sqliteFilePath = Path.Combine("D:\\Work\\Work Filpride\\", "journal.sqlite");
        //    var resultDict = new Dictionary<string, List<object>>();

        //    var stopwatch = Stopwatch.StartNew(); // Start the stopwatch

        //    using (var sqliteConnection = new SqliteConnection($"Data Source={sqliteFilePath};"))
        //    {
        //        sqliteConnection.Open();

        //        using (var command = new SqliteCommand("SELECT DISTINCT\r\ni.*,s.*\r\nFROM\r\n(SELECT DISTINCT\r\nMin(InTime) as Start,Max(OutTime) as End       \r\nFROM(\r\nSELECT DISTINCT\r\ndate(xYear||'-01-01','+'||(xMONTH-1)||' month','+'||(xDay-1)||' day') INV_DATE,\r\nxTANK,\r\nmin(time(substr(x.xSTAMP,9,2)||':'||substr(x.xSTAMP,11,2))) InTime,\r\nmax(time(substr(x.xSTAMP,9,2)||':'||substr(x.xSTAMP,11,2))) OutTime,  \r\nxOID                                   \r\nFROM xItems x \r\nINNER JOIN xTickets x1 ON (x.xTid=x1.id)\r\nINNER JOIN xFunctions x2 ON (x.xTid=x2.xTid)\r\nWHERE  x.xAPIFLAG=0 and (x2.xFLAG & 2)= 0 and (x1.xFLAG & 2)=0 and xFTYPE='H' and xFINDEX=12 and xTANK <>0 and xPRICEDB <> 0\r\n GROUP BY INV_DATE,xTANK,xITEMCODE,xPUMP,xNOZZLE,xPRICEDB) t WHERE 1=1  \r\n--AND INV_DATE='2023-12-01'\r\nand INV_DATE between strftime('%Y','now')||'-01-01' and strftime('%Y','now')||'-'||strftime('%m','now')||'-'||strftime('%d','now')\r\n ORDER BY xTANK,xOID) i,\r\n\r\n(SELECT DISTINCT\r\ndate(xYear||'-01-01','+'||(xMONTH-1)||' month','+'||(xDay-1)||' day') INV_DATE,\r\nxCORPCODE,xSITECODE,xTANK,xPUMP,xNOZZLE,\r\nxYEAR,xMONTH,xDAY,xTRANSACTION,                                          \r\navg(x.xPRICEDB) Price,    \r\nsum(x.xAmountDB) AmountDB,                                                                                                      \r\nsum(x.xAmountPaid) Amount,\r\n0.0+sum( case when x.xAmountPaid=0 and x.xTOTALTYPE=5 then xQUANTITY else 0.0 end) Calibration,\r\n0.0+sum( case when x.xAmountPaid > 0 and x.xTOTALTYPE < 100 and x.xTOTALTYPE<>5 then xQUANTITY else 0.0 end) Volume,                                                                                                                                     \r\nx.xITEMCODE ItemCode,\r\nx.xDESCRIPTION Particulars,\r\n0.0+min(nullif(x.xOPENTOTAL,0)+0) Opening,\r\nmax(x.xCLOSETOTAL) Closing,\r\nxSTAMPDOWN as nozdown,\r\nmin(time(substr(x1.xSTAMPFI,9,2)||':'||substr(x1.xSTAMPFI,11,2))) InTime,\r\nmax(time(substr(x1.xSTAMPLT,9,2)||':'||substr(x1.xSTAMPLT,11,2))) OutTime,                                               \r\n0.0+max(x.xCLOSETOTAL)-min(nullif(x.xOPENTOTAL,0)+0) Liters,\r\nxOID,xONAME,x1.xBATCH Shift, \r\nx1.xTABLE plateno,\r\nx1.xTENT pono,\r\nifnull(x3.xvip_fname,'') cust,\r\ncount(*) TransCount                                                                                   \r\nFROM xItems x \r\nINNER JOIN xTickets x1 ON (x.xTid=x1.id)\r\nINNER JOIN xFunctions x2 ON (x.xTid=x2.xTid)\r\nLEFT JOIN xVIP x3 ON (x.xTid=x3.xTid)\r\nWHERE  x.xAPIFLAG=0 and (x2.xFLAG & 2)= 0 and (x1.xFLAG & 2)=0 and xFTYPE='H' and xFINDEX=12 and xTANK <>0 and xPRICEDB <> 0\r\n GROUP BY INV_DATE,xTANK,xITEMCODE,xPUMP,xNOZZLE,xPRICEDB,xTRANSACTION,x.id) s WHERE 1=1  \r\n --AND INV_DATE='2023-12-01'\r\n--and xYEAR=strftime('%Y','now') and xMONTH=strftime('%m','now') and xDay=strftime('%d','now')\r\nand INV_DATE between strftime('%Y','now')||'-01-01' and strftime('%Y','now')||'-'||strftime('%m','now')||'-'||strftime('%d','now')\r\n ORDER BY xTANK,xOID\r\n;", sqliteConnection))
        //        {
        //            using (var reader = command.ExecuteReader())
        //            {
        //                // Get column names
        //                var columnNames = new List<string>();
        //                for (int i = 0; i < reader.FieldCount; i++)
        //                {
        //                    columnNames.Add(reader.GetName(i));
        //                }

        //                // Initialize dictionary with empty lists for each column
        //                foreach (var columnName in columnNames)
        //                {
        //                    resultDict[columnName] = new List<object>();
        //                }

        //                // Read data into dictionary
        //                while (reader.Read())
        //                {
        //                    foreach (var columnName in columnNames)
        //                    {
        //                        resultDict[columnName].Add(reader[columnName]);
        //                    }
        //                }
        //            }
        //        }
        //    }



        //    stopwatch.Stop(); // Stop the stopwatch
        //    var executionTime = stopwatch.ElapsedMilliseconds; // Get the elapsed time in milliseconds

        //    ViewBag.ExecutionTime = executionTime; // Pass the execution time to the view

        //    return View(resultDict);


        //}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ExportToCSV()
        {
            try
            {
                string sqliteFilePath = Path.Combine("D:\\RealSoft\\RealPOS\\realhq\\", "journal.sqlite");
                
                if (!System.IO.File.Exists(sqliteFilePath))
                {
                    TempData["ExportError"] = "Database is not found.";

                }

                var resultDict = new Dictionary<string, List<object>>();
                var resultDictSafedrop = new Dictionary<string, List<object>>();
                var resultDictLubes = new Dictionary<string, List<object>>();


                using (var sqliteConnection = new SqliteConnection($"Data Source={sqliteFilePath};"))
                {
                    sqliteConnection.Open();

                    using (var command = new SqliteCommand("SELECT DISTINCT\r\n    i.*,\r\n    s.*\r\nFROM\r\n    (SELECT DISTINCT\r\n         Min(InTime) AS Start,\r\n         Max(OutTime) AS End\r\n     FROM\r\n         (SELECT DISTINCT\r\n              date(xYear || '-01-01', '+' || (xMONTH - 1) || ' month', '+' || (xDay - 1) || ' day') INV_DATE,\r\n              xTANK,\r\n              min(time(substr(x.xSTAMP, 9, 2) || ':' || substr(x.xSTAMP, 11, 2))) InTime,\r\n              max(time(substr(x.xSTAMP, 9, 2) || ':' || substr(x.xSTAMP, 11, 2))) OutTime,\r\n              xOID\r\n          FROM\r\n              xItems x\r\n          INNER JOIN\r\n              xTickets x1 ON (x.xTid = x1.id)\r\n          INNER JOIN\r\n              xFunctions x2 ON (x.xTid = x2.xTid)\r\n          WHERE\r\n              x.xAPIFLAG = 0\r\n              AND (x2.xFLAG & 2) = 0\r\n              AND (x1.xFLAG & 2) = 0\r\n              AND xFTYPE = 'H'\r\n              AND xFINDEX = 12\r\n              AND xTANK <> 0\r\n              AND xPRICEDB <> 0\r\n          GROUP BY\r\n              INV_DATE,\r\n              xTANK,\r\n              xITEMCODE,\r\n              xPUMP,\r\n              xNOZZLE,\r\n              xPRICEDB) t\r\n     WHERE\r\n         INV_DATE BETWEEN date('now', 'start of year', '-1 year', '+11 months') AND date('now', 'start of month', '+1 month', '-1 day')\r\n     ORDER BY\r\n         xTANK,\r\n         xOID) i,\r\n\r\n    (SELECT DISTINCT\r\n         date(xYear || '-01-01', '+' || (xMONTH - 1) || ' month', '+' || (xDay - 1) || ' day') INV_DATE,\r\n         xCORPCODE,\r\n         xSITECODE,\r\n         xTANK,\r\n         xPUMP,\r\n         xNOZZLE,\r\n         xYEAR,\r\n         xMONTH,\r\n         xDAY,\r\n         xTRANSACTION,\r\n         avg(x.xPRICEDB) Price,\r\n         sum(x.xAmountDB) AmountDB,\r\n         sum(x.xAmountPaid) Amount,\r\n         0.0 + sum(CASE WHEN x.xAmountPaid = 0 AND x.xTOTALTYPE = 5 THEN xQUANTITY ELSE 0.0 END) Calibration,\r\n         0.0 + sum(CASE WHEN x.xAmountPaid > 0 AND x.xTOTALTYPE < 100 AND x.xTOTALTYPE <> 5 THEN xQUANTITY ELSE 0.0 END) Volume,\r\n         x.xITEMCODE ItemCode,\r\n         x.xDESCRIPTION Particulars,\r\n         0.0 + min(nullif(x.xOPENTOTAL, 0) + 0) Opening,\r\n         max(x.xCLOSETOTAL) Closing,\r\n         xSTAMPDOWN AS nozdown,\r\n         min(time(substr(x1.xSTAMPFI, 9, 2) || ':' || substr(x1.xSTAMPFI, 11, 2))) InTime,\r\n         max(time(substr(x1.xSTAMPLT, 9, 2) || ':' || substr(x1.xSTAMPLT, 11, 2))) OutTime,\r\n         0.0 + max(x.xCLOSETOTAL) - min(nullif(x.xOPENTOTAL, 0) + 0) Liters,\r\n         xOID,\r\n         xONAME,\r\n         x1.xBATCH Shift,\r\n         x1.xTABLE plateno,\r\n         x1.xTENT pono,\r\n         ifnull(x3.xvip_fname, '') cust,\r\n         count(*) TransCount\r\n     FROM\r\n         xItems x\r\n     INNER JOIN\r\n         xTickets x1 ON (x.xTid = x1.id)\r\n     INNER JOIN\r\n         xFunctions x2 ON (x.xTid = x2.xTid)\r\n     LEFT JOIN\r\n         xVIP x3 ON (x.xTid = x3.xTid)\r\n     WHERE\r\n         x.xAPIFLAG = 0\r\n         AND (x2.xFLAG & 2) = 0\r\n         AND (x1.xFLAG & 2) = 0\r\n         AND xFTYPE = 'H'\r\n         AND xFINDEX = 12\r\n         AND xTANK <> 0\r\n         AND xPRICEDB <> 0\r\n         AND INV_DATE BETWEEN date('now', 'start of year', '-1 year', '+11 months') AND date('now', 'start of month', '+1 month', '-1 day')\r\n     GROUP BY\r\n         INV_DATE,\r\n         xTANK,\r\n         xITEMCODE,\r\n         xPUMP,\r\n         xNOZZLE,\r\n         xPRICEDB,\r\n         xTRANSACTION,\r\n         x.id) s\r\nORDER BY\r\n    xYear,\r\n    xMonth,\r\n    xDay,\r\n    Opening,\r\n    xTANK,\r\n    xOID;\r\n", sqliteConnection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            var columnNames = new List<string>();
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                columnNames.Add(reader.GetName(i));
                            }

                            foreach (var columnName in columnNames)
                            {
                                resultDict[columnName] = new List<object>();
                            }

                            while (reader.Read())
                            {
                                foreach (var columnName in columnNames)
                                {
                                    resultDict[columnName].Add(reader[columnName]);
                                }
                            }
                        }
                    }

                    using (var commandSafedrop = new SqliteCommand("SELECT DISTINCT\r\n    s.*\r\nFROM (\r\n    SELECT DISTINCT\r\n        DATE(xYear || '-01-01', '+' || (xMONTH - 1) || ' month', '+' || (xDay - 1) || ' day') AS INV_DATE,\r\n        DATE(SUBSTR(x1.xSTAMP, 1, 4) || '-' || SUBSTR(x1.xSTAMP, 5, 2) || '-' || SUBSTR(x1.xSTAMP, 7, 2)) AS BDate,\r\n        xYEAR,\r\n        xMONTH,\r\n        xDAY,\r\n        xCORPCODE,\r\n        xSITECODE,\r\n        TIME(SUBSTR(x1.xSTAMP, 9, 2) || ':' || SUBSTR(x1.xSTAMP, 11, 2) || ':' || SUBSTR(x1.xSTAMP, 13, 2)) AS TTime,\r\n        x1.xSTAMP,\r\n        x1.xOID,\r\n        xONAME,\r\n        xBatch AS Shift,\r\n        xAMOUNT1 AS Amount\r\n    FROM \r\n        xFunctions x2 \r\n    INNER JOIN \r\n        xTickets x1 ON x2.xTid = x1.id\r\n    WHERE \r\n        x2.xACCOUNTCODE IN ('SD', 'LC')\r\n) s\r\nINNER JOIN \r\n    xml_USERINFO u ON 1=1\r\nWHERE \r\n    INV_DATE BETWEEN DATE(STRFTIME('%Y', 'now', '-1 year') || '-12-01') AND DATE('now')\r\nORDER BY \r\n    INV_DATE;\r\n", sqliteConnection))
                    {
                        using (var reader = commandSafedrop.ExecuteReader())
                        {
                            var columnNames = new List<string>();
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                columnNames.Add(reader.GetName(i));
                            }

                            foreach (var columnName in columnNames)
                            {
                                resultDictSafedrop[columnName] = new List<object>();
                            }

                            while (reader.Read())
                            {
                                foreach (var columnName in columnNames)
                                {
                                    resultDictSafedrop[columnName].Add(reader[columnName]);
                                }



                            }
                        }
                        using (var commandLubes = new SqliteCommand("SELECT DISTINCT\r\ns1.*\r\nFROM\r\n(SELECT DISTINCT\r\ndate(xYear||'-01-01','+'||(xMONTH-1)||' month','+'||(xDay-1)||' day') INV_DATE,\r\nxYEAR,xMONTH,xDAY,xCORPCODE,xSITECODE,                                          \r\navg(x.xPRICEDB) Price,    \r\nsum(x.xAmountDB) AmountDB,                                                                                                      \r\nsum(x.xAmountPaid) Amount,\r\nsum( case  when x.xTOTALTYPE < 100 then 0.0+xQUANTITY else 0.0 end) LubesQty,\r\nx.xITEMCODE ItemCode,\r\nx.xDESCRIPTION Particulars,\r\nxOID,xONAME Cashier,xBATCH as Shift, xTRANSACTION, x.xStamp,\r\nx1.xTABLE plateno,\r\nx1.xTENT pono,\r\nifnull(x3.xvip_fname,'') cust                      \r\nFROM xItems x \r\nINNER JOIN xTickets x1 ON (x.xTid=x1.id)\r\nINNER JOIN xFunctions x2 ON (x.xTid=x2.xTid)\r\nLEFT JOIN xVIP x3 ON (x.xTid=x3.xTid)\r\nWHERE  x.xAPIFLAG=0 and (x1.xFLAG & 2)= 0 and xFTYPE='H' and xFINDEX=12 and xTANK=0  and xPRICEDB <> 0 \r\nGROUP BY INV_DATE,xITEMCODE \r\nORDER BY INV_DATE,xITEMCODE) s1 \r\nWHERE  INV_DATE BETWEEN DATE(STRFTIME('%Y', 'now', '-1 year') || '-12-01') AND DATE('now')\r\nGROUP BY INV_DATE,ITEMCODE ORDER BY INV_DATE,ITEMCODE\r\n", sqliteConnection))
                        {
                            using (var reader = commandLubes.ExecuteReader())
                            {
                                var columnNames = new List<string>();
                                for (int i = 0; i < reader.FieldCount; i++)
                                {
                                    columnNames.Add(reader.GetName(i));
                                }

                                foreach (var columnName in columnNames)
                                {
                                    resultDictLubes[columnName] = new List<object>();
                                }

                                while (reader.Read())
                                {
                                    foreach (var columnName in columnNames)
                                    {
                                        resultDictLubes[columnName].Add(reader[columnName]);
                                    }



                                }
                            }
                        }

                        try
                        {
                            // Export Fuel Data to CSV
                            string fuelFolderPath = "D:\\RealSoft\\RealPOS\\realhq";
                            string fuelFileName = "1143-Fuels-" + DateTime.Now.Year + ".csv";
                            string fuelCsvFilePath = Path.Combine(fuelFolderPath, fuelFileName);

                            if (!Directory.Exists(fuelFolderPath))
                            {
                                Directory.CreateDirectory(fuelFolderPath);
                            }

                            using (StreamWriter sw = new StreamWriter(fuelCsvFilePath, false, Encoding.UTF8))
                            {
                                sw.WriteLine(string.Join(",", resultDict.Keys));

                                for (int i = 0; i < resultDict.Values.First().Count; i++)
                                {
                                    List<string> rowData = new List<string>();
                                    foreach (var key in resultDict.Keys)
                                    {
                                        rowData.Add(resultDict[key][i].ToString());
                                    }
                                    sw.WriteLine(string.Join(",", rowData));
                                }
                            }

                            // Export Safe Drop Data to CSV
                            string safeDropFolderPath = "D:\\RealSoft\\RealPOS\\realhq";
                            string safeDropFileName = "1143-Safedrop-" + DateTime.Now.Year + ".csv";
                            string safeDropCsvFilePath = Path.Combine(safeDropFolderPath, safeDropFileName);

                            if (!Directory.Exists(safeDropFolderPath))
                            {
                                Directory.CreateDirectory(safeDropFolderPath);
                            }

                            using (StreamWriter sw = new StreamWriter(safeDropCsvFilePath, false, Encoding.UTF8))
                            {
                                sw.WriteLine(string.Join(",", resultDictSafedrop.Keys));

                                for (int i = 0; i < resultDictSafedrop.Values.First().Count; i++)
                                {
                                    List<string> rowData = new List<string>();
                                    foreach (var key in resultDictSafedrop.Keys)
                                    {
                                        rowData.Add(resultDictSafedrop[key][i].ToString());
                                    }
                                    sw.WriteLine(string.Join(",", rowData));
                                }
                            }
                            // Export Lubes Data to CSV
                            string lubesFolderPath = "D:\\RealSoft\\RealPOS\\realhq";
                            string lubesFileName = "1143-Lubes-" + DateTime.Now.Year + ".csv";
                            string lubesCsvFilePath = Path.Combine(lubesFolderPath, lubesFileName);

                            if (!Directory.Exists(lubesFolderPath))
                            {
                                Directory.CreateDirectory(lubesFolderPath);
                            }

                            using (StreamWriter sw = new StreamWriter(lubesCsvFilePath, false, Encoding.UTF8))
                            {
                                sw.WriteLine(string.Join(",", resultDictLubes.Keys));

                                for (int i = 0; i < resultDictLubes.Values.First().Count; i++)
                                {
                                    List<string> rowData = new List<string>();
                                    foreach (var key in resultDictLubes.Keys)
                                    {
                                        rowData.Add(resultDictLubes[key][i].ToString());
                                    }
                                    sw.WriteLine(string.Join(",", rowData));
                                }
                            }



                            TempData["ExportSuccess"] = "Files has been generated successfully.";

                            int fuelsCount = resultDict.First().Value.Count; // Assuming each dictionary entry represents a row of data
                            int safeDropCount = resultDictSafedrop.First().Value.Count; // Assuming each dictionary entry represents a row of data
                            int lubesCount = resultDictLubes.First().Value.Count; // Assuming each dictionary entry represents a row of data

                            // Store the counts in TempData
                            TempData["FuelsRowCount"] = fuelsCount;
                            TempData["SafeDropRowCount"] = safeDropCount;
                            TempData["LubesRowCount"] = lubesCount;

                            return RedirectToAction("RowCountsView");

                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Error occurred while uptading the CSV files.");

                            // Set error message if CSV export fails
                            TempData["ExportError"] = "A CSV file is open.";
                            TempData["ExportErrorPrompt"] = "Please close all files before confirming.";
                            
                            
                            return RedirectToAction("Index");

                        }


                    }
                    
                }


               
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while uptading the CSV files.");

                // Set error message if CSV export fails
                TempData["ExportErrorPrompt"] = "Please Contact MIS - Enterprise.";


                return RedirectToAction("Index");

            }




        }
        public IActionResult RowCountsView()
        {
            // Retrieve row counts from TempData
            int fuelsCount = (int)TempData["FuelsRowCount"];
            int safeDropCount = (int)TempData["SafeDropRowCount"];
            int lubesCount = (int)TempData["LubesRowCount"];

            // Pass row counts to the view model
            var viewModel = new RowCountsViewModel
            {
                FuelsRowCount = fuelsCount,
                SafeDropRowCount = safeDropCount,
                LubesRowCount = lubesCount
            };

            // Render the view and pass the view model to it
            return View("RowCountsView", viewModel);
        }

    }


}


