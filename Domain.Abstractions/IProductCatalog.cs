using System.Collections.Generic;
using Domain.Primitivies;

namespace Domain.Abstractions
{
	public interface IProductCatalog
	{
		Product FindById(string id);
		IEnumerable<Product> Products { get; }
		string AddNewProduct(Product product);
		void ReplaceExistingProduct(string id, Product product);
		void AddNewProduct(string id, Product domainProduct);
	}
}