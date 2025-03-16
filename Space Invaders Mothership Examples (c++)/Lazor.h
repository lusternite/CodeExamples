#pragma once

#include "Bullet.h"


class CLazor : public CBullet
{
public:
	//Member functions
	CLazor();
	~CLazor();

	virtual bool Initialise(float _fPosX, float _fPosY, float _fVelocityX, float _fVelocityY);

	CBullet* SpawnTail(float _fDeltaTick);

	virtual void Process(float _fDeltaTick);

	float GetLifeTimer();
	
	float GetLiveForSeconds();
	void SetLiveForSeconds(float _fLiveForSeconds);

	bool HasSpawnedTail();
	virtual bool IsLazor();
	virtual bool CanSpawnTail();

	//Member variables
private:
	float m_fLiveForSeconds;
	float m_fLifeTimer;
	bool m_bSpawnedTail;
};

