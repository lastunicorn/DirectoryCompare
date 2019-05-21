namespace DustInTheWind.DirectoryCompare.DiskAnalysis
{
    public interface IDiskAnalyzerFactory
    {
        IDiskAnalyzer Create(AnalysisRequest request, IDiskAnalysisExport diskAnalysisExport);
    }
}