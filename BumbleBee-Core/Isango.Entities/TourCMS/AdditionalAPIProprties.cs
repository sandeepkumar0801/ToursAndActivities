using Isango.Entities.Bokun;
using System;
using System.Collections.Generic;

namespace Isango.Entities.TourCMS
{
    public class PickupPointsForTourCMS
    {
        public string PickupKey { get; set; }
        public string TimePickup { get; set; }
        public string PickUpName { get; set; }
        public string PickupId { get; set; }
        public string Description { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string PostCode { get; set; }
        public string GeoCode { get; set; }
    }

    public class QuestionsForTourCMS
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsRequired { get; set; }
        public string Answer { get; set; }
        public List<AnswerOption> AnswerOptions { get; set; }
    }
}
