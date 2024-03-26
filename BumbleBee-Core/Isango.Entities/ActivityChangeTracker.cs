using System;
using System.Collections.Generic;
using System.Linq;

namespace Isango.Entities
{
    public class ActivityChangeTracker
    {
        public int ActivityId
        { get; set; }

        public OperationType OperationType
        { get; set; }

        public bool IsProcessed
        { get; set; }

        public DateTime ProcessedDate
        { get; set; }

        public bool IsHbProduct
        { get; set; }
    }

    public static class Batching
    {
        public static IEnumerable<IEnumerable<T>> Batch<T>(this IEnumerable<T> items,
                                                           int maxItems)
        {
            return items.Select((item, inx) => new { item, inx })
                        .GroupBy(x => x.inx / maxItems)
                        .Select(g => g.Select(x => x.item));
        }
    }
}