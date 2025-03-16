#pragma once

#if !defined(__INVADER_H__)
#define __INVADER_H__

// Library Includes

// Local Includes
#include "entity.h"
#include "Bullet.h"
#include "Thunder.h"

// Types

// Constants

// Prototypes

class CInvader : public CEntity
{
	// Member Functions
public:
	CInvader();
	virtual ~CInvader();

	virtual bool Initialise();

	virtual void Draw();
	virtual void Process(float _fDeltaTick);
	virtual CBullet* FireBullet();

	void SetHit(bool _b);
	bool IsHit() const;
	virtual bool TakeDamage();
	virtual int GetHealth();

protected:

private:
	CInvader(const CInvader& _kr);
	CInvader& operator= (const CInvader& _kr);

	// Member Variables
public:

protected:
	bool m_bHit;

private:

};

#endif    // __BRICK_H__

