using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Isango.Entities.Tiqets
{
    public class ContentMedia
    {
        public int Factsheetid { get; set; }
        public string Mediatype { get; set; }

        public int Dpi { get; set; }

        public int Height { get; set; }
        public int Width { get; set; }

        public string Language { get; set; }

        public string SizeType { get; set; }
        public int Image_order { get; set; }

        public int VisualizationOrder { get; set; }
        public string Url { get; set; }
        public string Duration { get; set; }
        public int IsangoProductId { get; set; }
    }
}
