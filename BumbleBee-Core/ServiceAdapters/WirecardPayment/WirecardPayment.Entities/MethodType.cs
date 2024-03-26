namespace ServiceAdapters.WirecardPayment.WirecardPayment.Entities
{
    public enum MethodType
    {
        BookBack = 0,
        EnrollmentCheck = 1,
        EmiEnrollmentCheck = 2,
        Rollback = 3,
        CapturePreauthorize = 4,
        ProcessPayment = 5,
        ProcessPayment3D = 6,
        CapturePreauthorize3D = 7
    }
}