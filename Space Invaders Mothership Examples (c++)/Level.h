#pragma once

#if !defined(__LEVEL_H__)
#define __LEVEL_H__

// Library Includes
#include <Windows.h>
#include <mmsystem.h>
#include <vector>
#include <list>
#include <string>
#include <random>
#include <time.h>
#include <iostream>
#include <fstream>
using namespace std;

// Local Includes
#include "Game.h"
#include "Defender.h"
#include "Fleet.h"
#include "Splitter.h"
#include "Destroyer.h"
#include "Shooter.h"
#include "Squid.h"
#include "Mothership.h"
#include "UFO.h"
#include "Bullet.h"
#include "utils.h"
#include "backbuffer.h"
#include "Bunker.h"
#include "framecounter.h"
#include "resource.h"

// Types

// Constants
enum Direction{
	Left,
	Right
};

enum GameState {
	Title,
	Instructions,
	Game,
	Mothership,
	Victory,
	Defeat,
	Quit
};

// Prototypes

class CLevel
{
	// Member Functions
public:
	CLevel();
	virtual ~CLevel();

	virtual bool Initialise(int _iWidth, int _iHeight);

	virtual void Draw();
	virtual void Process(float _fDeltaTick);

	CDefender* GetDefender() const;

	int GetInvadersRemaining() const;
	void SetGameState(GameState _eGameState);

	void TransferToDlg(HWND _hDlg);
	void RetrieveFromDlg(HWND _hDlg);

	bool ResetGame();
	void ProcessDefeatRewards();

protected:
	void ProcessBulletWallCollision();
	void ProcessDefenderWallCollison();
	void ProcessBulletDefenderCollision();
	void ProcessBulletInvaderCollision();
	void ProcessBulletMothershipCollision();
	void ProcessBulletBunkerCollision();

	void ProcessCheckForWin();
	void ProcessCheckForLose();

	void ProcessBulletSpawn();
	void ProcessBulletBounds();
	void ProcessBulletDeletion();
	void ProcessLazor(float _fDeltaTick);
	void ProcessInvaderMovement();
	void ProcessMothershipMovement(float _fDeltaTime);
	void ProcessUFO(float _fDeltaTick);

	void UpdateScoreText();
	void ReduceLives();
	void DrawScore();
	void DrawLives();
	void DrawMothershipHealth();
	void DrawFPS();
	void DrawReward();

	void SetInvadersRemaining(int _i);

private:
	CLevel(const CLevel& _kr);
	CLevel& operator= (const CLevel& _kr);

	// Member Variables
public:

protected:
	CSprite* m_pBackground;
	CSprite* m_pTitle;
	CSprite* m_pInstructions;
	CSprite* m_pDefeat;
	CSprite* m_pVictory;

	GameState m_eGameState;

	list<CBullet*> m_lpBullets;
	CDefender* m_pDefender;
	vector<CFleet*> m_vpFleets;
	CUFO* m_pUFO;
	CBunker* m_pBunker;
	CFPSCounter* m_fpsCounter;

	CInvader* m_pMothership;

	int m_iWidth;
	int m_iHeight;
	float m_fInvaderSpeed;
	int m_iPlayerLives;

	int m_iInvadersRemaining;
	float m_fInvaderTimer;
	float m_fDefenderTimer;
	double m_dShootCooldown;
	double m_dInvaderCooldown;
	double m_dStateChangeCooldown;
	double m_dSecondsPerFrame;
	Direction m_eInvadersDirection;

	std::string m_strScore;
	std::string m_strLives;
	std::string m_strMothershipHealth;
	std::string m_strRewards;
	int m_iScore;
	int m_iHighScore;
	int m_iLives;

private:

};

#endif    // __LEVEL_H__
