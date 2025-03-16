#pragma once

#if !defined(__BALL_H__)
#define __BALL_H__

// Library Includes

// Local Includes
#include "entity.h"
#include "resource.h"
#include "utils.h"

// Types

// Constants

// Prototypes
class CSprite;

class CBullet : public CEntity
{
	// Member Functions
public:
	CBullet();
	virtual ~CBullet();

	virtual bool Initialise(float _fPosX, float _fPosY, float _fVelocityX, float _fVelocityY);

	virtual CBullet* SpawnTail(float _fDeltaTick);

	virtual void Draw();
	virtual void Process(float _fDeltaTick);

	virtual void SetLiveForSeconds(float _fLiveForSeconds);

	float GetVelocityX() const;
	float GetVelocityY() const;
	void SetVelocityX(float _fX);
	void SetVelocityY(float _fY);
	void SetDeletion(bool _bDeletion);
	bool IsDeletion();
	bool IsBounce();
	virtual bool IsLazor();
	virtual bool CanSpawnTail();

	float GetRadius() const;

protected:

private:
	CBullet(const CBullet& _kr);
	CBullet& operator= (const CBullet& _kr);

	// Member Variables
public:

protected:
	float m_fVelocityX;
	float m_fVelocityY;
	bool m_bDeletion;
	bool m_bBounce;

private:

};

#endif    // __BALL_H__
