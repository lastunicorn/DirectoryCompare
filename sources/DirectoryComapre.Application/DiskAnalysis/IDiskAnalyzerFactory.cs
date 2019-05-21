namespace DustInTheWind.DirectoryCompare.Application.DiskAnalysis
{
    public interface IDiskAnalyzerFactory
    {
        IDiskAnalyzer Create(AnalysisRequest request, IDiskAnalysisExport diskAnalysisExport);
    }
}