using System;
using System.Linq;
using System.Text;
using Gs2.Gs2Experience.Model;
using Gs2.Weave.Core.Editor;
using LitJson;
using UnityEditor;
using UnityEngine;

namespace Gs2.Weave.Experience
{
    [CustomEditor(typeof(ExperienceInstaller))]   
    public class ExperienceInstallerEditor: AbstractWeaveInstaller
    {
        public override void OnInspectorGUI()
        {
            var installer = target as ExperienceInstaller;
            // ReSharper disable once PossibleNullReferenceException
            var setting = installer.GetComponent<ExperienceSetting>();

            base.OnInspectorGUI();

            using (new EditorGUI.ChangeCheckScope())
            {
                if (outputs != null)
                {
                    foreach (var output in outputs)
                    {
                        if (output.name == "ExperienceNamespaceName")
                        {
                            installer.experienceNamespaceName = output.value;
                            EditorUtility.SetDirty(installer);
                            setting.experienceNamespaceName = output.value;
                            EditorUtility.SetDirty(setting);
                        }
                        if (output.name == "ExperienceModelName")
                        {
                            installer.experienceModelName = output.value;
                            EditorUtility.SetDirty(installer);
                            setting.experienceModelName = output.value;
                            EditorUtility.SetDirty(setting);
                        }
                        if (output.name == "EnableDebugIncreaseExperienceAction")
                        {
                            installer.enableDebugIncreaseExperienceAction = Convert.ToBoolean(output.value);
                            EditorUtility.SetDirty(installer);
                        }
                        if (output.name == "IdentifierIncreaseExperiencePolicyName")
                        {
                            installer.identifierIncreaseExperiencePolicyName = output.value;
                            EditorUtility.SetDirty(installer);
                        }
                        if (output.name == "IdentifierIncreaseExperienceUserName")
                        {
                            installer.identifierIncreaseExperienceUserName = output.value;
                            EditorUtility.SetDirty(installer);
                        }
                        if (output.name == "IdentifierIncreaseExperienceClientId")
                        {
                            setting.identifierIncreaseExperienceClientId = output.value;
                            EditorUtility.SetDirty(setting);
                        }
                        if (output.name == "IdentifierIncreaseExperienceClientSecret")
                        {
                            setting.identifierIncreaseExperienceClientSecret = output.value;
                            EditorUtility.SetDirty(setting);
                        }
                    }

                    outputs = null;
                }
            }

            DrawDefaultInspector();
        }

        public class ExperienceMaster
        {
            public string version = "2019-01-11";
            public ExperienceInstaller installer;
            
            public void WriteJson(JsonWriter writer)
            {
                writer.WriteObjectStart();
                if(this.version != null)
                {
                    writer.WritePropertyName("version");
                    writer.Write(this.version);
                }
                if(this.installer.threshold != null)
                {
                    writer.WritePropertyName("experienceModels");
                    writer.WriteArrayStart();
                    new ExperienceModel
                    {
                        name = installer.experienceModelName,
                        defaultRankCap = installer.levelCap,
                        maxRankCap = installer.levelCap,
                        rankThreshold = new Threshold
                        {
                            values = installer.threshold.Select(value => (long?)value).ToList(),
                        }
                    }.WriteJson(writer);
                    writer.WriteArrayEnd();
                }
                writer.WriteObjectEnd();
            }
        }

        protected override bool Validate()
        {
            var installer = target as ExperienceInstaller;
            // ReSharper disable once PossibleNullReferenceException
            if (string.IsNullOrEmpty(installer.experienceNamespaceName))
            {
                return false;
            }
            if (string.IsNullOrEmpty(installer.experienceModelName))
            {
                return false;
            }
            if (installer.threshold == null || installer.threshold.Count == 0)
            {
                return false;
            }
            if (installer.levelCap < 1)
            {
                return false;
            }
            if (string.IsNullOrEmpty(installer.identifierIncreaseExperiencePolicyName))
            {
                return false;
            }
            if (string.IsNullOrEmpty(installer.identifierIncreaseExperienceUserName))
            {
                return false;
            }
            return true;
        }

        class ExperienceMasterPostProcess : WeaveInstaller.PostProcess
        {
            private ExperienceInstaller _installer;
            
            public ExperienceMasterPostProcess(ExperienceInstaller installer)
            {
                _installer = installer;
            }
            
            public override string Execute(string original)
            {
                original = original.Replace("${Gs2::Weave::ExperienceNamespaceName}", _installer.experienceNamespaceName);
                original = original.Replace("${Gs2::Weave::ExperienceModelName}", _installer.experienceModelName);
                original = original.Replace("${Gs2::Weave::EnableDebugIncreaseExperienceAction}", _installer.enableDebugIncreaseExperienceAction.ToString());
                if (_installer.enableDebugIncreaseExperienceAction)
                {
                    original = original.Replace("${Gs2::Weave::IdentifierIncreaseExperiencePolicyName}", _installer.identifierIncreaseExperiencePolicyName);
                    original = original.Replace("${Gs2::Weave::IdentifierIncreaseExperienceUserName}", _installer.identifierIncreaseExperienceUserName);
                }
                else
                {
                    var json = JsonMapper.ToObject(original);
                    json["Globals"]["Alias"].Remove("IdentifierIncreaseExperiencePolicyName");
                    json["Globals"]["Alias"].Remove("IdentifierIncreaseExperienceUserName");
                    json["Resources"].Remove("IdentifierIncreaseExperienceUser");
                    json["Resources"].Remove("IdentifierIncreaseExperiencePolicy");
                    json["Resources"].Remove("IdentifierIncreaseExperienceUserAttachPolicy");
                    json["Resources"].Remove("IdentifierIncreaseExperienceIdentifier");
                    json["Outputs"].Remove("IdentifierIncreaseExperiencePolicyName");
                    json["Outputs"].Remove("IdentifierIncreaseExperienceUserName");
                    json["Outputs"].Remove("IdentifierIncreaseExperienceClientId");
                    json["Outputs"].Remove("IdentifierIncreaseExperienceClientSecret");
                    original = json.ToJson();
                }
                
                {
                    var master = new ExperienceMaster
                    {
                        installer = _installer
                    };

                    var stringBuilder = new StringBuilder();
                    var jsonWriter = new JsonWriter(stringBuilder);
                    master.WriteJson(jsonWriter);

                    var json = JsonMapper.ToObject(original);
                    json["Resources"]["ExperienceSettings"]["Properties"]["Settings"] = stringBuilder.ToString();
                    original = json.ToJson();
                }

                Debug.Log(original);
                return original;
            }
        }

        protected override WeaveInstaller.PostProcess PostProcess()
        {
            var installer = target as ExperienceInstaller;
            // ReSharper disable once PossibleNullReferenceException
            return new ExperienceMasterPostProcess(installer);
        }
    }
}