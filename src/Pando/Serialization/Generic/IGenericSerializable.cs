namespace Pando.Serialization.Generic;

/// <summary>
/// An object that can be constructed and deconstructed generically with a single parameter.
/// </summary>
public interface IGenericSerializable<out TSelf, T1> where TSelf : IGenericSerializable<TSelf, T1>
{
	public abstract static TSelf Construct(T1 t1);
	public void Deconstruct(out T1 t1);
}

/// <summary>
/// An object that can be constructed and deconstructed generically with two parameters.
/// </summary>
public interface IGenericSerializable<out TSelf, T1, T2> where TSelf : IGenericSerializable<TSelf, T1, T2>
{
	public abstract static TSelf Construct(T1 t1, T2 t2);
	public void Deconstruct(out T1 t1, out T2 t2);
}

/// <summary>
/// An object that can be constructed and deconstructed generically with three parameters.
/// </summary>
public interface IGenericSerializable<out TSelf, T1, T2, T3> where TSelf : IGenericSerializable<TSelf, T1, T2, T3>
{
	public abstract static TSelf Construct(T1 t1, T2 t2, T3 t3);
	public void Deconstruct(out T1 t1, out T2 t2, out T3 t3);
}

/// <summary>
/// An object that can be constructed and deconstructed generically with four parameters.
/// </summary>
public interface IGenericSerializable<out TSelf, T1, T2, T3, T4> where TSelf : IGenericSerializable<TSelf, T1, T2, T3, T4>
{
	public abstract static TSelf Construct(T1 t1, T2 t2, T3 t3, T4 t4);
	public void Deconstruct(out T1 t1, out T2 t2, out T3 t3, out T4 t4);
}

/// <summary>
/// An object that can be constructed and deconstructed generically with five parameters.
/// </summary>
public interface IGenericSerializable<out TSelf, T1, T2, T3, T4, T5> where TSelf : IGenericSerializable<TSelf, T1, T2, T3, T4, T5>
{
	public abstract static TSelf Construct(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5);
	public void Deconstruct(out T1 t1, out T2 t2, out T3 t3, out T4 t4, out T5 t5);
}

/// <summary>
/// An object that can be constructed and deconstructed generically with six parameters.
/// </summary>
public interface IGenericSerializable<out TSelf, T1, T2, T3, T4, T5, T6> where TSelf : IGenericSerializable<TSelf, T1, T2, T3, T4, T5, T6>
{
	public abstract static TSelf Construct(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6);
	public void Deconstruct(out T1 t1, out T2 t2, out T3 t3, out T4 t4, out T5 t5, out T6 t6);
}

/// <summary>
/// An object that can be constructed and deconstructed generically with seven parameters.
/// </summary>
public interface IGenericSerializable<out TSelf, T1, T2, T3, T4, T5, T6, T7> where TSelf : IGenericSerializable<TSelf, T1, T2, T3, T4, T5, T6, T7>
{
	public abstract static TSelf Construct(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7);
	public void Deconstruct(out T1 t1, out T2 t2, out T3 t3, out T4 t4, out T5 t5, out T6 t6, out T7 t7);
}

/// <summary>
/// An object that can be constructed and deconstructed generically with eight parameters.
/// </summary>
public interface IGenericSerializable<out TSelf, T1, T2, T3, T4, T5, T6, T7, T8> where TSelf : IGenericSerializable<TSelf, T1, T2, T3, T4, T5, T6, T7, T8>
{
	public abstract static TSelf Construct(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8);
	public void Deconstruct(out T1 t1, out T2 t2, out T3 t3, out T4 t4, out T5 t5, out T6 t6, out T7 t7, out T8 t8);
}

/// <summary>
/// An object that can be constructed and deconstructed generically with nine parameters.
/// </summary>
public interface IGenericSerializable<out TSelf, T1, T2, T3, T4, T5, T6, T7, T8, T9> where TSelf : IGenericSerializable<TSelf, T1, T2, T3, T4, T5, T6, T7, T8, T9>
{
	public abstract static TSelf Construct(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9);
	public void Deconstruct(out T1 t1, out T2 t2, out T3 t3, out T4 t4, out T5 t5, out T6 t6, out T7 t7, out T8 t8, out T9 t9);
}

