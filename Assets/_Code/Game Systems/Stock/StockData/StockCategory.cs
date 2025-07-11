/// <summary>
/// General category metadata for a given item
/// </summary>
public enum StockCategory
{
    /// <summary>
    /// This Stock can conform to all categories
    /// </summary>
    Any = 0,
    Game,
    Console,
    Accessory,          // controller, mouse, keyboard etc
    Audio,              // speakers, headphones etc
    Video,              // tv, monitor etc
    Internet            // router, cables etc
}