namespace Lambot.Adapters.OneBot;

internal class StringParser
{
    private readonly string _source;
    public bool IsEnd => _index > _source.Length - 1;
    private int _index = 0;
    public char Current => _source[_index];

    public StringParser(string source)
    {
        _source = source;
    }

    private void MoveTo(char ch, bool include = false)
    {
        var ch_index = _source.IndexOf(ch, _index + 1);
        if (ch_index == -1)
        {
            _index = _source.Length;
            return;
        }
        if (include)
        {
            ch_index++;
        }
        _index = ch_index;
    }

    public string ReadTo(char ch, bool include = false)
    {
        var start = _index;
        MoveTo(ch, include);
        return _source[start.._index];
    }
}