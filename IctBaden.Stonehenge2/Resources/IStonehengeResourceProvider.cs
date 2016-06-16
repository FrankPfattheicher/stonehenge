namespace IctBaden.Stonehenge2.Resources
{
    using System;
    using System.Collections.Generic;

    using Core;

    public interface IStonehengeResourceProvider : IDisposable
    {
        Resource Get(AppSession session, string resourceName, Dictionary<string, string> parameters);

        Resource Post(AppSession session, string resourceName, Dictionary<string, string> parameters, Dictionary<string, string> formData);

    }
}
