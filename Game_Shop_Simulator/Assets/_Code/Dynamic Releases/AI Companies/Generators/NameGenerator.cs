using UnityEngine;

public static class NameGenerator
{
    // Company Name Generation //
    private static string[] companyFirstPart = new string[] {
    "Black", "Silver", "Quantum", "Crimson", "Neo", "Blue", "Retro", "Burning", "Golden", "Pixel",
    "Iron", "Electric", "Midnight", "Turbo", "Silent", "Frost", "Digital", "Ghost", "Rocket", "Obsidian"
    };

    private static string[] companySecondPart = new string[] {
    "Pixels", "Forge", "Studio", "Games", "Interactive", "Lab", "Factory", "Play", "Arts", "Logic",
    "Works", "Realm", "Core", "Vision", "Hive", "Storm", "Box", "Engine", "Crew", "Machine"
    };

    public static string GetCompanyName()
    {
        string company_name = companyFirstPart[Random.Range(0, companyFirstPart.Length - 1)];
        if (Random.Range(0, 1) == 0)
        {
            company_name += $" {companySecondPart[Random.Range(0, companySecondPart.Length - 1)]}";
        }
        return company_name;
    }
}