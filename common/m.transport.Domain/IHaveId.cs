namespace m.transport.Domain
{
	public interface IHaveId<T>
	{
		T Id { get; }
	}
}