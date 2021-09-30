using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Eu4ModViewer.ModParser.Parser
{
	class EncodingFinder
	{
		public Encoding Windows1252 { get; }
		public Encoding UTF8 => Encoding.UTF8;
		public EncodingFinder()
		{
			Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
			Windows1252 = Encoding.GetEncoding(1252);
		}

		public string Convert(string val)
		{
			var valBytes = Windows1252.GetBytes(val);
			var utf8Btyes = Encoding.Convert(Windows1252, UTF8, valBytes);
			return UTF8.GetString(utf8Btyes);
		}
	}

	static class ParserUtils
	{
		public static readonly char PropertySeperator = '=';
		public static readonly char ObjectOpen = '{';
		public static readonly char ObjectClose = '}';
		public static readonly char[] Spaces = new[] { ' ', '\t' };
		public static readonly char Quote = '"';
		public static readonly char Comment = '#';
		public static readonly char Eof = (char)65535;
		public static readonly char[] NewLines = new[] { '\n', '\r' };
		public static readonly char[] NewLinesWithEof = new[] { '\n', '\r', Eof };
		public static readonly char[] NewLinesAndSpaces = NewLines.Concat(Spaces).ToArray();



		public static EncodingFinder Encoding = new EncodingFinder();

		/// <summary>
		/// Reads the property name, starting from the first character of the name
		/// </summary>
		/// <param name="sr"></param>
		/// <returns></returns>
		public static (char, string) ReadPropertyName(StreamReader sr, char startChar)
		{
			// Pessimistic approach to which characters might be in the prop seperator
			var propNameSepChars = NewLines.Concat(new[] { PropertySeperator }).Concat(Spaces);
			string name;
			// Read until we get a char which isn't part of the propname
			(startChar, name) = ReadUntil(sr, startChar, propNameSepChars);
			// Skip until we find a char which isn't a sep char, thats the start of the value
			startChar = SkipUntilNot(sr, startChar, propNameSepChars);
			return (startChar, name);
		}

		// Can handle a single set of quotes, ends on a space or newline
		public static (char, string) ReadPropertyStringValue(StreamReader sr, char startChar)
		{
			var name = "";
			(startChar, name) = ReadUntil(sr, startChar, new[] { Quote }.Concat(NewLines).Concat(Spaces), new[] { ObjectClose } /*Sometimes there isn't a line but an object close, and we need to return BEFORE it*/);
			if (startChar == Quote)
			{
				// We found a quote, read until we get another one.
				startChar = (char)sr.Read();
				(startChar, name) = ReadUntil(sr, startChar, new[] { Quote });
				name = Quote + name + Quote;
			}
			return (startChar, name);
		}

		public static (char, string) ReadUntil(StreamReader sr, char currentChar, IEnumerable<char> untilChars, IEnumerable<char> peekUntilChars = null)
		{
			var buffer = new StringBuilder();
			while (true)
			{
				if (currentChar == Eof)
				{
					break;
				}
				if (currentChar == Comment)
				{
					currentChar = SkipUntil(sr, currentChar, NewLines, new[] { '}' });
					currentChar = SkipUntilNot(sr, currentChar, NewLines.Concat(Spaces), peekUntilChars);
				}
				if (peekUntilChars != null)
				{
					if (peekUntilChars.Contains((char)sr.Peek()))
					{
						break;
					}
				}
				if (untilChars.Contains(currentChar))
				{
					break;
				}
				buffer.Append(currentChar);
				currentChar = (char)sr.Read();
			}
			return (currentChar, buffer.ToString());
		}

		public static (char, string) ReadUntilNot(StreamReader sr, char currentChar, IEnumerable<char> untilNotChars)
		{
			var buffer = new StringBuilder();
			while (true)
			{
				if (currentChar == Eof)
				{
					break;
				}
				if (currentChar == Comment)
				{
					currentChar = SkipUntil(sr, currentChar, NewLines);
					currentChar = SkipUntilNot(sr, currentChar, NewLines.Concat(Spaces));
					return (currentChar, buffer.ToString());
				}
				if (!untilNotChars.Contains(currentChar))
				{
					break;
				}
				buffer.Append(currentChar);
				currentChar = (char)sr.Read();
			}
			return (currentChar, buffer.ToString());
		}

		public static char SkipUntil(StreamReader sr, char currentChar, IEnumerable<char> untilChars, IEnumerable<char> peekUntilChars = null)
		{
			while (true)
			{
				if (currentChar == Eof)
				{
					return currentChar;
				}
				if (currentChar == Comment)
				{
					// Stop potential infinite recursion
					currentChar = (char)sr.Read();

					currentChar = SkipUntil(sr, currentChar, NewLines, peekUntilChars);
					currentChar = SkipUntilNot(sr, currentChar, NewLines.Concat(Spaces), peekUntilChars);
					return currentChar;
				}
				if (peekUntilChars != null)
				{
					if (peekUntilChars.Contains((char)sr.Peek()))
					{
						return currentChar;
					}
				}
				if (untilChars.Contains(currentChar))
				{
					return currentChar;
				}
				currentChar = (char)sr.Read();
			}
		}

		public static object ParseStringValue(string value, Type type)
		{
			if (type == typeof(string))
			{
				return Encoding.Convert(value);
			}
			else if (type == typeof(int))
			{
				return int.Parse(value);
			}
			else if (type == typeof(long))
			{
				return long.Parse(value);
			}
			else if (type == typeof(decimal))
			{
				return decimal.Parse(value);
			}
			else if (type == typeof(bool))
			{
				return value.ToLower() == "yes";
			}
			throw new Exception("Didn't recognise type of string value");
		}

		public static char SkipUntilNot(StreamReader sr, char currentChar, IEnumerable<char> untilNotChars, IEnumerable<char> peekUntilChars = null)
		{
			while (true)
			{
				if (currentChar == Eof)
				{
					return currentChar;
				}
				if (currentChar == Comment)
				{
					currentChar = SkipUntil(sr, currentChar, NewLines, peekUntilChars);
					currentChar = SkipUntilNot(sr, currentChar, NewLines.Concat(Spaces), peekUntilChars);
					return currentChar;
				}
				if (peekUntilChars != null)
				{
					if (peekUntilChars.Contains((char)sr.Peek()))
					{
						return currentChar;
					}
				}
				if (!untilNotChars.Contains(currentChar))
				{
					return currentChar;
				}
				currentChar = (char)sr.Read();
			}
		}
	}
}
