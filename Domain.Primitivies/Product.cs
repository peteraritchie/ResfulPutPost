namespace Domain.Primitivies
{
	public class Product
	{
		public Product()
		{
		}

		public Product(string name, decimal price)
		{
			Price = price;
			Name = name;
		}

		public string Name { get; }
		public decimal Price { get; }
	}
}