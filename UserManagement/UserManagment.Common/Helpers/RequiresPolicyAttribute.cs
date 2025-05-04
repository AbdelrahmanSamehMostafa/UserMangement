namespace UserManagment.Common.Helpers
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class RequiresPolicyAttribute : Attribute
    {
        public string PolicyName { get; }

        public RequiresPolicyAttribute(string policyName)
        {
            PolicyName = policyName;
        }
    }
}