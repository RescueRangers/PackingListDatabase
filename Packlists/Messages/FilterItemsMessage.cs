namespace Packlists.Messages
{
    public class FilterItemsMessage
    {
        public string ItemName { get; private set; }

        public FilterItemsMessage(string itemName)
        {
            ItemName = itemName;
        }
    }
}