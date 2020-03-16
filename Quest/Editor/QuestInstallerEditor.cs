using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gs2.Gs2Quest.Model;
using Gs2.Unity.Gs2Quest.Model;
using Gs2.Weave.Core.Editor;
using LitJson;
using UnityEditor;
using UnityEngine;

namespace Gs2.Weave.Quest
{
    [CustomEditor(typeof(QuestInstaller))]   
    public class QuestInstallerEditor: AbstractWeaveInstaller
    {
        public override void OnInspectorGUI()
        {
            var installer = target as QuestInstaller;
            // ReSharper disable once PossibleNullReferenceException
            var setting = installer.GetComponent<QuestSetting>();

            base.OnInspectorGUI();

            using (new EditorGUI.ChangeCheckScope())
            {
                if (outputs != null)
                {
                    foreach (var output in outputs)
                    {
                        if (output.name == "QuestNamespaceName")
                        {
                            installer.questNamespaceName = output.value;
                            EditorUtility.SetDirty(installer);
                            setting.questNamespaceName = output.value;
                            EditorUtility.SetDirty(setting);
                        }
                        if (output.name == "QuestGroupName")
                        {
                            installer.questGroupName = output.value;
                            EditorUtility.SetDirty(installer);
                            setting.questGroupName = output.value;
                            EditorUtility.SetDirty(setting);
                        }
                        if (output.name == "QuestKeyNamespaceName")
                        {
                            installer.keyNamespaceName = output.value;
                            EditorUtility.SetDirty(installer);
                        }
                        if (output.name == "QuestKeyName")
                        {
                            installer.keyName = output.value;
                            EditorUtility.SetDirty(installer);
                        }
                        if (output.name == "QuestKeyId")
                        {
                            setting.questKeyId = output.value;
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
            var installer = target as QuestInstaller;
            // ReSharper disable once PossibleNullReferenceException
            if (string.IsNullOrEmpty(installer.questNamespaceName))
            {
                return false;
            }
            if (string.IsNullOrEmpty(installer.questGroupName))
            {
                return false;
            }
            if (installer.quests == null || installer.quests.Count == 0)
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

        public class QuestMaster
        {
            public string version = "2019-05-14";
            public List<QuestGroupModel> groups;
            
            public void WriteJson(JsonWriter writer)
            {
                writer.WriteObjectStart();
                if(this.version != null)
                {
                    writer.WritePropertyName("version");
                    writer.Write(this.version);
                }
                if(this.groups != null)
                {
                    writer.WritePropertyName("groups");
                    writer.WriteArrayStart();
                    foreach(var item in this.groups)
                    {
                        item.WriteJson(writer);
                    }
                    writer.WriteArrayEnd();
                }
                writer.WriteObjectEnd();
            }
        }

        class MoneyStoreMasterPostProcess : WeaveInstaller.PostProcess
        {
            private QuestInstaller _installer;
            
            public MoneyStoreMasterPostProcess(QuestInstaller installer)
            {
                _installer = installer;
            }
            
            public override string Execute(string original)
            {
                original = original.Replace("${Gs2::Weave::QuestNamespaceName}", _installer.questNamespaceName);
                original = original.Replace("${Gs2::Weave::QuestGroupName}", _installer.questGroupName);
                original = original.Replace("${Gs2::Weave::QuestKeyNamespaceName}", _installer.keyNamespaceName);
                original = original.Replace("${Gs2::Weave::QuestKeyName}", _installer.keyName);
                
                {
                    var master = new QuestMaster
                    {
                        groups = new List<QuestGroupModel>
                        {
                            new QuestGroupModel
                            {
                                name = _installer.questGroupName,
                                quests = _installer.quests.Select(
                                    quest => quest.ToModel(
                                        _installer.staminaInstaller.staminaNamespaceName,
                                        _installer.staminaInstaller.staminaModelName,
                                        _installer.goldInstaller.inventoryNamespaceName,
                                        _installer.goldInstaller.inventoryModelName,
                                        _installer.goldInstaller.itemModelName
                                    )
                                ).ToList(),
                            }
                        }
                    };
                
                    var stringBuilder = new StringBuilder();
                    var jsonWriter = new JsonWriter(stringBuilder);
                    master.WriteJson(jsonWriter);

                    var json = JsonMapper.ToObject(original);
                    json["Resources"]["QuestSettings"]["Properties"]["Settings"] = stringBuilder.ToString();
                    original = json.ToJson();
                }

                return original;
            }
        }

        protected override WeaveInstaller.PostProcess PostProcess()
        {
            var installer = target as QuestInstaller;
            // ReSharper disable once PossibleNullReferenceException
            return new MoneyStoreMasterPostProcess(installer);
        }
    }
}