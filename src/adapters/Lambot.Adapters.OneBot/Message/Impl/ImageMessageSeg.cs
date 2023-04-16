﻿namespace Lambot.Adapters.OneBot;

public class ImageMessageSeg : MessageSeg
{
    public ImageMessageSeg(Dictionary<string, string> props)
    {
        File = props.GetValueOrDefault("file");
        Url = props.GetValueOrDefault("url");
    }

    public override MessageSegType Type => MessageSegType.Image;
    public string File { get; set; }
    public string Url { get; set; }

    protected override Dictionary<string, string> GetProps()
    {
        return new()
        {
            { "file", File },
            { "url", Url }
        };
    }
}