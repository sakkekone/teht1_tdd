using Newtonsoft.Json;
using Xunit.Abstractions;

namespace teht1_xunit
{
	public class UnitTest1
	{
		private readonly ITestOutputHelper output;
		public UnitTest1(ITestOutputHelper testOutputHelper)
		{
		output = testOutputHelper;
		}
		string jsonInput = @"
		{
			'prices':
			[
				{
					'price': 13.494,
					'startDate': '2022-11-14T22:00:00.000Z',
					'endDate': '2022-11-14T23:00:00.000Z'
				},
				{
					'price': 17.62,
					'startDate': '2022-11-14T21:00:00.000Z',
					'endDate': '2022-11-14T22:00:00.000Z'
				},
				{
					'price': 1.286,
					'startDate': '2022-11-12T23:00:00.000Z',
					'endDate': '2022-11-13T00:00:00.000Z'
				}
			]
		}";

		public List<PriceDifference> readJson()
		{
		decimal fixedprice = 10.0M;
		List<PriceDifference> pd_list = new List<PriceDifference>();
		PriceList priceList = JsonConvert.DeserializeObject<PriceList>(jsonInput);
		if(priceList.prices.Length <= 0)
		{
		output.WriteLine("priceList is empty");
		Assert.Fail("could not deserialize json data");
		}
		for(int i = 0;i < priceList.prices.Length;i++)
		{
		Price p = priceList.prices[i];
		decimal pp = p.price - fixedprice;
		ElectricityContractType type = ElectricityContractType.FixedPrice;
		if(pp < 0)
		{
		pp *= -1;
		type = ElectricityContractType.MarketPrice;
		}
		PriceDifference pd = new PriceDifference(p.startDate,p.endDate,pp,type);
		pd_list.Add(pd);
		}
		return pd_list;
		}

		[Fact]
		public void test_timestamps()//tests if timestamp data was read correctly
		{
		List<PriceDifference> pd_list = readJson();
		//test timestamps:
		output.WriteLine("testing if timestamps are read correctly...");
		Assert.Equal(new DateTime(2022,11,14,22,0,0),pd_list[0].StartDate);
		Assert.Equal(new DateTime(2022,11,14,23,0,0),pd_list[0].EndDate);

		Assert.Equal(new DateTime(2022,11,14,21,0,0),pd_list[1].StartDate);
		Assert.Equal(new DateTime(2022,11,14,22,0,0),pd_list[1].EndDate);

		Assert.Equal(new DateTime(2022,11,12,23,0,0),pd_list[2].StartDate);
		Assert.Equal(new DateTime(2022,11,13,00,0,0),pd_list[2].EndDate);
		output.WriteLine("pass.");
		}


		[Theory]
		[InlineData("2022-11-14T22:00",0)]
		[InlineData("2022-11-14T21:00",1)]
		[InlineData("2022-11-12T23:00",2)]
		public void test_timestamps_theory(string dateTimeString,int index)//tests if timestamp data was read correctly
		{
		List<PriceDifference> pd_list = readJson();
		var dateTime = DateTime.Parse(dateTimeString);
		Assert.Equal(dateTime,pd_list[index].StartDate);
		}


		[Fact]
		public void test_prices()//tests if pricedifference was calculated correctly
		{
		List<PriceDifference> pd_list = readJson();
		//test pricedifference:
		output.WriteLine("testing if price is calculated correctly...");
		Assert.Equal(3.494m,pd_list[0].PriceDifferenceValue,3);
		Assert.Equal(ElectricityContractType.FixedPrice,pd_list[0].CheaperContract);
		Assert.Equal(7.620m,pd_list[1].PriceDifferenceValue,3);
		Assert.Equal(ElectricityContractType.FixedPrice,pd_list[1].CheaperContract);
		Assert.Equal(8.714m,pd_list[2].PriceDifferenceValue,3);
		Assert.Equal(ElectricityContractType.MarketPrice,pd_list[2].CheaperContract);
		output.WriteLine("pass.");
		}
		
		[Theory]
		[InlineData(3.494,0)]
		[InlineData(7.620,1)]
		[InlineData(8.714,2)]
		public void test_prices_theory(double expected,int index)//tests if pricedifference was calculated correctly
		{
		List<PriceDifference> pd_list = readJson();
		decimal expectedDecimal = (decimal)expected;
		output.WriteLine(expectedDecimal.ToString());
		Assert.Equal(expectedDecimal,pd_list[index].PriceDifferenceValue);
		}
	}
}
