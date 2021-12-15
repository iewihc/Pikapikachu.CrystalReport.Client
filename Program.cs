using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Xml;
using ChoETL;
using CommandLine;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using Pikapikachu.CrystalReport.Client.Models;
using Pikapikachu.CrystalReport.Client.Options;

namespace Pikapikachu.CrystalReport.Client
{
    internal class Program
    {
        static int Main(string[] args)
        {
            var exitCode = Parser.Default.ParseArguments<StandardOptions, CleanOptions, SourceOptions, TestOptions>(args).MapResult(
                (StandardOptions o) => DoMake(o)
                , (CleanOptions o) => DoClean(o)
                , (SourceOptions o) => DoSource(o)
                , (TestOptions o) => DoTest(o),
                error => DoError(error));

            return exitCode;
        }


        private static int DoTest(TestOptions opt)
        {
            var sw = new Stopwatch();
            sw.Start();

            var dir = Directory.GetCurrentDirectory();

            var outputPDFPath = opt.IsSimple ? Path.Combine(dir, "easy.pdf") : Path.Combine(dir, "complex.pdf");

            if (File.Exists(outputPDFPath))
                File.Delete(outputPDFPath);

            var rd = new ReportDocument();

            var currentDirectory = Directory.GetCurrentDirectory();

            var rdPath = opt.IsSimple ? Path.Combine(dir, "Source\\easy.rpt") : Path.Combine(dir, "Source\\complex.rpt");
            rd.Load(rdPath);

            var jsonPath = opt.IsSimple ? Path.Combine(currentDirectory, "Source\\easy.json") : Path.Combine(currentDirectory, "Source\\complex.json");
            var jsonString = File.ReadAllText(jsonPath);

            var ds = JsonStringConvertToDataSet(jsonString);

            ds.WriteXml(Path.Combine(currentDirectory, "Source\\output_sample.xml"));
            rd.SetDataSource(ds);
            rd.ExportToDisk(ExportFormatType.PortableDocFormat, outputPDFPath);

            Console.WriteLine($"【輸出路徑】: {outputPDFPath}");
            sw.Stop();


            Console.WriteLine($"共耗費： {sw.Elapsed.TotalMilliseconds.ToString()}毫秒");

            return 1;
        }

        private static int DoSource(SourceOptions opt)
        {
            var jsonString = File.ReadAllText(opt.JsonFilePath);

            var ds = JsonStringConvertToDataSet(jsonString);

            var rootPath = Path.Combine(Directory.GetCurrentDirectory(), $"Temp");
            if (!Directory.Exists(rootPath))
                Directory.CreateDirectory(rootPath);

            var outputXMLPath = Path.Combine(Directory.GetCurrentDirectory(), "source.xml");
            ds.WriteXml(outputXMLPath);

            Console.WriteLine(outputXMLPath);

            return 1;
        }

        private static int DoClean(CleanOptions opt)
        {
            // 輸出路徑為空
            if (string.IsNullOrEmpty(opt.OutputPath))
            {
                opt.OutputPath = Path.Combine(Directory.GetCurrentDirectory(), "Temp");
            }

            Console.WriteLine($"Cleaning... {opt.OutputPath}");

            var dir = new DirectoryInfo(opt.OutputPath);
            var files = dir.GetFiles();
            foreach (var file in files)
            {
                file.Delete();
            }

            Console.WriteLine("Files deleted successfully");
            return 1;
        }

