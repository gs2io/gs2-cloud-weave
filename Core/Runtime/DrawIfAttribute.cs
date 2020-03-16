using UnityEngine;
using System;

namespace Gs2.Weave.Core
{
    public enum ComparisonType
    {
        Equals = 1,
        NotEqual = 2,
        GreaterThan = 3,
        SmallerThan = 4,
        SmallerOrEqual = 5,
        GreaterOrEqual = 6
    }
    
    public enum DisablingType
    {
        ReadOnly = 0,
        DontDraw = 1
    }
    
    /// <summary>
    /// Draws the field/property ONLY if the copared property compared by the comparison type with the value of comparedValue returns true.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true)]
    public class DrawIfAttribute : PropertyAttribute
    {
        public string ComparedPropertyName { get; private set; }
        public object ComparedValue { get; private set; }
        public ComparisonType ComparisonType { get; private set; }
        public DisablingType DisablingType { get; private set; }
 
        /// <summary>
        /// Only draws the field only if a condition is met.
        /// </summary>
        /// <param name="comparedPropertyName">The name of the property that is being compared (case sensitive).</param>
        /// <param name="comparedValue">The value the property is being compared to.</param>
        /// <param name="comparisonType">The type of comperison the values will be compared by.</param>
        /// <param name="disablingType">The type of disabling that should happen if the condition is NOT met. Defaulted to DisablingType.DontDraw.</param>
        public DrawIfAttribute(string comparedPropertyName, object comparedValue, ComparisonType comparisonType, DisablingType disablingType = DisablingType.DontDraw)
        {
            ComparedPropertyName = comparedPropertyName;
            ComparedValue = comparedValue;
            ComparisonType = comparisonType;
            DisablingType = disablingType;
        }
    }
}