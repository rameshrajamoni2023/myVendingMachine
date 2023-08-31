namespace myVendingMachine.Helper
{
    public static class Utils
    {
        private static int currentId = 0;

        public static int GetReceiptId()
        {
            // Increment the current ID and return it
            return Interlocked.Increment(ref currentId);
        }

    }

}
