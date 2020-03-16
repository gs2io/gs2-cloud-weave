using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gs2.Gs2Showcase.Model;
using Gs2.Unity.Gs2Showcase.Model;
using Gs2.Weave.Core.Editor;
using LitJson;
using UnityEditor;
using UnityEngine;

namespace Gs2.Weave.Gacha
{
    [CustomEditor(typeof(GachaInstaller))]   
    public class GachaInstallerEditor: AbstractWeaveInstaller
    {
        public override void OnInspectorGUI()
        {
            var installer = target as GachaInstaller;
            // ReSharper disable once PossibleNullReferenceException
            var setting = installer.GetComponent<GachaSetting>();

            base.OnInspectorGUI();

            using (new EditorGUI.ChangeCheckScope())
            {
                if (outputs != null)
                {
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
                        if (output.name == "LotteryNamespaceName")
                        {
                            installer.lotteryNamespaceName = output.value;
                            EditorUtility.SetDirty(installer);
                            setting.lotteryNamespaceName = output.value;
                            EditorUtility.SetDirty(setting);
                        }
                        if (output.name == "KeyNamespaceName")
                        {
                            installer.keyNamespaceName = output.value;
                            EditorUtility.SetDirty(installer);
                        }
                        if (output.name == "ShowcaseKeyName")
                        {
                            installer.storeKeyName = output.value;
                            EditorUtility.SetDirty(installer);
                        }
                        if (output.name == "LotteryKeyName")
                        {
                            installer.lotteryKeyName = output.value;
                            EditorUtility.SetDirty(installer);
                        }
                        if (output.name == "ShowcaseKeyId")
                        {
                            setting.showcaseKeyId = output.value;
                            EditorUtility.SetDirty(setting);
                        }
                        if (output.name == "LotteryKeyId")
                        {
                            setting.lotteryKeyId = output.value;
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

        public class LotteryMaster
        {
            [SerializeField]
            public string version = "2019-02-21";
            [SerializeField]
            public string inventoryNamespaceName;
            [SerializeField]
            public string inventoryModelName;
            [SerializeField]
            public WeaveLotteryPrizeSetting prizeSetting;
            
            public void WriteJson(JsonWriter writer)
            {
                writer.WriteObjectStart();
                if(this.version != null)
                {
                    writer.WritePropertyName("version");
                    writer.Write(this.version);
                }
                if(this.prizeSetting != null)
                {
                    writer.WritePropertyName("lotteryModels");
                    writer.WriteArrayStart();
                    foreach(var item in this.prizeSetting.ToLotteryModel())
                    {
                        item.WriteJson(writer);
                    }
                    writer.WriteArrayEnd();
                    
                    writer.WritePropertyName("prizeTables");
                    writer.WriteArrayStart();
                    foreach(var item in this.prizeSetting.ToPrizeTableModel(
                        inventoryNamespaceName,
                        inventoryModelName
                    ))
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
            var installer = target as GachaInstaller;
            // ReSharper disable once PossibleNullReferenceException
            if (installer.lotteryNamespaceName == null)
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
            if (installer.gachaSettings == null)
            {
                return false;
            }
            if (installer.gachaSettings.ssrPrizes.Count == 0)
            {
                return false;
            }
            if (installer.gachaSettings.srPrizes.Count == 0)
            {
                return false;
            }
            if (installer.gachaSettings.rPrizes.Count == 0)
            {
                return false;
            }
            if (installer.gachaSettings.nPrizes.Count == 0)
            {
                return false;
            }
            if (string.IsNullOrEmpty(installer.keyNamespaceName))
            {
                return false;
            }
            if (string.IsNullOrEmpty(installer.storeKeyName))
            {
                return false;
            }
            if (string.IsNullOrEmpty(installer.lotteryKeyName))
            {
                return false;
            }
            return true;
        }

        class GachaMasterPostProcess : WeaveInstaller.PostProcess
        {
            private GachaInstaller _installer;
            
            public GachaMasterPostProcess(GachaInstaller installer)
            {
                _installer = installer;
            }
            
            public override string Execute(string original)
            {
                original = original.Replace("${Gs2::Weave::ShowcaseNamespaceName}", _installer.showcaseNamespaceName);
                original = original.Replace("${Gs2::Weave::ShowcaseModelName}", _installer.showcaseModelName);
                original = original.Replace("${Gs2::Weave::LotteryNamespaceName}", _installer.lotteryNamespaceName);
                original = original.Replace("${Gs2::Weave::KeyNamespaceName}", _installer.keyNamespaceName);
                original = original.Replace("${Gs2::Weave::ShowcaseKeyName}", _installer.storeKeyName);
                original = original.Replace("${Gs2::Weave::LotteryKeyName}", _installer.lotteryKeyName);
                original = original.Replace("${Gs2::Weave::JobQueueNamespaceName}", _installer.jobQueueInstaller.jobQueueNamespaceName);
                
                {
                    var showcaseMaster = new ShowcaseMaster
                    {
                        showcases = new List<Showcase>
                        {
                            new Showcase
                            {
                                name = _installer.showcaseModelName,
                                displayItems = _installer.products.Select(
                                    product => product.ToModel(
                                        _installer.moneyInstaller.moneyNamespaceName,
                                        _installer.lotteryNamespaceName
                                    )
                                ).ToList(),
                            }
                        }
                    };

                    {
                        var stringBuilder = new StringBuilder();
                        var jsonWriter = new JsonWriter(stringBuilder);
                        showcaseMaster.WriteJson(jsonWriter);

                        var json = JsonMapper.ToObject(original);
                        json["Resources"]["ShowcaseSettings"]["Properties"]["Settings"] = stringBuilder.ToString();
                        original = json.ToJson();
                    }

                    var lotteryMaster = new LotteryMaster
                    {
                        prizeSetting = _installer.gachaSettings,
                        inventoryNamespaceName = _installer.unitInstaller.inventoryNamespaceName,
                        inventoryModelName = _installer.unitInstaller.inventoryModelName,
                    };

                    {
                        var stringBuilder = new StringBuilder();
                        var jsonWriter = new JsonWriter(stringBuilder);
                        lotteryMaster.WriteJson(jsonWriter);

                        var json = JsonMapper.ToObject(original);
                        json["Resources"]["LotterySettings"]["Properties"]["Settings"] = stringBuilder.ToString();
                        original = json.ToJson();
                    }
                }

                return original;
            }
        }

        protected override WeaveInstaller.PostProcess PostProcess()
        {
            var installer = target as GachaInstaller;
            // ReSharper disable once PossibleNullReferenceException
            return new GachaMasterPostProcess(installer);
        }
    }
}