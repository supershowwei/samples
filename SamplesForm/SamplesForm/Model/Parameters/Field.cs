using System;

namespace SamplesForm.Model.Parameters
{
    public class Field<T>
    {
        public Field(T value)
        {
            this.Value = value;
        }

        public T Value { get; private set; }

        public static implicit operator Field<T>(long value)
        {
            return CreateObjectOfT(value);
        }

        public static implicit operator Field<T>(T value)
        {
            return new Field<T>(value);
        }

        public static implicit operator T(Field<T> field)
        {
            return field.Value;
        }

        private static Field<T> CreateObjectOfT(object value)
        {
            if (value is T)
            {
                return new Field<T>((T)value);
            }

            return new Field<T>((T)Convert.ChangeType(value, typeof(T)));
        }
    }
}