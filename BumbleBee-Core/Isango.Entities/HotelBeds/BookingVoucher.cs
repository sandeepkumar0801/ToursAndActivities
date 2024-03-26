using System.Collections.Generic;

namespace Isango.Entities.HotelBeds
{
    public class BookingVoucher
    {
        public string Language { get; set; }

        public string Url { get; set; }

        public BookingVoucherType Type { get; set; }

        /// <summary>
        /// If IsCustomizable is true then this field will have downloaded qr image name
        /// Otherwise it will have downloaded HB PDF voucher name
        /// </summary>
        public List<BookingVoucherHtmlORpdf> DownloadFileNames { get; set; }
    }

    public class BookingVoucherHtmlORpdf
    {
        public string Url { get; set; }

        /// <summary>
        /// Qr or bar code value or any other type of value.
        /// </summary>
        public string CodeType { get; set; }

        /// <summary>
        /// Qr or bar code value or any other types' value.
        /// </summary>
        public string CodeValue { get; set; }

        public string DownloadedFile { get; set; }

        public BookingVoucherQRCodeType QRCodeType { get; set; }
    }

    /// <summary>
    /// Determines the voucher type
    /// </summary>
    public enum BookingVoucherType
    {
        UNDEFINED = 0,
        HTML = 1,
        PDF = 2,
    }

    /// <summary>
    /// Code can be given as text , image or non customizable supplier voucher containing bar/qr code
    /// </summary>
    public enum BookingVoucherQRCodeType
    {
        UNDEFINED = 0,

        /// <summary>
        /// Use if QR code as text is received and we have to create image from it
        /// </summary>
        STRING = 1,

        /// <summary>
        /// Hotel beds give html from where qr image is downloaded
        /// </summary>
        HTMLIMAGE = 2,

        /// <summary>
        /// In case of hotelBeds , if no image from html is found then we have to download pdf voucher
        /// </summary>
        PDF = 3
    }

    ///// <summary>
    ///// Bar code / qr code or  any other code types
    ///// </summary>
    //public enum CodeTypes
    //{
    //    UNDEFINED = 0,

    //    /// <summary>
    //    /// QRCODE
    //    /// </summary>
    //    QRCODE = 1,

    //    /// <summary>
    //    /// BARCODE
    //    /// </summary>
    //    BARCODE = 2,
    //}
}