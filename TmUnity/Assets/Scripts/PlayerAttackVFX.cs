using UnityEngine;
using TmUnity.Node;
using System.Threading.Tasks;
using Lean.Pool;
using UnityEngine.UI;
namespace TmUnity
{
    class PlayerAttackVFX : MonoBehaviour
    {
        static int id = -1;
        Image image = null;
        void Awake() => image = GetComponent<Image>();
        public async Task Attack(Vector3 start, Vector3 end, Color color, float vel)
        {
            image.color = color;
            var dir = (end - start).normalized;
            transform.position = start;
            while ((transform.position - end).sqrMagnitude > 200f)
            {
                transform.position = transform.position + dir * vel * Time.deltaTime;
                await Task.Delay((int)(Time.deltaTime * 1000f));
            }
            LeanPool.Despawn(gameObject);
        }
    }

}
