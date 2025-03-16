#pragma once

#include <random>

#include "Invader.h"
#include "Lazor.h"
#include "Blast.h"
#include "Thunder.h"
#include "Weave.h"
#include "Game.h"

class CMothership : public CInvader
{
public:
	CMothership();
	~CMothership();

	bool Initialise();

	virtual CBullet* FireBullet();

	bool TakeDamage();
	int GetHealth();

	//Member Variables
private:
	int m_iHealth;
};

