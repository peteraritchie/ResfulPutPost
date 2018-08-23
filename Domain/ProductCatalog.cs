using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Domain.Abstractions;
using Domain.Primitivies;

namespace Domain
{
	[ExcludeFromCodeCoverage/*TODO: Implement this in a real service*/]
	public class ProductCatalog : IProductCatalog
	{
		public Product FindById(string id)
		{
			return null;	// TODO: implement real FindById
		}

		public IEnumerable<Product> Products { get; }
		public string AddNewProduct(Product product)
		{
			return null;    // TODO: implement real AddNewProduct
		}

		public void ReplaceExistingProduct(string id, Product product)
		{
			// TODO: implement real ReplaceExistingProduct
		}

		public void AddNewProduct(string id, Product domainProduct)
		{
			// TODO: implement real AddNewProduct
		}
	}
}