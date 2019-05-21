using DustInTheWind.DirectoryCompare.Common.Utils;

namespace DustInTheWind.DirectoryCompare.DiskAnalysis
{
    public class AnalysisRequest
    {
        public string RootPath { get; set; }

        public PathCollection BlackList { get; set; }
    }
}