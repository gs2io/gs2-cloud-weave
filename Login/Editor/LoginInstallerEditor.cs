using Gs2.Weave.Core.Editor;
using UnityEditor;
using UnityEngine;

namespace Gs2.Weave.Login
{
    [CustomEditor(typeof(LoginInstaller))]   
    public class LoginInstallerEditor: AbstractWeaveInstaller
    {
        public override void OnInspectorGUI()
        {
            var installer = target as LoginInstaller;
            // ReSharper disable once PossibleNullReferenceException
            var setting = installer.GetComponent<LoginSetting>();

            base.OnInspectorGUI();

            using (new EditorGUI.ChangeCheckScope())
            {
                if (outputs != null)
                {
                    foreach (var output in outputs)
                    {
                        switch (output.name)
                        {
                            case "AccountNamespaceName":
                                installer.accountNamespaceName = output.value;
                                setting.accountNamespaceName = output.value;
                                EditorUtility.SetDirty(installer);
                                EditorUtility.SetDirty(setting);
                                break;
                            case "KeyNamespaceName":
                                installer.keyNamespaceName = output.value;
                                EditorUtility.SetDirty(installer);
                                break;
                            case "KeyName":
                                installer.keyName = output.value;
                                EditorUtility.SetDirty(installer);
                                break;
                            case "AccountEncryptionKeyId":
                                setting.accountEncryptionKeyId = output.value;
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
            var installer = target as LoginInstaller;
            // ReSharper disable once PossibleNullReferenceException
            if (string.IsNullOrEmpty(installer.accountNamespaceName))
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

        class LoginPostProcess : WeaveInstaller.PostProcess
        {
            private LoginInstaller _installer;
            
            public LoginPostProcess(
                LoginInstaller installer
            )
            {
                _installer = installer;
            }
            
            public override string Execute(string original)
            {
                original = original.Replace("${Gs2::Weave::AccountNamespaceName}", _installer.accountNamespaceName);
                original = original.Replace("${Gs2::Weave::KeyNamespaceName}", _installer.keyNamespaceName);
                original = original.Replace("${Gs2::Weave::KeyName}", _installer.keyName);
                return original;
            }
        }

        protected override WeaveInstaller.PostProcess PostProcess()
        {
            var installer = target as LoginInstaller;
            // ReSharper disable once PossibleNullReferenceException
            return new LoginPostProcess(
                installer
            );
        }
    }
}