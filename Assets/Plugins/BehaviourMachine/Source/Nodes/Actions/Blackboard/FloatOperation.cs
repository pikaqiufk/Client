//----------------------------------------------
//            Behaviour Machine
// Copyright © 2014 Anderson Campos Cardoso
//----------------------------------------------

using UnityEngine;
using System.Collections;

namespace BehaviourMachine
{

    /// <summary>
    /// Performs math operations on float values, stores the result in "Store".
    /// <summary>
    [NodeInfo(category = "Action/Blackboard/",
                icon = "Math",
                description = "Performs math operations on float \"A\" and \"B\", stores the result in \"Store\"")]
    public class FloatOperation : ActionNode
    {

        /// <summary>
        /// The float value to perform the operation.
        /// </summary>
        [VariableInfo(tooltip = "The float value to perform the operation")]
        public FloatVar targetValue;

        /// <summary>
        /// The float values to perform the operation.
        /// </summary>
        [VariableInfo(tooltip = "The float values to perform the operation")]
        public FloatVar[] values;

        /// <summary>
        /// The operation to perform.
        /// </summary>
        [Tooltip("The operation to perform")]
        public Operation operation;

        /// <summary>
        /// Stores the operation result.
        /// </summary>
        [VariableInfo(canBeConstant = false, tooltip = "Stores the operation result")]
        public FloatVar store;

        public override void Reset()
        {
            targetValue = new ConcreteFloatVar(0f);
            values = new FloatVar[] { new ConcreteFloatVar(0f), new ConcreteFloatVar(0f) };
            operation = Operation.Add;
            store = new ConcreteFloatVar();
        }

        public override Status Update()
        {
            // Validate members
            if (targetValue.isNone || values.Length < 1 || store.isNone)
                return Status.Error;

            float result = targetValue.Value;

            // Do operation
            switch (operation)
            {
                case Operation.Add:
                    {
                        var __array1 = values;
                        var __arrayLength1 = __array1.Length;
                        for (int __i1 = 0; __i1 < __arrayLength1; ++__i1)
                        {
                            var v = __array1[__i1];
                            result += v.Value;
                        }
                    }
                    break;
                case Operation.Subtract:
                    {
                        var __array2 = values;
                        var __arrayLength2 = __array2.Length;
                        for (int __i2 = 0; __i2 < __arrayLength2; ++__i2)
                        {
                            var v = __array2[__i2];
                            result -= v.Value;
                        }
                    }
                    break;
                case Operation.Multiply:
                    result = 1f;
                    {
                        var __array3 = values;
                        var __arrayLength3 = __array3.Length;
                        for (int __i3 = 0; __i3 < __arrayLength3; ++__i3)
                        {
                            var v = __array3[__i3];
                            result *= v.Value;
                        }
                    }
                    break;
                case Operation.Divide:
                    result = values[0];
                    for (int i = 0; i < values.Length; i++) result /= values[i].Value;
                    break;
                case Operation.Max:
                    var parametersMax = new float[values.Length];
                    for (int i = 0; i < values.Length; i++) parametersMax[i] = values[i].Value;
                    result = Mathf.Max(parametersMax);
                    break;
                case Operation.Min:
                    var parametersMin = new float[values.Length];
                    for (int i = 0; i < values.Length; i++) parametersMin[i] = values[i].Value;
                    result = Mathf.Min(parametersMin);
                    break;
            }

            store.Value = result;

            return Status.Success;
        }
    }
}