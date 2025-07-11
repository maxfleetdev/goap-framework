public class AIHeldItem
{
    private int stockID = 0;
    public int StockID
    {
        get => stockID;
        private set => stockID = value;
    }
    
    private int amount = 0;
    public int Amount
    {
        get => amount;
        private set => amount = value;
    }

    private bool isStolen = false;
    public bool IsStolen
    {
        get => isStolen;
        private set => isStolen = value;
    }

    public AIHeldItem(int stockID)
    {
        this.stockID = stockID;
    }

    public AIHeldItem(int stock_id, int amount, bool stolen)
    {
        this.stockID = stock_id;
        this.amount = amount;
        this.isStolen = stolen;
    }
}