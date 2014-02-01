﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Bonsai.Expressions
{
    class BinaryOperatorTypeDescriptionProvider : TypeDescriptionProvider
    {
        static readonly TypeDescriptionProvider parentProvider = TypeDescriptor.GetProvider(typeof(BinaryOperatorBuilder));

        public BinaryOperatorTypeDescriptionProvider()
            : base(parentProvider)
        {
        }

        public override ICustomTypeDescriptor GetExtendedTypeDescriptor(object instance)
        {
            return new BinaryOperatorTypeDescriptor(instance);
        }

        class BinaryOperatorTypeDescriptor : CustomTypeDescriptor
        {
            BinaryOperatorBuilder builder;
            static readonly Attribute[] emptyAttributes = new Attribute[0];

            public BinaryOperatorTypeDescriptor(object instance)
            {
                if (instance == null)
                {
                    throw new ArgumentNullException("instance");
                }

                builder = (BinaryOperatorBuilder)instance;
            }

            public override PropertyDescriptorCollection GetProperties()
            {
                return GetProperties(emptyAttributes);
            }

            public override PropertyDescriptorCollection GetProperties(Attribute[] attributes)
            {
                var operand = builder.Operand;
                if (operand != null)
                {
                    var propertyDescriptor = new WorkflowPropertyDescriptor("Value", emptyAttributes, operand);
                    return new PropertyDescriptorCollection(new[] { propertyDescriptor });
                }
                else return PropertyDescriptorCollection.Empty;
            }
        }
    }
}
