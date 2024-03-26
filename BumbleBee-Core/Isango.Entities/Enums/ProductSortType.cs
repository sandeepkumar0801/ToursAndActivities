namespace Isango.Entities.Enums
{
    public enum ProductSortType
    {
        /// <summary>
        /// No sorting will be applied
        /// </summary>
        None = 0,

        /// <summary>
        /// Default or Isango Recommended sorting
        /// </summary>
        Default = 1,

        /// <summary>
        /// Sorting by price in asceding order
        /// </summary>
        Price = 2,

        /// <summary>
        /// Sorting by Top Seller Tag
        /// </summary>
        TopSeller = 3,

        /// <summary>
        /// Sorting by On Sale tag
        /// </summary>
        OnSale = 4,

        /// <summary>
        /// Sorting by User Review Rating value in desceding order
        /// </summary>
        UserReviewRating = 5,

        /// <summary>
        /// Sorting by Product name alphabetically by name,from A to Z
        /// </summary>
        Name = 6,

        /// <summary>
        /// Sorting by Offers
        /// </summary>
        Offers = 7
    }
}