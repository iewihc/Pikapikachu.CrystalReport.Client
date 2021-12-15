using CommandLine;

namespace Pikapikachu.CrystalReport.Client.Options
{
    [Verb("make", HelpText = "製作報表檔")]
    public class StandardOptions
    {
        [Option('r', "ReportName", Required = true, HelpText = "報表名稱(可包含後綴，或不包含) e.g. aa.rpt")]
        public string ReportName { get; set; }

        [Option('o', "OutputPath", Required = false, HelpText = "輸出路徑(包含xml , pdf)的主路徑 , 不輸入即輸出在當前資料夾temp底下 e.g.")]
        public string OutputPath { get; set; }

        [Option('f', "FileName", Required = false, HelpText = "輸出的檔案名稱，不輸入即使用yyyyMMdd_HHmmss")]
        public string FileName { get; set; }

        [Option('j', "JsonFilePath", Required = false, HelpText = "讀取Json檔案的路徑,可為txt或json格式，與[-t]擇一。")]
        public string JsonFilePath { get; set; }

        [Option('t', "JsonFileText", Required = false, HelpText = "Json String, 為 escape字符，與[-j]擇一")]
        public string JsonText { get; set; }

        [Option('l', "loadRptPath", Required = false, HelpText = "報表Source路徑, 不輸入默認讀取./Source")]
        public string LoadReportPath { get; set; }

        [Option('u', "UseFactoryMode", Required = false, HelpText = "使用工廠模式生產")]
        public bool IsUseFactory { get; set; }

        [Option('d', "isDebugMode", Required = false, HelpText = "開發者模式")]
        public bool DebugMode { get; set; }


        
        public string DebugString { get; set; } = "[Debug]";
    }
}