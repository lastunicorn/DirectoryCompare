using DustInTheWind.DirectoryCompare.Application;
using DustInTheWind.DirectoryCompare.Application.DiskAnalysis;

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