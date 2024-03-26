using System;

namespace Isango.Mailer.ServiceContracts
{
    public interface IMailAttachmentService
    {
        Tuple<byte[], string, string> GetBookedVoucher(string bookingRefNo, bool isPDFVoucher = false, int Source = 3);
        
        Tuple<byte[], string, string> GetBookedVoucherNew(string bookingRefNo, bool isPDFVoucher = false, int Source = 3, int? bookedoptionid = null, bool? iscancelled = false);

        byte[] GetBookedInvoice(string bookingRefNo, bool isPDFVoucher = false, int Source = 4);

        byte[] GetCancelledVoucher(string bookingRefNo, string bookingOptionId, bool isPDFVoucher = false);

        string FilterIllegalCharacterFromPath(string filename);

        bool GenerateQrCode(string qrCodeValue, string imageName, string codeType = "");
    }
}