/// <summary>
/// An object that can be constructed and deconstructed generically with ten parameters.
/// </summary>
public interface IGenericSerializable<out TSelf, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> where TSelf : IGenericSerializable<TSelf, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>
{
	public abstract static TSelf Construct(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10);
	public void Deconstruct(out T1 t1, out T2 t2, out T3 t3, out T4 t4, out T5 t5, out T6 t6, out T7 t7, out T8 t8, out T9 t9, out T10 t10);
}

/// <summary>
/// An object that can be constructed and deconstructed generically with eleven parameters.
/// </summary>
public interface IGenericSerializable<out TSelf, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> where TSelf : IGenericSerializable<TSelf, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>
{
	public abstract static TSelf Construct(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11);
	public void Deconstruct(out T1 t1, out T2 t2, out T3 t3, out T4 t4, out T5 t5, out T6 t6, out T7 t7, out T8 t8, out T9 t9, out T10 t10, out T11 t11);
}

/// <summary>
/// An object that can be constructed and deconstructed generically with twelve parameters.
/// </summary>
public interface IGenericSerializable<out TSelf, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> where TSelf : IGenericSerializable<TSelf, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>
{
	public abstract static TSelf Construct(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12);
	public void Deconstruct(out T1 t1, out T2 t2, out T3 t3, out T4 t4, out T5 t5, out T6 t6, out T7 t7, out T8 t8, out T9 t9, out T10 t10, out T11 t11, out T12 t12);
}

/// <summary>
/// An object that can be constructed and deconstructed generically with thirteen parameters.
/// </summary>
public interface IGenericSerializable<out TSelf, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> where TSelf : IGenericSerializable<TSelf, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>
{
	public abstract static TSelf Construct(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12, T13 t13);
	public void Deconstruct(out T1 t1, out T2 t2, out T3 t3, out T4 t4, out T5 t5, out T6 t6, out T7 t7, out T8 t8, out T9 t9, out T10 t10, out T11 t11, out T12 t12, out T13 t13);
}

/// <summary>
/// An object that can be constructed and deconstructed generically with fourteen parameters.
/// </summary>
public interface IGenericSerializable<out TSelf, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> where TSelf : IGenericSerializable<TSelf, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>
{
	public abstract static TSelf Construct(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12, T13 t13, T14 t14);
	public void Deconstruct(out T1 t1, out T2 t2, out T3 t3, out T4 t4, out T5 t5, out T6 t6, out T7 t7, out T8 t8, out T9 t9, out T10 t10, out T11 t11, out T12 t12, out T13 t13, out T14 t14);
}

/// <summary>
/// An object that can be constructed and deconstructed generically with fifteen parameters.
/// </summary>
public interface IGenericSerializable<out TSelf, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> where TSelf : IGenericSerializable<TSelf, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>
{
	public abstract static TSelf Construct(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12, T13 t13, T14 t14, T15 t15);
	public void Deconstruct(out T1 t1, out T2 t2, out T3 t3, out T4 t4, out T5 t5, out T6 t6, out T7 t7, out T8 t8, out T9 t9, out T10 t10, out T11 t11, out T12 t12, out T13 t13, out T14 t14, out T15 t15);
}

/// <summary>
/// An object that can be constructed and deconstructed generically with sixteen parameters.
/// </summary>
public interface IGenericSerializable<out TSelf, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16> where TSelf : IGenericSerializable<TSelf, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>
{
	public abstract static TSelf Construct(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12, T13 t13, T14 t14, T15 t15, T16 t16);
	public void Deconstruct(out T1 t1, out T2 t2, out T3 t3, out T4 t4, out T5 t5, out T6 t6, out T7 t7, out T8 t8, out T9 t9, out T10 t10, out T11 t11, out T12 t12, out T13 t13, out T14 t14, out T15 t15, out T16 t16);
}
