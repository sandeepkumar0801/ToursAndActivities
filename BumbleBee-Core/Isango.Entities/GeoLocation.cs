using Isango.Entities.Enums;

namespace Isango.Entities
{
    public struct GeoLocation
    {
        public float Longitute { get; set; }

        public float Latitude { get; set; }

        public string ReferencePoint { get; set; }

        public float DistanceFromReference { get; set; }

        public Distance DistanceUnit { get; set; }

        public string CityCenterName { get; set; }

        public float CityCenterDistance { get; set; }
    }
}