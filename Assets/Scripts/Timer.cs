using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer : ITimer
{
	public float startTime;
	public float Elapsed => isLevelFinished? finishedTime:Time.time - startTime - pausedTime;

    private float lastPauseTime;
    private float pausedTime;

	private float finishedTime;

	private bool isLevelFinished;
	private bool isPausing;
	public Timer()
	{
		startTime = Time.time;
	}
	public void Reset()
	{
		startTime = Time.time;
	}

    public void Pause()
    {
        lastPauseTime = Time.time;
		isPausing = true;
    }
	public void Finish() 
	{
		finishedTime = Elapsed;
		isLevelFinished = true;
	}
    public void Continue()
    {
        pausedTime += Time.time - lastPauseTime;
		isPausing = false;
    }

	public float GetTime()
	{
		float fakePasuedTime = 0;
		if(lastPauseTime != 0) {
			fakePasuedTime = pausedTime + Time.time - lastPauseTime;
		}
		
		return isLevelFinished? finishedTime : isPausing?  Time.time - startTime - fakePasuedTime : Time.time - startTime - pausedTime;
	}
	public static bool operator >(Timer timer, float duration)
		=> timer.Elapsed > duration;
	public static bool operator <(Timer timer, float duration)
		=> timer.Elapsed < duration;
	public static bool operator >=(Timer timer, float duration)
		=> timer.Elapsed >= duration;
	public static bool operator <=(Timer timer, float duration)
		=> timer.Elapsed <= duration;
}
public interface ITimer
{
	float Elapsed {
		get;
	}
	void Reset();
}