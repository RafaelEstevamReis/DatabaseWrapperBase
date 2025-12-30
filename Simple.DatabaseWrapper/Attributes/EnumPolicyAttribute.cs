using System;

namespace Simple.DatabaseWrapper.Attributes
{
    /// <summary>
    /// Specify enum handling policy
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class EnumPolicyAttribute : Attribute
    {
        /// <summary>
        /// Available policies
        /// </summary>
        public enum Policies
        {
            /// <summary>
            /// Should be stored as number
            /// </summary>
            AsNumber = 0,
            /// <summary>
            /// Should be stored as text
            /// </summary>
            AsText = 1,
        }
        /// <summary>
        /// Current Policy
        /// </summary>
        public Policies Policy { get; set; }
        /// <summary>
        /// Creates a new instance
        /// </summary>
        /// <param name="policy">Policy to be used</param>
        public EnumPolicyAttribute(Policies policy)
        {
            Policy = policy;
        }
    }
}
