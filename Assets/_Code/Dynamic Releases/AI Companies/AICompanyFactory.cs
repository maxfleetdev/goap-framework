using UnityEngine;

/// <summary>
/// Static class for generating new AICompanyData using predefined data
/// </summary>
public static class AICompanyFactory
{
    private static readonly int INDIE_QUALITY_MIN = 40;
    private static readonly int AA_QUALITY_MIN = 60;
    private static readonly int AAA_QUALITY_MIN = 80;
    private static readonly int MAX_QUALITY = 100;

    public static AICompanyData CreateCompany(AICompanyTier tier)
    {
        AICompanyData companyData = new();
        companyData.SetName(NameGenerator.GetCompanyName());
        companyData.SetTier(tier);

        switch (tier)
        {
            case AICompanyTier.Indie:
                companyData.SetQuality(Random.Range(INDIE_QUALITY_MIN, AA_QUALITY_MIN));
                break;
            
            case AICompanyTier.AA:
                companyData.SetQuality(Random.Range(AA_QUALITY_MIN, AAA_QUALITY_MIN));
                break;
            
            case AICompanyTier.AAA:
                companyData.SetQuality(Random.Range(AA_QUALITY_MIN, MAX_QUALITY));
                break;
        }
        return companyData;
    }
}