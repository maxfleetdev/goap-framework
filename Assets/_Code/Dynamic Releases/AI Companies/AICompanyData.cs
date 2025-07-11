public class AICompanyData
{
    // Company Metadata
    private string companyName;
    private AICompanyTier companyTier;

    // Company Product Variables
    private int qualityBaseline;

    public string CompanyName
    {
        get => companyName;
    }
    public AICompanyTier CompanyTier
    {
        get => companyTier;
    }
    public int QualityBaseline
    {
        get => qualityBaseline;
    }

    public void SetName(string name)
    {
        companyName = name;
    }

    public void SetTier(AICompanyTier tier)
    {
        companyTier = tier;
    }

    public void SetQuality(int quality)
    {
        qualityBaseline = quality;
    }
}