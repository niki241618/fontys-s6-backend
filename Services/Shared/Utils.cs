namespace Shared;

public class Utils
{
	public static Dictionary<string, string> ExtractDetailsFromConnectionString(string? connectionString)
	{
		if(string.IsNullOrWhiteSpace(connectionString))
			return new Dictionary<string, string>();
		
		var details = new Dictionary<string, string>();
		var parts = connectionString.Split(';');
		
		foreach (var part in parts)
		{
			if (string.IsNullOrEmpty(part)) 
				continue;
			
			var keyValue = part.Split('=');
			details.Add(keyValue[0].Trim(), keyValue[1].Trim());
		}

		return details;
	}
}