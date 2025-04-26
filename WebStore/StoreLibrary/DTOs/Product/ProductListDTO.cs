using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoreLibrary.DTOs.Product
{
	public class ProductListDTO
	{
		public int PkProduct { get; set; }
		public string Name { get; set; }
		public string Ean { get; set; }
		public string? ImageUrl { get; set; }
		public List<ProductCategoryDTO> Categories { get; set; } = new();
	}
}
