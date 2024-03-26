using Isango.Entities.Enums;

namespace Isango.Entities
{
    public class ProductImage
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string FileName { get; set; }
        public string Description { get; set; }
        public ImageType ImageType { get; set; }

        public int? ImageSequence { get; set; }
        public bool? Thumbnail { get; set; }
    }
}