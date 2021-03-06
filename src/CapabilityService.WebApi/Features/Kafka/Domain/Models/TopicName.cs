using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using DFDS.CapabilityService.WebApi.Domain.Models;
using DFDS.CapabilityService.WebApi.Features.Kafka.Domain.Exceptions;

namespace DFDS.CapabilityService.WebApi.Features.Kafka.Domain.Models
{
	public class TopicName : ValueObject
	{
		private TopicName(string name)
		{
			Name = name;
		}

		public string Name { get; }
		private static int MAX_TOPIC_NAME_LENGTH = 55;

		protected override IEnumerable<object> GetEqualityComponents()
		{
			yield return Name;
		}

		public static TopicName FromString(string fullName)
		{
			return new TopicName(fullName);
		}

		public static TopicName Create(
			string capabilityRootId,
			string topicName,
			TopicAvailability topicAvailability
		)
		{
			var cleanTopicName = CleanString(topicName);

			if (cleanTopicName.Length < 1)
			{
				throw new TopicNameTooShortException();
			}

			var topicAvailableSection = topicAvailability is TopicAvailabilityPublic ? "pub." : "";
			
			var combinedString = topicAvailableSection + CreatePrefix(capabilityRootId) + "." + cleanTopicName;
			if (combinedString.Length > MAX_TOPIC_NAME_LENGTH)
			{
				throw new TopicNameTooLongException(combinedString.ToLower(), MAX_TOPIC_NAME_LENGTH);
			}

			var charStringInLowerCase = combinedString.ToLower();

			return new TopicName(charStringInLowerCase);
		}

		public static string CreatePrefix(string capabilityRootId)
		{
			return CleanString(capabilityRootId);
		}

		private static string CleanString(string input)
		{
			var inputLinted = input
				.Replace(' ', '-')
				.Replace('.', '-')
				.Replace('_', '-')
				.Replace(
					oldValue: "æ",
					newValue: "ae",
					ignoreCase: true,
					culture: CultureInfo.InvariantCulture
				)
				.Replace(
					oldValue: "ø",
					newValue: "oe",
					ignoreCase: true,
					culture: CultureInfo.InvariantCulture
				)
				.Replace(
					oldValue: "å",
					newValue: "aa",
					ignoreCase: true,
					culture: CultureInfo.InvariantCulture
				);

			return Regex.Replace(inputLinted, "[^a-zA-Z0-9-]", "");
		}
	}
}
