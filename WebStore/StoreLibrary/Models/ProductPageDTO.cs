using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StoreLibrary.DbModels;

namespace StoreLibrary.Models
{
	public class ProductPageDTO : ProductDTO
	{
		public List<string> ImagePathList { get; set; } = new List<string>();
		public List<ReviewDTO> ReviewList { get; set; } = new List<ReviewDTO>();
	}
}
