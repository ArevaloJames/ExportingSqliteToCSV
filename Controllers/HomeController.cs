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

                    using (var command = new SqliteCommand(@"
                    SELECT DISTINCT
                    i.*,
                    s.*
                    FROM
                    (
                    SELECT DISTINCT
                    Min(InTime) AS Start,
                    Max(OutTime) AS End
                    FROM
                    (
                    SELECT DISTINCT
                    date(xYear || '-01-01', '+' || (xMONTH - 1) || ' month', '+' || (xDay - 1) || ' day') INV_DATE,
                    xTANK,
                    min(time(substr(x.xSTAMP, 9, 2) || ':' || substr(x.xSTAMP, 11, 2))) InTime,
                    max(time(substr(x.xSTAMP, 9, 2) || ':' || substr(x.xSTAMP, 11, 2))) OutTime,
                    xOID
                    FROM
                    xItems x
                    INNER JOIN
                    xTickets x1 ON (x.xTid = x1.id)
                    INNER JOIN
                    xFunctions x2 ON (x.xTid = x2.xTid)
                    WHERE
                    x.xAPIFLAG = 0
                    AND (x2.xFLAG & 2) = 0
                    AND (x1.xFLAG & 2) = 0
                    AND xFTYPE = 'H'
                    AND xFINDEX = 12
                    AND xTANK <> 0
                    AND xPRICEDB <> 0
                    GROUP BY
                    INV_DATE,
                    xTANK,
                    xITEMCODE,
                    xPUMP,
                    xNOZZLE,
                    xPRICEDB
                    ) t
                    WHERE
                    INV_DATE BETWEEN date('now', 'start of year', '-1 year', '+11 months') AND date('now', 'start of month', '+1 month', '-1 day')
                    ORDER BY
                    xTANK,
                    xOID
                    ) i,

                    (
                    SELECT DISTINCT
                    date(xYear || '-01-01', '+' || (xMONTH - 1) || ' month', '+' || (xDay - 1) || ' day') INV_DATE,
                    xCORPCODE,
                    xSITECODE,
                    xTANK,
                    xPUMP,
                    xNOZZLE,
                    xYEAR,
                    xMONTH,
                    xDAY,
                    xTRANSACTION,
                    avg(x.xPRICEDB) Price,
                    sum(x.xAmountDB) AmountDB,
                    sum(x.xAmountPaid) Amount,
                    0.0 + sum(CASE WHEN x.xAmountPaid = 0 AND x.xTOTALTYPE = 5 THEN xQUANTITY ELSE 0.0 END) Calibration,
                    0.0 + sum(CASE WHEN x.xAmountPaid > 0 AND x.xTOTALTYPE < 100 AND x.xTOTALTYPE <> 5 THEN xQUANTITY ELSE 0.0 END) Volume,
                    x.xITEMCODE ItemCode,
                    x.xDESCRIPTION Particulars,
                    0.0 + min(nullif(x.xOPENTOTAL, 0) + 0) Opening,
                    max(x.xCLOSETOTAL) Closing,
                    xSTAMPDOWN AS nozdown,
                    min(time(substr(x1.xSTAMPFI, 9, 2) || ':' || substr(x1.xSTAMPFI, 11, 2))) InTime,
                    max(time(substr(x1.xSTAMPLT, 9, 2) || ':' || substr(x1.xSTAMPLT, 11, 2))) OutTime,
                    0.0 + max(x.xCLOSETOTAL) - min(nullif(x.xOPENTOTAL, 0) + 0) Liters,
                    xOID,
                    xONAME,
                    x1.xBATCH Shift,
                    x1.xTABLE plateno,
                    x1.xTENT pono,
                    ifnull(x3.xvip_fname, '') cust,
                    count(*) TransCount
                    FROM
                    xItems x
                    INNER JOIN
                    xTickets x1 ON (x.xTid = x1.id)
                    INNER JOIN
                    xFunctions x2 ON (x.xTid = x2.xTid)
                    LEFT JOIN
                    xVIP x3 ON (x.xTid = x3.xTid)
                    WHERE
                    x.xAPIFLAG = 0
                    AND (x2.xFLAG & 2) = 0
                    AND (x1.xFLAG & 2) = 0
                    AND xFTYPE = 'H'
                    AND xFINDEX = 12
                    AND xTANK <> 0
                    AND xPRICEDB <> 0
                    AND INV_DATE BETWEEN date('now', 'start of year', '-1 year', '+11 months') AND date('now', 'start of month', '+1 month', '-1 day')
                    GROUP BY
                    INV_DATE,
                    xTANK,
                    xITEMCODE,
                    xPUMP,
                    xNOZZLE,
                    xPRICEDB,
                    xTRANSACTION,
                    x.id
                    ) s
                    ORDER BY
                    xYear,
                    xMonth,
                    xDay,
                    Opening,
                    xTANK,
                    xOID;", sqliteConnection))
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

                    using (var commandSafedrop = new SqliteCommand(@"
                        SELECT DISTINCT
                        s.*
                        FROM
                        (
                        SELECT DISTINCT
                        DATE(xYear || '-01-01', '+' || (xMONTH - 1) || ' month', '+' || (xDay - 1) || ' day') AS INV_DATE,
                        DATE(SUBSTR(x1.xSTAMP, 1, 4) || '-' || SUBSTR(x1.xSTAMP, 5, 2) || '-' || SUBSTR(x1.xSTAMP, 7, 2)) AS BDate,
                        xYEAR,
                        xMONTH,
                        xDAY,
                        xCORPCODE,
                        xSITECODE,
                        TIME(SUBSTR(x1.xSTAMP, 9, 2) || ':' || SUBSTR(x1.xSTAMP, 11, 2) || ':' || SUBSTR(x1.xSTAMP, 13, 2)) AS TTime,
                        x1.xSTAMP,
                        x1.xOID,
                        xONAME,
                        xBatch AS Shift,
                        xAMOUNT1 AS Amount
                        FROM 
                        xFunctions x2 
                        INNER JOIN 
                        xTickets x1 ON x2.xTid = x1.id
                        WHERE 
                        x2.xACCOUNTCODE IN ('SD', 'LC')
                        ) s
                        INNER JOIN 
                        xml_USERINFO u ON 1=1
                        WHERE 
                        INV_DATE BETWEEN DATE(STRFTIME('%Y', 'now', '-1 year') || '-12-01') AND DATE('now')
                        ORDER BY 
                        INV_DATE;", sqliteConnection))
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
                        using (var commandLubes = new SqliteCommand(@"
                        SELECT DISTINCT
                        s1.*
                        FROM
                        (
                        SELECT DISTINCT
                        date(xYear||'-01-01','+'||(xMONTH-1)||' month','+'||(xDay-1)||' day') INV_DATE,
                        xYEAR,
                        xMONTH,
                        xDAY,
                        xCORPCODE,
                        xSITECODE,
                        avg(x.xPRICEDB) Price,
                        sum(x.xAmountDB) AmountDB,
                        sum(x.xAmountPaid) Amount,
                        sum(CASE WHEN x.xTOTALTYPE < 100 THEN 0.0+xQUANTITY ELSE 0.0 END) LubesQty,
                        x.xITEMCODE ItemCode,
                        x.xDESCRIPTION Particulars,
                        xOID,
                        xONAME Cashier,
                        xBATCH as Shift,
                        xTRANSACTION,
                        x.xStamp,
                        x1.xTABLE plateno,
                        x1.xTENT pono,
                        ifnull(x3.xvip_fname,'') cust
                        FROM
                        xItems x 
                        INNER JOIN
                        xTickets x1 ON (x.xTid=x1.id)
                        INNER JOIN
                        xFunctions x2 ON (x.xTid=x2.xTid)
                        LEFT JOIN
                        xVIP x3 ON (x.xTid=x3.xTid)
                        WHERE
                        x.xAPIFLAG=0 
                        AND (x1.xFLAG & 2) = 0 
                        AND xFTYPE='H' 
                        AND xFINDEX=12 
                        AND xTANK=0  
                        AND xPRICEDB <> 0 
                        GROUP BY
                        INV_DATE,
                        xITEMCODE 
                        ORDER BY
                        INV_DATE,
                        xITEMCODE
                        ) s1 
                        WHERE  
                        INV_DATE BETWEEN DATE(STRFTIME('%Y', 'now', '-1 year') || '-12-01') AND DATE('now')
                        GROUP BY 
                        INV_DATE,
                        ITEMCODE 
                        ORDER BY 
                        INV_DATE,
                        ITEMCODE
", sqliteConnection))
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


