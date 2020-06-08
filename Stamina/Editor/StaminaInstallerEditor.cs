using System;
using System.Collections.Generic;
using System.Text;
using Gs2.Gs2Stamina.Model;
using Gs2.Weave.Core.Editor;
using Gs2.Util.LitJson;
using UnityEditor;

namespace Gs2.Weave.Stamina
{
    [CustomEditor(typeof(StaminaInstaller))]   
    public class StaminaInstallerEditor: AbstractWeaveInstaller
    {
        public override void OnInspectorGUI()
        {
            var installer = target as StaminaInstaller;
            // ReSharper disable once PossibleNullReferenceException
            var setting = installer.GetComponent<StaminaSetting>();

            base.OnInspectorGUI();

            using (new EditorGUI.ChangeCheckScope())
            {
                if (outputs != null)
                {
                    foreach (var output in outputs)
                    {
                        if (output.name == "StaminaNamespaceName")
                        {
                            installer.staminaNamespaceName = output.value;
                            EditorUtility.SetDirty(installer);
                            setting.staminaNamespaceName = output.value;
                            EditorUtility.SetDirty(setting);
                        }
                        if (output.name == "StaminaModelName")
                        {
                            installer.staminaModelName = output.value;
                            EditorUtility.SetDirty(installer);
                            setting.staminaModelName = output.value;
                            EditorUtility.SetDirty(setting);
                        }
                        if (output.name == "Capacity")
                        {
                            installer.capacity = Convert.ToInt32(output.value);
                            EditorUtility.SetDirty(installer);
                        }
                        if (output.name == "RecoverValue")
                        {
                            installer.recoverValue = Convert.ToInt32(output.value);
                            EditorUtility.SetDirty(installer);
                        }
                        if (output.name == "RecoverIntervalMinutes")
                        {
                            installer.recoverIntervalMinutes = Convert.ToInt32(output.value);
                            EditorUtility.SetDirty(installer);
                        }
                    }

                    outputs = null;
                }
            }

            DrawDefaultInspector();
        }

        protected override bool Validate()
        {
            var installer = target as StaminaInstaller;
            // ReSharper disable once PossibleNullReferenceException
            if (string.IsNullOrEmpty(installer.staminaNamespaceName))
            {
                return false;
            }
            if (string.IsNullOrEmpty(installer.staminaModelName))
            {
                return false;
            }
            if (installer.capacity <= 0)
            {
                return false;
            }
            if (installer.recoverValue <= 0)
            {
                return false;
            }
            if (installer.recoverIntervalMinutes <= 0)
            {
                return false;
            }
            return true;
        }
        
        public class StaminaMaster
        {
            public string version = "2019-02-14";
            public List<StaminaModel> staminaModels;
            
            public void WriteJson(JsonWriter writer)
            {
                writer.WriteObjectStart();
                if(this.version != null)
                {
                    writer.WritePropertyName("version");
                    writer.Write(this.version);
                }
                if(this.staminaModels != null)
                {
                    writer.WritePropertyName("staminaModels");
                    writer.WriteArrayStart();
                    foreach(var item in this.staminaModels)
                    {
                        item.WriteJson(writer);
                    }
                    writer.WriteArrayEnd();
                }
                writer.WriteObjectEnd();
            }
        }

        class StaminaMasterPostProcess : WeaveInstaller.PostProcess
        {
            private StaminaInstaller _installer;
            
            public StaminaMasterPostProcess(StaminaInstaller installer)
            {
                _installer = installer;
            }
            
            public override string Execute(string original)
            {
                original = original.Replace("${Gs2::Weave::StaminaNamespaceName}", _installer.staminaNamespaceName);
                original = original.Replace("${Gs2::Weave::StaminaModelName}", _installer.staminaModelName);
                original = original.Replace("${Gs2::Weave::Capacity}", _installer.capacity.ToString());
                original = original.Replace("${Gs2::Weave::RecoverValue}", _installer.recoverValue.ToString());
                original = original.Replace("${Gs2::Weave::RecoverIntervalMinutes}", _installer.recoverIntervalMinutes.ToString());
                
                {
                    var master = new StaminaMaster
                    {
                        staminaModels = new List<StaminaModel>
                        {
                            new StaminaModel
                            {
                                name = _installer.staminaModelName,
                                initialCapacity = _installer.capacity,
                                maxCapacity = _installer.capacity,
                                recoverValue = _installer.recoverValue,
                                recoverIntervalMinutes = _installer.recoverIntervalMinutes,
                            }
                        }
                    };
                
                    var stringBuilder = new StringBuilder();
                    var jsonWriter = new JsonWriter(stringBuilder);
                    master.WriteJson(jsonWriter);

                    var json = JsonMapper.ToObject(original);
                    json["Resources"]["StaminaSettings"]["Properties"]["Settings"] = stringBuilder.ToString();
                    original = json.ToJson();
                }

                return original;
            }
        }
        
        protected override WeaveInstaller.PostProcess PostProcess()
        {
            var installer = target as StaminaInstaller;
            return new StaminaMasterPostProcess(installer);
        }
    }
}