#pragma once

#ifndef _FPSCOUNTER_H_
#define _FPSCOUNTER_H_

// Library Includes
#include <Windows.h>
#include <string>

// Local Includes


// Types

// Constants

// Prototypes

class CFPSCounter
{
	//Member Functions
public:
	CFPSCounter();
	~CFPSCounter();

	void CountFramesPerSecond(float _fDeltaTime);
	bool Initialise();

	void UpdateFPSText();
	void DrawFPSText(HDC _hdc, int _iWidth, int _iHeight);

protected:

private:

	//Member Variables
public:

protected:

private:
	int m_iFrameCount;
	float m_fTimeElapsed;
	float m_fFrameRate;
	std::wstring m_strFPS;
};

#endif // 
