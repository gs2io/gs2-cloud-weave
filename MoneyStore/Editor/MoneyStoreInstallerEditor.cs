using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gs2.Gs2Showcase.Model;
using Gs2.Unity.Gs2Showcase.Model;
using Gs2.Weave.Core.Editor;
using Gs2.Util.LitJson;
using UnityEditor;
using UnityEngine;

namespace Gs2.Weave.MoneyStore
{
    [CustomEditor(typeof(MoneyStoreInstaller))]   
    public class MoneyStoreInstallerEditor: AbstractWeaveInstaller
    {
        public override void OnInspectorGUI()
        {
            var installer = target as MoneyStoreInstaller;
            // ReSharper disable once PossibleNullReferenceException
            var setting = installer.GetComponent<MoneyStoreSetting>();

            base.OnInspectorGUI();

            using (new EditorGUI.ChangeCheckScope())
            {
                if (outputs != null)
                {
                    setting.moneyNamespaceName = installer.moneyInstaller.moneyNamespaceName;
                    
                    foreach (var output in outputs)
                    {
                        if (output.name == "ShowcaseNamespaceName")
                        {
                            installer.showcaseNamespaceName = output.value;
                            EditorUtility.SetDirty(installer);
                            setting.showcaseNamespaceName = output.value;
                            EditorUtility.SetDirty(setting);
                        }
                        if (output.name == "ShowcaseModelName")
                        {
                            installer.showcaseModelName = output.value;
                            EditorUtility.SetDirty(installer);
                            setting.showcaseModelName = output.value;
                            EditorUtility.SetDirty(setting);
                        }
                        if (output.name == "ShowcaseKeyNamespaceName")
                        {
                            installer.keyNamespaceName = output.value;
                            EditorUtility.SetDirty(installer);
                        }
                        if (output.name == "ShowcaseKeyName")
                        {
                            installer.keyName = output.value;
                            EditorUtility.SetDirty(installer);
                        }
                        if (output.name == "ShowcaseKeyId")
                        {
                            setting.showcaseKeyId = output.value;
                            EditorUtility.SetDirty(setting);
                        }
                    }

                    outputs = null;
                }
            }
            
            DrawDefaultInspector();
        }
        
        public class ShowcaseMaster
        {
            [SerializeField]
            public string version = "2019-04-04";
            [SerializeField]
            public List<Showcase> showcases;
            
            public void WriteJson(JsonWriter writer)
            {
                writer.WriteObjectStart();
                if(this.version != null)
                {
                    writer.WritePropertyName("version");
                    writer.Write(this.version);
                }
                if(this.showcases != null)
                {
                    writer.WritePropertyName("showcases");
                    writer.WriteArrayStart();
                    foreach(var item in this.showcases)
                    {
                        item.WriteJson(writer);
                    }
                    writer.WriteArrayEnd();
                }
                writer.WriteObjectEnd();
            }
        }

        protected override bool Validate()
        {
            var installer = target as MoneyStoreInstaller;
            // ReSharper disable once PossibleNullReferenceException
            if (installer.moneyInstaller == null)
            {
                return false;
            }
            if (string.IsNullOrEmpty(installer.showcaseNamespaceName))
            {
                return false;
            }
            if (string.IsNullOrEmpty(installer.showcaseModelName))
            {
                return false;
            }
            if (installer.products == null || installer.products.Count == 0)
            {
                return false;
            }
            if (string.IsNullOrEmpty(installer.keyNamespaceName))
            {
                return false;
            }
            if (string.IsNullOrEmpty(installer.keyName))
            {
                return false;
            }
            return true;
        }

        class MoneyStoreMasterPostProcess : WeaveInstaller.PostProcess
        {
            private MoneyStoreInstaller _installer;
            
            public MoneyStoreMasterPostProcess(MoneyStoreInstaller installer)
            {
                _installer = installer;
            }
            
            public override string Execute(string original)
            {
                original = original.Replace("${Gs2::Weave::ShowcaseNamespaceName}", _installer.showcaseNamespaceName);
                original = original.Replace("${Gs2::Weave::ShowcaseModelName}", _installer.showcaseModelName);
                original = original.Replace("${Gs2::Weave::ShowcaseKeyNamespaceName}", _installer.keyNamespaceName);
                original = original.Replace("${Gs2::Weave::ShowcaseKeyName}", _installer.keyName);
                
                {
                    var master = new ShowcaseMaster
                    {
                        showcases = new List<Showcase>
                        {
                            new Showcase
                            {
                                name = _installer.showcaseModelName,
                                displayItems = _installer.products.Select(
                                    product => product.ToModel(_installer.moneyInstaller.moneyNamespaceName)
                                ).ToList(),
                            }
                        }
                    };
                
                    var stringBuilder = new StringBuilder();
                    var jsonWriter = new JsonWriter(stringBuilder);
                    master.WriteJson(jsonWriter);

                    var json = JsonMapper.ToObject(original);
                    json["Resources"]["ShowcaseSettings"]["Properties"]["Settings"] = stringBuilder.ToString();
                    original = json.ToJson();
                }

                return original;
            }
        }

        protected override WeaveInstaller.PostProcess PostProcess()
        {
            var installer = target as MoneyStoreInstaller;
            // ReSharper disable once PossibleNullReferenceException
            return new MoneyStoreMasterPostProcess(installer);
        }
    }
}