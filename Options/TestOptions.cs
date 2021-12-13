using CommandLine;

namespace Pikapikachu.CrystalReport.Client.Options
{
    [Verb("test", HelpText = "testing")]
    public class TestOptions
    {
        [Option('s', "Simple", Required = false, HelpText = "測試一對多表")]
        public bool IsSimple { get; set; }
    }
}