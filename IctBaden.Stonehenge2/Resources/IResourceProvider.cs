namespace IctBaden.Stonehenge2.Resources
{
    using System;
    using System.Collections.Generic;

    using IctBaden.Stonehenge2.Core;

    public interface IResourceProvider : IDisposable
    {
        Resource Get(AppSession session, string resourceName);

        Resource Post(AppSession session, string resourceName, object[] postParams, Dictionary<string, string> formData);

    }
}
