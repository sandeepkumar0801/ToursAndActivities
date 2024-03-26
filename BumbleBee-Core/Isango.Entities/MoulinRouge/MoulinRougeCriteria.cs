namespace Isango.Entities.MoulinRouge
{
    public class MoulinRougeCriteria : Criteria
    {
        public MoulinRougeCriteria()
        {
            MoulinRougeContext = new APIContextMoulinRouge();
        }

        public APIContextMoulinRouge MoulinRougeContext { get; set; }
    }
}