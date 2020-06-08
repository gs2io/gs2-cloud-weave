using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gs2.Gs2Exchange.Model;
using Gs2.Weave.Core.Editor;
using Gs2.Util.LitJson;
using UnityEditor;

namespace Gs2.Weave.StaminaStore
{
    [CustomEditor(typeof(StaminaStoreInstaller))]   
    public class StaminaStoreInstallerEditor: AbstractWeaveInstaller
    {
        public override void OnInspectorGUI()
        {
            var installer = target as StaminaStoreInstaller;
            // ReSharper disable once PossibleNullReferenceException
            var setting = installer.GetComponent<StaminaStoreSetting>();

            base.OnInspectorGUI();

            using (new EditorGUI.ChangeCheckScope())
            {
                if (outputs != null)
                {
                    foreach (var output in outputs)
                    {
                        if (output.name == "ExchangeNamespaceName")
                        {
                            installer.exchangeNamespaceName = output.value;
                            EditorUtility.SetDirty(installer);
                            setting.exchangeNamespaceName = output.value;
                            EditorUtility.SetDirty(setting);
                        }
                        if (output.name == "ExchangeKeyNamespaceName")
                        {
                            installer.keyNamespaceName = output.value;
                            EditorUtility.SetDirty(installer);
                        }
                        if (output.name == "ExchangeKeyName")
                        {
                            installer.keyName = output.value;
                            EditorUtility.SetDirty(installer);
                        }
                        if (output.name == "ExchangeKeyId")
                        {
                            setting.exchangeKeyId = output.value;
                            EditorUtility.SetDirty(setting);
                        }
                    }

                    outputs = null;
                }
            }

            DrawDefaultInspector();
        }

        public class StaminaMaster
        {
            public string version = "2019-08-19";
            public List<RateModel> rateModels;
            
            public void WriteJson(JsonWriter writer)
            {
                writer.WriteObjectStart();
                if(this.version != null)
                {
                    writer.WritePropertyName("version");
                    writer.Write(this.version);
                }
                if(this.rateModels != null)
                {
                    writer.WritePropertyName("rateModels");
                    writer.WriteArrayStart();
                    foreach(var item in this.rateModels)
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
            var installer = target as StaminaStoreInstaller;
            // ReSharper disable once PossibleNullReferenceException
            if (installer.staminaInstaller == null)
            {
                return false;
            }
            if (installer.moneyInstaller == null)
            {
                return false;
            }
            if (string.IsNullOrEmpty(installer.exchangeNamespaceName))
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

        class StaminaStoreMasterPostProcess : WeaveInstaller.PostProcess
        {
            private StaminaStoreInstaller _installer;
            
            public StaminaStoreMasterPostProcess(StaminaStoreInstaller installer)
            {
                _installer = installer;
            }
            
            public override string Execute(string original)
            {
                original = original.Replace("${Gs2::Weave::ExchangeNamespaceName}", _installer.exchangeNamespaceName);
                original = original.Replace("${Gs2::Weave::ExchangeKeyNamespaceName}", _installer.keyNamespaceName);
                original = original.Replace("${Gs2::Weave::ExchangeKeyName}", _installer.keyName);
                
                {
                    var master = new StaminaMaster
                    {
                        rateModels = _installer.products.Select(
                            product => product.ToModel(
                                _installer.staminaInstaller.staminaNamespaceName,
                                _installer.staminaInstaller.staminaModelName,
                                _installer.moneyInstaller.moneyNamespaceName
                            )
                        ).ToList()
                    };

                    var stringBuilder = new StringBuilder();
                    var jsonWriter = new JsonWriter(stringBuilder);
                    master.WriteJson(jsonWriter);

                    var json = JsonMapper.ToObject(original);
                    json["Resources"]["ExchangeSettings"]["Properties"]["Settings"] = stringBuilder.ToString();
                    original = json.ToJson();
                }

                return original;
            }
        }

        protected override WeaveInstaller.PostProcess PostProcess()
        {
            var installer = target as StaminaStoreInstaller;
            // ReSharper disable once PossibleNullReferenceException
            return new StaminaStoreMasterPostProcess(installer);
        }
    }
}