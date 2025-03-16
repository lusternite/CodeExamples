#pragma once

#include "Sprite.h"
#include "Entity.h"
#include "Utils.h"
#include "resource.h"

class CBunker : public CEntity
{
public:
	//Member functions
	CBunker();
	~CBunker();

	bool Initialise(float _fX, float _fY);

	void Draw();
	void Process(float _fDeltaTick);

	void TakeDamage();
	int GetHealth();
	void SetHealth(int _iHealth);
	bool IsDestroyed();
	void SetDestroyed(bool _bDestroyed);

protected:

private:

	//Member variables
public:

protected:

private:
	CSprite* m_pBunkerFull;
	CSprite* m_pBunkerDamaged;


	int m_iHealth;
	bool m_bDestroyed;

};

