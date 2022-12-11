using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class IceCube : SokobanObject {
    public IceCube UpAttachedIceCube;
    public IceCube DownAttachedIceCube;
    public IceCube LeftAttachedIceCube;
    public IceCube RightAttachedIceCube;
    public override bool IsPushable()
    {
        return true;
    }
    public override void IsPushed()
    {
        //要检测能不能推,然后开始推之后继续检测啥的
    }

    public List<IceCube> GetAllAttachedIceCubes() {
        List<IceCube> results = new List<IceCube>();
        List<IceCube> cubes = GetMyAttachedIceCubes();
        // while(cubes.Count > 0) {
        //     results.Union(cubes);
        //     foreach (IceCube cube in cubes) {
        //         cube.GetMyAttachedIceCubes();
        //     }
        // }




        return cubes;
    }

    public List<IceCube> GetMyAttachedIceCubes() {
        List<IceCube> cubes = new List<IceCube>();
        if(UpAttachedIceCube != null) {
            cubes.Add(UpAttachedIceCube);
        }
        if(DownAttachedIceCube != null) {
            cubes.Add(DownAttachedIceCube);
        }
        if(LeftAttachedIceCube != null) {
            cubes.Add(LeftAttachedIceCube);
        }
        if(RightAttachedIceCube != null) {
            cubes.Add(RightAttachedIceCube);
        }

        return cubes;
    }
    public void SetIceCubeWithDirection(Direction dir,IceCube cube) {
        switch (dir)
        {
            case Direction.UP:
            UpAttachedIceCube = cube;
            break;
            case Direction.DOWN:
            DownAttachedIceCube = cube;
            break;
            case Direction.LEFT:
            LeftAttachedIceCube = cube;
            break;
            case Direction.RIGHT:
            RightAttachedIceCube = cube;
            break;
        }

    }
}
