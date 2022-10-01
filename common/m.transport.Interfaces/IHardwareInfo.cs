namespace m.transport.Interfaces
{
	public interface IHardwareInfo
	{
		string Version { get; }
		string Manufacturer { get; }
		string Carrier { get; }
		string Name { get; }
		int Height { get; }
		int Width { get; }
		string Model { get; }
        bool IsPortrait { get; }
		void ClearData();
		bool GetClearDataCompleted();
		void SetClearDataCompleted();
		void Landscape();
		void Portrait();
	}
}

