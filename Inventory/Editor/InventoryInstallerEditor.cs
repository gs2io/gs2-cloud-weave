using System;
using System.Collections.Generic;
using System.Text;
using Gs2.Weave.Core.Editor;
using LitJson;
using UnityEditor;

namespace Gs2.Weave.Inventory
{
    [CustomEditor(typeof(InventoryInstaller))]   
    public class InventoryInstallerEditor: AbstractWeaveInstaller
    {
        public override void OnInspectorGUI()
        {
            var installer = target as InventoryInstaller;
            // ReSharper disable once PossibleNullReferenceException
            var setting = installer.GetComponent<InventorySetting>();

            base.OnInspectorGUI();

            using (new EditorGUI.ChangeCheckScope())
            {
                if (outputs != null)
                {
                    foreach (var output in outputs)
                    {
                        if (output.name == "InventoryNamespaceName")
                        {
                            installer.inventoryNamespaceName = output.value;
                            EditorUtility.SetDirty(installer);
                            setting.inventoryNamespaceName = output.value;
                            EditorUtility.SetDirty(setting);
                        }
                        if (output.name == "EnableDebugAcquireItemAction")
                        {
                            installer.enableDebugAcquireItemAction = Convert.ToBoolean(output.value);
                            EditorUtility.SetDirty(installer);
                        }
                        if (output.name == "IdentifierAcquireItemPolicyName")
                        {
                            installer.identifierAcquireItemPolicyName = output.value;
                            EditorUtility.SetDirty(installer);
                        }
                        if (output.name == "IdentifierAcquireItemUserName")
                        {
                            installer.identifierAcquireItemUserName = output.value;
                            EditorUtility.SetDirty(installer);
                        }
                        if (output.name == "InventoryModelName")
                        {
                            setting.inventoryModelName = output.value;
                            EditorUtility.SetDirty(setting);
                        }
                        if (output.name == "IdentifierAcquireItemClientId")
                        {
                            setting.identifierAcquireItemClientId = output.value;
                            EditorUtility.SetDirty(setting);
                        }
                        if (output.name == "IdentifierAcquireItemClientSecret")
                        {
                            setting.identifierAcquireItemClientSecret = output.value;
                            EditorUtility.SetDirty(setting);
                        }
                    }

                    outputs = null;
                }
            }

            DrawDefaultInspector();
        }

        protected override bool Validate()
        {
            var installer = target as InventoryInstaller;
            // ReSharper disable once PossibleNullReferenceException
            if (string.IsNullOrEmpty(installer.inventoryNamespaceName))
            {
                return false;
            }
            if (string.IsNullOrEmpty(installer.identifierAcquireItemPolicyName))
            {
                return false;
            }
            if (string.IsNullOrEmpty(installer.identifierAcquireItemUserName))
            {
                return false;
            }
            return true;
        }

        public class InventoryMaster
        {
            public string version = "2019-02-05";
            public List<WeaveInventoryModel> inventoryModels;
            
            public void WriteJson(JsonWriter writer)
            {
                writer.WriteObjectStart();
                if(this.version != null)
                {
                    writer.WritePropertyName("version");
                    writer.Write(this.version);
                }
                if(this.inventoryModels != null)
                {
                    writer.WritePropertyName("inventoryModels");
                    writer.WriteArrayStart();
                    foreach(var item in this.inventoryModels)
                    {
                        item.ToModel().WriteJson(writer);
                    }
                    writer.WriteArrayEnd();
                }
                writer.WriteObjectEnd();
            }
        }

        class InventoryMasterPostProcess : WeaveInstaller.PostProcess
        {
            private InventoryInstaller _installer;
            
            public InventoryMasterPostProcess(InventoryInstaller installer)
            {
                _installer = installer;
            }
            
            public override string Execute(string original)
            {
                original = original.Replace("${Gs2::Weave::InventoryNamespaceName}", _installer.inventoryNamespaceName);
                original = original.Replace("${Gs2::Weave::EnableDebugAcquireItemAction}", _installer.enableDebugAcquireItemAction.ToString());
                if (_installer.enableDebugAcquireItemAction)
                {
                    original = original.Replace("${Gs2::Weave::IdentifierAcquireItemPolicyName}", _installer.identifierAcquireItemPolicyName);
                    original = original.Replace("${Gs2::Weave::IdentifierAcquireItemUserName}", _installer.identifierAcquireItemUserName);
                }
                else
                {
                    var json = JsonMapper.ToObject(original);
                    json["Globals"]["Alias"].Remove("IdentifierAcquireItemPolicyName");
                    json["Globals"]["Alias"].Remove("IdentifierAcquireItemUserName");
                    json["Resources"].Remove("IdentifierAcquireItemUser");
                    json["Resources"].Remove("IdentifierAcquireItemPolicy");
                    json["Resources"].Remove("IdentifierAcquireItemUserAttachPolicy");
                    json["Resources"].Remove("IdentifierAcquireItemIdentifier");
                    json["Outputs"].Remove("IdentifierAcquireItemPolicyName");
                    json["Outputs"].Remove("IdentifierAcquireItemUserName");
                    json["Outputs"].Remove("IdentifierAcquireItemClientId");
                    json["Outputs"].Remove("IdentifierAcquireItemClientSecret");
                    original = json.ToJson();
                }

                {
                    var master = new InventoryMaster
                    {
                        inventoryModels = new List<WeaveInventoryModel>
                        {
                            _installer.inventoryModel
                        }
                    };
                
                    var stringBuilder = new StringBuilder();
                    var jsonWriter = new JsonWriter(stringBuilder);
                    master.WriteJson(jsonWriter);

                    var json = JsonMapper.ToObject(original);
                    json["Resources"]["InventorySettings"]["Properties"]["Settings"] = stringBuilder.ToString();
                    original = json.ToJson();
                }

                return original;
            }
        }

        protected override WeaveInstaller.PostProcess PostProcess()
        {
            var installer = target as InventoryInstaller;
            // ReSharper disable once PossibleNullReferenceException
            return new InventoryMasterPostProcess(installer);
        }
    }
}