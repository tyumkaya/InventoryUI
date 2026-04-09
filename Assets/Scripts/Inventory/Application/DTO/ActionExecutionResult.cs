namespace InventoryUI.Inventory.Application.DTO
{
    public sealed class ActionExecutionResult
    {
        public bool IsSuccess { get; }
        public bool RequiresSave { get; }
        public string Message { get; }

        private ActionExecutionResult(bool isSuccess, bool requiresSave, string message)
        {
            IsSuccess = isSuccess;
            RequiresSave = requiresSave;
            Message = message;
        }

        public static ActionExecutionResult Success(string message, bool requiresSave = true)
        {
            return new ActionExecutionResult(true, requiresSave, message);
        }

        public static ActionExecutionResult Failure(string message)
        {
            return new ActionExecutionResult(false, false, message);
        }

        public static ActionExecutionResult Pending(string message)
        {
            return new ActionExecutionResult(false, false, message);
        }
    }
}
