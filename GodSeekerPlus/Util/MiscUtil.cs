using System;
using System.IO;
using Newtonsoft.Json;

namespace GodSeekerPlus.Util {
	internal static class MiscUtil {
		internal static bool EnclosedWith(this string self, string start, string end) =>
			self.StartsWith(start) && self.EndsWith(end);

		internal static string StripStart(this string self, string val) =>
			self.StartsWith(val) ? self.Substring(val.Length) : self;

		internal static string StripEnd(this string self, string val) =>
			self.EndsWith(val) ? self.Substring(0, self.Length - val.Length) : self;


		internal static string ReadToString(this Stream self) =>
			new StreamReader(self).ReadToEnd();


		internal static T DeserializeJson<T>(string json) =>
			(T) JsonConvert.DeserializeObject(json, typeof(T));


		internal static T Try<T>(Func<T> f, T @default) {
			try {
				return f();
			} catch {
				return @default;
			}
		}
	}
}