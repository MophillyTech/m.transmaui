using System.Collections.Generic;

namespace m.transport.Interfaces
{
	public interface IRepository<T,TKey>
	{
		T GetById(TKey id);
		IEnumerable<T> GetAll();

		void Save(T item);
		void SaveAll(IEnumerable<T> items);

		void Delete(T item);

		void DeleteAll();

	}
}