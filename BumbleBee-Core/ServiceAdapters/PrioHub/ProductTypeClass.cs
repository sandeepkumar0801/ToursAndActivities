namespace ServiceAdapters.PrioHub.PrioHub.Entities
{
    public enum ProductTypeClass
    {
        STANDARD = 0,
        //STANDARD-These types will always be age-restricted
        INDIVIDUAL = 1,
        //INDIVIDUAL-These types will never be age-restricted.
        ITEM = 2,
        //ITEM//Product types in the item class do not refer to
        //actual persons, instead they could, for example, be packages (Regular, Silver, Diamond), objects (Merchandise, private tours),
        //a type of event, class identifier (Economy, Business) and much more.
        GROUP = 3,//GROUP:Product types in the group class always 
        //consist of multiple persons. It can, for example, be a family of 2 Adults and 2 Childs
        CUSTOM = 4,//CUSTOM:
        //Product types in the custom class are completely dynamic and therefore require explicit 
        //mapping with external systems. 
        //They do not return as CUSTOM, instead they can take any form.
    }
}