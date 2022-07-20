using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    // Used to add temporary data to GameObjects
    public class Metadata : MonoBehaviour
    {
        private Dictionary<string, object> _metadata = new();

        public void AddData(string key, object val)
        {
            _metadata[key] = val;
        }

        public T GetData<T>(string key)
        {
            return (T)_metadata[key];
        }
    }
}
