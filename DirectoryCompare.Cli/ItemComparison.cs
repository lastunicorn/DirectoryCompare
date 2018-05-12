namespace DirectoryCompare
{
    internal struct ItemComparison
    {
        public string RootPath { get; set; }

        public XItem Item1 { get; set; }

        public XItem Item2 { get; set; }

        public string FullName1 => (RootPath ?? string.Empty) + Item1?.Name;

        public string FullName2 => (RootPath ?? string.Empty) + Item2?.Name;
    }
}