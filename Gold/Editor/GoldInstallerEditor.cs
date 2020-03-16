using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gs2.Core.Util;
using Gs2.Gs2Inventory.Model;
using Gs2.Unity.Gs2Inventory.Model;
using Gs2.Weave.Core.Editor;
using LitJson;
using UnityEditor;
using UnityEngine;

namespace Gs2.Weave.Gold
{
    [CustomEditor(typeof(GoldInstaller))]   
    public class GoldInstallerEditor: AbstractWeaveInstaller
    {
        public override void OnInspectorGUI()
        {
            var installer = target as GoldInstaller;
            // ReSharper disable once PossibleNullReferenceException
            var setting = installer.GetComponent<GoldSetting>();

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
                        if (output.name == "ItemModelName")
                        {
                            installer.itemModelName = output.value;
                            EditorUtility.SetDirty(installer);
                            setting.itemModelName = output.value;
                            EditorUtility.SetDirty(setting);
                        }
                        if (output.name == "LimitOfCount")
                        {
                            installer.limitOfCount = Convert.ToInt64(output.value);
                            EditorUtility.SetDirty(installer);
                        }
                        if (output.name == "EnableDebugAcquireGoldAction")
                        {
                            if (!string.IsNullOrEmpty(output.value))
                            {
                                installer.enableDebugAcquireGoldAction = Convert.ToBoolean(output.value);
                                EditorUtility.SetDirty(installer);
                            }
                        }
                        if (output.name == "IdentifierAcquireGoldPolicyName")
                        {
                            installer.identifierAcquireGoldPolicyName = output.value;
                            EditorUtility.SetDirty(installer);
                        }
                        if (output.name == "IdentifierAcquireGoldUserName")
                        {
                            installer.identifierAcquireGoldUserName = output.value;
                            EditorUtility.SetDirty(installer);
                        }
                        if (output.name == "IdentifierAcquireGoldClientId")
                        {
                            setting.identifierAcquireGoldClientId = output.value;
                            EditorUtility.SetDirty(setting);
                        }
                        if (output.name == "IdentifierAcquireGoldClientSecret")
                        {
                            setting.identifierAcquireGoldClientSecret = output.value;
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
            var installer = target as GoldInstaller;
            // ReSharper disable once PossibleNullReferenceException
            if (string.IsNullOrEmpty(installer.inventoryNamespaceName))
            {
                return false;
            }
            if (installer.limitOfCount < 0)
            {
                return false;
            }
            if (string.IsNullOrEmpty(installer.identifierAcquireGoldPolicyName))
            {
                return false;
            }
            if (string.IsNullOrEmpty(installer.identifierAcquireGoldUserName))
            {
                return false;
            }
            return true;
        }

        class InventoryMasterPostProcess : WeaveInstaller.PostProcess
        {
            private GoldInstaller _installer;
            
            public InventoryMasterPostProcess(GoldInstaller installer)
            {
                _installer = installer;
            }
            
            public override string Execute(string original)
            {
                original = original.Replace("${Gs2::Weave::InventoryNamespaceName}", _installer.inventoryNamespaceName);
                original = original.Replace("${Gs2::Weave::InventoryModelName}", _installer.inventoryModelName);
                original = original.Replace("${Gs2::Weave::ItemModelName}", _installer.itemModelName);
                original = original.Replace("${Gs2::Weave::LimitOfCount}", _installer.limitOfCount.ToString());
                original = original.Replace("${Gs2::Weave::EnableDebugAcquireGoldAction}", _installer.enableDebugAcquireGoldAction.ToString());
                if (_installer.enableDebugAcquireGoldAction)
                {
                    original = original.Replace("${Gs2::Weave::IdentifierAcquireGoldPolicyName}", _installer.identifierAcquireGoldPolicyName);
                    original = original.Replace("${Gs2::Weave::IdentifierAcquireGoldUserName}", _installer.identifierAcquireGoldUserName);
                }
                else
                {
                    var json = JsonMapper.ToObject(original);
                    json["Globals"]["Alias"].Remove("IdentifierAcquireGoldPolicyName");
                    json["Globals"]["Alias"].Remove("IdentifierAcquireGoldUserName");
                    json["Resources"].Remove("IdentifierAcquireGoldUser");
                    json["Resources"].Remove("IdentifierAcquireGoldPolicy");
                    json["Resources"].Remove("IdentifierAcquireGoldUserAttachPolicy");
                    json["Resources"].Remove("IdentifierAcquireGoldIdentifier");
                    json["Outputs"].Remove("IdentifierAcquireGoldPolicyName");
                    json["Outputs"].Remove("IdentifierAcquireGoldUserName");
                    json["Outputs"].Remove("IdentifierAcquireGoldClientId");
                    json["Outputs"].Remove("IdentifierAcquireGoldClientSecret");
                    original = json.ToJson();
                }

                return original;
            }
        }

        protected override WeaveInstaller.PostProcess PostProcess()
        {
            var installer = target as GoldInstaller;
            return new InventoryMasterPostProcess(installer);
        }
    }
}