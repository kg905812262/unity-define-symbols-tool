using System;
using UnityEngine;

namespace Khanekg.EditorTools
{
    [CreateAssetMenu(menuName = "Define Symbols Tool/Scripting Define Symbol")]
    public class ScriptingDefineSymbol : ScriptableObject
    {
        public string Value => value;

        [SerializeField]
        private string value;

        public static ScriptingDefineSymbol Create(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Value cannot be null, string.Empty, or consists exclusively of white-space characters.");

            var instance = ScriptableObject.CreateInstance<ScriptingDefineSymbol>();
            instance.value = value;
            return instance;
        }

        public static implicit operator ScriptingDefineSymbol(string value) => ScriptingDefineSymbol.Create(value);
    }
}
