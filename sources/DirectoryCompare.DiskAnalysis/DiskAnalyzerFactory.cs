using DustInTheWind.DirectoryCompare.Application;

namespace DustInTheWind.DirectoryCompare.DiskAnalysis
{
    public class DiskAnalyzerFactory : IDiskAnalyzerFactory
    {
        public IDiskAnalyzer Create(AnalysisRequest request, IDiskAnalysisExport diskAnalysisExport)
        {
            return new DiskAnalyzer(request.RootPath, diskAnalysisExport)
            {
                BlackList = request.BlackList
            };
        }
    }
}