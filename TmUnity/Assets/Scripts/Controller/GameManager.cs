// using UnityEngine;
// using Eccentric.Utils;
// namespace TmUnity
// {
//     class GameManager : TSingletonMonoBehavior<GameManager>
//     {
//         float aspcetFactor = .5625f;
//         int lastWidth = 0;
//         int lastHeight = 0;

//         // Update is called once per frame
//         void Update()
//         {
//             var width = Screen.width;
//             var height = Screen.height;

//             // if (lastWidth != width) // if the user is changing the width
//             // {
//             //     // update the height
//             //     var heightAccordingToWidth = width / 16.0 * 9.0;
//             //     Screen.SetResolution(width, Mathf.Round(heightAccordingToWidth), false, 0);
//             // }
//             if (lastHeight != height) // if the user is changing the height
//             {
//                 // update the width
//                 var widthAccordingToHeight = (int)Mathf.Round(height * aspcetFactor);
//                 Screen.SetResolution(widthAccordingToHeight, height, FullScreenMode.Windowed, 60);
//                 lastWidth = widthAccordingToHeight;
//             }
//             lastHeight = height;
//         }
//     }

// }
