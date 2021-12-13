using CommandLine;

namespace Pikapikachu.CrystalReport.Client.Options
{
    [Verb("clear", HelpText = "清理TEMP檔案")]
    class CleanOptions
    {
        [Option('o', "OutputPath", Required = false, HelpText = "輸出路徑(包含xml , pdf)的主路徑 , 不輸入即輸出在當前資料夾temp底下 e.g.")]
        public string OutputPath { get; set; }
    }
}