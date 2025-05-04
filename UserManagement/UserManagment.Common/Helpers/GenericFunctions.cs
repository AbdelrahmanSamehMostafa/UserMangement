namespace UserManagement.Common.Helpers
{
    public class GenericFunctions
    {
        public static async Task UpdateCollectionAsync<T>(
         IEnumerable<T> existingItems,
         IEnumerable<T> newItems,
         Func<Guid, List<T>, CancellationToken, Task> addAction,
         Func<Guid, List<T>, CancellationToken, Task> removeAction,
         Guid id,  // Change from object to Guid
         CancellationToken cancellationToken)
        {
            var existingItemSet = new HashSet<T>(existingItems);

            // Determine new and removed items
            var itemsToAdd = newItems.Except(existingItemSet).ToList();
            var itemsToRemove = existingItemSet.Except(newItems).ToList();

            // Add new items
            if (itemsToAdd.Count > 0)
            {
                await addAction(id, itemsToAdd, cancellationToken);
            }

            // Remove old items
            if (itemsToRemove.Count > 0)
            {
                await removeAction(id, itemsToRemove, cancellationToken);
            }
        }
    }
}
