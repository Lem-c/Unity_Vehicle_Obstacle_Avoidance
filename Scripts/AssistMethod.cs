using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Wingman
{
    public class AssistMethod
    {
		// The list saving the radom numbers
		static List<int> list = new List<int>();


		public static int GetRandomIntNum(int min, int max)
		{
			int random = Random.Range(min, max);
			while (true)
			{

				if (!list.Contains(random))
				{
					list.Add(random);
					break;
				}
				else
				{
					random = Random.Range(min, max);

					if (list.Count >= max)
					{
						break;
					}
				}
			}

			return random;
		}

		/// <summary>
		/// Using ray cast to detect the object in front of the '_from'
		/// Single layer detect
		/// </summary>
		/// <param name="_layer">The index of layer that want to detect</param>
		/// <param name="_from">The object casts the ray</param>
		/// <param name="_rayDistance">The max distance that ray detected</param>
		/// <returns>Whether detected the obstacle</returns>
		public static bool ObstacleDetective(int _layer, Transform _from, float _rayDistance)
        {
			int layerMask = 1 << _layer;
			RaycastHit hit;

			// Ignore layerMask to uncomment it
			// layerMask = ~layerMask;


			if(Physics.Raycast(_from.position,
							   _from.TransformDirection(Vector3.forward),
							   out hit,
							   _rayDistance, layerMask))
            {
				// if ray intersect with any obstacles
				return true;
            }

			return false;
        }
    }
}
