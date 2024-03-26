namespace Isango.Entities.Review
{
    public enum ReviewStatus
    {
        UNDEFINED = 0,
        USER_APPROVED = 1,
        USER_PENDING = 2,
        MODERATOR_APPROVED = 3,
        MODERATOR_PENDING = 4,
        MODERATOR_REJECTED = 5
    }
}