        private static int DoMake(StandardOptions opt)
        {
            ValidationOptions(opt);

            # region Report Running

            try
            {
                var sw = new Stopwatch();
                sw.Start();

                var outputPDFPath = Path.Combine(opt.OutputPath, $"{opt.FileName}.pdf");
                var outputXMLPath = Path.Combine(opt.OutputPath, $"output.xml");

                // source
                if (!Directory.Exists(opt.OutputPath))
                    Directory.CreateDirectory(opt.OutputPath);

                if (File.Exists(outputXMLPath))
                    File.Delete(outputXMLPath);

                if (File.Exists(outputPDFPath))
                    File.Delete(outputPDFPath);

                Console.WriteLine($"{opt.DebugString}【報表名稱】: {opt.ReportName}");
                Console.WriteLine($"{opt.DebugString}【輸出路徑】: {outputPDFPath}");

                var jsonString = string.IsNullOrEmpty(opt.JsonText) ? File.ReadAllText(opt.JsonFilePath) : opt.JsonText;
                var ds = JsonStringConvertToDataSet(jsonString);
                if (opt.DebugMode)
                {
                    Console.WriteLine($"{opt.DebugString}【xmlPath】:{outputXMLPath}");
                    Console.WriteLine($"{opt.DebugString}【jsonPath】:{opt.JsonFilePath}");
                    ds.WriteXml(outputXMLPath);
                }

                var rd = new ReportDocument();
                rd.Load(Path.Combine(opt.LoadReportPath, $"{opt.ReportName}.rpt"));


                if (opt.IsUseFactory)
                {
                    var dt = CrystalReportFactory.CreateReport(opt.ReportName, jsonString);
                    if (opt.DebugMode)
                    {
                        Console.WriteLine($"{opt.DebugString}【xmlPath】:{outputXMLPath}");
                        Console.WriteLine($"{opt.DebugString}【jsonPath】:{opt.JsonFilePath}");
                        dt.WriteXml(outputXMLPath);
                    }

                    rd.SetDataSource(dt);
                }
                else
                {
                    rd.SetDataSource(ds);
                }

                rd.ExportToDisk(ExportFormatType.PortableDocFormat, outputPDFPath);

                Console.WriteLine($"{opt.DebugString} 完成");
                sw.Stop();

                if (!opt.DebugMode)
                {
                    Console.WriteLine($"{opt.DebugString} [DELETE] Delete JsonFilePath】");
                    if (File.Exists(opt.JsonFilePath))
                        File.Delete(opt.JsonFilePath);
                }

                Console.WriteLine($"{opt.DebugString} 共耗費： {sw.Elapsed.TotalMilliseconds.ToString()}毫秒");
            }
            catch (IndexOutOfRangeException e)
            {
                Console.WriteLine(e);
                Console.WriteLine(e.Source);
                Console.WriteLine(e.Data);
                Console.WriteLine("傳入參數個數錯誤，請傳入正確數目參數, 1.報表名稱 2.根路徑 3. 檔案名稱" + e.Message);
                throw new Exception(e.Message);
            }
            catch (CrystalReportsException e)
            {
                Console.WriteLine(e);
                Console.WriteLine(e.Source);
                Console.WriteLine(e.Data);
                Console.WriteLine("請檢察來源報表路徑" + e.Message);
                throw new Exception(e.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Console.WriteLine(e.Source);
                Console.WriteLine(e.Data);
                Console.WriteLine("未知錯誤" + e.Message);
                throw new Exception(e.Message);
            }

            return 1;

            # endregion
        }

        private static DataSet JsonStringConvertToDataSet(string json)
        {
            var ds = new DataSet();
            var xml = new StringBuilder();
            using (var r = ChoJSONReader.LoadText(json))
            {
                using (var w = new ChoXmlWriter(xml).WithNodeName("Source").WithRootName("Sources"))
                {
                    w.Write(r);
                }
            }

            var reader = new XmlTextReader(xml.ToString(), XmlNodeType.Document, null);
            ds.ReadXml(reader);

            return ds;
        }

        private static void ValidationOptions(StandardOptions opt)
        {
            // 報表名稱(可包含後綴，或不包含) e.g. aa.rpt
            if (!string.IsNullOrEmpty(opt.ReportName))
            {
                if (!opt.ReportName.Contains("."))
                {
                    opt.ReportName = opt.ReportName.Split('.')[0];
                }
            }

            // 輸出路徑為空
            if (string.IsNullOrEmpty(opt.OutputPath))
            {
                opt.OutputPath = Path.Combine(Directory.GetCurrentDirectory(), "Temp");
            }

            // 檔案名稱
            if (string.IsNullOrEmpty(opt.FileName))
            {
                opt.FileName = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            }

            if (opt.FileName.Contains("."))
            {
                opt.FileName = opt.FileName.Split('.')[0];
            }

            if (string.IsNullOrEmpty(opt.LoadReportPath))
            {
                opt.LoadReportPath = Path.Combine(Directory.GetCurrentDirectory(), "Source");
            }

            if (!opt.DebugMode)
            {
                opt.DebugString = "[NORMAL]";
            }

            if (string.IsNullOrEmpty(opt.JsonText) && string.IsNullOrEmpty(opt.JsonFilePath))
            {
                throw new Exception("必須輸入JsonText和JsonFilePath");
            }
        }

        private static int DoError(IEnumerable<Error> error)
        {
            Console.WriteLine(@"Crystal Reports Wei. Version v1.0.0  Copyright(c) 2021");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(@"
　 へ　　　　　／|
　　/＼7　　　 ∠＿/
　 /　│　　 ／　／
　│　Z ＿,＜　／　　 /`ヽ
　│　　　　　ヽ　　 /　　〉
　 Y　　　　　`　 /　　/
　ｲ●　､　●　　⊂⊃〈　　/
　()　 へ　　　　|　＼〈
　　>ｰ ､_　 ィ　 │ ／／
　 / へ　　 /　ﾉ＜| ＼＼
　 ヽ_ﾉ　　(_／　 │／／
　　7　　　　　　　|／
　　＞―r￣￣`ｰ―＿
");
            Console.ForegroundColor = ConsoleColor.DarkMagenta;
            Console.WriteLine(@"--------------------------------------------------------------------------------
make : 製作報表檔
--------------------------------------------------------------------------------
*e.g: .\Pikapikachu.CrystalReport.Client.exe make -r shpbah -o D:\crystalreport\src -f 2110091300001 -j D:\crystalreport\ecan\src\tempjson.txt -l D:\crystalreport\src\ -d
*e.g: .\Pikapikachu.CrystalReport.Client.exe make -r shpbah -j D:\crystalreport\src\tempjson.txt -d");

            Console.ForegroundColor = ConsoleColor.DarkBlue;
            Console.WriteLine(@"--------------------------------------------------------------------------------
clear : 清除報表檔
--------------------------------------------------------------------------------
*e.g: .\Pikapikachu.CrystalReport.Client.exe clear");

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(@"--------------------------------------------------------------------------------
source : 製作XML來源文件
--------------------------------------------------------------------------------
*e.g: .\Pikapikachu.CrystalReport.Client.exe source -j D:\crystalreport\ecan\src\tempjson.txt");
            Console.ResetColor();
            return 1;
        }
    }
}