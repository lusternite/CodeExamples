#pragma once

#include <random>

#include "Invader.h"
#include "Thunder.h"

class CDestroyer : public CInvader
{
public:
	//Member Functions
	CDestroyer();
	~CDestroyer();

	bool Initialise();

	virtual void Draw();
	virtual void Process(float _fDeltaTick);
	virtual CBullet* FireBullet();

	//Member Variables
private:

};

