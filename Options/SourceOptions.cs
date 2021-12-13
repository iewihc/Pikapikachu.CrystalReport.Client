using CommandLine;

namespace Pikapikachu.CrystalReport.Client.Options
{
    [Verb("source", HelpText = "製作資料來源XML")]
    class SourceOptions
    {
        [Option('j', "JsonFilePath", Required = true, HelpText = "讀取Json檔案的路徑,可為txt或json格式。")]
        public string JsonFilePath { get; set; }
    }
}