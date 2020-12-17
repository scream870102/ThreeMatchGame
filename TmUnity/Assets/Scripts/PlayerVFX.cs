using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
using Lean.Pool;
namespace TmUnity
{
    class PlayerVFX : MonoBehaviour
    {
        Image image = null;
        void Awake() => image = GetComponent<Image>();
        async public Task Attack(Vector3 start, Vector3 end, Color color, float vel)
        {
            image.color = color;
            var dir = (end - start).normalized;
            transform.position = start;
            //NOTE: 假定fps為30 根據速率計算出每一個frame走得平均距離 將其平方以後*1.5以確保 單位時間走得距離不會超過導致停不下來
            var benchmark = Mathf.Pow((vel / 30f), 2f) * 1.5f;
            while ((transform.position - end).sqrMagnitude > benchmark)
            {
                transform.position = transform.position + dir * vel * Time.deltaTime;
                await Task.Delay((int)(Time.deltaTime * 1000f));
            }
            LeanPool.Despawn(gameObject);
        }
    }

}
