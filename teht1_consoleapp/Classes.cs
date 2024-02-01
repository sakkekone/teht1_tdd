public enum ElectricityContractType
{
	FixedPrice,
	MarketPrice
}

public class PriceDifference
{
	public DateTime StartDate { get; set; }
	public DateTime EndDate { get; set; }
	public decimal PriceDifferenceValue { get; set; }
	public ElectricityContractType CheaperContract { get; set; }

	public PriceDifference(DateTime startDate,DateTime endDate,decimal priceDifference,ElectricityContractType cheaperContract)
	{
	StartDate = startDate;
	EndDate = endDate;
	PriceDifferenceValue = priceDifference;
	CheaperContract = cheaperContract;
	}
}

public class PriceList
{
	public Price[] prices { get; set; }
}

public class Price
{
	public decimal price { get; set; }
	public DateTime startDate { get; set; }
	public DateTime endDate { get; set; }
}