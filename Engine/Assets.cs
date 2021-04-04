using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Engine.Rendering;

namespace Engine
{
    public sealed class Assets
    {
        private record AssetDefinition(string AssetName, string FullPath, Assembly Assembly);
        
        private static readonly Dictionary<string, AssetDefinition> _nameToAsset = new ();
        private static readonly Dictionary<Type, Func<StreamReader, object>> _typeToLoader = new ();

        static Assets()
        {
            AddAssembly(Assembly.GetCallingAssembly());
            AddLoader<VertexShader>(stream => new VertexShader(stream));
            AddLoader<FragmentShader>(stream => new FragmentShader(stream));
            AddLoader<ComputeShader>(stream => new ComputeShader(stream));
        }

        public static void AddAssembly(Assembly assembly)
        {
            string[] assetPaths = assembly.GetManifestResourceNames();
            string assemblyName = assembly.GetName().Name!;
            foreach (string assetPath in assetPaths)
            {
                string assetName = assetPath[(assemblyName.Length + 1)..];
                _nameToAsset.Add(assetName, new AssetDefinition(assetName, assetPath, assembly));
            }
        }

        public static void AddLoader<T>(Func<StreamReader, object> loaderFunction)
        {
            _typeToLoader.Add(typeof(T), loaderFunction);
        }

        public static T GetAsset<T>(string name)
        {
            if (!_nameToAsset.TryGetValue(name, out AssetDefinition asset))
            {
                throw new Exception($"Asset {name} does not exist. Try adding the assembly it to the asset loader.");
            }

            if (!_typeToLoader.TryGetValue(typeof(T), out Func<StreamReader, object> loaderFunction))
            {
                throw new Exception($"No loader function found for {typeof(T)}. Try adding the loader to the asset loader.");
            }

            Stream? stream = asset.Assembly.GetManifestResourceStream(asset.FullPath);
            StreamReader streamReader = new StreamReader(stream!);
            object obj = loaderFunction.Invoke(streamReader);
            if (obj is T returnVal)
            {
                return returnVal;
            }

            throw new Exception(
                $"Either {name} is not of type {typeof(T)}, or the loader for {typeof(T)} is not working correctly.");
        }
    }
}