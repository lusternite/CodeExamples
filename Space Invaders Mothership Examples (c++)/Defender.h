#pragma once

#if !defined(__DEFENDER_H__)
#define __DEFENDER_H__

// Library Includes

// Local Includes
#include "entity.h"
#include "Sprite.h"

// Types

// Constants

// Prototypes
class CDefender : public CEntity
{
	// Member Functions
public:
	CDefender();
	virtual ~CDefender();

	virtual bool Initialise();

	virtual void Draw();
	virtual void Process(float _fDeltaTick);

	float GetMovementSpeed();
	void SetMovementSpeed(float _fMS);

	void GainThunder();
	void GainWeave();
	void GainBlast();
	bool HasThunder();
	bool HasWeave();
	bool HasBlast();

protected:

private:
	CDefender(const CDefender& _kr);
	CDefender& operator= (const CDefender& _kr);
	float m_fMovementSpeed;

	// Member Variables
public:

protected:

private:
	bool m_bHasThunder;
	bool m_bHasWeave;
	bool m_bHasBlast;

};

#endif    // __PADDLE_H__
