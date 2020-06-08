using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gs2.Core.Util;
using Gs2.Unity.Gs2Money.Model;
using Gs2.Weave.Core.Editor;
using Gs2.Util.LitJson;
using UnityEditor;
using UnityEngine;

namespace Gs2.Weave.Money
{
    [CustomEditor(typeof(MoneyInstaller))]   
    public class MoneyInstallerEditor: AbstractWeaveInstaller
    {
        public override void OnInspectorGUI()
        {
            var installer = target as MoneyInstaller;
            // ReSharper disable once PossibleNullReferenceException
            var setting = installer.GetComponent<MoneySetting>();

            base.OnInspectorGUI();

            using (new EditorGUI.ChangeCheckScope())
            {
                if (outputs != null)
                {
                    foreach (var output in outputs)
                    {
                        if (output.name == "MoneyNamespaceName")
                        {
                            installer.moneyNamespaceName = output.value;
                            EditorUtility.SetDirty(installer);
                            setting.moneyNamespaceName = output.value;
                            EditorUtility.SetDirty(setting);
                        }
                        if (output.name == "EnableDebugDepositAction")
                        {
                            installer.enableDebugDepositAction = Convert.ToBoolean(output.value);
                            EditorUtility.SetDirty(installer);
                        }
                        if (output.name == "IdentifierDepositPolicyName")
                        {
                            installer.identifierDepositPolicyName = output.value;
                            EditorUtility.SetDirty(installer);
                        }
                        if (output.name == "IdentifierDepositUserName")
                        {
                            installer.identifierDepositUserName = output.value;
                            EditorUtility.SetDirty(installer);
                        }
                        if (output.name == "IdentifierDepositClientId")
                        {
                            setting.identifierDepositClientId = output.value;
                            EditorUtility.SetDirty(setting);
                        }
                        if (output.name == "IdentifierDepositClientSecret")
                        {
                            setting.identifierDepositClientSecret = output.value;
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
            var installer = target as MoneyInstaller;
            // ReSharper disable once PossibleNullReferenceException
            if (string.IsNullOrEmpty(installer.moneyNamespaceName))
            {
                return false;
            }
            if (string.IsNullOrEmpty(installer.identifierDepositPolicyName))
            {
                return false;
            }
            if (string.IsNullOrEmpty(installer.identifierDepositUserName))
            {
                return false;
            }
            return true;
        }

        class MoneyMasterPostProcess : WeaveInstaller.PostProcess
        {
            private MoneyInstaller _installer;
            
            public MoneyMasterPostProcess(MoneyInstaller installer)
            {
                _installer = installer;
            }
            
            public override string Execute(string original)
            {
                original = original.Replace("${Gs2::Weave::MoneyNamespaceName}", _installer.moneyNamespaceName);
                original = original.Replace("${Gs2::Weave::EnableDebugDepositAction}", _installer.enableDebugDepositAction.ToString());
                if (_installer.enableDebugDepositAction)
                {
                    original = original.Replace("${Gs2::Weave::IdentifierDepositPolicyName}", _installer.identifierDepositPolicyName);
                    original = original.Replace("${Gs2::Weave::IdentifierDepositUserName}", _installer.identifierDepositUserName);
                }
                else
                {
                    var json = JsonMapper.ToObject(original);
                    json["Globals"]["Alias"].Remove("IdentifierDepositPolicyName");
                    json["Globals"]["Alias"].Remove("IdentifierDepositUserName");
                    json["Resources"].Remove("IdentifierDepositUser");
                    json["Resources"].Remove("IdentifierDepositPolicy");
                    json["Resources"].Remove("IdentifierDepositUserAttachPolicy");
                    json["Resources"].Remove("IdentifierDepositIdentifier");
                    json["Outputs"].Remove("IdentifierDepositPolicyName");
                    json["Outputs"].Remove("IdentifierDepositUserName");
                    json["Outputs"].Remove("IdentifierDepositClientId");
                    json["Outputs"].Remove("IdentifierDepositClientSecret");
                    original = json.ToJson();
                }

                return original;
            }
        }

        protected override WeaveInstaller.PostProcess PostProcess()
        {
            var installer = target as MoneyInstaller;
            // ReSharper disable once PossibleNullReferenceException
            return new MoneyMasterPostProcess(installer);
        }
    }
}