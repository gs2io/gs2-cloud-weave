﻿﻿using System;
  using LitJson;

namespace Gs2.Weave.EditorExtension.Editor.Model
{
    [Serializable]
    public class PackageSubset
    {
        public string name;

        public static PackageSubset FromDict(JsonData data)
        {
            return new PackageSubset
            {
                name = data.ToString()
            };
        }
    }
}