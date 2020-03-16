using Gs2.Weave.Core.Editor;
using LitJson;
using UnityEditor;

namespace Gs2.Weave.Credential
{
    [CustomEditor(typeof(CredentialInstaller))]   
    public class CredentialInstallerEditor: AbstractWeaveInstaller
    {
        public override void OnInspectorGUI()
        {
            var installer = target as CredentialInstaller;
            // ReSharper disable once PossibleNullReferenceException
            var setting = installer.GetComponent<CredentialSetting>();

            base.OnInspectorGUI();

            using (new EditorGUI.ChangeCheckScope())
            {
                if (outputs != null)
                {
                    foreach (var output in outputs)
                    {
                        switch (output.name)
                        {
                            case "ApplicationUserName":
                                installer.applicationUserName = output.value;
                                EditorUtility.SetDirty(installer);
                                break;
                            case "ApplicationClientId":
                                setting.applicationClientId = output.value;
                                EditorUtility.SetDirty(setting);
                                break;
                            case "ApplicationClientSecret":
                                setting.applicationClientSecret = output.value;
                                EditorUtility.SetDirty(setting);
                                break;
                        }
                    }

                    outputs = null;
                }
            }

            DrawDefaultInspector();
        }

        protected override bool Validate()
        {
            var installer = target as CredentialInstaller;
            if (string.IsNullOrEmpty(installer.applicationUserName))
            {
                return false;
            }
            return true;
        }

        class CredentialPostProcess : WeaveInstaller.PostProcess
        {
            private CredentialInstaller _installer;
            
            public CredentialPostProcess(
                CredentialInstaller installer
            )
            {
                _installer = installer;
            }
            
            public override string Execute(string original)
            {
                original = original.Replace("${Gs2::Weave::ApplicationUserName}", _installer.applicationUserName);
                return original;
            }
        }

        protected override WeaveInstaller.PostProcess PostProcess()
        {
            var installer = target as CredentialInstaller;
            // ReSharper disable once PossibleNullReferenceException
            return new CredentialPostProcess(
                installer
            );
        }
    }
}