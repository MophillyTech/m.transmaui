using System;
using m.transport.Interfaces;
using Foundation;
using AVFoundation;
using m.transport.iOS;
using System.IO;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

[assembly: Dependency(typeof(Sound))]
namespace m.transport.iOS
{
	public class Sound :ISound
	{
		public Sound()
		{
		}

		public void PlaySound(SoundType sound)
		{
			try
			{
				string soundfile = string.Empty;

				switch (sound) {
					case SoundType.SuccessLoad:
						soundfile = "successLoading.mp3";
						break;
					case SoundType.ErrorLoad:
						soundfile = "errorLoading.mp3";
						break;
					case SoundType.Success:
						soundfile = "beep.mp3";
						break;
					case SoundType.Error:
						soundfile = "error.mp3";
						break;
					default:
						break;
				}

				if (!string.IsNullOrEmpty(soundfile)) {
					NSUrl soundURL = NSUrl.FromFilename(soundfile);

					using (AVAudioPlayer player = AVAudioPlayer.FromUrl(soundURL)) {
						player.Volume = 1.0f;
						player.PrepareToPlay();
						player.Play();
						Console.WriteLine("Played: " + soundfile);
						System.Threading.Thread.Sleep(1000);
					}
				}

			}
			catch (Exception e)
			{
				Console.WriteLine (e.Message);
			}

		}


	}
}

