using System;
using m.transport;
using m.transport.Android;
using System.Threading.Tasks;
using m.transport.Interfaces;
using Android.Media;
using Android.Content.Res;
using Android.Content;
using Android.App;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

[assembly: Dependency(typeof(Sound))]
namespace m.transport.Android
{
	public class Sound : ISound
	{
		private readonly Context _context;
		private SoundPool pool = new SoundPool(1, Stream.Music, 0);
		private string[] soundList = {"successLoading.mp3","errorLoading.mp3","beep.mp3", "error.mp3"};
		private int[] soundID;

		public Sound ()
		{
			_context = Forms.Context;
			soundID = new int[soundList.Length];

			for(int i = 0; i < soundList.Length; i++) {
				soundID[i] = pool.Load(_context.Assets.OpenFd(soundList[i]),1);
			}
		}


		public void PlaySound(SoundType sound)
		{
			try
			{
				switch (sound) {
				case SoundType.SuccessLoad:
					pool.Play(soundID[0], 1.0f, 1.0f, 10, 0, 1.0f);
					break;
				case SoundType.ErrorLoad:
					pool.Play(soundID[1], 1.0f, 1.0f, 10, 0, 1.0f);
					break;
				case SoundType.Success:
					pool.Play(soundID[2], 1.0f, 1.0f, 10, 0, 1.0f);
					break;
				case SoundType.Error:
					pool.Play(soundID[3], 1.0f, 1.0f, 10, 0, 1.0f);
					break;
				default:
					break;
				}
			}
			catch (Exception e)
			{
				Console.WriteLine (e.Message);
			}
		}
	}
}

