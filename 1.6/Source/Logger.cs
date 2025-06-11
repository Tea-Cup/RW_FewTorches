namespace Foxy.FewTorches {
	internal static class Logger {
		public static void Log(string message, params object[] args) {
			if (args.Length > 0) message = string.Format(message, args);
			Verse.Log.Message("[FewTorches] " + message);
		}
		public static void Log(object message) {
			Log(message ?? "<null>");
		}

		public static void Warn(string message, params object[] args) {
			if (args.Length > 0) message = string.Format(message, args);
			Verse.Log.Warning("[FewTorches] " + message);
		}
		public static void Warn(object message) {
			Warn(message ?? "<null>");
		}

		public static void Error(string message, params object[] args) {
			if (args.Length > 0) message = string.Format(message, args);
			Verse.Log.Error("[FewTorches] " + message);
		}
		public static void Error(object message) {
			Error(message ?? "<null>");
		}

		public static string Stringify(this object value) {
			if(value == null) return "<null>";
			return value.ToString();
		}
	}
}