using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gs2.Gs2Inventory.Model;
using Gs2.Weave.Core.Editor;
using LitJson;
using UnityEditor;

namespace Gs2.Weave.Unit
{
    [CustomEditor(typeof(UnitInstaller))]   
    public class UnitInstallerEditor: AbstractWeaveInstaller
    {
        public override void OnInspectorGUI()
        {
            var installer = target as UnitInstaller;
            // ReSharper disable once PossibleNullReferenceException
            var setting = installer.GetComponent<UnitSetting>();

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
                        if (output.name == "InventoryModelName")
                        {
                            installer.inventoryModelName = output.value;
                            EditorUtility.SetDirty(installer);
                            setting.inventoryModelName = output.value;
                            EditorUtility.SetDirty(setting);
                        }
                        if (output.name == "EnableDebugAcquireUnitAction")
                        {
                            installer.enableDebugAcquireUnitAction = Convert.ToBoolean(output.value);
                            EditorUtility.SetDirty(installer);
                        }
                        if (output.name == "IdentifierAcquireUnitPolicyName")
                        {
                            installer.identifierAcquireUnitPolicyName = output.value;
                            EditorUtility.SetDirty(installer);
                        }
                        if (output.name == "IdentifierAcquireUnitUserName")
                        {
                            installer.identifierAcquireUnitUserName = output.value;
                            EditorUtility.SetDirty(installer);
                        }
                        if (output.name == "IdentifierAcquireUnitClientId")
                        {
                            setting.identifierAcquireUnitClientId = output.value;
                            EditorUtility.SetDirty(setting);
                        }
                        if (output.name == "IdentifierAcquireUnitClientSecret")
                        {
                            setting.identifierAcquireUnitClientSecret = output.value;
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
            var installer = target as UnitInstaller;
            // ReSharper disable once PossibleNullReferenceException
            if (string.IsNullOrEmpty(installer.inventoryNamespaceName))
            {
                return false;
            }
            if (string.IsNullOrEmpty(installer.inventoryModelName))
            {
                return false;
            }
            if (installer.units == null || installer.units.Count == 0)
            {
                return false;
            }
            if (string.IsNullOrEmpty(installer.identifierAcquireUnitPolicyName))
            {
                return false;
            }
            if (string.IsNullOrEmpty(installer.identifierAcquireUnitUserName))
            {
                return false;
            }
            return true;
        }

        public class UnitMaster
        {
            public string version = "2019-02-05";
            public List<InventoryModel> inventoryModels;
            
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
                        item.WriteJson(writer);
                    }
                    writer.WriteArrayEnd();
                }
                writer.WriteObjectEnd();
            }
        }

        class UnitMasterPostProcess : WeaveInstaller.PostProcess
        {
            private UnitInstaller _installer;
            
            public UnitMasterPostProcess(UnitInstaller installer)
            {
                _installer = installer;
            }
            
            public override string Execute(string original)
            {
                original = original.Replace("${Gs2::Weave::InventoryNamespaceName}", _installer.inventoryNamespaceName);
                original = original.Replace("${Gs2::Weave::InventoryModelName}", _installer.inventoryModelName);
                original = original.Replace("${Gs2::Weave::EnableDebugAcquireUnitAction}", _installer.enableDebugAcquireUnitAction.ToString());
                if (_installer.enableDebugAcquireUnitAction)
                {
                    original = original.Replace("${Gs2::Weave::IdentifierAcquireUnitPolicyName}", _installer.identifierAcquireUnitPolicyName);
                    original = original.Replace("${Gs2::Weave::IdentifierAcquireUnitUserName}", _installer.identifierAcquireUnitUserName);
                }
                else
                {
                    var json = JsonMapper.ToObject(original);
                    json["Globals"]["Alias"].Remove("IdentifierAcquireUnitPolicyName");
                    json["Globals"]["Alias"].Remove("IdentifierAcquireUnitUserName");
                    json["Resources"].Remove("IdentifierAcquireUnitUser");
                    json["Resources"].Remove("IdentifierAcquireUnitPolicy");
                    json["Resources"].Remove("IdentifierAcquireUnitUserAttachPolicy");
                    json["Resources"].Remove("IdentifierAcquireUnitIdentifier");
                    json["Outputs"].Remove("IdentifierAcquireUnitPolicyName");
                    json["Outputs"].Remove("IdentifierAcquireUnitUserName");
                    json["Outputs"].Remove("IdentifierAcquireUnitClientId");
                    json["Outputs"].Remove("IdentifierAcquireUnitClientSecret");
                    original = json.ToJson();
                }

                {
                    var master = new UnitMaster
                    {
                        inventoryModels = new List<InventoryModel>
                        {
                            new InventoryModel
                            {
                                name = _installer.inventoryModelName,
                                initialCapacity = _installer.capacity,
                                maxCapacity = _installer.capacity,
                                itemModels = _installer.units.Select(
                                    unit => unit.ToModel()
                                ).ToList(),
                            }
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
            var installer = target as UnitInstaller;
            // ReSharper disable once PossibleNullReferenceException
            return new UnitMasterPostProcess(installer);
        }
    }
}