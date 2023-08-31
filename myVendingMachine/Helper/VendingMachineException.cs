namespace myVendingMachine.Helper
{
    public class VendingMachineException : Exception
    {
        public VendingMachineException()
        {
        }

        public VendingMachineException(string message)
            : base(message)
        {
        }

        public VendingMachineException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
