﻿﻿using System;
using System.Collections.Generic;
using System.Linq;
  using Gs2.Util.LitJson;

namespace Gs2.Weave.EditorExtension.Editor.Model
{
    [Serializable]
    public class PackageSubsets
    {
        public List<PackageSubset> packages;

        public static PackageSubsets FromDict(JsonData data)
        {
            return new PackageSubsets
            {
                packages = data.Cast<JsonData>().Select(PackageSubset.FromDict).ToList(),
            };
        }
    }
}