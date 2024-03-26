using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebAPI.Models.RequestModels
{
	public class ComponentActivityDetail
	{
		[Required]
		public int ComponentActivityId { get; set; }
		[Required]
		public List<PaxDetail> PaxDetails { get; set; }
		[Required]
		public DateTime CheckinDate { get; set; }
		[Required]
		public DateTime CheckoutDate { get; set; }
	}
}