using System.Collections.Generic;
using System.Linq;

namespace Adsb.Reader
{
    public class MessageBuffer
    {
        private readonly List<string> _buffer = new();

        public List<string> AddData(string data)
        {
            if (_buffer.Any())
            {
                data = $"{_buffer.First()}{data}";
            }

            string[] delimiter = {"\r\n"};
            var lines = data.Split(delimiter, System.StringSplitOptions.None).Where(x => x.Length > 0).ToList();
            _buffer.Clear();
            var carryoverLine = lines.Last();
            _buffer.Add(carryoverLine);
            lines.Remove(carryoverLine);
            return lines;
        }
    }
}