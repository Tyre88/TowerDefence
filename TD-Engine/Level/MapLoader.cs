using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;

namespace TD_Engine
{
    public class MapLoader
    {
        [ContentTypeWriter]
        public class MapLoaderWriter : ContentTypeWriter<MapLoader>
        {
            protected override void Write(ContentWriter output, MapLoader value)
            {
                output.WriteObject(value.MapContentNames);
            }

            public override string GetRuntimeReader(Microsoft.Xna.Framework.Content.Pipeline.TargetPlatform targetPlatform)
            {
                return typeof(MapLoader.MapLoaderReader).AssemblyQualifiedName;
            }
        }

        [ContentSerializer]
        private List<string> MapContentNames
        {
            get;
            set;
        }

        [ContentSerializerIgnore]
        public List<Map> Maps
        {
            get;
            private set;
        }

        [ContentSerializerIgnore]
        public static MapLoader _singleton;

        public MapLoader()
        {
            MapContentNames = new List<string>();
            Maps = new List<Map>();
        }

        public class MapLoaderReader : ContentTypeReader<MapLoader>
        {
            protected override MapLoader Read(ContentReader input, MapLoader existingInstance)
            {
                MapLoader mapLoader = new MapLoader();

                mapLoader.MapContentNames.AddRange(input.ReadObject<List<string>>());

                foreach (string item in mapLoader.MapContentNames)
                {
                    Map test = input.ContentManager.Load<Map>(string.Format("Maps\\{0}\\{0}", item));
                    mapLoader.Maps.Add(input.ContentManager.Load<Map>(string.Format("Maps\\{0}\\{0}", item)));
                }

                return mapLoader;
            }
        }

    }
}
