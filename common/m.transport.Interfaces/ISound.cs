using System;

namespace m.transport.Interfaces
{
	public enum SoundType {
		Success, Error, SuccessLoad, ErrorLoad
	}

	public interface ISound
	{
		void PlaySound(SoundType sound);
	}
}

