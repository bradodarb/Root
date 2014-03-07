using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Web.Caching;
using System.Web.Hosting;



namespace Web.Root.Embedded
{
    public class EmbeddedVirtualPathProvider : VirtualPathProvider
    {
        private VirtualPathProvider _Previous;
        private string _EmbedPrefix = "Embedded";
        private Type _SourceType;

        public EmbeddedVirtualPathProvider(Type sourceType, string embeddedprefix, VirtualPathProvider previous)
        {
            _SourceType = sourceType;
            _EmbedPrefix = embeddedprefix;
            _Previous = previous;
        }
        public override bool FileExists(string virtualPath)
        {
            if (IsEmbeddedPath(virtualPath))
                return true;
            else
                return _Previous.FileExists(virtualPath);
        }
        public override CacheDependency GetCacheDependency(string virtualPath, IEnumerable virtualPathDependencies, DateTime utcStart)
        {
            if (IsEmbeddedPath(virtualPath))
            {
                return null;
            }
            else
            {
                return _Previous.GetCacheDependency(virtualPath, virtualPathDependencies, utcStart);
            }
        }
        public override VirtualDirectory GetDirectory(string virtualDir)
        {
            return _Previous.GetDirectory(virtualDir);
        }
        public override bool DirectoryExists(string virtualDir)
        {
            return _Previous.DirectoryExists(virtualDir);
        }
        public override VirtualFile GetFile(string virtualPath)
        {
            if (IsEmbeddedPath(virtualPath))
            {
                string fileNameWithExtension = virtualPath.Replace(_EmbedPrefix + "/", "");
                string nameSpace = _SourceType.Assembly.GetName().Name;

                string manifestResourceName = string.Format("{0}.{1}", nameSpace,
                    fileNameWithExtension.Replace("/", "."));

                var target = _SourceType.Assembly.GetManifestResourceNames().FirstOrDefault(x => x.ToLower() == manifestResourceName.ToLower());

                var stream = _SourceType.Assembly.GetManifestResourceStream(target);
                return new EmbeddedVirtualFile(virtualPath, stream);
            }
            else
                return _Previous.GetFile(virtualPath);
        }
        private bool IsEmbeddedPath(string path)
        {
            return path.Contains(_EmbedPrefix);
        }

    }

    public class EmbeddedVirtualFile : VirtualFile
    {
        private Stream _Stream;

        public EmbeddedVirtualFile(string virtualPath, Stream stream)
            : base(virtualPath)
        {
            _Stream = stream;
        }
        public override Stream Open()
        {
            return _Stream;
        }
    }
}